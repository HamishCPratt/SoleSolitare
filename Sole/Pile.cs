/*Hamish Pratt 
 * Sole Solitare Game
 * 2017
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sole
{
    class Pile
    {
        public List<Card> faceUp;
        public List<Card> faceDown;
        public List<PictureBox> storedCards;
        public PictureBox toTurn;

        public Pile()
        {
            faceUp = new List<Card>();
            faceDown = new List<Card>();
            storedCards = new List<PictureBox>();
            toTurn = null;
        }

        public Card popFaceDown()
        {
            Card topCard = faceDown[0];
            faceDown.RemoveAt(0);
            return topCard;
        }

        public void revealNext()
        {
            Card next = popFaceDown();
            faceUp.Add(next);
        }

        //Adds a stack of cards held onto its current pile
        public bool addCard(List<Card> addStack)
        {
            Card add = addStack[0];
            //Can place a king on an empty pile
            if (faceUp.Count() == 0 && faceDown.Count() == 0)
            {
                if (add.number == 13)
                {
                    for (int i = 0; i < addStack.Count(); i++)
                    {
                        faceUp.Add(addStack[i]);
                    }
                    return true;
                }
                return false;
            }
            //Make sure the last card is compatible
            else
            {
                int lastLoc = faceUp.Count() - 1;
                Card lastCard = faceUp[lastLoc];
                if (lastCard.colour != add.colour && add.number + 1 == lastCard.number)
                {
                    for (int i = 0; i < addStack.Count(); i++)
                    {
                        faceUp.Add(addStack[i]);
                    }
                    return true;
                }
                return false;
            }
        }
    }
}
