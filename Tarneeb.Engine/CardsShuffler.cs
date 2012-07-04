using System;
using System.Collections.Generic;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine
{
    public class CardsShuffler
    {
        private IList<Card> _deck;
        private IList<Card> Deck
        {
            get { return _deck ?? (_deck = new List<Card>()); }
        }

        public IList<Card> GetDeck()
        {
            Deck.Clear();
            GenerateDeck();

            return Deck;
        }

        public IList<Card> GetShuffledDeck()
        {
            var random = new Random();
            for (var i = 0; i < Deck.Count; i++)
            {
                var randomIndex = random.Next(0, Deck.Count);
                Deck.Swap(i, randomIndex);
            }

            return Deck;
        }

        private void GenerateDeck()
        {
            for (var i = 1; i < 14; i++)
            {
                for (var j = 1; j < 5; j++)
                {
                    Deck.Add(new Card(i, (Suit)j));
                }
            }
        }
    }
}