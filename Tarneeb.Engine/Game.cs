
using System;
using System.Collections.Generic;
using System.Linq;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine
{
    public class Game
    {
        private readonly Dictionary<Card, Player> _cardsPlayers = new Dictionary<Card, Player>();
        private Pair<Bid, Team> _bidTeam;
        private Team _firstTeam;
        private Team _secondTeam;
        private Round _round;
        public bool IsDoube { get; set; }
        public string Id { get; set; }

        public void PlayCard(Card card)
        {
            if (card.IsPlayed)
                throw new InvalidOperationException("This card has been player before.");

            if (_round == null)
            {
                _round = new Round { BaseSuit = card.Suit };
                _round.RoundClosed += OnRoundClosed;
            }

            _round.PlayCard(card);
        }

        public Card GetCardByRankSuit(int rank, Suit suit)
        {
            return _cardsPlayers.Keys.Single(c => c.Rank == rank && c.Suit == suit);
        }

        private void ResetGame()
        {
            _cardsPlayers.Clear();
            _bidTeam = null;
            IsDoube = false;
        }

        private void OnRoundClosed(Round round)
        {
            var card = round.GetWinningCard();
            var bid = _bidTeam.First;
            var biddingTeam = _bidTeam.Second;
            var nonBiddingTeam = GetNoneBiddingTeam(_bidTeam.Second.Name);

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

            if (IsGameScoreLimitReached(_firstTeam) || IsGameScoreLimitReached(_secondTeam))
            {

            }
        }

        private static bool IsGameScoreLimitReached(Team team)
        {
            return team.TeamScore >= 31 || team.TeamScore <= -31;
        }

        private Team GetNoneBiddingTeam(string biddingTeamName)
        {
            return _firstTeam.Name == biddingTeamName ? _secondTeam : _firstTeam;
        }

        private Team Add(string name)
        {
            if (team == null)
            {
                team = new Team { Name = name };
                _teams.Add(team);
            }

            return team;
        }

        public void AddPlayerToTeam(string teamName, Player player)
        {
            var team = Add(teamName);
            team.AddPlayer(player);
        }
    }
}
