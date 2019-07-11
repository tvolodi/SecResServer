using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SecResServer.Model;
using SecResServer.Model.SimFin;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Jobs
{
    public class SimFinDataDownloader 
    {

        private SecResDbContext dbContext;
        private string dbConnectionString = string.Empty;
        private string baseAddress = "https://simfin.com/api/v1/";

        private const string logFileName = "SimFinImportLog.txt";

        private DateTime stmtPublishDate = DateTime.MinValue;


        string apiKey = "6sFg1i77TDYXdtBfRb1Ga93lD4XGTuZZ";

        public SimFinDataDownloader(string dbConnectionString)
        {

            this.dbConnectionString = dbConnectionString;
            
        }

        public void ExecuteDaily()
        {         

            Task.Run(async () =>
            {
                await DownloadAllEntitiesAsync();
            });
        }

        public async Task<int> DownloadAllEntitiesAsync()
        {
            int totalQnt = 0;
            string serviceName = "info/all-entities";
            string httpReqString = $"{baseAddress}{serviceName}?api-key={apiKey}";
            List<SimFinEntity> simFinEntities = await Libs.HttpReqExec.GetAsync<List<SimFinEntity>>(httpReqString);
            using (SecResDbContext dbContext = new SecResDbContext(dbConnectionString))
            {
                for (int i = 0; i < simFinEntities.Count; i++)
                {
                    SimFinEntity currSimFinEntity = simFinEntities[i];

                    SimFinEntity simFinEntity = await dbContext.SimFinEntities.Where(se => se.Ticker == currSimFinEntity.Ticker).FirstOrDefaultAsync();
                    if(simFinEntity == null)
                    {
                        
                        simFinEntity = new SimFinEntity
                        {
                            Name = currSimFinEntity.Name,
                            SimFinId = currSimFinEntity.SimFinId,
                            Ticker = currSimFinEntity.Ticker
                        };
                        await dbContext.AddAsync(simFinEntity);
                    }
                    else
                    {
                        simFinEntity.LastUpdateDT = DateTime.Now;
                        dbContext.Entry(simFinEntity).State = EntityState.Modified;
                    }

                }
                await dbContext.SaveChangesAsync();

            }


            totalQnt = simFinEntities.Count();
            return totalQnt;
        }

        public async Task<int> DownloadAllStmtAsync()
        {
            int totalQnt = 0;
            string serviceName = "companies/id";
            string subServiceName = "statements/list";

            int daysQntToUpdate = 100;
            
            int simFinEntityId = 0;
                 
            using (SecResDbContext dbContext = new SecResDbContext(dbConnectionString))
            {
                List<SimFinEntity> simFinEntities = await dbContext.SimFinEntities
                                                                   .Where(sfe => sfe.LastUpdateDT > DateTime.Now.AddDays(-daysQntToUpdate)
                                                                                                || sfe.IsStmtRegistryLoaded == false)
                                                                   .ToListAsync();
                for (int i = 0; i < simFinEntities.Count; i++)
                {
                    SimFinEntity entity = simFinEntities[i];
                    simFinEntityId = entity.SimFinId;
                    string httpReqString = $"{baseAddress}{serviceName}/{simFinEntityId}/{subServiceName}?api-key={apiKey}";

                    JObject simFinStmtRegistry = await Libs.SimFinHttpReqExec.ExecSimFinHttpReqAsync<JObject>(httpReqString, dbConnectionString);

                    string[] stmtTypes = new string[] { "pl", "bs", "cf" };
                    for(int itemCnt = 0; itemCnt < stmtTypes.Length; itemCnt++)
                    {
                        try
                        {
                            await RegisterStmtAsync(stmtTypes[itemCnt], simFinStmtRegistry, simFinEntityId);
                        } catch(Exception e)
                        {
                            await WriteLog(e.ToString());
                            throw e;
                        }
                    }

                    // Do update that statement registry for this entity is loaded for now.
                    entity.IsStmtRegistryLoaded = true;
                    entity.LastUpdateDT = DateTime.Now;
                    dbContext.Entry(entity).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
            }

            totalQnt = 1;
            return totalQnt;
        }

        private async Task RegisterStmtAsync(string stmtTypeName, JObject jObject, int simFinEntityIdCode)
        {
            string logText = string.Empty;
            try
            {
                logText = $"Register stmt: {stmtTypeName} {simFinEntityIdCode}";
                await WriteLog(logText);
            } catch (Exception e)
            {
                await WriteLog(e.ToString());
                throw e;
            }


            int simFinEntityId = 0;
            JToken stmtTypeJson = jObject[stmtTypeName];

            if (stmtTypeJson == null) return;

            List<StmtEntity> stmts = new List<StmtEntity>();
            using(SecResDbContext dbContext = new SecResDbContext(dbConnectionString))
            {
                // Cycle through all statements of given type and register them in DB
                foreach (var item in stmtTypeJson)
                {
                    // Get a statement item
                    StmtEntity stmtEntity = JsonConvert.DeserializeObject<StmtEntity>(item.ToString());

                    if (stmtEntity.Period.StartsWith("TTM")) continue;
                    // if (stmtEntity.Period.StartsWith("FY")) continue;
                    if (stmtEntity.Period.StartsWith("H1")) continue;
                    if (stmtEntity.Period.StartsWith("H2")) continue;
                    if (stmtEntity.Period.StartsWith("9M")) continue;

                    // Search DB for the statement
                    SimFinStmtRegistry stmt = await dbContext
                                                    .SimFinStmtRegistries
                                                    .Include(sr => sr.SimFinEntity)
                                                    .Include(sr => sr.StmtType)
                                                    .Include(sr => sr.PeriodType)
                                                    .Where(sr => sr.PeriodType.Name == stmtEntity.Period
                                                                        && sr.FYear == stmtEntity.FYear
                                                                        && sr.StmtType.Name == stmtTypeName
                                                                        && sr.SimFinEntity.SimFinId == simFinEntityIdCode)
                                                    .FirstOrDefaultAsync();

                    //if(stmt != null && stmt.FYear == 2010 && stmt.PeriodType.Name == "Q1" && stmt.StmtType.Name == "pl")
                    //{
                    //    Console.WriteLine("That is!");
                    //}

                    // Init publish date with mininum value to know, that it is filled on original stmt
                    stmtPublishDate = DateTime.MinValue;

                    // If a statement entry is not registered in DB then register
                    if (stmt == null)
                    {
                        // Get period type Id
                        PeriodType periodType = await dbContext.PeriodTypes.Where(pt => pt.Name == stmtEntity.Period).FirstOrDefaultAsync();
                        // If the period type is not registered in DB then register it
                        if (periodType == null)
                        {
                            periodType = new PeriodType
                            {
                                Name = stmtEntity.Period
                            };
                            await dbContext.AddAsync(periodType);
                            try
                            {
                                await dbContext.SaveChangesAsync();
                            } catch (Exception e)
                            {
                                await WriteLog(e.ToString());
                                throw e;
                            }
                        }

                        // Get statement type Id
                        StmtType stmtType = await dbContext.StmtTypes.Where(st => st.Name == stmtTypeName).FirstOrDefaultAsync();
                        // If the statement type is not registered then register in DB
                        if (stmtType == null)
                        {
                            stmtType = new StmtType
                            {
                                Name = stmtTypeName
                            };
                            await dbContext.AddAsync(stmtType);
                            try
                            {
                                await dbContext.SaveChangesAsync();
                            }
                            catch (Exception e)
                            {
                                await WriteLog(e.ToString());
                                throw e;
                            }
                        }

                        SimFinEntity simFinEntity = await dbContext.SimFinEntities
                                                                .Where(e => e.SimFinId == simFinEntityIdCode)
                                                                .FirstOrDefaultAsync();                        

                        // Create an statement instance
                        stmt = new SimFinStmtRegistry
                        {
                            IsCalculated = stmtEntity.IsCalculated,
                            PeriodTypeId = periodType.Id,
                            PeriodType = periodType,
                            SimFinEntity = simFinEntity,
                            SimFinEntityId = simFinEntity.Id,
                            StmtType = stmtType,
                            StmtTypeId = stmtType.Id,
                            LoadDateTime = DateTime.Now,                            
                            FYear = stmtEntity.FYear                            
                        };

                        try
                        {
                            await dbContext.AddAsync(stmt);
                        } catch (Exception e)
                        {
                            await WriteLog(e.ToString());
                            throw e;
                        }

                        try
                        {
                            await dbContext.SaveChangesAsync();
                        }
                        catch (Exception e)
                        {
                            await WriteLog(e.ToString());
                        }
                    }

                    // Load original statement
                    try
                    {
                        await LoadOriginalStmtAsync(stmt, stmtTypeName);
                        stmt.OrigStmtLoadDT = DateTime.Now;
                        dbContext.Entry(stmt).State = EntityState.Modified;
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        await WriteLog(e.ToString() + $" for FYear = {stmtEntity.FYear} period = {stmt.PeriodType.Name}  SimFinId = {stmt.SimFinEntity.SimFinId}" );
                        throw e;
                    }

                    // Load standardized statement
                    try
                    {
                        await LoadStdStmtAsync(stmt, stmtTypeName);
                        stmt.StdStmtLoadDT = DateTime.Now;
                        dbContext.Entry(stmt).State = EntityState.Modified;
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        await WriteLog(e.ToString());
                        throw e;
                    }                   
                }
            }
        }

        private async Task WriteLog(string logText)
        {
            await System.IO.File.AppendAllTextAsync(logFileName, logText);
            await System.IO.File.AppendAllTextAsync(logFileName, "\n");
            await System.IO.File.AppendAllTextAsync(logFileName, "==============================================================");
            await System.IO.File.AppendAllTextAsync(logFileName, "\n");
        }

        private async Task LoadStdStmtAsync(SimFinStmtRegistry stmt, string stmtTypeName)
        {
            string logText = $"Loading std stmt {stmt.FYear} {stmtTypeName} {stmt.SimFinEntity.SimFinId} {stmt.PeriodType.Name}";
            await WriteLog(logText);

            // Check that the statement is not loaded already
            bool isToLoad = false;
            SimFinStdStmt simFinStdStmt = null;
            using (SecResDbContext dbContext = new SecResDbContext(dbConnectionString))
            {
                // Get standardized statement for this type of statement
                simFinStdStmt = await dbContext.SimFinStdStmts
                                                    .Where(ss => ss.SimFinStmtRegistryId == stmt.Id)
                                                    .FirstOrDefaultAsync();

                // If the original statement is not created then load from the site and create

                JObject simFinStmtDetails = null;
                if (simFinStdStmt == null)
                {
                    if (simFinStmtDetails == null)
                    {
                        // Read the statement data from SimFin site -> return json object
                        simFinStmtDetails = await GetStdStmtDetails(stmt, stmtTypeName);
                    }

                    if (simFinStmtDetails == null) return;

                    List<JObject> calculationSchemeList = simFinStmtDetails["calculationScheme"].Children<JObject>().ToList();
                    if(calculationSchemeList != null && calculationSchemeList.Count > 0)
                    {
                        await WriteLog($"Multiple scheme for std statement for {stmt.SimFinEntity.SimFinId} stmt type {stmtTypeName} year {stmt.FYear}");
                        // throw new Exception("multiple scheme");
                    }


                    string periodEndDateStr = simFinStmtDetails["periodEndDate"].ToString();
                    DateTime periodEndDate = DateTime.MinValue;
                    if (periodEndDateStr != string.Empty)
                    {
                        periodEndDate = DateTime.ParseExact(periodEndDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    } else
                    {
                        periodEndDate = stmtPublishDate;
                    }
                    

                    int fYearInt = stmt.FYear;

                    int periodTypeId = stmt.PeriodTypeId;

                    string industryTemplateStr = simFinStmtDetails["industryTemplate"].ToString();
                    SimFinStmtIndustryTemplate industryTemplate = await dbContext.SimFinStmtIndustryTemplates
                                                                                 .Where(it => it.Name == industryTemplateStr)
                                                                                 .FirstOrDefaultAsync();
                    if(industryTemplate == null)
                    {
                        industryTemplate = new SimFinStmtIndustryTemplate
                        {
                            Name = industryTemplateStr
                        };
                        await dbContext.AddAsync(industryTemplate);
                        await dbContext.SaveChangesAsync();
                    }

                    simFinStdStmt = new SimFinStdStmt
                    {
                        // CurrencyId = currencyId,
                        // FirstPublishedDate = firstPublishedDate,
                        FYear = fYearInt,
                        IsStmtDetailsLoaded = false,
                        PeriodEndDate = periodEndDate,
                        PeriodTypeId = periodTypeId,
                        SimFinStmtRegistryId = stmt.Id,
                        SimFinStmtIndustryTemplateId = industryTemplate.Id,                        
                    };
                    await dbContext.AddAsync(simFinStdStmt);
                    try
                    {
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        await WriteLog(e.ToString());
                        throw e;
                    }

                    //}
                }

                if (simFinStdStmt.IsStmtDetailsLoaded == false)
                {
                    if (simFinStmtDetails == null)
                    {
                        simFinStmtDetails = await GetStdStmtDetails(stmt, stmtTypeName);
                    }

                    if (simFinStmtDetails == null) return;

                    // Fill statement details? create or update
                    List<JObject> stmtRows = simFinStmtDetails["values"].Children<JObject>().ToList();
                    foreach (JObject rowDetails in stmtRows)
                    {
                        string tidStr = rowDetails["tid"].ToString();
                        int tid = int.Parse(tidStr);

                        string uidStr = rowDetails["uid"].ToString();
                        int uid = int.Parse(uidStr);

                        string valueName = rowDetails["standardisedName"].ToString();
                        StmtDetailName stmtDetailName = await dbContext.StmtDetailNames.Where(n => n.Name == valueName).FirstOrDefaultAsync();
                        if (stmtDetailName == null)
                        {
                            stmtDetailName = new StmtDetailName
                            {
                                Name = valueName
                            };
                            await dbContext.AddAsync(stmtDetailName);
                            await dbContext.SaveChangesAsync();
                        }

                        string parentTIdStr = rowDetails["parent_tid"].ToString();
                        int parentTId = int.Parse(parentTIdStr);

                        string displayLevelStr = rowDetails["displayLevel"].ToString();
                        int displayLevel = int.Parse(displayLevelStr);

                        string valueAssignedStr = rowDetails["valueAssigned"].ToString();
                        if (valueAssignedStr == "") valueAssignedStr = "0";
                        double valueAssigned = double.Parse(valueAssignedStr);

                        string valueCalculatedStr = rowDetails["valueCalculated"].ToString();
                        if (valueCalculatedStr == "") valueCalculatedStr = "0";
                        double valueCalculated = double.Parse(valueCalculatedStr);

                        string valueChosenStr = rowDetails["valueChosen"].ToString();
                        if (valueChosenStr == "") valueChosenStr = "0";
                        double valueChosen = double.Parse(valueChosenStr);

                        // Search for the statement details in DB
                        SimFinStdStmtDetail simFinStdStmtDetail = await dbContext.SimFinStdStmtDetails
                                                                                    .Where(ssd => ssd.StmtDetailNameId == stmtDetailName.Id
                                                                                                    && ssd.SimFinStdStmtId == simFinStdStmt.Id
                                                                                                    && ssd.TId == tid)
                                                                                    .FirstOrDefaultAsync();
                        if (simFinStdStmtDetail == null)
                        {
                            simFinStdStmtDetail = new SimFinStdStmtDetail
                            {
                                SimFinStdStmtId = simFinStdStmt.Id,
                                DisplayLevel = displayLevel,
                                ParentTId = parentTId,
                                StmtDetailNameId = stmtDetailName.Id,
                                TId = tid,
                                UId = uid,
                                ValueAssigned = valueAssigned,
                                ValueCalculated = valueCalculated,
                                ValueChosen = valueChosen
                            };
                            try
                            {
                                await dbContext.AddAsync(simFinStdStmtDetail);
                                await dbContext.SaveChangesAsync();

                            }
                            catch (Exception ex)
                            {
                                await WriteLog(ex.ToString());
                                throw ex;

                            }
                        }
                    }

                    simFinStdStmt.IsStmtDetailsLoaded = true;
                    dbContext.Entry(simFinStdStmt).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task<JObject> GetStdStmtDetails(SimFinStmtRegistry stmt, string stmtType)
        {
            string serviceName = "companies/id";
            string subServiceName = "statements/standardised";

            JObject simFinStmtDetails = null;

            string httpReqString = string.Empty;
            try
            {
                httpReqString = $"{baseAddress}{serviceName}/{stmt.SimFinEntity.SimFinId}/{subServiceName}?stype={stmtType}&ptype={stmt.PeriodType.Name}&fyear={stmt.FYear}&api-key={apiKey}";
            } catch (Exception e)
            {
                await WriteLog(e.ToString());
                throw e;
            }         

            // Get and parse json for the original statement
            try
            {
                simFinStmtDetails = await Libs.SimFinHttpReqExec.ExecSimFinHttpReqAsync<JObject>(httpReqString, dbConnectionString);
            } catch (Exception e)
            {
                await WriteLog(e.ToString());
                throw e;
            }
            

            return simFinStmtDetails;
        }

        private async Task LoadOriginalStmtAsync(SimFinStmtRegistry stmt, string stmtType)
        {
            string logText = string.Empty;
            try
            {
                logText = $"LoadOriginalStmtAsync: {stmtType} {stmt.PeriodType.Name} {stmt.SimFinEntity.SimFinId} {stmt.FYear}";
                await WriteLog(logText);
            }
            catch (Exception e)
            {
                await WriteLog(e.ToString());
                throw e;
            }


            // Check that the statement is not loaded already
            bool isToLoad = false;
            SimFinOriginalStmt simFinOriginalStmt = null;
            using (SecResDbContext dbContext = new SecResDbContext(dbConnectionString))
            {
                // Get original statement for this type of statement
                simFinOriginalStmt = await dbContext.SimFinOriginalStmts
                                                    .Where(os => os.SimFinStmtRegistryId == stmt.Id)
                                                    .FirstOrDefaultAsync();

                // If the original statement is not created then load from the site and create

                JObject simFinStmtDetails = null;
                if (simFinOriginalStmt == null)
                {
                    if (simFinStmtDetails == null)
                    {
                        // Read the statement data from SimFin site -> return json object
                        simFinStmtDetails = await GetOrigStmtDetails(stmt, stmtType);
                    }

                    if (simFinStmtDetails == null) return;

                    string periodEndDateStr = string.Empty;
                    DateTime periodEndDate = DateTime.MinValue;
                    periodEndDateStr = simFinStmtDetails["periodEndDate"].ToString();
                    if(periodEndDateStr != string.Empty)
                    {
                        // periodEndDateStr = periodDateToken.ToString();
                        periodEndDate = DateTime.ParseExact(periodEndDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                     
                    List<JObject> metaTokens = simFinStmtDetails["metaData"].Children<JObject>().ToList();

                    // Don't understand when several meta can be used. Trying to catch this case.
                    //if (metaTokens.Count > 1)
                    //{
                    //    throw new Exception("Found more then 1 meta. Exit");
                    //}

                    // !!!! Ignore meta data and calculation scheme. They are to get known how everything was calculated for this period
                    JObject metaDataJObject = metaTokens[0];

                    string firstPublished = metaDataJObject["firstPublished"].ToString();
                    DateTime firstPublishedDate = DateTime.ParseExact(firstPublished, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    stmtPublishDate = firstPublishedDate;
                    string fYear = metaDataJObject["fyear"].ToString();
                    int fYearInt = int.Parse(fYear);
                    string currencyStr = metaDataJObject["currency"].ToString();
                    string periodTypeStr = metaDataJObject["period"].ToString();

                    if(periodEndDateStr == string.Empty)
                    {
                        periodEndDate = firstPublishedDate;
                    }

                    int currencyId = await dbContext.Currencies.Where(c => c.CharCode == currencyStr).Select(c => c.Id).FirstOrDefaultAsync();

                    PeriodType periodType = await dbContext.PeriodTypes.Where(p => p.Name == periodTypeStr).FirstOrDefaultAsync();
                    if(periodType == null)
                    {
                        periodType = new PeriodType
                        {
                            Name = periodTypeStr
                        };

                        await dbContext.AddAsync(periodType);
                        await dbContext.SaveChangesAsync();
                    }


                    simFinOriginalStmt = new SimFinOriginalStmt
                    {
                        CurrencyId = currencyId,
                        FirstPublishedDate = firstPublishedDate,
                        FYear = fYearInt,
                        IsStmtDetailsLoaded = false,
                        PeriodEndDate = periodEndDate,
                        PeriodTypeId = periodType.Id,
                        SimFinStmtRegistryId = stmt.Id
                    };
                    await dbContext.AddAsync(simFinOriginalStmt);
                    try
                    {
                        await dbContext.SaveChangesAsync();
                    } catch(Exception e)
                    {
                    await WriteLog(e.ToString() + $" for FYear = {fYearInt} period = {periodType.Name}  SimFinId = {stmt.SimFinEntity.SimFinId} ");
                    throw e;
                    }
                        
                    //}
                }

                if(simFinOriginalStmt.IsStmtDetailsLoaded == false)
                { 
                    if(simFinStmtDetails == null)
                    {
                        simFinStmtDetails = await GetOrigStmtDetails(stmt, stmtType);
                    }

                    if (simFinStmtDetails == null) return;

                    // Fill statement details? create or update
                    List<JObject> stmtRows = simFinStmtDetails["values"].Children<JObject>().ToList();
                    foreach (JObject rowDetails in stmtRows)
                    {
                        string lineIdStr = rowDetails["lineItemId"].ToString();
                        int lineId = int.Parse(lineIdStr);
                        string lineItemNameStr = rowDetails["lineItemName"].ToString();
                        string valueStr = rowDetails["value"].ToString();
                        double value = double.Parse(valueStr);

                        // Search for the statement details in DB
                        SimFinOrigStmtDetail simFinOrigStmtDetail = await dbContext.SimFinOrigStmtDetails
                                                                                    .Where(osd => osd.LineItemId == lineId
                                                                                                    && osd.SimFinOriginalStmtId == simFinOriginalStmt.Id)
                                                                                    .FirstOrDefaultAsync();

                        if (simFinOrigStmtDetail == null)
                        {
                            StmtDetailName detailName = await dbContext.StmtDetailNames
                                                                        .Where(dn => dn.Name == lineItemNameStr)
                                                                        .FirstOrDefaultAsync();
                            if (detailName == null)
                            {
                                detailName = new StmtDetailName
                                {
                                    Name = lineItemNameStr.Trim()
                                };
                                await dbContext.AddAsync(detailName);
                                try
                                { 
                                    await dbContext.SaveChangesAsync();
                                }
                                catch (Exception e)
                                {
                                    await WriteLog(e.ToString());
                                    throw e;
                                }
                            }

                            simFinOrigStmtDetail = new SimFinOrigStmtDetail
                            {
                                LineItemId = lineId,
                                StmtDetailNameId = detailName.Id,
                                SimFinOriginalStmtId = simFinOriginalStmt.Id,
                                Value = value
                            };

                            try
                            {
                                await dbContext.AddAsync(simFinOrigStmtDetail);
                                try
                                {
                                    await dbContext.SaveChangesAsync();
                                }
                                catch (Exception e)
                                {
                                    await WriteLog(e.ToString());
                                    throw e;
                                }

                            } catch (Exception e)
                            {
                                await WriteLog(e.ToString());
                                throw e;
                            }
                        }
                    }

                    simFinOriginalStmt.IsStmtDetailsLoaded = true;
                    dbContext.Entry(simFinOriginalStmt).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task<JObject> GetOrigStmtDetails(SimFinStmtRegistry stmt, string stmtType)
        {
            string serviceName = "companies/id";
            string subServiceName = "statements/original";

            JObject simFinStmtDetails = null;

            string httpReqString = $"{baseAddress}{serviceName}/{stmt.SimFinEntity.SimFinId}/{subServiceName}?stype={stmtType}&ptype={stmt.PeriodType.Name}&fyear={stmt.FYear}&api-key={apiKey}";

            // Get and parse json for the original statement
            simFinStmtDetails = await Libs.SimFinHttpReqExec.ExecSimFinHttpReqAsync<JObject>(httpReqString, dbConnectionString);

            return simFinStmtDetails;
        }
    }
}
