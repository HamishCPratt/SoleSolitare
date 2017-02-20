/*Hamish Pratt 
 * Sole Solitare Game
 * 2017
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sole
{
    public partial class Form1 : Form
    {

        Deck startDeck;
        List<Pile> piles;
        List<int> numPerPile;
        List<Foundation> foundations;
        bool carrying;
        bool carriedFromHand;
        List<Card> hand;
        PictureBox heldFromHand;

        List<Card> held;
        Panel heldFromPiles;

        List<Panel> panelPiles;
        int panelLoc;
        int cardLoc;

        public Form1()
        {
            InitializeComponent();
            setup();
        }

        private void setup()
        {

            carrying = false;
            carriedFromHand = false;
            startDeck = new Deck();
            hand = new List<Card>();
            panelLoc = 0;
            cardLoc = 0;
            held = null;
            heldFromHand = null;
            held = new List<Card>();
            heldFromPiles = null;
            panelPiles = poolPanels();
            numPerPile = generateNumPiles();
            piles = createPiles();
            foundations = createFoundations();
            startDeck.createDeck();
            startDeck.shuffleDeck();
            dealTable();
            colourDeck();
        }

        //Creates the piles
        private List<Pile> createPiles()
        {
            List<Pile> piles = new List<Pile>();
            for (int i = 0; i < 7; i++)
            {
                piles.Add(new Pile());
            }
            return piles;
        }

        //Creates the foundations
        private List<Foundation> createFoundations()
        {
            List<Foundation> foundations = new List<Foundation>();
            string[] suitList = new string[4] { "hearts", "diamonds", "spades", "clubs" };
            for (int i = 0; i < 4; i++)
            {
                foundations.Add(new Foundation(suitList[i]));
            }
            return foundations;
        }

        //Puts the cards in the piles
        private void dealTable()
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    piles[i].faceDown.Add(startDeck.handOff());
                }
                piles[i].faceUp.Add(startDeck.handOff());
            }
        }

        //Initalizes the panel vector of panels on the screen
        private List<Panel> poolPanels()
        {
            List<Panel> temp = new List<Panel>();
            temp.Add(pileOne);
            temp.Add(pileTwo);
            temp.Add(pileThree);
            temp.Add(pileFour);
            temp.Add(pileFive);
            temp.Add(pileSix);
            temp.Add(pileSeven);
            return temp;
        }

        //Number of cards in each panel for overlay purposes
        //Facedown cards count as only one card as they are all stacked on themselves 
        private List<int> generateNumPiles()
        {
            List<int> temp = new List<int>();
            
            for (int i = 0; i < 7; i++)
            {
                //First pile only consists of the one faceup card 
                if (i == 0)
                {
                    temp.Add(1);
                }
                else
                {
                    temp.Add(2);
                }
                
            }
            return temp;
        }

        //Sets start up images and adds click events to the board 
        private void colourDeck()
        {

            updateImage(deckBacking, @"\cards\cardBacking.png");

            drawnCardOne.Visible = false;
            drawnCardTwo.Visible = false;
            drawnCardThree.Visible = false;

            drawnCardOne.Refresh();
            drawnCardTwo.Refresh();
            drawnCardThree.Refresh();

            drawnCardOne.Click += new EventHandler(cardHandClick);
            drawnCardTwo.Click += new EventHandler(cardHandClick);
            drawnCardThree.Click += new EventHandler(cardHandClick);

            foundationClubs.Click += new EventHandler(foundationClick);
            foundationSpades.Click += new EventHandler(foundationClick);
            foundationHearts.Click += new EventHandler(foundationClick);
            foundationDiamonds.Click += new EventHandler(foundationClick);

            updateImage(foundationHearts, @"\cards\heartFound.png");
            updateImage(foundationDiamonds, @"\cards\diamondFound.png");
            updateImage(foundationSpades, @"\cards\clubFound.png");
            updateImage(foundationClubs, @"\cards\spadeFound.png");

            dropButton.BackColor = Color.Red;
            colourPiles();

        }

        //Sets the backing images for the piles and dynamically adds their click events. 
        private void colourPiles()
        {
           for (int i = 0; i < 7; i++)
            {
                PictureBox temp = new PictureBox();
                temp.Click += new EventHandler(pilecardHandClick);
                if (piles[i].faceDown.Count() != 0)
                {
                    temp.Top += 20;

                    PictureBox backFill = new PictureBox();
                    backFill.Width = pileOne.Width;
                    backFill.Height = 89;
                    updateImage(backFill, @"\cards\cardBacking.png");
                    backFill.SizeMode = PictureBoxSizeMode.StretchImage;
                    backFill.Click += new EventHandler(newCardTurnOver);
                    panelPiles[i].Controls.Add(backFill);
                    piles[i].toTurn = backFill;
                }
                temp.Width = pileOne.Width;
                temp.Height = 89;
                string details = extractCard(piles[i].faceUp[0]);
                updateImage(temp, details);
                panelPiles[i].Controls.Add(temp);
                panelPiles[i].Controls.SetChildIndex(temp,0);
                piles[i].storedCards.Add(temp);
            }
            pileOne.Click += new EventHandler(emptySlot);
            pileTwo.Click += new EventHandler(emptySlot);
            pileThree.Click += new EventHandler(emptySlot);
            pileFour.Click += new EventHandler(emptySlot);
            pileFive.Click += new EventHandler(emptySlot);
            pileSix.Click += new EventHandler(emptySlot);
            pileSeven.Click += new EventHandler(emptySlot);
        }

        //Clicking on an empty panel
        //The 'top' card must be a king
        //But can move all base cards along
        private void emptySlot(object sender, EventArgs e)
        {
            if (carrying)
            {
                Panel targetPanel = sender as Panel;
                int targetPanelLoc = 0;
                //Find the pile representation from the panels 
                for (int i = 0; i < 7; i++)
                {
                    if (targetPanel == panelPiles[i])
                    {
                        targetPanelLoc = i;
                    }
                }

                //Successful - add images to this panel and remove them from the previous;
                //First card must be a king
                if (piles[targetPanelLoc].addCard(held))
                {
                    //Place new stack onto the pile and add images
                    for (int i = 0; i < held.Count(); i++)
                    {
                        string details = extractCard(held[i]);
                        PictureBox newCard = newCardImage(details, numPerPile[targetPanelLoc]);
                        numPerPile[targetPanelLoc]++;
                        panelPiles[targetPanelLoc].Controls.Add(newCard);
                        panelPiles[targetPanelLoc].Controls.SetChildIndex(newCard, 0);
                        piles[targetPanelLoc].storedCards.Add(newCard);
                    }
                    //Delete the images and the cards from their origin 
                    if (carriedFromHand)
                    {
                        deleteHandImage();
                    }
                    else
                    {
                        deletePileImage();
                    }

                }
                cleanHand();
            }
        }

        //Cleans the held cards when an incorrect placement occurs
        private void cleanHand()
        {
            carrying = false;
            carriedFromHand = false;
            held = new List<Card>();
            heldFromPiles = null;
            panelLoc = 0;
            cardLoc = 0;
        }

        //Turning over a new card on the pile 
        private void newCardTurnOver(object sender, EventArgs e)
        {
            carrying = false;
            PictureBox template = sender as PictureBox;
            Panel targetPanel = (Panel)template.Parent;
            int targetPanelLoc = 0;
            //Find the pile representation from the panels 
            for (int i = 0; i < 7; i++)
            {
                if (targetPanel == panelPiles[i])
                {
                    targetPanelLoc = i;
                }
            }
            if (piles[targetPanelLoc].faceUp.Count() == 0)
            {
                //Last facedown card is being flipped up
                //Need to get rid of facedown card images
                if (piles[targetPanelLoc].faceDown.Count() == 1)
                {
                    //Remove old card and create new card in its place
                    piles[targetPanelLoc].faceUp.Add(piles[targetPanelLoc].faceDown[0]);
                    string details = extractCard(piles[targetPanelLoc].faceUp[0]);
                    piles[targetPanelLoc].faceDown.RemoveAt(0);
                    targetPanel.Controls.Remove(template);
                    PictureBox newCardGenerated = new PictureBox();
                    newCardGenerated.Width = pileOne.Width;
                    newCardGenerated.Height = 89;
                    newCardGenerated.Click += new EventHandler(pilecardHandClick);
                    updateImage(newCardGenerated, details);
                    piles[targetPanelLoc].storedCards.Add(newCardGenerated);
                    panelPiles[targetPanelLoc].Controls.Add(newCardGenerated);
                    panelPiles[targetPanelLoc].Controls.SetChildIndex(newCardGenerated, 0);
                }
                //Don't need to remove facedown image
                else
                {
                    //Add new card on stack
                    PictureBox newCardGenerated = new PictureBox();
                    newCardGenerated.Width = pileOne.Width;
                    newCardGenerated.Height = 89;
                    string details = extractCard(piles[targetPanelLoc].faceDown[piles[targetPanelLoc].faceDown.Count()-1]);
                    newCardGenerated.Click += new EventHandler(pilecardHandClick);
                    updateImage(newCardGenerated, details);
                    newCardGenerated.Top += 20;
                    panelPiles[targetPanelLoc].Controls.Add(newCardGenerated);
                    panelPiles[targetPanelLoc].Controls.SetChildIndex(newCardGenerated, 0);
                    piles[targetPanelLoc].storedCards.Add(newCardGenerated);
                    int finalLoc = piles[targetPanelLoc].faceDown.Count() - 1;
                    piles[targetPanelLoc].faceUp.Add(piles[targetPanelLoc].faceDown[finalLoc]);
                    piles[targetPanelLoc].faceDown.RemoveAt(finalLoc);
                    numPerPile[targetPanelLoc]++;
                }
            }
            if (isAutoReady())
            {
                autoCompleteButton.Text = " Auto Complete Ready";
            }

        }

        //Generating a new card to be placed onto a pile
        private PictureBox newCardImage(string path, int numBefore)
        {
            PictureBox temp = new PictureBox();
            temp.Click += new EventHandler(pilecardHandClick);
            temp.Width = pileOne.Width;
            temp.Height = 89;
            temp.Top = numBefore * (20);
            updateImage(temp, path);
            return temp;
        }

        //Clicking any card on one of the piles on the board
        private void pilecardHandClick(object sender, EventArgs e)
        {
            //Already holding a set of cards
            //Aiming to place cards on another pile
            if (carrying)
            {
                PictureBox template = sender as PictureBox;
                Panel targetPanel = (Panel)template.Parent;
                int targetPanelLoc = 0;
                for (int i = 0; i < 7; i++)
                {
                    if (targetPanel == panelPiles[i])
                    {
                        targetPanelLoc = i;
                    }
                }

                //Successful - add images to this panel and remove them from the previous
                if (piles[targetPanelLoc].addCard(held))
                {
                    for (int i = 0; i < held.Count(); i++)
                    {
                        string details = extractCard(held[i]);
                        PictureBox newCard = newCardImage(details, numPerPile[targetPanelLoc]);
                        numPerPile[targetPanelLoc]++;
                        panelPiles[targetPanelLoc].Controls.Add(newCard);
                        panelPiles[targetPanelLoc].Controls.SetChildIndex(newCard, 0);
                        piles[targetPanelLoc].storedCards.Add(newCard);
                        
                    }
                    if (carriedFromHand)
                    {
                        deleteHandImage();
                    }
                    else
                    {
                        deletePileImage();
                    }
                    
                }
                cleanHand();
                if (isAutoReady())
                {
                    autoCompleteButton.Text = " Auto Complete Ready";
                }
            }
            //No cards held already
            //Selecting to hold the set of cards
            else
            {
                carrying = true;
                PictureBox template = sender as PictureBox;
                heldFromPiles =  (Panel)template.Parent;

                //Find the pile selected
                for (int i = 0; i < 7; i++)
                {
                    if (heldFromPiles == panelPiles[i])
                    {
                        panelLoc = i;
                    }
                }

                //Find the card selected
                for (int k = 0; k < piles[panelLoc].storedCards.Count(); k++)
                {
                    if (template == piles[panelLoc].storedCards[k])
                    {
                        cardLoc = k;
                    }
                }
                //Add all the cards "lower" than the card selected
                for (int i = cardLoc; i < piles[panelLoc].faceUp.Count(); i++)
                {
                    held.Add(piles[panelLoc].faceUp[i]);
                }
            }
            updateDropColour();
        }
        
        //Checks the winning conidition of all kings being placed
        public bool allFilled()
        {
            for (int i = 0; i < foundations.Count(); i++)
            {
                if (!foundations[i].filled)
                {
                    return false;
                }
            }
            return true;
        }

        //Deal a set of new cards from the pile
        private void deckBacking_Click(object sender, EventArgs e)
        {
            if (hand.Count() != 0)
            {
                startDeck.dumpHand(hand);
            }
            if (startDeck.deal.Count() == 0)
            {
                startDeck.reloadDeck();
            }

            hand = startDeck.draw();
            drawnCardOne.Visible = false;
            drawnCardTwo.Visible = false;
            drawnCardThree.Visible = false;
            drawnCardOne.Refresh();
            drawnCardTwo.Refresh();
            drawnCardThree.Refresh();
            for (int i = 0; i < hand.Count(); i++)
            {
                PictureBox template;
                if (i == 0)
                {
                   template = drawnCardOne;
                }
                else if ( i == 1)
                {
                    template = drawnCardTwo;
                }
                else
                {
                    template = drawnCardThree;
                }
                string cardPath = extractCard(hand[i]);

                updateImage(template, cardPath);

            }
            updateDropColour();

        }

        //Gives a picture box a card images
        private void updateImage(PictureBox template, string cardPath)
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            string fullPath = path + @"\Resources";
            template.Image = Image.FromFile
                               (fullPath + cardPath);
            template.Refresh();
            template.SizeMode = PictureBoxSizeMode.StretchImage;
            template.Visible = true;
            template.Refresh();
        }

        //Gets the information of a card in the form of string path
        private string extractCard(Card card)
        {
            string detailNumber = card.number.ToString();
            if (detailNumber == "11")
            {
                detailNumber = "jack";
            }
            else if (detailNumber == "12")
            {
                detailNumber = "queen";
            }
            else if (detailNumber == "13")
            {
                detailNumber = "king";
            }
            else if (detailNumber == "1")
            {
                detailNumber = "ace";
            }
            string detail = @"\cards\" + detailNumber + "_of_" + card.suit + ".png";
            return detail;
        }

        //Allows autocomplete
        //All the cards must be out on the table face up or on the foundations
        private bool isAutoReady()
        {
            for (int i = 0; i < hand.Count(); i++)
            {
                if (hand[i].suit != "dump")
                {
                    return false;
                }
            }
            if (startDeck.deal.Count() != 0 || startDeck.waste.Count() != 0)
            {
                return false;
            }

            for (int i = 0; i < piles.Count(); i++)
            {
                if (piles[i].faceDown.Count != 0)
                {
                    return false;
                }
            }
            return true;
        }

        //Clicking a card from the dealt hand
        //Can only select the left-most card 
        private void cardHandClick(object sender, EventArgs e)
        {
            PictureBox template = sender as PictureBox;
            if (!carrying && template != null)
            {
                int pos;
                if (template.Visible == true)
                {
                    if (template.Name == "drawnCardOne")
                    {
                        pos = 0;
                    }
                    else if (template.Name == "drawnCardTwo")
                    {
                        pos = 1;
                    }
                    else
                    {
                        pos = 2;
                    }
                    //Makes sure that the chosen card is the left-most
                    bool optionBefore = false;
                    for (int i = pos-1; i >= 0; i-- )
                    {
                        if (hand[i].suit != "dump")
                        {
                            optionBefore = true;
                        }
                    }
                    if (!optionBefore)
                    {
                        held = new List<Card>();
                        held.Add(hand[pos]);
                        carrying = true;
                        carriedFromHand = true;
                        heldFromHand = template;
                    }
                }
            }
            else
            {
                cleanHand();
            }
            updateDropColour();
        }

        //Drop button is green when a card(s) is being held
        //Red when no card selected
        private void updateDropColour()
        {
            if (carrying)
            {
                dropButton.BackColor = Color.Green;
                dropButton.Text = "Drop Card";
            }
            else
            {
                dropButton.BackColor = Color.Red;
                dropButton.Text = "Not Holding";
            }
        }

        //Removes a card from the hand that was moved
        private void deleteHandImage()
        {
            heldFromHand.Visible = false;
            heldFromHand.Refresh();
            int handPos = findCardInHand(held[0]);
            hand[handPos] = new Card();
            hand[handPos].suit = "dump";
        }

        //Removes cards from the old pile when moved
        private void deletePileImage()
        {
            for (int i = piles[panelLoc].storedCards.Count() -1 ; i >= cardLoc; i--)
            {
                panelPiles[panelLoc].Controls.Remove(piles[panelLoc].storedCards[i]);
                piles[panelLoc].storedCards.RemoveAt(i);
                piles[panelLoc].faceUp.RemoveAt(i);
                numPerPile[panelLoc]--;
            }
        }

        //Adding a card to the foundations
        private void foundationClick(object sender, EventArgs e)
        {
            PictureBox found = sender as PictureBox;
            int pos;
            if (found.Name == "foundationHearts")
            {
                pos = 0;
            }
            else if (found.Name == "foundationDiamonds")
            {
                pos = 1;
            }
            else if (found.Name == "foundationClubs")
            {
                pos = 2;
            }
            else
            {
                pos = 3;
            }
            //Can only add a single card
            if (carrying == true && held.Count == 1) 
            {
                if(foundations[pos].addCard(held[0]))
                {
                    string cardPath = extractCard(held[0]);
                    updateImage(found, cardPath);
                    if (carriedFromHand)
                    {
                        deleteHandImage();
                    }
                    else
                    {
                        deletePileImage();
                    }
                }
                cleanHand();
            }
            //Checking end of game or auto-complete possible
            if (allFilled())
            {
                MessageBox.Show("You Won! Congratulations!");
            }
            if (isAutoReady())
            {
                autoCompleteButton.Text = "Auto Complete Ready";
            }
            updateDropColour();

        }

        //Finding the location of a card in the current delt hand
        private int findCardInHand(Card card)
        {
            int pos = -1;
            for (int i = 0; i < hand.Count(); i++)
            {
                if (card == hand[i])
                {
                    pos = i; 
                }
            }
            return pos;
        }

        //If auto-complete is possible, ends game
        private void autoCompleteButton_Click(object sender, EventArgs e)
        {
            if (isAutoReady())
            {
                MessageBox.Show("AutoComplete Finish. Congratulations!");
                updateImage(foundationHearts, @"\cards\king_of_hearts.png");
                updateImage(foundationDiamonds, @"\cards\king_of_diamonds.png");
                updateImage(foundationClubs, @"\cards\king_of_clubs.png");
                updateImage(foundationSpades, @"\cards\king_of_spades.png");
                for (int i = 0; i < piles.Count(); i++)
                {
                    for (int j = 0; j < piles[i].faceUp.Count(); j++)
                    {
                        panelPiles[i].Controls.Remove(piles[i].storedCards[j]);
                        panelPiles[i].Controls.Remove(piles[i].toTurn);
                    }
                }
            }

        }

        //Drops of a card back to its original location
        private void dropButton_Click(object sender, EventArgs e)
        {
            cleanHand();
            dropButton.BackColor = Color.Red;
        }
    }
}