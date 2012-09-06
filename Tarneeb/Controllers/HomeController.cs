using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Tarneeb.Engine;
using Tarneeb.Engine.Models;

namespace TarneebMVC4.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var gameSession = new Engine();
            Player biddingPlayer = null;
            Dictionary<Card, Player> cardsPlayers = null;
            var newRound = true;
            var baseSuit = Suit.NoTrump;

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
                                                      cardsPlayers = args.CardsPlayers;
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
                                            biddingPlayer = args.Caller;
                                        };
            gameSession.CardPlayed += (sender, args) =>
                                          {
                                              biddingPlayer = args.NextPlayer;
                                          };
            gameSession.RoundEnded += (sender, args) =>
                                          {
                                              newRound = true;
                                          };

            gameSession.RoundsEnded += (sender, args) =>
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

            var random = new Random();
            do
            {
                Card card = null;
                if (newRound)
                {
                    var cards = cardsPlayers.Where(kv => !kv.Key.IsPlayed &&
                                                         kv.Value.Id == biddingPlayer.Id)
                        .Select(kv => kv.Key).ToList();
                    card = cards[random.Next(0, cards.Count)];
                    newRound = false;
                    baseSuit = card.Suit;
                }
                else
                {
                    if (cardsPlayers.Any(kv => !kv.Key.IsPlayed &&
                                               kv.Value.Id == biddingPlayer.Id &&
                                               kv.Key.Suit == baseSuit))
                    {
                        var cards = cardsPlayers.Where(kv => !kv.Key.IsPlayed &&
                                                             kv.Value.Id == biddingPlayer.Id &&
                                                             kv.Key.Suit == baseSuit).
                            Select(kv => kv.Key).ToList();
                        card = cards[random.Next(0, cards.Count)];
                    }
                    else
                    {
                        var cards = cardsPlayers.Where(kv => !kv.Key.IsPlayed &&
                                                             kv.Value.Id == biddingPlayer.Id)
                            .Select(kv => kv.Key).ToList();
                        card = cards[random.Next(0, cards.Count)];
                    }

                    gameSession.PlayCard(card);
                }
            } while (cardsPlayers.Keys.Any(c => !c.IsPlayed));

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