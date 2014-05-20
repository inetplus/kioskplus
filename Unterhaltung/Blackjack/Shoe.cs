using System;
using System.Drawing;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace kioskplus.Unterhaltung.Blackjack
{
	/// <summary>
	/// Summary description for Shoe.
	/// </summary>
	[Serializable()]
	public class Shoe
	{
		private const int		CARDSPERDECK = 52;

        private List<Card> cards = new List<Card>(); 
		//private Card[]			cards;
		private int				numberOfDecks = 5;
		private static Image	backImage;
		private BackTypes		backType;
		private int				shoeLocation;

        private ArrayList iCardsNumber = new ArrayList();

		//	Integer values that correspond to the resource image names
		public enum BackTypes 
		{
			CrossHatch = 53,
			Weave1 = 54,
			Weave2 = 55,
			Robot = 56,
			Flowers = 57,
			Vine1 = 58,
			Vine2 = 59,
			Fish1 = 60,
			Fish2 = 61,
			Shells = 62,
			Castle = 63,
			Island = 64,
			Sleeve = 65,
			Bellagio = 66
		}

		// A delegate type for hooking up change notifications.
		public delegate void ShoeEventHandler(object sender, EventArgs e);
		
		public event ShoeEventHandler EndOfShoeEvent;
		public event ShoeEventHandler BackChangedEvent;
		public event ShoeEventHandler ShuffleEvent;

		public int NumberOfDecks 
		{
			get 
			{ 
				return numberOfDecks;	
			}
			set 
			{
				if (numberOfDecks != value) 
					numberOfDecks = value;
			}
		}

		public int ShoeLocation
		{
			get { return shoeLocation; }
		}

		public Shoe()
		{
			Init();
			// Set back type to Bellagio if none specified
			backType = BackTypes.Bellagio;
            backImage = ResourcesBlackjack.GetImage("card"+((int)backType).ToString());
		}

		public Shoe( BackTypes backType )
		{
			Init();
			backType = backType;
            backImage = ResourcesBlackjack.GetImage((int)backType);
		}

		public static Image BackImage 
		{
			get { return backImage; }
		}

		public BackTypes CardBack 
		{
			get { return backType; }
			set 
			{ 
				backType = value;
                backImage = ResourcesBlackjack.GetImage(((int)value).ToString(CultureInfo.InvariantCulture));
				BackChangedEvent(this, EventArgs.Empty);
			}
		}

		public void Init()
		{
			shoeLocation = 0;

            ArrayList iX, iY;
            iX = new ArrayList();
            iY = new ArrayList();

            int yy;

			// cards = new Card[ numberOfDecks * CARDSPERDECK ];
			int current = 0;

			for( int j=0; j < numberOfDecks; j++ )
			{
				for ( int y = 0; y < 4; y++) 
				{
                    do
                    {
                        yy = (new Random()).Next(0, 4);
                    } while (iY.Contains(yy));

                    iY.Add(yy);
					for ( int x = 0; x < 13; x++) 
					{

                       
                        Card c = new Card((Card.CardType)x, (Card.Suits)yy);
                        cards.Add(c);
                        current++;
					}
                    iX.Clear();
				}

                iX.Clear();
                iY.Clear();
			}
            
		}

		//	End-of-Deck property
		public bool Eod
		{
			get { return (shoeLocation >=  cards.Count * .66); }
		}
		
		//	Beginning-of-Deck property
		public bool Bod 
		{
			get { return (shoeLocation == 0); }
		}

		public void Shuffle()
		{
			Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            Init();
			for( int i = 0; i < rand.Next(10,100); i++ )
			{
                List<Card> shuffledCards = new List<Card>();//Card[numberOfDecks * CARDSPERDECK];
                Random index = new Random();
                int upperLimit = cards.Count; /*GetUpperBound(0); */

				for( int j = 0; j < numberOfDecks * CARDSPERDECK; j++ )
				{
					int k = index.Next(0, upperLimit);
					shuffledCards[j] = cards[k];
					cards[k] = cards[upperLimit--];
				}
				cards = shuffledCards;
			}
			shoeLocation = 0;

			ShuffleEvent(this, EventArgs.Empty );
		}

		//	Indexer
		public Card this[int index] 
		{	
			get 
			{
                if (index < 0 || index > 52)
                    throw new ArgumentOutOfRangeException();
                else
                    return cards[index];/*cards[index];*/
			}
		}

		public Card Next()
		{
			if( cards.Count >0 )
			{
				if( Eod )
					EndOfShoeEvent(this, EventArgs.Empty );

                System.Threading.Thread.Sleep(600);
                shoeLocation = (new Random()).Next(0, cards.Count);
                Card c = cards[shoeLocation];// cards[shoeLocation++];
                cards.RemoveAt(shoeLocation);
				
                return c ;
			}
			else
			{
				// It shouldn't come to this, but in the unlikely event that we
				// run out of cards, reshuffle the shoe.
				Shuffle();
                shoeLocation = (new Random()).Next(0, cards.Count);
                Card c = cards[shoeLocation];// cards[shoeLocation++];
                cards.RemoveAt(shoeLocation);
                return c;
			}
		}

		//	Designed to be used by Select Back forms
		public static Hashtable GetAllBackImages() 
		{
			Hashtable images = new Hashtable(12);
			for (int x = (int)BackTypes.CrossHatch; x < (int)BackTypes.Bellagio; x++) 
			{
                images.Add(x, ResourcesBlackjack.GetImage(x));
			}

			return images;
		}

	}
}
