using System;
using System.Collections.Generic;
using Tarneeb.Engine.Models;

namespace Tarneeb.Engine
{
    public class CardsShuffler
    {
        private readonly IList<Card> _deck = new List<Card>();

        public IList<Card> GetDeck()
        {
            GenerateDeck();

            return _deck;
        }

        public IList<Card> GetShuffledDeck()
        {
            GenerateDeck();

            var random = new Random();
            for (var i = 0; i < _deck.Count; i++)
            {
                var randomIndex = random.Next(0, _deck.Count);
                _deck.Swap(i, randomIndex);
            }

            return _deck;
        }

        private void GenerateDeck()
        {
            _deck.Clear();

            for (var i = 1; i < 14; i++)
            {
                for (var j = 1; j < 5; j++)
                {
                    _deck.Add(new Card(i, (Suit)j));
                }
            }
        }
    }
}