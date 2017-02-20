/*Hamish Pratt 
 * Sole Solitare Game
 * 2017
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sole
{
    class Deck
    {
        public List<Card> deal;
        public List<Card> waste;
        public Card revealed;

        public Deck()
        {
            deal = new List<Card>();
            waste = new List<Card>();
            revealed = null;
        }

        //Create all cards for the deck
        public void createDeck()
        {
            string[] colourList = new string[2] { "red", "black" };
            int[] numberList = new int[13] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            string[] suitList = new string[4] { "hearts", "diamonds", "spades", "clubs" };

            for (int numberCount = 0; numberCount < 13; numberCount++)
            {
                for (int suitCount = 0; suitCount < 4; suitCount++)
                {
                    string colour;
                    if (suitCount < 2)
                    {
                        colour = "red";
                    }
                    else
                    {
                        colour = "black";
                    }
                    deal.Add(new Card(numberList[numberCount], colour, suitList[suitCount]));
                }
            }
        }

        //Fisher–Yates shuffle method
        public void shuffleDeck()
        {
            Random rnd = new Random();
            for (int i = deal.Count - 1; i > 0; i--)
            {
                int pos = rnd.Next(0, (i + 1));
                Card temp = deal[pos];
                deal[pos] = deal[i];
                deal[i] = temp;
            }
        }
        public List<Card> draw()
        {
            int drawn = 0;
            List<Card> hand = new List<Card>();
            while (drawn < 3 && deal.Count() != 0)
            {
                hand.Add(deal[0]);
                deal.RemoveAt(0);
                drawn++;
            }
            return hand;
        }

        public Card handOff()
        {
            Card next = deal[0];
            deal.RemoveAt(0);
            return next;
        }

        //When all cards in the deck have been drawn
        //Waste is reused
        public void reloadDeck()
        {
            deal = waste;
            waste = new List<Card>();
            revealed = null;
        }

        //Cards with suit dump have been used up
        //Rest go into waste to be reused
        public void dumpHand(List<Card> hand)
        {
            for (int i = 0; i < hand.Count(); i++)
            {
                if (hand[i].suit != "dump")
                {
                    waste.Add(hand[i]);
                }
            }
        }
    }
}
