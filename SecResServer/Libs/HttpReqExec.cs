using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SecResServer.Libs
{
    public class HttpReqExec
    {
        public static async Task<T> GetAsync<T>(string uri)
        {
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

    }
}
