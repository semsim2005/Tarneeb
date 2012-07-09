
using System;
using System.Collections.Generic;
using System.Linq;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine
{
    public class Game
    {
        private int _roundNumber;
        private readonly Dictionary<Card, Player> _cardsPlayers = new Dictionary<Card, Player>();
        private KeyValuePair<Bid, Team> _bidTeam;
        private Round _round;
        public string Id { get; set; }

        public Game()
        {
            ResetGame();
        }

        public void PlayCard(Card card)
        {
            if (card.IsPlayed)
                throw new InvalidOperationException("This card has been player before.");

            if (_round == null)
            {
                _roundNumber++;
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

        private void ResetGame()
        {
            _cardsPlayers.Clear();
            _roundNumber = 0;
        }

        private void OnRoundClosed(Round round)
        {
            var card = round.GetWinningCard();
            _cardsPlayers[card].Score++;
            _round = null;

            if (_roundNumber < 13)
                return;

            // todo: buisness logic to calculate scores and check whether >= 31
            var isBidSatisfied = _bidTeam.Key.IsBidSatisfied(_bidTeam.Value.PlayersScore);
        }
    }
}
