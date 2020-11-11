using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Intercom_JsonRead.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;

namespace Intercom_JsonRead.Controllers
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
            string url = "https://s3.amazonaws.com/intercom-take-home-test/customers.txt";
            double OfficeLatitude = 53.339428;
            double Officelongitude = -6.257664;

            var customer = LoadJson(url).Result;

            List<Customer> guest = new List<Customer>();

            foreach (Customer c in customer)
            {
                double _distance = distance(OfficeLatitude, Officelongitude, c.latitude, c.longitude, 'K');
                if (_distance <= 100)
                {
                    c.distance = Math.Round(_distance, 2);
                    guest.Add(c);
                }

            }
            guest = guest.OrderBy(q => q.user_id).ToList();
            return View(guest);
        }


        /// <summary>
        /// Read the Json from URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<List<Customer>> LoadJson(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        string mycontent = content.ReadAsStringAsync().Result;

                        List<Customer> customers = new List<Customer>();
                        using (StreamReader r = new StreamReader(content.ReadAsStreamAsync().Result))
                        {
                            string json = r.ReadToEnd();
                            var jsonReader = new JsonTextReader(new StringReader(json))
                            {
                                SupportMultipleContent = true
                            };
                            var jsonSerializer = new JsonSerializer();
                            while (jsonReader.Read())
                            {
                                customers.Add(jsonSerializer.Deserialize<Customer>(jsonReader));
                            }

                        }

                        return customers;
                    }
                }
            }



        }

        /// <summary>
        /// Convert Degree to Radian
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        /// <summary>
        /// convert radians to decimal degrees
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }


        /// <summary>
        /// Calculate the distance 
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        private double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }

                return (dist);
            }
        }



    }
}
