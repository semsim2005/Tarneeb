using System;
using System.Collections.Generic;

namespace Tarneeb.Models
{
    /// <summary>
    /// Encapsulates the shuffling algorithm
    /// </summary>
    public class CardsShuffler
    {
        private const int NumberOfPlayers = 4;

        /// <summary>
        /// Shuffles the cards and returns list of lists that represent
        /// the hand of each player.
        /// Note: the hand is sorted from lowest card to the highest card and 
        /// suits are separated
        /// </summary>
        public List<List<Card>> Shuffle()
        {
            var shuffledCards = new List<List<Card>>();
            for (var i = 0; i < NumberOfPlayers; i++)
            {
                shuffledCards.Add(new List<Card>());
            }

            List<Card> allCards = AllCards;
            var random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            for (var i = allCards.Count - 1; i >= 1; i--)
            {
                var j = random.Next(0, i);
                var tempCard = allCards[j];
                allCards[j] = allCards[i];
                allCards[i] = tempCard;
            }


            var tempCount = 1;
            var tempPlayer = 1;
            var cardsPerPlayer = (int)(allCards.Count / (float)NumberOfPlayers);
            foreach (var card in allCards)
            {
                if (tempCount > cardsPerPlayer)
                {
                    tempCount = 1;
                    tempPlayer++;
                }

                shuffledCards[tempPlayer - 1].Add(card);

                tempCount++;
            }

            shuffledCards.ForEach(playerCards => playerCards.Sort());

            return shuffledCards;
        }

        /// <summary>
        /// Generates the un-shuffled cards
        /// </summary>
        private static List<Card> AllCards
        {
            get
            {
                var allCards = new List<Card>();
                for (var i = (int)Suit.Spades; i <= (int)Suit.Clubs; i++)
                {
                    for (int j = 1; j <= 13; j++)
                    {
                        var card = new Card(j, (Suit)i);
                        allCards.Add(card);
                    }
                }

                return allCards;
            }
        }
    }
}