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
        private Trio<Bid, Player, Team> _bidPlayerTeam;
        private Round _round;
        private int _biddingPlayerIndex;
        private bool _isDouble;

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
        public event EventHandler<CardPlayerArgs> CardPlayed;
        public event EventHandler<CardPlayerArgs> RoundEnded;
        public event EventHandler<ScoreArgs> RoundsEnded;
        public event EventHandler<ScoreArgs> GameEnded;

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

            PlayerJoined.SafelyInvoke(this, new PlayersInformationEventArgs { Teams = _teams });

            if (teamCount == 1 && _teams.Count == 2)
            {
                TeamsCompleted.SafelyInvoke(this, new PlayersInformationEventArgs { Teams = _teams });
            }

            if (_players.Count != 4) return;

            PlayersCompleted.SafelyInvoke(this, new PlayersInformationEventArgs { Teams = _teams });
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
            BidCalled.SafelyInvoke(this,
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
            BidEnded.SafelyInvoke(this, bidArgs);
            PlayStarted.SafelyInvoke(this, bidArgs);
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
            CardPlayed.SafelyInvoke(this, new CardPlayerArgs
                                              {
                                                  Card = card,
                                                  Player = _cardsPlayers[card]
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

            ReArrangePlayersSeatting();
            for (var i = 0; i < deck.Count; i++)
            {
                if (i != 0 && i % 13 == 0)
                    playerIndex++;
                _cardsPlayers.Add(deck[i], _players[playerIndex]);
            }

            GameSetupCompleted.SafelyInvoke(this, new GameSetupCompletedEventArgs { CardsAndPlayers = _cardsPlayers });
            BiddingStarted.SafelyInvoke(this, new BidEventArgs { NextCaller = _players[_biddingPlayerIndex] });
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
            RoundEnded.SafelyInvoke(this, new CardPlayerArgs
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
                GameEnded.SafelyInvoke(this, scoreArgs);
            }
            else
            {
                RoundsEnded.SafelyInvoke(this, scoreArgs);
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