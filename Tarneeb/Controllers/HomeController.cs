using System.Globalization;
using System.Web.Mvc;
using Tarneeb.Engine;
using Tarneeb.Engine.Models;

namespace TarneebMVC4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var gameSession = new GameSession();
            Player biddingPlayer = null;
            gameSession.PlayerJoined += (sender, args) =>
                                            {

                                            };
            gameSession.TeamsCompleted += (sender, args) =>
                                              {

                                              };
            gameSession.BiddingStarted += (sender, args) =>
                                              {
                                                  biddingPlayer = args.NextCaller;
                                              };
            gameSession.GameSetupCompleted += (sender, args) =>
                                                  {

                                                  };
            gameSession.PlayStarted += (sender, args) =>
                                           {

                                           };
            gameSession.PlayersCompleted += (sender, args) =>
                                                {

                                                };

            gameSession.BidCalled += (sender, args) =>
                                         {
                                             biddingPlayer = args.NextCaller;
                                         };

            gameSession.BidEnded += (sender, args) =>
                                        {

                                        };

            for (var i = 0; i < 4; i++)
            {
                gameSession.Join(i.ToString(CultureInfo.InvariantCulture),
                                 string.Format("Player {0}", i), string.Format("Team {0}", i % 2));
            }

            gameSession.Bid(biddingPlayer, new Bid(CallType.Call, 2, Suit.Diamonds));

            for (var i = 1; i < 4; i++)
            {
                gameSession.Bid(biddingPlayer,
                                new Bid(CallType.Pass));
            }
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
