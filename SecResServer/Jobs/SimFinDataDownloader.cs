using SecResServer.Model;
using SecResServer.Model.SimFin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecResServer.Jobs
{
    public class SimFinDataDownloader 
    {

        private SecResDbContext dbContext;
        private string dbConnectionString = string.Empty;
        private string baseAddress = "https://simfin.com/api/v1/info/";


        string apiKey = "6sFg1i77TDYXdtBfRb1Ga93lD4XGTuZZ";

        public SimFinDataDownloader(string dbConnectionString)
        {

            this.dbConnectionString = dbConnectionString;
            
        }

        public void ExecuteDaily()
        {
            // string baseAddress = "https://simfin.com/api/v1/info/";
            
            //string apiName = "descriptions";
            //string resourceName = "companies";
            //string fieldName = "primarysymbol";
            //int lastTimeRecordQnt = 0;
            //bool isFirstRun = true;
            //int recordsLimit = 999;
            //int recordsOffset = 0;
            

            Task.Run(async () =>
            {
                await DownloadAllEntitiesAsync();
            });
        }

        public async Task<int> DownloadAllEntitiesAsync()
        {
            int totalQnt = 0;
            string serviceName = "all-entities";
            string httpReqString = $"{baseAddress}{serviceName}?api-key={apiKey}";
            List<SimFinEntity> simFinEntities = await Libs.HttpReqExec.GetAsync<List<SimFinEntity>>(httpReqString);
            with(Si)
            for (int i = 0; i< simFinEntities.Count; i++)
            {

            }


            totalQnt = simFinEntities.Count();
            return totalQnt;
        }
    }
}
