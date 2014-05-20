using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using kioskplus.Unterhaltung.Blackjack;
using System.Configuration;
using kioskplus.Utils;
using System.Threading;

namespace kioskplus
{
    public partial class dlgBlackJack : Form
    {
        private Boolean ibStartMove = false;
        private Point mausPoint = new Point();
        
        Rectangle[] rect = new Rectangle[5];
        GraphicsPath[] gr = new GraphicsPath[5];
        Rectangle[] r = new Rectangle[5];
        int index = -1;
        int anzeigen = 0; // 0 = start

        Rectangle rectweiter;
        Rectangle rectStehen;
        Rectangle rectNehmen; 
        Rectangle rectSplitten;
        Rectangle rectDoppeln;

        Rectangle rectEinsatzDel;
       
        GraphicsPath grWeiter = new GraphicsPath();
        GraphicsPath grStehen = new GraphicsPath();
        GraphicsPath grNehmen = new GraphicsPath();
        GraphicsPath grDoppeln = new GraphicsPath();
        GraphicsPath grSplitten = new GraphicsPath();

        GraphicsPath grEinsatzDel = new GraphicsPath();

        int einsatz = 0;
        String sError = String.Empty;

        private Shoe shoe = new Shoe();
        private Player[] players = new Player[0];
        // The one and only dealer object
        private Dealer dealer;
        // Whether or not the dealer's down card is visible
        private bool showDealerDownCard;
        // Indicates we are in insurance mode
        private bool insurance = false;
        // This one is used by the timer to flash the highlight
        private bool showInsurance = false;
        // This keeps track of how many times we've flashed the insurance highlight
        private int tickCount = 0;
        // currentPlayer keeps track of which player is active
        // currentPlayer > the number of Players (or -1) means the dealer is active
        private int currentPlayer = -1;
        // Determines what kind of label is drawn to the upper left of each player hand
        // Normally display the card total, but after a hand, display the outcome
        private Player.LabelType labelType = Player.LabelType.bothHands;
        // Statistics variables
        private int deals = 0;
        private int shuffles = 0;
        // variables to keep track of which player we are changing 
        private int playerNumber;
        // Boolean indicates end of shoe.  It is set via event from the shoe
        private bool endOfShoe;
        // Scaling variables to make resizing faster and easier
        private float scaleX = 1.0F;
        private float scaleY = 1.0F;
        // This form is used to display the strategy for each player;
        // StrategyForm strategyForm = new StrategyForm();
        // This keeps track of whether we've hidden the form while minimized
        // private bool strategyFormMinimized;
        // Preload the background image on form_load for speed.  Store it here.
        private Bitmap backgroundImage;
        private System.Windows.Forms.Timer tmrAutoDeal;
        // This tells the paint routine that we're dealing so the advice doesn't get shown
        // until we know if this is an insurance round or not.
        bool dealing = false;
        // over the Ellipse
        short iOver = 0;

        // the text for buttons and errors
        // 
        String isSplittenText, isDoppelnText, isWeiterText, isStehenText, isNehmenText;
        String isErrorEinsatz, isEinsatz, isEinsatzDelete;

        int maxEinsatz = 0;
        Boolean ibEnde = false;
        Boolean ibEinsatzDel = true, ibEinsatzOver=false;
        inetDateTime invDateTime;
        Boolean ibClickedNehmen = false, ibPlayGestarted = false;

        String isMaxEinsatz = String.Empty;

        public dlgBlackJack()
        {
            InitializeComponent();
            
            // 500, 100, 25 ,10 ,5
            rect[0] = new Rectangle(new Point(770, 632), new Size(33, 33));
            rect[1] = new Rectangle(new Point(814, 620), new Size(33, 33));
            rect[2] = new Rectangle(new Point(856, 605), new Size(33, 33));
            rect[3] = new Rectangle(new Point(898, 586), new Size(33, 33));
            rect[4] = new Rectangle(new Point(936, 566), new Size(33, 33));

            r[0] = new Rectangle(new Point(575, 472), new Size(29, 27));
            r[1] = new Rectangle(new Point(610, 473), new Size(29, 27));
            r[2] = new Rectangle(new Point(641, 452), new Size(29, 27));
            r[3] = new Rectangle(new Point(671, 440), new Size(29, 27));
            r[4] = new Rectangle(new Point(701, 423), new Size(29, 27));

            for (int i = 0; i < 5; i++)
            {
                gr[i] = new GraphicsPath();
                gr[i].AddEllipse(r[i]);
            }

            rectweiter = new Rectangle(new Point(344, 514), kioskplus.weiter_normal.Size);
            Size s = new Size(kioskplus.stehen_normal.Size.Width / 2, kioskplus.stehen_normal.Height / 2);
            rectStehen = new Rectangle(new Point(420, 524), s);
            s = new Size(kioskplus.nehmen_normal.Size.Width / 2, kioskplus.nehmen_normal.Height / 2);
            rectNehmen = new Rectangle(new Point(440 + (kioskplus.nehmen_normal.Size.Width / 2), 514), s);

            rectEinsatzDel = new Rectangle(new Point(920, 650), new Size(120, 27));
            grEinsatzDel.AddRectangle(new Rectangle(new Point(700, 480), new Size(120, 27)));

            grWeiter.AddRectangle(rectweiter);
            grNehmen.AddRectangle(rectNehmen);
            grStehen.AddRectangle(rectStehen);

            //
            isSplittenText = Program.getMyLangString("blackSplitten");
            isDoppelnText = Program.getMyLangString("blackDoppeln");
            isWeiterText = Program.getMyLangString("blackWeiter");
            isStehenText = Program.getMyLangString("blackStehen");
            isNehmenText = Program.getMyLangString("blackNehmen");
            
            isErrorEinsatz = Program.getMyLangString("blackErrorEinsatz");
            isEinsatz = Program.getMyLangString("blackEinsatz");
            isEinsatzDelete = Program.getMyLangString("blackEinsatzDelete");

            isMaxEinsatz = Program.getMyLangString("blackMaxEinsatz");
            //

            shoe.EndOfShoeEvent += new Shoe.ShoeEventHandler(EndOfShoe);
            shoe.ShuffleEvent += new Shoe.ShoeEventHandler(ShuffleShoe);
            shoe.BackChangedEvent += new Shoe.ShoeEventHandler(BackChanged);
        }

