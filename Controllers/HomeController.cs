using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCJpgKeywords.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult  Index()
        {
//return "Hello World! (from jpg_keywords) Title: " + MyClasses.MyAppSettings.PageTitlePrefix;
            ViewBag.Title = MyClasses.MyAppSettings.PageTitlePrefix + " - Index";
            return View();
        }

    }
}
