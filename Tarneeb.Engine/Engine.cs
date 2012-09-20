using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Tarneeb.Engine.EventArguments;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine
{
    public class Engine
    {
        #region Members

        private readonly Dictionary<Card, Player> _cardsPlayers = new Dictionary<Card, Player>(52);
        private readonly List<Team> _teams = new List<Team>(2);
        private readonly List<Player> _players = new List<Player>(4);
        private Trio<Bid, Player, Team> _bidPlayerTeam;
        private Round _round;
        private int _biddingPlayerIndex;
        private bool _isDouble;
        private readonly EventHandlerList _eventHandlerList = new EventHandlerList();

        #endregion

        #region Constants

        private const string PlayerJoinedConstant = "PlayerJoined";
        private const string TeamsCompletedConstant = "TeamsCompleted";
        private const string PlayersCompletedConstant = "PlayersCompleted";
        private const string GameSetupCompletedConstant = "GameSetupCompleted";
        private const string BiddingStartedConstant = "BiddingStarted";
        private const string BidCalledConstant = "BidCalled";
        private const string BidEndedConstant = "BidEnded";
        private const string CardPlayedConstant = "CardPlayed";
        private const string RoundEndedConstant = "RoundEnded";
        private const string RoundsEndedConstant = "RoundsEnded";
        private const string GameEndedConstant = "GameEnded";

        #endregion

        #region Events

        public event EventHandler<PlayersInformationEventArgs> PlayerJoined
        {
            add { _eventHandlerList.AddHandler(PlayerJoinedConstant, value); }
            remove { _eventHandlerList.RemoveHandler(PlayerJoinedConstant, value); }
        }

        public event EventHandler<PlayersInformationEventArgs> TeamsCompleted
        {
            add { _eventHandlerList.AddHandler(TeamsCompletedConstant, value); }
            remove { _eventHandlerList.RemoveHandler(TeamsCompletedConstant, value); }
        }

        public event EventHandler<PlayersInformationEventArgs> PlayersCompleted
        {
            add { _eventHandlerList.AddHandler(PlayersCompletedConstant, value); }
            remove { _eventHandlerList.RemoveHandler(PlayersCompletedConstant, value); }
        }

        public event EventHandler<GameSetupCompletedEventArgs> GameSetupCompleted
        {
            add { _eventHandlerList.AddHandler(GameSetupCompletedConstant, value); }
            remove { _eventHandlerList.RemoveHandler(GameSetupCompletedConstant, value); }
        }

        public event EventHandler<BidEventArgs> BiddingStarted
        {
            add { _eventHandlerList.AddHandler(BiddingStartedConstant, value); }
            remove { _eventHandlerList.RemoveHandler(BiddingStartedConstant, value); }
        }

        public event EventHandler<BidEventArgs> BidCalled
        {
            add { _eventHandlerList.AddHandler(BidCalledConstant, value); }
            remove { _eventHandlerList.RemoveHandler(BidCalledConstant, value); }
        }

        public event EventHandler<BidEventArgs> BidEnded
        {
            add { _eventHandlerList.AddHandler(BidEndedConstant, value); }
            remove { _eventHandlerList.RemoveHandler(BidEndedConstant, value); }
        }

        public event EventHandler<CardPlayerArgs> CardPlayed
        {
            add { _eventHandlerList.AddHandler(CardPlayedConstant, value); }
            remove { _eventHandlerList.RemoveHandler(CardPlayedConstant, value); }
        }

        public event EventHandler<CardPlayerArgs> RoundEnded
        {
            add { _eventHandlerList.AddHandler(RoundEndedConstant, value); }
            remove { _eventHandlerList.RemoveHandler(RoundEndedConstant, value); }
        }

        public event EventHandler<ScoreArgs> RoundsEnded
        {
            add { _eventHandlerList.AddHandler(RoundsEndedConstant, value); }
            remove { _eventHandlerList.RemoveHandler(RoundsEndedConstant, value); }
        }

        public event EventHandler<ScoreArgs> GameEnded
        {
            add { _eventHandlerList.AddHandler(GameEndedConstant, value); }
            remove { _eventHandlerList.RemoveHandler(GameEndedConstant, value); }
        }

        #endregion

        #region Public Methods

        public void Join(string playerId, string playerName, string teamName)
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
            var player = new Player(playerId, playerName);
            _players.Add(player);

            if (!team.AddPlayer(player))
            {
                throw new Exception("Maximum number of players per team has been reached!");
            }

            _eventHandlerList[PlayerJoinedConstant].SafelyInvoke(this, new PlayersInformationEventArgs { Teams = _teams });

            if (teamCount == 1 && _teams.Count == 2)
            {
                _eventHandlerList[TeamsCompletedConstant].SafelyInvoke(this,
                                                                       new PlayersInformationEventArgs { Teams = _teams });
            }

            if (_players.Count != 4) return;

            _eventHandlerList[PlayersCompletedConstant].SafelyInvoke(this,
                                                                     new PlayersInformationEventArgs { Teams = _teams });
            ReArrangePlayersSeatting();
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

            _biddingPlayerIndex = (_biddingPlayerIndex + 1) % 4;
            _eventHandlerList[BidCalledConstant].SafelyInvoke(this,
                                                              new BidEventArgs
                                                                  {
                                                                      Bid = bid,
                                                                      Caller = player,
                                                                      NextCaller = _players[_biddingPlayerIndex]
                                                                  });

            if (bid.CallType != CallType.Double &&
                (bid.CallType != CallType.Pass || _players[_biddingPlayerIndex] != _bidPlayerTeam.Second))
                return;
            if (bid.CallType == CallType.Double)
                _isDouble = true;

            _cardsPlayers.Keys.ForEach(c => c.Trump = _bidPlayerTeam.First.Suit);
            var bidArgs = new BidEventArgs
                              {
                                  Bid = _bidPlayerTeam.First,
                                  Caller = _bidPlayerTeam.Second
                              };
            _eventHandlerList[BidEndedConstant].SafelyInvoke(this, bidArgs);
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
            var player = _cardsPlayers[card];
            _eventHandlerList[CardPlayedConstant].SafelyInvoke(this, new CardPlayerArgs
                                                                         {
                                                                             Card = card,
                                                                             Player = player,
                                                                             NextPlayer =
                                                                                 _players[
                                                                                     (_players.IndexOf(player) + 1) % 4]
                                                                         });
            _round.PlayCard(card);
        }

        public Player GetPlayerById(string playerId)
        {
            return _players.FirstOrDefault(p => p.Id == playerId);
        }

        public Card GetCardByRankSuit(int rank, Suit suit)
        {
            return _cardsPlayers.Keys.Single(c => c.Rank == rank && c.Suit == suit);
        }

        #endregion

        #region Private Methods

        private void SetupGame()
        {
            _cardsPlayers.Clear();
            _bidPlayerTeam = null;
            _round = null;
            _biddingPlayerIndex = 0;
            _isDouble = false;
            _players.ForEach(p => p.Score = 0);
            var playerIndex = 0;
            var cardsShuffler = new CardsShuffler();
            var deck = cardsShuffler.GetShuffledDeck();

            for (var i = 0; i < deck.Count; i++)
            {
                if (i != 0 && i % 13 == 0)
                    playerIndex++;
                _cardsPlayers.Add(deck[i], _players[playerIndex]);
            }

            _eventHandlerList[GameSetupCompletedConstant].SafelyInvoke(this,
                                                                       new GameSetupCompletedEventArgs { CardsPlayers = _cardsPlayers });
            _eventHandlerList[BiddingStartedConstant].SafelyInvoke(this,
                                                                   new BidEventArgs { NextCaller = _players[_biddingPlayerIndex] });
        }

        private void OnRoundClosed(Round round)
        {
            var winningCard = round.GetWinningCard();
            var winningPlayer = _cardsPlayers[winningCard];
            var bid = _bidPlayerTeam.First;
            var biddingTeam = _bidPlayerTeam.Third;
            var nonBiddingTeam = GetNoneBiddingTeam(_bidPlayerTeam.Second.Name);

            winningPlayer.Score++;
            _round = null;
            _eventHandlerList[RoundEndedConstant].SafelyInvoke(this, new CardPlayerArgs
                                                                         {
                                                                             Card = winningCard,
                                                                             Player = winningPlayer
                                                                         });

            if (_cardsPlayers.Keys.Any(c => !c.IsPlayed))
                return;

            if (bid.IsBidSatisfied(biddingTeam.PlayersScore))
                biddingTeam.TeamScore += _isDouble
                                             ? biddingTeam.PlayersScore * 2
                                             : biddingTeam.PlayersScore;
            else
            {
                if (!_isDouble)
                {
                    biddingTeam.TeamScore -= bid.TricksRequired;
                    nonBiddingTeam.TeamScore += nonBiddingTeam.PlayersScore;
                }
                else if (_isDouble && nonBiddingTeam.PlayersScore >= bid.TricksRequired)
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

            var scoreArgs = new ScoreArgs
                                {
                                    Teams = _teams
                                };

            if (IsGameScoreLimitReached(biddingTeam) || IsGameScoreLimitReached(nonBiddingTeam))
            {
                _eventHandlerList[GameEndedConstant].SafelyInvoke(this, scoreArgs);
            }
            else
            {
                _eventHandlerList[RoundsEndedConstant].SafelyInvoke(this, scoreArgs);
                SetupGame();
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

        private void ReArrangePlayersSeatting()
        {
            var random = new Random();
            var indexOfFirstTeam = random.Next(0, 2);
            var indexOfFirstPlayer = random.Next(0, 2);
            var indexOfSecondPlayer = random.Next(0, 2);

            _players.Swap(0, _players.IndexOf(_teams[indexOfFirstTeam].Players[indexOfFirstPlayer]));
            _players.Swap(1, _players.IndexOf(_teams[1 - indexOfFirstTeam].Players[indexOfSecondPlayer]));
            _players.Swap(2, _players.IndexOf(_teams[indexOfFirstTeam].Players[1 - indexOfFirstPlayer]));
        }

        private void AssignBidByPlayer(Player player, Bid bid)
        {
            _bidPlayerTeam = new Trio<Bid, Player, Team>
                                 {
                                     First = bid,
                                     Second = player,
                                     Third = _teams.First(t => t.Players.Contains(player))
                                 };
        }

        #endregion
    }
}