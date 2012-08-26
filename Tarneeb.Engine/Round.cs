using System.Collections.Generic;
using System.Linq;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine
{
    public class Round
    {
        public Suit BaseSuit { get; set; }
        private readonly IList<Card> _cards = new List<Card>();
        public delegate void RoundClosedDelegate(Round round);
        public event RoundClosedDelegate RoundClosed;

        public void PlayCard(Card card)
        {
            card.IsPlayed = true;
            _cards.Add(card);
            if (_cards.Count == 4 && RoundClosed != null)
                RoundClosed(this);
        }

        public Card GetWinningCard()
        {
            var maxWeight = _cards.Where(c => c.Suit == BaseSuit || c.Suit == c.Trump).Max(c => c.Weight);
            return _cards.Single(c => c.Weight == maxWeight);
        }
    }
}
