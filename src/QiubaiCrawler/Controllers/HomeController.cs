using System;
using Microsoft.AspNetCore.Mvc;
using QiubaiCrawler.Services;

namespace QiubaiCrawler.Controllers
{
    public class HomeController : Controller
    {
        private QiubaiCrawlerService _qiubaiCrawlerService;
        public HomeController(QiubaiCrawlerService qiubaiCrawlerService)
        {
            _qiubaiCrawlerService = qiubaiCrawlerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Crawler()
        {

            return View();
        }
    }
}