        private void dlgBlackJack_MouseDown(object sender, MouseEventArgs e)
        {
            mausPoint.X = e.X;
            mausPoint.Y = e.Y;
            ibStartMove = true;
        }

        private void dlgBlackJack_MouseMove(object sender, MouseEventArgs e)
        {
            // Double-Buffer erst dann aktivieren
            //
            if (!this.DoubleBuffered)
                this.DoubleBuffered = true;

            if (ibStartMove)
            {
                Point currentPos = Control.MousePosition;
                currentPos.X -= mausPoint.X;
                currentPos.Y -= mausPoint.Y;
                this.Location = currentPos;
                return;
            }

            if (ibEinsatzDel && grEinsatzDel.IsVisible(e.Location))
                ibEinsatzOver = true;
            else
                ibEinsatzOver = false;

            bool b = false;
            for (int i = 0; i < 5; i++)
            {
                if (gr[i].IsVisible(e.Location))
                {
                    this.Cursor = Cursors.Hand;
                    index = i;
                    b = true;
                }
            }

            if (b)
                this.Cursor = Cursors.Default;
            else
            {
                index = -1;
                this.Cursor = Cursors.Default;
            }

            switch (anzeigen)
            {
                case 0:
                    {
                        if (grWeiter.IsVisible(e.Location))
                            iOver = 1;
                        else
                            iOver = 0;
                        break;
                    }
                case 1: // Stehen normal
                    {
                        if (grStehen.IsVisible(e.Location))
                            iOver = 1;
                        else if (grNehmen.IsVisible(e.Location))
                            iOver = 2;
                        else
                            iOver = 0;
                        break;
                    }
                case 2: // Stehen, nehmen, splitten
                    {
                        if (grStehen.IsVisible(e.Location))
                            iOver = 1;
                        else if (grNehmen.IsVisible(e.Location))
                            iOver = 2;
                        else if (grSplitten.IsVisible(e.Location))
                            iOver = 3;
                        else
                            iOver = 0;

                        break;
                    }
                case 3: // Stehen & Nehmen & Doppeln
                    {
                        if (grStehen.IsVisible(e.Location))
                            iOver = 1;
                        else if (grNehmen.IsVisible(e.Location))
                            iOver = 2;
                        else if (grDoppeln.IsVisible(e.Location))
                            iOver = 3;
                        else
                            iOver = 0;

                        break;
                    }
                case 4: // stehen & nehmen & doppeln & splitten
                    {
                        if (grStehen.IsVisible(e.Location))
                            iOver = 1;
                        else if (grNehmen.IsVisible(e.Location))
                            iOver = 2;
                        else if (grDoppeln.IsVisible(e.Location))
                            iOver = 3;
                        else if (grSplitten.IsVisible(e.Location))
                            iOver = 4;
                        else
                            iOver = 0;

                        break;
                    }
            }

            this.Refresh();
        }

        private void dlgBlackJack_MouseUp(object sender, MouseEventArgs e)
        {
            ibStartMove = false;
        }

        private void dlgBlackJack_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                e.Graphics.ScaleTransform(0.75F, 0.75F);
                e.Graphics.DrawImage(kioskplus.inet_blackjack, 0, 0);

                try
                {
                    // Max. Einsatz anzeigen
                    if (invDateTime != null && invDateTime.GetRestDauerAsMin() > 10)
                    {
                        e.Graphics.DrawString(String.Format(isMaxEinsatz, invDateTime.GetRestDauerAsMin() - 10), new Font("Arial", 14, FontStyle.Bold), Brushes.Red, 315F, 22F);
                    }
                }
                catch 
                {
                }

                if (index > -1 || (iOver != 0))
                {
                    this.Cursor = Cursors.Hand;
                    if (index > -1)
                        e.Graphics.DrawEllipse(new Pen(Brushes.RoyalBlue, 6), rect[index]);
                }

                if (ibEinsatzDel)
                {
                    if (ibEinsatzOver)
                    {
                        e.Graphics.FillRectangle(Brushes.White, rectEinsatzDel);
                        e.Graphics.DrawString(isEinsatzDelete, new Font("Arial", 10), Brushes.Blue, 920F, 655F);
                    }
                    else
                    {
                        e.Graphics.DrawRectangle(new Pen(Brushes.White, 2), rectEinsatzDel);
                        e.Graphics.DrawString(isEinsatzDelete, new Font("Arial", 10), Brushes.White, 920F, 655F);
                    }
                }

