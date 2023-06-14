using System;
using System.Net.Http;
using System.Threading.Tasks;
using Lylo.University.Immigration.Models;
using Newtonsoft.Json;

namespace Lylo.University.Immigration.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ExchangeRateService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://v6.exchangerate-api.com/v6/725222c8720cbe47c2028ea9/latest/EUR";

        public ExchangeRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetExchangeRate(string baseCurrency, string targetCurrency)
        {
            string url = $"https://v6.exchangerate-api.com/v6/725222c8720cbe47c2028ea9/latest/{baseCurrency}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();


                var responseBody = await response.Content.ReadAsStringAsync();

                //var exchangeRateResponse = JsonConvert.DeserializeObject<ExchangeRateResponse>(responseBody);

                //if (exchangeRateResponse.Result != "success" || !exchangeRateResponse.ConversionRates.ContainsKey(targetCurrency))
                //{
                //    throw new Exception("Error retrieving exchange rate.");
                //}

                //return exchangeRateResponse.ConversionRates[targetCurrency];

                return responseBody;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error retrieving exchange rate.", ex);
            }
        }
    }

    public class ExchangeRateResponse
    {
        public string Result { get; set; }
        public string Documentation { get; set; }
        public string TermsOfUse { get; set; }
        public long TimeLastUpdateUnix { get; set; }
        public string TimeLastUpdateUtc { get; set; }
        public long TimeNextUpdateUnix { get; set; }
        public string TimeNextUpdateUtc { get; set; }
        public string BaseCode { get; set; }
        public Dictionary<string, decimal> ConversionRates { get; set; }
    }
}
