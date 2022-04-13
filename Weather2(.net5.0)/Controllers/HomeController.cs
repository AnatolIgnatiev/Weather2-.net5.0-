using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Weather2_.net5._0_.Models;

namespace Weather2_.net5._0_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Weather()
        {
            return View();
        }

        public IActionResult GitHubSearch()
        {
            return View();
        }
        [HttpPost]
        public List<string> GetGitHubSearchResults(string query, int pageNumber, int resultsPerPage)
        {
            string accessToken = "Token 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"; //expiers at May10
            string url = string.Format("https://api.github.com/search/repositories?q={0}&page={1}&per_page={2}", query, pageNumber, resultsPerPage); //+"{&page,per_page,sort,order}"

            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "AnatolIgnatiev");
                client.Headers.Add("Authorization", accessToken);
                string json = client.DownloadString(url);

                Root dataFound = JsonSerializer.Deserialize<Root>(json);

                List<string> resultJsonStrings = new List<string>();
                foreach (var item in dataFound.items)
                {
                    if (string.IsNullOrEmpty(item.description))
                    {
                        item.description = "No description";
                    }
                    resultJsonStrings.Add(JsonSerializer.Serialize(new GitHubResultViewModel 
                    { 
                        name = item.name, 
                        description = Regex.Replace(item.description, @"[^\u0000-\u007F]+", string.Empty),
                        html_url = item.html_url,
                        updated_at = item.updated_at.ToShortDateString(),
                        language = item.language,
                        totalResultsCount = dataFound.total_count
                    }));
                }
                
                return resultJsonStrings;
            }
        }
        [HttpPost]
        public String WeatherDetail(string City)
        {
            string apiKey = "c9d4ccfe809da00bc96e8e43a677d056";

            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&cnt=1&APPID={1}", City, apiKey);

            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(url);
 
                RootObject weatherInfo = JsonSerializer.Deserialize<RootObject>(json);

                ResultViewModel result = new ResultViewModel();

                result.Country = weatherInfo.sys.country;
                result.City = weatherInfo.name;
                //result.Latitute = Convert.ToString(weatherInfo.coordinates.latitute);
                //result.Longitude = Convert.ToString(weatherInfo.coordinates.longitude);
                result.Description = weatherInfo.weather[0].description;
                result.Humidity = Convert.ToString(weatherInfo.main.humidity);
                result.Temp = Convert.ToString(weatherInfo.main.temp);
                result.TempFeelsLike = Convert.ToString(weatherInfo.main.feels_like);
                result.TempMax = Convert.ToString(weatherInfo.main.temp_max);
                result.TempMin = Convert.ToString(weatherInfo.main.temp_min);
                result.WeatherIcon = weatherInfo.weather[0].icon;

                var resultJsonString = JsonSerializer.Serialize(result);

                return resultJsonString;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