                switch (anzeigen)
                {
                    case 0: // Weiter Mousemove-Normal / Over
                        {
                            if (iOver == 0)
                                e.Graphics.DrawImage(kioskplus.weiter_normal, 500, 700);
                            else
                                e.Graphics.DrawImage(kioskplus.weiter_over, 500, 700);

                            e.Graphics.DrawString(isWeiterText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, 515F, 755F); // 520
                            break;
                        }
                    case 1: // stehen & normal
                        {
                            if (iOver == 0)
                            {
                                e.Graphics.DrawImage(kioskplus.stehen_normal, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_normal, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            else if (iOver == 1) // Stehen
                            {
                                e.Graphics.DrawImage(kioskplus.stehen_over, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_normal, 560 + kioskplus.nehmen_normal.Size.Width, 700);

                            }
                            else if (iOver == 2) // Nehmen
                            {
                                e.Graphics.DrawImage(kioskplus.stehen_normal, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_over, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            e.Graphics.DrawString(isStehenText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, 560F, 755F);
                            e.Graphics.DrawString(isNehmenText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, 570F + kioskplus.nehmen_normal.Size.Width, 755F);
                            break;
                        }
                    case 2: // splitten & stehen & normal
                        {
                            if (iOver == 0)
                            {
                                if (!ibClickedNehmen)
                                    e.Graphics.DrawImage(kioskplus.splitten_normal, 540 - kioskplus.splitten_normal.Size.Width, 700);
                                e.Graphics.DrawImage(kioskplus.stehen_normal, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_normal, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            else if (iOver == 3) // Splitten
                            {
                                if (!ibClickedNehmen)
                                    e.Graphics.DrawImage(kioskplus.splitten_over, 540 - kioskplus.splitten_normal.Size.Width, 700);
                                e.Graphics.DrawImage(kioskplus.stehen_normal, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_normal, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            else if (iOver == 2) // Nehmen 
                            {
                                if (!ibClickedNehmen)
                                    e.Graphics.DrawImage(kioskplus.splitten_normal, 540 - kioskplus.splitten_normal.Size.Width, 700);
                                e.Graphics.DrawImage(kioskplus.stehen_normal, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_over, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            else if (iOver == 1) // Stehen
                            {
                                if (!ibClickedNehmen)
                                    e.Graphics.DrawImage(kioskplus.splitten_normal, 540 - kioskplus.splitten_normal.Size.Width, 700);
                                e.Graphics.DrawImage(kioskplus.stehen_over, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_normal, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            if (!ibClickedNehmen)
                                e.Graphics.DrawString(isSplittenText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, (550 - kioskplus.splitten_normal.Size.Width), 755F);
                            e.Graphics.DrawString(isStehenText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, 560F, 755F);
                            e.Graphics.DrawString(isNehmenText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, 570F + kioskplus.nehmen_normal.Size.Width, 755F);
                            break;
                        }
                    case 3: // doppeln & stehen & nehmen
                        {
                            if (iOver == 0)
                            {
                                if (!ibClickedNehmen)
                                    e.Graphics.DrawImage(kioskplus.doppelt_normal, 540 - kioskplus.doppelt_normal.Size.Width, 700);
                                e.Graphics.DrawImage(kioskplus.stehen_normal, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_normal, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            else if (iOver == 3) // Doppeln
                            {
                                if (!ibClickedNehmen)
                                    e.Graphics.DrawImage(kioskplus.doppelt_over, 540 - kioskplus.doppelt_normal.Size.Width, 700);
                                e.Graphics.DrawImage(kioskplus.stehen_normal, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_normal, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            else if (iOver == 2) // Nehmen
                            {
                                if (!ibClickedNehmen)
                                    e.Graphics.DrawImage(kioskplus.doppelt_normal, 540 - kioskplus.doppelt_normal.Size.Width, 700);
                                e.Graphics.DrawImage(kioskplus.stehen_normal, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_over, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            else if (iOver == 1) // Stehen
                            {
                                if (!ibClickedNehmen)
                                    e.Graphics.DrawImage(kioskplus.doppelt_normal, 540 - kioskplus.doppelt_normal.Size.Width, 700);
                                e.Graphics.DrawImage(kioskplus.stehen_over, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_normal, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            if (!ibClickedNehmen)
                                e.Graphics.DrawString(isDoppelnText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, (550 - kioskplus.doppelt_normal.Size.Width), 755F);
                            e.Graphics.DrawString(isStehenText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, 560F, 755F);
                            e.Graphics.DrawString(isNehmenText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, 570F + kioskplus.nehmen_normal.Size.Width, 755F);
                            break;
                        }
                    case 4: // splitten & doppeln & stehen & nehmen over
                        {
                            if (iOver == 0)
                            {
                                if (!ibClickedNehmen)
                                {
                                    e.Graphics.DrawImage(kioskplus.splitten_normal, 530 - 2 * kioskplus.splitten_normal.Size.Width, 700);
                                    e.Graphics.DrawImage(kioskplus.doppelt_normal, 540 - kioskplus.doppelt_normal.Size.Width, 700);
                                }
                                e.Graphics.DrawImage(kioskplus.stehen_normal, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_normal, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            else if (iOver == 1) // Stehen
                            {
                                if (!ibClickedNehmen)
                                {
                                    e.Graphics.DrawImage(kioskplus.splitten_normal, 530 - 2 * kioskplus.splitten_normal.Size.Width, 700);
                                    e.Graphics.DrawImage(kioskplus.doppelt_normal, 540 - kioskplus.doppelt_normal.Size.Width, 700);
                                }
                                e.Graphics.DrawImage(kioskplus.stehen_over, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_normal, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            else if (iOver == 2) // Nehmen
                            {
                                if (!ibClickedNehmen)
                                {
                                    e.Graphics.DrawImage(kioskplus.splitten_normal, 530 - 2 * kioskplus.splitten_normal.Size.Width, 700);
                                    e.Graphics.DrawImage(kioskplus.doppelt_normal, 540 - kioskplus.doppelt_normal.Size.Width, 700);
                                }
                                e.Graphics.DrawImage(kioskplus.stehen_normal, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_over, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            else if (iOver == 3) // doppeln
                            {
                                if (!ibClickedNehmen)
                                {
                                    e.Graphics.DrawImage(kioskplus.splitten_normal, 530 - 2 * kioskplus.splitten_normal.Size.Width, 700);
                                    e.Graphics.DrawImage(kioskplus.doppelt_over, 540 - kioskplus.doppelt_normal.Size.Width, 700);
                                }
                                e.Graphics.DrawImage(kioskplus.stehen_normal, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_normal, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }
                            else if (iOver == 4) // Splitten
                            {
                                if (!ibClickedNehmen)
                                {
                                    e.Graphics.DrawImage(kioskplus.splitten_over, 530 - 2 * kioskplus.splitten_normal.Size.Width, 700);
                                    e.Graphics.DrawImage(kioskplus.doppelt_normal, 540 - kioskplus.doppelt_normal.Size.Width, 700);
                                }
                                e.Graphics.DrawImage(kioskplus.stehen_normal, 550, 700);
                                e.Graphics.DrawImage(kioskplus.nehmen_normal, 560 + kioskplus.nehmen_normal.Size.Width, 700);
                            }

                            if (!ibClickedNehmen)
                            {
                                e.Graphics.DrawString(isSplittenText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, (540 - 2 * kioskplus.splitten_normal.Size.Width), 755F);
                                e.Graphics.DrawString(isDoppelnText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, (550 - kioskplus.splitten_normal.Size.Width), 755F);
                            }
                            e.Graphics.DrawString(isStehenText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, 560F, 755F);
                            e.Graphics.DrawString(isNehmenText, new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, 570F + kioskplus.nehmen_normal.Size.Width, 755F);
                            break;
                        }
                }

                try{
                if (einsatz > -1)
                    e.Graphics.DrawString(String.Format(isEinsatz, einsatz), new Font("Arial", 14, FontStyle.Bold), Brushes.GreenYellow, 550F, 22F);
                }
                catch 
                {  }

                if (!sError.Equals(""))
                    e.Graphics.DrawString(sError, new Font("Arial", 18, FontStyle.Bold), Brushes.DarkRed, 500F, 300F);

                // If the form has been initialized, there will be a shoe
                if (shoe != null)
                {
                    // Draw the dealer's hand
                    dealer.DrawHand(e.Graphics, showDealerDownCard);
                    int pl = -1;
                    foreach (Player player in players)
                    {
                        // Draw the player's hand

                        if (player.Active) { player.DrawHands(e.Graphics, labelType, dealer.Hand, player == CurrentPlayer); }

                        if (insurance)
                        {
                            // Möchten Sie versichern?
                            insurance = false;

                        }

                        if (player.GetKennzeichen() != -1 && ibEnde == false && ibPlayGestarted)
                        {
                            ibEnde = true;
                            ibPlayGestarted = false;
                            ibClickedNehmen = false;
                            pl = player.GetKennzeichen();
                            player.SetKennzeichen(-1);

                            if (Program.gnvSplash.ibKennzSpiele)
                                SendDataToDB(pl);
                        }
                    }
                }

                e.Graphics.ResetTransform();
            }
            catch 
            { }
        }

        private void SendDataToDB(int argKennzeichen)
        {

            if (einsatz <= 0)
                return;

            int tmpEinsatz = 0;

            String lsKennz = "m";
            String lsOnline = String.Empty;
            inetDateTime ldt = new inetDateTime();
           
            switch (argKennzeichen)
            {
                case 0: // unentschieden
                    break;
                case 1: // gewonnen
                    {
                        lsKennz = "p";
                        tmpEinsatz = einsatz * (-1);
                        break;
                    }
                case 2: // verloren
                    tmpEinsatz = einsatz;
                       break;
                case 9: // gewonnen - blackjack
                    {
                        lsKennz = "p";
                        tmpEinsatz = (int)Math.Round(einsatz * (-1.5));
                        einsatz = (int)Math.Round(einsatz * 1.5);
                        break;
                    }
            }

            anzeigen = 0;
            ibEinsatzDel = true;

            try
            {
                if (argKennzeichen != 0 /* && ibEnde == false */)
                {
                    ldt.SetEndeDateTime(einsatz);
                    lsOnline = ldt.GetGesamtOnlineZeit();
                    ldt = null;
Console.WriteLine(lsOnline + "/" + argKennzeichen);

                    if (Program.gnvSplash.DialogNewUser != null)
                    {
                        Program.gnvSplash.DialogNewUser.SetNewOnlineTime(tmpEinsatz);
                        Program.gnvSplash.SetUmsatz(0, ref lsOnline, lsKennz, 0,"");
                    }
                }

               
// maxEinsatz = invDateTime.GetRestDauerAsMin();//Program.gnvSplash.DialogNewUser.GetRestDauerAsMinuten();
            }
            catch
            {
                anzeigen = 0;
                ibEinsatzDel = true;
              
                this.Refresh();  
            }
            //this.Refresh();
        }

        private void dlgBlackJack_Click(object sender, EventArgs e)
        {

            if (ibEinsatzOver)
                einsatz = 0;

            if (anzeigen == 0) // Start
            {
                if (ibEnde)
                {
                    ibEinsatzDel = true;
                    dealer.Reset();
                    foreach (Player pl in players)
                    {
                        pl.Reset();
                        pl.ResetCount(6);
                        pl.Bet = 0;
                    }
                    ibEnde = false;
                    einsatz = 0;
                }
            }


            if (ibEnde)
            {
                einsatz = 0;
                ibEnde = false;
            }

            sError = "";
            int tmpEinsatz = 0;
            if (!ibPlayGestarted)
            {
                switch (index)
                {
                    case 0:
                        {
                            tmpEinsatz = 500;
                            break;
                        }
                    case 1:
                        {
                            tmpEinsatz = 100;
                            break;
                        }
                    case 2:
                        {
                            tmpEinsatz = 25;
                            break;
                        }
                    case 3:
                        {
                            tmpEinsatz = 10;
                            break;
                        }

                    case 4:
                        {
                            tmpEinsatz = 5;
                            break;
                        }
                }

                if (index >= 0)
                {
                    if (invDateTime.GetRestDauerAsMin() <= (einsatz + tmpEinsatz + 10))
                        sError = String.Format(Program.getMyLangString("blackMaxEinsatzUeber"), Environment.NewLine, Environment.NewLine);
                    else
                        einsatz += tmpEinsatz;
                }
            }
            
            if (anzeigen == 0) // Start
            {
                
                if (einsatz <= 0)
                {
                    if (sError.Equals(""))
                        sError = String.Format(Program.getMyLangString("blackErrorEinsatz"), Environment.NewLine, Environment.NewLine) ;
                }
                else
                {
                    if (iOver == 1)
                    {
                        ibEinsatzDel = false;
                        ibPlayGestarted = true;
                        players[0].Bet = einsatz;

                        DealStart();
                        SetButtons(null);
                    }
                }
            }else
                SetAction();

            this.Refresh();
        }


        private void SetAction()
        {
           
            switch (iOver)
            {
                case 0:
                    {
                       
                        break;
                    }
                case 1: // stehen
                    {
                        ibClickedNehmen = true;
                        NoCard();
                        break;
                    }
                case 2: // nehmen
                    {
                        ibClickedNehmen = true;
                        GetNewCard();
                        break;
                    }
                case 3: // doppeln || splitten
                    {
                        if (anzeigen == 2)
                        {
                            if (invDateTime.GetRestDauerAsMin() > (2 * einsatz + 10))
                                SetSplitten();
                            else
                                sError = String.Format(Program.getMyLangString("blackSplittenError"),Environment.NewLine,Environment.NewLine);
                        }
                        else
                        {
                            if (invDateTime.GetRestDauerAsMin() > (2 * einsatz + 10))
                                SetDoppeln();
                            else
                                sError = String.Format(Program.getMyLangString("blackDoppelnError"),Environment.NewLine,Environment.NewLine);
                        }
                            break;
                    }
                case 4: // splitten
                    {
                        if (invDateTime.GetRestDauerAsMin() > (2 * einsatz + 10))
                            SetSplitten();
                        else
                            sError = Program.getMyLangString("blackSplittenError");
                        break;
                    }
            }

        }

        private void SetSplitten()
        {
            einsatz = 2 * einsatz;

            ibClickedNehmen = true;

            // Make sure we're working with a player and not the dealer
            // and that it's not an insurance round
            if (CurrentPlayer != null && !insurance)
            {
                // See if the player is able to split
                if (CurrentPlayer.Split())
                {
                   // CurrentPlayer.Bet = einsatz;

                    // Refresh so that the split hands show up
                    this.Refresh();

                    // Splitting aces is a special case...
                    if (CurrentPlayer.CurrentHand[0].FaceValue == Card.CardType.Ace)
                    {
                        // Player only gets one more card for each hand
                        NextCard();
                        NextHand();

                        // Then move on to the next player
                        NextPlayer();
                    }
                    else
                    {
                        // Normal split, deal another card to the current hand
                        labelType = Player.LabelType.drawToHand;
                        NextCard();
                        
                        // If they hit 21, move to the next hand automatically
                        if (CurrentPlayer.CurrentHand.Total() == 21)
                        {
                            NextHand();
                            // And if that hand has 21, move to the next player
                            if (CurrentPlayer.CurrentHand.Total() == 21)
                                NextPlayer();
                        }
                    }
                }
            }
        }


        private void SetDoppeln()
        {
            einsatz = 2 * einsatz;
            ibClickedNehmen = true;
            // Make sure we're working with a player and not the dealer
            // and that it's not an insurance round
            if (CurrentPlayer != null && !insurance)
            {
                if (CurrentPlayer.DoubleDown(CurrentPlayer.CurrentHand))
                {
                    CurrentPlayer.Bet = einsatz;

                    // Deal one more card
                    NextCard();

                    if (CurrentPlayer.LastHand())
                        // Move to the next player...
                        NextPlayer();
                    else
                        // or next hand if the player split
                        NextHand();
                }
            }

        }


        private void GetNewCard()
        {
            // Make sure we're working with a player and not the dealer
            // and that it's not an insurance round
            if (CurrentPlayer != null && !insurance)
            {
                int handTotal = 0;

                // Give them another card
                NextCard();

                do
                {
                    // Now loop until we find a player that hasn't busted or dealt 21
                    handTotal = CurrentPlayer.CurrentHand.Total();
                    if (handTotal >= 21)
                    {
                        if (CurrentPlayer.LastHand())
                        {
                            // Move to the next player
                            NextPlayer();

                            // and get their total so the loop can continue if need be
                            if (CurrentPlayer != null)
                                handTotal = CurrentPlayer.CurrentHand.Total();
                        }
                        else
                        {
                            // The player has split, move to the next hand
                            NextHand();

                            // Get the hand total so the loop can continue if need be
                            handTotal = CurrentPlayer.CurrentHand.Total();
                        }
                    }
                } while (handTotal >= 21 && CurrentPlayer != null);
            }

        }


        private void NoCard()
        {
            int handTotal = 0;

            // Make sure we're working with a player and not the dealer
            // and that it's not an insurance round
            if (CurrentPlayer != null && !insurance)
            {
                if (CurrentPlayer.LastHand())
                    NextPlayer();
                else
                    // If the player has split, move to the next hand...
                    NextHand();

                // Make sure we didn't skip past the last player above
                if (CurrentPlayer != null)
                {
                    do
                    {
                        // Now loop until we find a player that hasn't busted or dealt 21
                        handTotal = CurrentPlayer.CurrentHand.Total();
                        if (handTotal >= 21)
                        {
                            if (CurrentPlayer.LastHand())
                            {
                                // Move to the next player
                                NextPlayer();

                                // and get their total so the loop can continue if need be
                                if (CurrentPlayer != null)
                                    handTotal = CurrentPlayer.CurrentHand.Total();
                            }
                            else
                            {
                                // The player has split, move to the next hand
                                NextHand();

                                // Get the hand total so the loop can continue if need be
                                handTotal = CurrentPlayer.CurrentHand.Total();
                            }
                        }
                    } while (handTotal >= 21 && CurrentPlayer != null);
                }
            }

        }


        private void DealStart()
        {
            // Keep track of statistics
            deals++;

            // Reset the form for play
            ////////txtNumberOfPlayers.Enabled = false;
            ////////txtNumberOfDecks.Enabled = false;

            // Reset the players
            foreach (Player player in players)
            {
                // Reset all players regardless of whether they are active
                player.Reset();
            }

            // Reset insurance
            insurance = false;

            // Set the buttons in the correct state
            //   SetButtonState(false);

            // Reset the dealer
            showDealerDownCard = false;
            dealer.Reset();

            // Reset the player pointer
            currentPlayer = 0;

            // Shuffle if we need to
            if (shoe.Eod)
            {
                endOfShoe = false;

                // This clears the screen so it looks like the dealer picked up all the cards
                this.Refresh();

                // Then shuffle the deck
                ///////Shuffle(true);
            }

            // Get the recommended wager from the counting system
            foreach (Player player in players)
            {
                if (player.Active) player.GetWager();
            }

            // Set the label type to draw all labels
            labelType = Player.LabelType.bothHands;

            // Update statistics
            //////pnlStatistics.Panels[0].Text = "Hands: " + deals.ToString(CultureInfo.InvariantCulture);
            //////pnlStatistics.Panels[1].Text = "Shuffles: " + shuffles.ToString(CultureInfo.InvariantCulture);

            Card newCard = null;
            Card dealerCard = null;

            dealing = true;

            // Deal the cards
            for (int k = 0; k < 2; k++)
            {
                foreach (Player player in players)
                {
                    if (player.Active)
                    {
                        // Get a card
                        newCard = shoe.Next();
                        // Put it in the player's hand
                        player.GetHands()[0].Add(newCard);
                        // And give all players the opportunity to count it
                        CountCard(newCard);
                        // Then update the screen so the new card shows up
                        this.Refresh();
                    }
                }
                // Now give the dealer a card
                dealerCard = shoe.Next();
                dealer.Hand.Add(dealerCard);
                // And if it is the up card, give the players the opportunity to count it
                if (k == 1)
                    CountCard(newCard);

                // refresh the screen so we see each card as it is dealt.
                this.Refresh();
            }

            dealing = false;

            // If dealer has an ace showing, 
            // check to see if anybody wants insurance.
            if (dealer.Hand[1].FaceValue == Card.CardType.Ace)
            {
                // Highlight the board to indicate insurance is available
                insurance = true;
                ////SetButtonState(false);
                ////tmrInsurance.Enabled = true;
                this.Refresh();
            }
            else if (dealer.Hand.IsBlackjack())
            {
                // Dealer blackjack
                labelType = Player.LabelType.outcome;
                currentPlayer = players.GetUpperBound(0) + 1;

                // Show down card
                showDealerDownCard = true;
                // Let everyone count it
                this.Refresh();

                // Let the players count it 
                CountCard(dealer.Hand[0]);

                // Take the money from everybody unless they have Blackjack
                foreach (Player player in players)
                {
                    if (player.Active)
                    {
                        foreach (Hand hand in player.GetHands())
                        {
                            if (hand.IsBlackjack())
                                // If the player also had Blackjack, Push
                                player.Push(hand);
                        }
                       // player.Bet.Visible = true;
                    }
                }
                this.Refresh();

                // Reset the form for entry
                ////////txtNumberOfPlayers.Enabled = true;
                ////////txtNumberOfDecks.Enabled = true;
                ////SetButtonState(true);

                //////if (chkAuto.Checked)
                //////    tmrAutoDeal.Enabled = true;
            }
            else
            {
                this.Refresh();

                // Loop through the players until we find one that doesn't have 21
                int handTotal = 0;
                do
                {
                    handTotal = CurrentPlayer.CurrentHand.Total();
                    if (handTotal == 21)
                        NextPlayer();
                } while (handTotal == 21 && CurrentPlayer != null);

                // Tell the strategy form which player to show strategy for
                ////////if (strategyForm.Visible)
                ////////{
                ////////    strategyForm.CurrentPlayer = CurrentPlayer;
                ////////    if (dealer.Hand.Count > 0)
                ////////        strategyForm.DealerCard = dealer.Hand[1];
                ////////    strategyForm.Refresh();
                ////////}

            //    SetButtonState(false);
            }

            // The next deal will begin with a shuffle, tell the players
            // to reset their card count now so the suggested bet for human
            // players is accurate.
            if (shoe.Eod)
            {
                foreach (Player player in players)
                {
                    if (player.Active) player.ResetCount(6);
                }
            }
        }


        private Player CreatePlayer(Point point, string background, double bank, NumericUpDown betControl, string type, string strategy, string method, int numDecks)
        {
            Player player = new Player(point,
                                        background,
                                        bank,
                                       // betControl,
                                        type.ToLower() == "human" ? Player.playerType.human : Player.playerType.computer,
                                        Strategy.NewStrategy(strategy),
                                        CountMethod.NewMethod(method, numDecks));
            player.Active = true;
            return player;
        }


        private void InitializePlayers(int numPlayers)
        {
            players = new Player[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
               // NumericUpDown plyrBet = null;
                switch (i)
                {
                    case 0:  // PLAYER 1
                       //// plyrBet = CreateBetControl(1, new Point(955, 299));
                       // if (ConfigurationSettings.AppSettings["player1"] != null)
                       // {
                       //     try
                       //     {
                       //         string[] playerConfig = ConfigurationSettings.AppSettings["player1"].ToString().Split(new char[] { '|' });
                       //         players[i] = CreatePlayer(new Point(961, 299), "player1_back", double.Parse(playerConfig[1]), plyrBet, playerConfig[0], playerConfig[2], playerConfig[3], 6);
                       //     }
                       //     catch
                       //     {
                       //         players[i] = new Player(new Point(961, 299), "player1_back", 5000.00, plyrBet, Player.playerType.human, new BasicMultiDeck(), new HiLo(6));
                       //         // Note that there must be a player 1
                       //        // players[i].Active = true;
                       //     }
                       // }
                       // else
                       // {
                            try
                            {
                                players[i] = new Player(new Point(500, 400), "player1_back", 999999999.00, /*plyrBet,*/ Player.playerType.human, new BasicMultiDeck(), new HiLo(6));
                                // Note that there must be a player 1
                             //   players[i].Active = true;
                            }catch 
                            { /*Console.WriteLine(er.Message);*/ }
                       // }
                        break;
                }
            }

            // This line just forces the resize event to execute, moving the controls into
            // the appropriate positions.
            //Blackjack_Resize(null, System.EventArgs.Empty);
            this.Refresh();
        }


        private void CountCard(Card newCard)
        {
            // Tell each of the players what the new card is
            // so they can count it if they want to.
            foreach (Player player in players)
            {
                if (player.Active) player.CountCard(newCard);
            }
        }

        private Player CurrentPlayer
        {
            get
            {
                if (currentPlayer >= 0 &&
                    currentPlayer <= players.GetUpperBound(0) &&
                    players[currentPlayer].Active)
                {
                    return players[currentPlayer];
                }
                else
                {
                    return null;
                }
            }
        }

        private void NextCard()
        {
            // Get the next card
            Card newCard = shoe.Next();

            // Tell each of the players what the card is 
            CountCard(newCard);

            // Add a card to the player's hand
            CurrentPlayer.CurrentHand.Add(newCard);

            // and show it.
            this.Refresh();

            // And if the strategy form is visible, refresh it too
            //if (strategyForm.Visible)
            //    strategyForm.Refresh();

            //// The strategy may change with each card so highlight the correct button
            //SetButtonState(false);
        }

        private void NextHand()
        {
            // Advance to the player's next hand (for splits only)
            CurrentPlayer.NextHand();

            // They always get another card on a new, split hand
            NextCard();

            // And refresh the strategy form to show the situation with the new hand
            //if (strategyForm.Visible)
            //    strategyForm.Refresh();

            //// The strategy may change with each card so highlight the correct button
            //SetButtonState(false);
        }

        private void NextPlayer()
        {
            // Make sure both of the current players hands are visible and have labels
            labelType = Player.LabelType.bothHands;

            do
            {
                currentPlayer++;
                this.Refresh();

                // Dealer's turn
                if (CurrentPlayer == null)
                {
                    if (insurance && !dealer.Hand.IsBlackjack())
                    {
                        foreach (Player player in players)
                        {
                            if (player.Insurance)
                            {
                                player.LostInsurance();
                                player.Insurance = false;
                            }
                        }
                        // We're no longer in insurance mode
                        insurance = false;
                        currentPlayer = 0;
                        //SetButtonState(false);

                        this.Refresh();
                        //if (strategyForm.Visible)
                        //{
                        //    strategyForm.CurrentPlayer = CurrentPlayer;
                        //    if (dealer.Hand.Count > 0)
                        //        strategyForm.DealerCard = dealer.Hand[1];
                        //    strategyForm.Refresh();
                        //}
                    }
                    else
                    {
                        // Tell the dealer to show his down card
                        showDealerDownCard = true;

                        // Update the screen so we can see it
                        this.Refresh();

                        // Let the players count it if they want to
                        CountCard(dealer.Hand[0]);

                        // Only hit to the dealer if someone didn't bust
                        bool hitToDealer = false;

                        foreach (Player player in players)
                        {
                            if (player.Active)
                            {
                                foreach (Hand hand in player.GetHands())
                                {
                                    if (hand.Total() > 0 && hand.Total() <= 21 && !hand.IsBlackjack())
                                    {
                                        hitToDealer = true;
                                        break;
                                    }
                                }
                            }
                            if (hitToDealer)
                                break;
                        }

                        if (hitToDealer)
                        {
                            while (dealer.Total() < 17)
                            {
                                Card dealerCard = shoe.Next();
                                dealer.AddCard(dealerCard);
                                this.Refresh();

                                // Give each player the opportunity to count the card
                                CountCard(dealerCard);
                            }
                        }

                        labelType = Player.LabelType.outcome;

                        foreach (Player player in players)
                        {
                            if (player.Active)
                            {
                                if (insurance && dealer.Hand.IsBlackjack())
                                    player.WonInsurance();
                                else
                                    player.LostInsurance();

                                foreach (Hand hand in player.GetHands())
                                {
                                    switch (hand.Outcome(dealer.Hand, player.NumberOfHands))
                                    {
                                        case Hand.OutcomeType.Won:
                                            player.Won(hand);
                                            break;
                                        case Hand.OutcomeType.Lost:
                                            // Do nothing, the money was taken at the beginning of play
                                            break;
                                        case Hand.OutcomeType.Push:
                                            player.Push(hand);
                                            break;
                                        case Hand.OutcomeType.Blackjack:
                                            player.Blackjack(hand);
                                            break;
                                        default:
                                            // Hand not in play
                                            break;
                                    }
                                }
                                //player.Bet.Visible = true;
                            }
                        }

                        // Reset the form for entry
                        insurance = false;
                        ////txtNumberOfPlayers.Enabled = true;
                        ////txtNumberOfDecks.Enabled = true;
//                        SetButtonState(true);

                        //if (chkAuto.Checked)
                        //    tmrAutoDeal.Enabled = true;

                        // The next deal will begin with a shuffle, tell the players
                        // to reset their card count now so the suggested bet for human
                        // players is accurate.
                        if (shoe.Eod)
                        {
                            foreach (Player player in players)
                            {
                                if (player.Active) player.ResetCount(6);
                            }
                        }
                        this.Refresh();
                    }
                }
                else
                {
                    ////if (strategyForm.Visible)
                    ////{
                    ////    strategyForm.CurrentPlayer = CurrentPlayer;
                    ////    if (dealer.Hand.Count > 0)
                    ////        strategyForm.DealerCard = dealer.Hand[1];
                    ////    strategyForm.Refresh();
                    ////}

                    ////SetButtonState(false);
                }

            } while (CurrentPlayer != null && CurrentPlayer.CurrentHand.Total() >= 21 && !insurance);
        }


        private void SetButtons(Graphics gr)
        {
            if (CurrentPlayer != null)
            {
                if (CurrentPlayer.Type == Player.playerType.computer)
                {
                    // nichts anzeigen
                    Console.WriteLine("bin computer");
                }
                else
                {
                    anzeigen = 1;
                    if (CurrentPlayer.CanDouble(CurrentPlayer.CurrentHand) && CurrentPlayer.CanSplit() ) // Doppeln und Splitten
                    {
                        anzeigen = 4;
                       rectSplitten = new Rectangle(new Point(280, 514), kioskplus.splitten_normal.Size);
                       rectDoppeln = new Rectangle(new Point(320, 514), kioskplus.splitten_normal.Size);
                       grSplitten.Reset();
                       grDoppeln.Reset();
                       grDoppeln.AddRectangle(rectDoppeln);
                       grSplitten.AddRectangle(rectSplitten);
                    }
                    else if (CurrentPlayer.CanDouble(CurrentPlayer.CurrentHand)) // Doppeln
                    {
                        anzeigen = 3;
                        grDoppeln.Reset();
                        rectDoppeln = new Rectangle(new Point(320, 514), kioskplus.splitten_normal.Size);
                        grDoppeln.AddRectangle(rectDoppeln);
                    }
                    else if (CurrentPlayer.CanSplit()) // Splitten
                    {
                        anzeigen = 2;
                        grSplitten.Reset();
                        rectSplitten = new Rectangle(new Point(320, 514), kioskplus.splitten_normal.Size);
                        grSplitten.AddRectangle(rectSplitten);
                    }
                    this.Refresh();
                }
            }

        }

        private void BackChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void EndOfShoe(object sender, EventArgs e)
        {
            endOfShoe = true;
        }

        private void ShuffleShoe(object sender, EventArgs e)
        {
            endOfShoe = false;
        }

        private void dlgBlackJack_Load(object sender, EventArgs e)
        {
            dealer = new Dealer(new Point(500, 100));

            InitializePlayers(1);

            if (Program.gnvSplash.DialogNewUser != null)
            {
                try
                {
                    invDateTime = Program.gnvSplash.DialogNewUser.GetDateTimeObject();
                    maxEinsatz = invDateTime.GetRestDauerAsMin();//Program.gnvSplash.DialogNewUser.GetRestDauerAsMinuten();
                }
                catch 
                {
                    maxEinsatz = 0;
                }
            }
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (ibPlayGestarted)
                return;
           
            
            try
            {
                Program.gnvSplash.DialogNewUser.CloseBlackJack();
            }
            catch { }
        }

        private void dlgBlackJack_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ibPlayGestarted)
                e.Cancel = true;
        }

    }
}