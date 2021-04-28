using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ImgCompress.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ImgCompress.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            var html = "";
            var file = new FileInfo(webRootPath + "/file/123.jpg");
            using (var d = file.OpenRead())
            {
                using (var ms = new MemoryStream())
                {
                    html += "<br/>Start Resize:"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                    ImageHelper.resizeImage("123.jpg", d, ms, new System.Drawing.Size(3000, 3000));
                    html += "<br/>3000 Resize Complete:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                    ImageHelper.resizeImage("123.jpg", d, ms, new System.Drawing.Size(2000, 2000));
                    html += "<br/>2000 Resize Complete:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                    ImageHelper.resizeImage("123.jpg", d, ms, new System.Drawing.Size(400, 400));
                    html += "<br/>400 Resize Complete:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
                }
            };

            return View(new HtmlDTO { html = html });
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

    public class HtmlDTO { 
    
        public string html { set; get; }
    }
}
