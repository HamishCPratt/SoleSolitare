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
    class Card
    {
        public int number { get; set; }
        public string colour { get; set; }
        public string suit { get; set; }
        public string path { get; set; }

        public Card(int inNumber, string inColour, string inSuit, string inPath)
        {
            number = inNumber;
            colour = inColour;
            suit = inSuit;
            path = inPath;
        }

        public Card(int inNumber, string inColour, string inSuit)
        {
            number = inNumber;
            colour = inColour;
            suit = inSuit;
        }

        public Card() { }
    }
}
