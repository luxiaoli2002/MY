using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MY.Web.Models;
using Baidu.Aip;

namespace MY.Web.Controllers
{
    public class HomeController : Controller
    {
         string APP_ID = "16162924";
        string API_KEY = "0HGntNQWcLernoX1QA2ILFcA";
        string SECRET_KEY = "ugNveTHaBPInxv2t0304COOtlIKQ7rIH";
        Baidu.Aip.Face.Face client;

        public HomeController(){
            client = new Baidu.Aip.Face.Face(API_KEY,SECRET_KEY);
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
