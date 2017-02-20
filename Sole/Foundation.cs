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
    class Foundation
    {
        public List<Card> placed;
        public string suit;
        public bool filled;

        public Foundation(string inSuit)
        {
            placed = new List<Card>();
            suit = inSuit;
            filled = false;
        }

        //Adds a single card to the foundation if it is next in the order
        public bool addCard(Card add)
        {
            if (add.suit == suit)
            {
                if (placed.Count() == 0)
                {
                    if (add.number == 1)
                    {
                        placed.Add(add);
                        return true;
                    }
                    return false;
                }
                else
                {
                    int lastLoc = placed.Count() - 1;
                    if (add.number == placed[lastLoc].number + 1)
                    {
                        placed.Add(add);
                        if (add.number == 13)
                        {
                            filled = true;
                        }
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
    }
}
