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
                            Console.WriteLine(e.ToString());
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
            int simFinEntityId = 0;
            JToken stmtTypeJson = jObject[stmtTypeName];
            List<StmtEntity> stmts = new List<StmtEntity>();
            using(SecResDbContext dbContext = new SecResDbContext(dbConnectionString))
            {
                // Cycle through all statements of given type and register them in DB
                foreach (var item in stmtTypeJson)
                {
                    // Get a statement item
                    StmtEntity stmtEntity = JsonConvert.DeserializeObject<StmtEntity>(item.ToString());

                    if (stmtEntity.Period.StartsWith("TTM")) continue;
                    if (stmtEntity.Period.StartsWith("FY")) continue;
                    if (stmtEntity.Period.StartsWith("H1")) continue;
                    if (stmtEntity.Period.StartsWith("H2")) continue;
                    if (stmtEntity.Period.StartsWith("9M")) continue;

                    // Search DB for the statement
                    SimFinStmtRegistry stmt = await dbContext
                                                    .SimFinStmtRegistries.Include(sr => sr.SimFinEntity)
                                                    .Include(sr => sr.StmtType)
                                                    .Include(sr => sr.PeriodType)
                                                    .Where(sr => sr.PeriodType.Name == stmtEntity.Period
                                                                        && sr.FYear == stmtEntity.FYear
                                                                        && sr.StmtType.Name == stmtTypeName
                                                                        && sr.SimFinEntity.SimFinId == simFinEntityIdCode)
                                                    .FirstOrDefaultAsync();

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
                                Console.WriteLine(e.ToString());
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
                                Console.WriteLine(e.ToString());
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
                            Console.WriteLine(e.ToString());
                            throw e;
                        }

                        try
                        {
                            await dbContext.SaveChangesAsync();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
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
                        Console.WriteLine(e.ToString());
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
                        Console.WriteLine(e.ToString());
                        throw e;
                    }

                    // Load standardized statements                      
                }
            }
        }

        private async Task LoadStdStmtAsync(SimFinStmtRegistry stmt, string stmtTypeName)
        {
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


                    string periodEndDateStr = simFinStmtDetails["periodEndDate"].ToString();
                    DateTime periodEndDate = DateTime.ParseExact(periodEndDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    List<JObject> metaTokens = simFinStmtDetails["metaData"].Children<JObject>().ToList();

                    // Don't understand when several meta can be used. Trying to catch this case.
                    //if (metaTokens.Count > 1)
                    //{
                    //    throw new Exception("Found more then 1 meta. Exit");
                    //}

                    //SimFinOriginalStmt origStmt = null;
                    // !!!! The last will be saved only
                    JObject metaDataJObject = metaTokens[0];
                    //foreach (JObject metaDataJObject in metaTokens)
                    //{
                    string firstPublished = metaDataJObject["firstPublished"].ToString();
                    DateTime firstPublishedDate = DateTime.ParseExact(firstPublished, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    string fYear = metaDataJObject["fyear"].ToString();
                    int fYearInt = int.Parse(fYear);
                    string currencyStr = metaDataJObject["currency"].ToString();
                    string periodTypeStr = metaDataJObject["period"].ToString();

                    int currencyId = await dbContext.Currencies.Where(c => c.CharCode == currencyStr).Select(c => c.Id).FirstOrDefaultAsync();

                    int periodTypeId = await dbContext.PeriodTypes.Where(p => p.Name == periodTypeStr).Select(c => c.Id).FirstOrDefaultAsync();

                    simFinOriginalStmt = new SimFinOriginalStmt
                    {
                        CurrencyId = currencyId,
                        FirstPublishedDate = firstPublishedDate,
                        FYear = fYearInt,
                        IsStmtDetailsLoaded = false,
                        PeriodEndDate = periodEndDate,
                        PeriodTypeId = periodTypeId,
                        SimFinStmtRegistryId = stmt.Id
                    };
                    await dbContext.AddAsync(simFinOriginalStmt);
                    try
                    {
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        throw e;
                    }

                    //}
                }

                if (simFinOriginalStmt.IsStmtDetailsLoaded == false)
                {
                    if (simFinStmtDetails == null)
                    {
                        simFinStmtDetails = await GetOrigStmtDetails(stmt, stmtType);
                    }
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
                                    Console.WriteLine(e.ToString());
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
                                    Console.WriteLine(e.ToString());
                                    throw e;
                                }

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
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

        private async Task LoadOriginalStmtAsync(SimFinStmtRegistry stmt, string stmtType)
        {
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


                    string periodEndDateStr = simFinStmtDetails["periodEndDate"].ToString();
                    DateTime periodEndDate = DateTime.ParseExact(periodEndDateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    List<JObject> metaTokens = simFinStmtDetails["metaData"].Children<JObject>().ToList();

                    // Don't understand when several meta can be used. Trying to catch this case.
                    //if (metaTokens.Count > 1)
                    //{
                    //    throw new Exception("Found more then 1 meta. Exit");
                    //}

                    //SimFinOriginalStmt origStmt = null;
                    // !!!! The last will be saved only
                    JObject metaDataJObject = metaTokens[0];
                    //foreach (JObject metaDataJObject in metaTokens)
                    //{
                        string firstPublished = metaDataJObject["firstPublished"].ToString();
                        DateTime firstPublishedDate = DateTime.ParseExact(firstPublished, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                        string fYear = metaDataJObject["fyear"].ToString();
                        int fYearInt = int.Parse(fYear);
                        string currencyStr = metaDataJObject["currency"].ToString();
                        string periodTypeStr = metaDataJObject["period"].ToString();

                        int currencyId = await dbContext.Currencies.Where(c => c.CharCode == currencyStr).Select(c => c.Id).FirstOrDefaultAsync();

                        int periodTypeId = await dbContext.PeriodTypes.Where(p => p.Name == periodTypeStr).Select(c => c.Id).FirstOrDefaultAsync();

                        simFinOriginalStmt = new SimFinOriginalStmt
                        {
                            CurrencyId = currencyId,
                            FirstPublishedDate = firstPublishedDate,
                            FYear = fYearInt,
                            IsStmtDetailsLoaded = false,
                            PeriodEndDate = periodEndDate,
                            PeriodTypeId = periodTypeId,
                            SimFinStmtRegistryId = stmt.Id
                        };
                        await dbContext.AddAsync(simFinOriginalStmt);
                        try
                        {
                            await dbContext.SaveChangesAsync();
                        } catch(Exception e)
                        {
                            Console.WriteLine(e.ToString());
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
                                    Console.WriteLine(e.ToString());
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
                                    Console.WriteLine(e.ToString());
                                    throw e;
                                }

                            } catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
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
