using System;
using System.Collections.Generic;
using System.Linq;
using Tarneeb.Engine.EventArguments;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine
{
    public class GameSession
    {
        #region Members

        private readonly Dictionary<Card, Player> _cardsPlayers = new Dictionary<Card, Player>(52);
        private readonly List<Team> _teams = new List<Team>(2);
        private readonly List<Player> _players = new List<Player>(4);
        private readonly Trio<Bid, Player, Team> _bidPlayerTeam = new Trio<Bid, Player, Team>();
        private Round _round;
        private int _biddingPlayerIndex;

        #endregion

        #region Events

        public event EventHandler<PlayersInformationEventArgs> PlayerJoined;
        public event EventHandler<PlayersInformationEventArgs> TeamsCompleted;
        public event EventHandler<PlayersInformationEventArgs> PlayersCompleted;
        public event EventHandler<GameSetupCompletedEventArgs> GameSetupCompleted;
        public event EventHandler<BidEventArgs> BiddingStarted;
        public event EventHandler<BidEventArgs> BidCalled;
        public event EventHandler<BidEventArgs> BidEnded;
        public event EventHandler<BidEventArgs> PlayStarted;
        public event EventHandler CardPlayed;
        public event EventHandler RoundEnded;
        public event EventHandler GameEnded;

        #endregion

        #region Properties

        public bool IsDoube { get; set; }

        #endregion

        #region Public Methods

        public void Join(string playerName, string teamName)
        {
            var teamCount = _teams.Count;

            if (_teams.All(t => t.Name != teamName))
            {
                if (_teams.Count < 2)
                {
                    _teams.Add(new Team { Name = teamName });
                }
                else
                {
                    throw new Exception("Maximum number of teams has been reached!");
                }
            }

            var team = _teams.First(t => t.Name == teamName);
            var player = new Player(playerName);
            _players.Add(player);

            if (!team.AddPlayer(player))
            {
                throw new Exception("Maximum number of players per team has been reached!");
            }

            SafelyInvokeEvent(PlayerJoined, new PlayersInformationEventArgs { Teams = _teams });

            if (teamCount == 1 && _teams.Count == 2)
            {
                SafelyInvokeEvent(TeamsCompleted, new PlayersInformationEventArgs { Teams = _teams });
            }

            if (_players.Count != 4) return;

            SafelyInvokeEvent(PlayersCompleted, new PlayersInformationEventArgs { Teams = _teams });
            SetupGame();
        }

        public void Bid(Player player, Bid bid)
        {
            if (_bidPlayerTeam == null)
                AssignBidByPlayer(player, bid);
            else if (bid.CallType == CallType.Call && _bidPlayerTeam.First.CompareTo(bid) > 0)
                throw new Exception("Invalid bid is called!");
            else if (bid.CallType == CallType.Call)
                AssignBidByPlayer(player, bid);

            _biddingPlayerIndex++;
            SafelyInvokeEvent(BidCalled,
                              new BidEventArgs
                                  {
                                      Bid = bid,
                                      Caller = player,
                                      NextCaller = _players[_biddingPlayerIndex]
                                  });

            if (bid.CallType != CallType.Double &&
                (bid.CallType != CallType.Pass || _players[_biddingPlayerIndex] != _bidPlayerTeam.Second))
                return;
            var bidArgs = new BidEventArgs
                              {
                                  Bid = _bidPlayerTeam.First,
                                  Caller = _bidPlayerTeam.Second
                              };
            SafelyInvokeEvent(BidEnded, bidArgs);
            SafelyInvokeEvent(PlayStarted, bidArgs);
        }

        public void PlayCard(Card card)
        {
            if (card.IsPlayed)
                throw new InvalidOperationException("This card has been player before.");

            if (_round == null)
            {
                _round = new Round { BaseSuit = card.Suit };
                _round.RoundClosed += OnRoundClosed;
            }

            card.IsPlayed = true;
            _round.PlayCard(card);
        }

        public Card GetCardByRankSuit(int rank, Suit suit)
        {
            return _cardsPlayers.Keys.Single(c => c.Rank == rank && c.Suit == suit);
        }

        #endregion

        #region Private Methods

        private void SafelyInvokeEvent<T>(EventHandler<T> eventHandler, T eventArgs)
            where T : EventArgs
        {
            if (eventHandler != null)
            {
                eventHandler(this, eventArgs);
            }
        }

        private void SetupGame()
        {
            _cardsPlayers.Clear();
            _biddingPlayerIndex = 0;
            var cardsShuffler = new CardsShuffler();
            var deck = cardsShuffler.GetShuffledDeck();
            var playerIndex = 0;

            for (var i = 0; i < deck.Count; i++)
            {
                _cardsPlayers.Add(deck[i], _players[playerIndex]);
                if (i % 13 == 0)
                    playerIndex++;
            }

            SafelyInvokeEvent(GameSetupCompleted, new GameSetupCompletedEventArgs { CardsAndPlayers = _cardsPlayers });
            SafelyInvokeEvent(BiddingStarted, new BidEventArgs { NextCaller = _players[_biddingPlayerIndex] });
        }

        private void OnRoundClosed(Round round)
        {
            var card = round.GetWinningCard();
            var bid = _bidPlayerTeam.First;
            var biddingTeam = _bidPlayerTeam.Third;
            var nonBiddingTeam = GetNoneBiddingTeam(_bidPlayerTeam.Second.Name);

            _cardsPlayers[card].Score++;
            _round = null;

            if (_cardsPlayers.Keys.Any(c => !c.IsPlayed))
                return;

            if (bid.IsBidSatisfied(biddingTeam.PlayersScore))
                biddingTeam.TeamScore += IsDoube
                                             ? biddingTeam.PlayersScore * 2
                                             : biddingTeam.PlayersScore;
            else
            {
                if (!IsDoube)
                {
                    biddingTeam.TeamScore -= bid.TricksRequired;
                    nonBiddingTeam.TeamScore += nonBiddingTeam.PlayersScore;
                }
                else if (IsDoube && nonBiddingTeam.PlayersScore >= bid.TricksRequired)
                {
                    biddingTeam.TeamScore -= bid.TricksRequired * 2;
                    nonBiddingTeam.TeamScore += nonBiddingTeam.PlayersScore * 2;
                }
                else
                {
                    // TODO: buisness logic, asking for up and down score calculation
                    return;
                }
            }

            if (IsGameScoreLimitReached(biddingTeam) || IsGameScoreLimitReached(nonBiddingTeam))
            {

            }
        }

        private Team GetNoneBiddingTeam(string biddingTeamName)
        {
            return _teams[0].Name == biddingTeamName ? _teams[1] : _teams[0];
        }

        private static bool IsGameScoreLimitReached(Team team)
        {
            return team.TeamScore >= 31 || team.TeamScore <= -31;
        }

        private void AssignBidByPlayer(Player player, Bid bid)
        {
            _bidPlayerTeam.First = bid;
            _bidPlayerTeam.Second = player;
            _bidPlayerTeam.Third = _teams.First(t => t.Players.Contains(player));
        }

        #endregion
    }
}
