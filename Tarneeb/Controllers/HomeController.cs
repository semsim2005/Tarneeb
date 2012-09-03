using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tarneeb.Engine;

namespace TarneebMVC4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var gameSession = new GameSession();
            gameSession.PlayerJoined += (sender, args) =>
                                            {
                                                args.Teams.Clear();
                                            };
            gameSession.Join("Tamer", "First Team");
            gameSession.Join("Tamer 1", "First Team");
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }

        public ActionResult SignalR()
        {
            ViewBag.Message = "Your quintessential contact page.";

            return View();
        }
    }
}
