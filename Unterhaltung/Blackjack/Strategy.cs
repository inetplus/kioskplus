using System;
using System.Drawing;
using System.Collections;
using System.Globalization;

namespace kioskplus.Unterhaltung.Blackjack
{
	/// <summary>
	/// Summary description for Strategy.
	/// </summary>
	[Serializable()]
	public abstract class Strategy
	{
		public const int S = 100;		// Stand
		public const int H = 200;		// Hit
		public const int D = 300;		// Double Down
		public const int P = 400;		// Split
		public const int N = -100;		// None
		public const int L = -200;		// Null

		public enum AdviceType
		{
			None = N,
			Hit = H,
			Stand = S,
			DoubleDown = D,
			Split = P,
			Null = L
		}

		//protected string name = "";
		protected int fontSize = 8;
		protected string fontName = "Arial";
		[NonSerialized]
		protected int[,] Pairs;
		[NonSerialized]
		protected string[] PairsLabels;
		[NonSerialized]
		protected int[,] Aces;
		[NonSerialized]
		protected string[] DoubleLabels;
		[NonSerialized]
		protected int[,] DoubleH;
		[NonSerialized]
		protected int[,] DoubleS;
		[NonSerialized]
		protected string[] AcesLabels;
		[NonSerialized]
		protected int[,] Hand;
		[NonSerialized]
		protected string[] HandLabels;

		protected Strategy() {}
		public abstract AdviceType GetAdvice( Hand h, Card c, bool b, double cc );
		public abstract bool GetInsuranceAdvice( double count, double cardCount, int decks );
		public virtual string StrategyName{ get{ return ""; }}
		public virtual void Draw( Graphics drawingSurface, Rectangle size, Hand hand, Card card, double count )
		{
			Draw( drawingSurface, new Point(0,0), size, new Rectangle(0,0,1,1), hand, card, count);
		}

		public virtual void Draw( Graphics drawingSurface, Point origin, Rectangle size, Rectangle space, Hand currentHand, Card dealerCard, double count )
		{
			SizeF labelSize;
			bool highlight = false;

			// Get largest string and move the table over to make room for the labels
			try
			{
				labelSize = drawingSurface.MeasureString("Seven",new Font(fontName,fontSize));
			}
			catch
			{
				labelSize = drawingSurface.MeasureString("Seven",new Font("Arial",fontSize));
			}

			StringFormat myFormat = new StringFormat();
			myFormat.Alignment = StringAlignment.Center;

			// Draw the Dealer's header row
			int dealerRowHeight = (int)labelSize.Height;
			drawingSurface.DrawString("Dealer's up card", new Font(fontName,fontSize),Brushes.Black, origin.X + labelSize.Width + (size.Width*10 + space.Width*10)/2, origin.Y, myFormat);

			// Draw the Dealer's up card labels
			for(int k=0; k<10; k++)
			{
				drawingSurface.DrawString(((Card.CardType)k).ToString(CultureInfo.InvariantCulture), new Font(fontName,fontSize), Brushes.Black, origin.X + k*space.Width + k*size.Width + (int)labelSize.Width + size.Width/2, origin.Y + dealerRowHeight, myFormat);
			}

			// Draw the Player's header column
			//			StringFormat myVerticalFormat = new StringFormat();
			//			myVerticalFormat.FormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft;
			//			myVerticalFormat.Alignment = StringAlignment.Center;
			//			g.DrawString("Player's Hand",new Font(fontName,fontSize),Brushes.Black,origin.X + (int)labelSize.Width+ (int)labelSize.Height + size.Width*10 + space.Width*10,origin.Y + dealerRowHeight + (size.Height*10 + space.Height*10)/2,myVerticalFormat);
			
			// Draw the actual Advice values, Total | Aces | Pairs
			myFormat.Alignment = StringAlignment.Center;
			myFormat.LineAlignment = StringAlignment.Center;

			// Draw the strategies for a hard hand
			for(int j=0; j<=Hand.GetUpperBound(0); j++)
			{
				drawingSurface.DrawString(HandLabels[j].ToString(CultureInfo.InvariantCulture), new Font(fontName,fontSize), Brushes.Black, origin.X + (int)labelSize.Width/2, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + size.Height/2 + dealerRowHeight, myFormat);
				for(int i=0; i<=Hand.GetUpperBound(1); i++)
				{
					highlight = false;
					if( currentHand != null && dealerCard != null && !currentHand.IsPair && currentHand.Total() - 3 == j && dealerCard.TrueValue == i && (!currentHand.IsSoft || currentHand.Count > 2) )
						highlight = true;

					DrawSquare( drawingSurface, (AdviceType)Hand[j,i], highlight, (currentHand != null && currentHand.Count == 2), new Rectangle(origin.X + i*space.Width + i*size.Width + (int)labelSize.Width, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + dealerRowHeight, size.Width, size.Height ) );
				}
			}

			origin.Y += size.Height * (Hand.GetUpperBound(0)+1) + space.Height * (Hand.GetUpperBound(0)+1);

			// Draw the 17+ row
			drawingSurface.DrawString("17+", new Font(fontName,fontSize), Brushes.Black, origin.X + (int)labelSize.Width/2, origin.Y + (int)labelSize.Height + size.Height/2 + dealerRowHeight, myFormat );
			for(int j=0; j<10; j++)
			{
				highlight = false;
				if( currentHand != null && dealerCard != null && !currentHand.IsPair && currentHand.Total() >= 17 && dealerCard.TrueValue == j && (!currentHand.IsSoft || currentHand.Count > 2))
					highlight = true;

				DrawSquare( drawingSurface, AdviceType.Stand, highlight, (currentHand != null && currentHand.Count == 2), new Rectangle(origin.X + j*space.Width + j*size.Width + (int)labelSize.Width, origin.Y + (int)labelSize.Height + dealerRowHeight, size.Width, size.Height ) );
			}

			origin.Y += size.Height + space.Height;

			// Draw the strategies for soft hands
			for(int j=0; j<=Aces.GetUpperBound(0); j++)
			{
				drawingSurface.DrawString(AcesLabels[j], new Font(fontName,fontSize), Brushes.Black, origin.X + (int)labelSize.Width/2, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + size.Height/2 + dealerRowHeight, myFormat);
				for(int i=0; i<=Aces.GetUpperBound(1); i++)
				{
					highlight = false;
					if( currentHand != null && dealerCard != null && !currentHand.IsPair && currentHand.IsSoft && currentHand.Count == 2 )
					{
						if( currentHand[0].FaceValue == Card.CardType.Ace && currentHand[1].TrueValue - 1 == j && dealerCard.TrueValue == i )
							highlight = true;
						else if( currentHand[1].FaceValue == Card.CardType.Ace && currentHand[0].TrueValue - 1 == j && dealerCard.TrueValue == i )
							highlight = true;
					}
					DrawSquare( drawingSurface, (AdviceType)Aces[j,i], highlight, (currentHand != null && currentHand.Count == 2), new Rectangle(origin.X + i*space.Width + i*size.Width + (int)labelSize.Width, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + dealerRowHeight, size.Width, size.Height ) );
				}
			}

			origin.Y += size.Height * (Aces.GetUpperBound(0)+1) + space.Height * (Aces.GetUpperBound(0)+1);

			// Draw the strategies for pairs
			for(int j=0; j<=Pairs.GetUpperBound(0); j++)
			{
				drawingSurface.DrawString(PairsLabels[j], new Font(fontName,fontSize), Brushes.Black, origin.X + (int)labelSize.Width/2, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + size.Height/2 + dealerRowHeight, myFormat);
				for(int i=0; i<=Pairs.GetUpperBound(1); i++)
				{
					highlight = false;
					if( currentHand != null && dealerCard != null && currentHand.IsPair )
					{
						if( currentHand[0].TrueValue == j && dealerCard.TrueValue == i )
							highlight = true;
					}
					DrawSquare( drawingSurface, (AdviceType)Pairs[j,i], highlight, (currentHand != null && currentHand.Count == 2), new Rectangle(origin.X + i*space.Width + i*size.Width + (int)labelSize.Width, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + dealerRowHeight, size.Width, size.Height ) );
				}
			}
		}

		protected void DrawSquare( Graphics drawingSurface, Strategy.AdviceType advice, bool highlight, bool newDeal, Rectangle square )
		{
			string legend = "";
			Brush paintBrush = null;

			switch( advice )
			{
				case Strategy.AdviceType.DoubleDown:
					if( newDeal )
					{
						legend = "D";
						paintBrush = Brushes.LimeGreen;
					}
					else
					{
						legend = "H";
						paintBrush = Brushes.Yellow;
					}
					break;
				case Strategy.AdviceType.Hit:
					legend = "H";
					paintBrush = Brushes.Yellow;
					break;
				case Strategy.AdviceType.Split:
					legend = "SP";
					paintBrush = Brushes.CornflowerBlue;
					break;
				case Strategy.AdviceType.Stand:
					legend = "S";
					paintBrush = Brushes.Crimson;
					break;
				default:
					legend = "";
					paintBrush = Brushes.White;
					break;
			}
			StringFormat myFormat = new StringFormat();
			myFormat.Alignment = StringAlignment.Center;
			myFormat.LineAlignment = StringAlignment.Center;
			if( highlight )
			{
				drawingSurface.FillRectangle( Brushes.Black, square );
				square.Inflate(-2,-2);
				drawingSurface.FillRectangle( paintBrush, square );
			}
			else
			{
				drawingSurface.FillRectangle( paintBrush, square );
			}
			drawingSurface.DrawString( legend, new Font(fontName, fontSize), Brushes.Black, square, myFormat );
		}

		public static ArrayList GetStrategies()
		{
			ArrayList strategies = new ArrayList();

			strategies.Add( "Basic Single Deck" );
			strategies.Add( "Basic Multi Deck" );
			strategies.Add( "Aggressive Multi Deck" );
			strategies.Add( "Smart Multi Deck" );
			strategies.Add( "High Low Multi Deck" );

			return strategies;
		}
		public static Strategy NewStrategy( string strategyName )
		{
			Strategy returnValue = null;

			switch( strategyName.ToLower() )
			{
				case "basic single deck":
					returnValue = new BasicSingleDeck();
					break;
				case "basic multi deck":
					returnValue = new BasicMultiDeck();
					break;
				case "aggressive multi deck":
					returnValue = new AggresiveMultiDeck();
					break;
				case "smart multi deck":
					returnValue = new SmartMultiDeck();
					break;
				case "high low multi deck":
					returnValue = new HighLowMultiDeck();
					break;
				default:
					returnValue = null;
					break;
			}
			return returnValue;
		}
	}

	[Serializable()]
	public class BasicSingleDeck : Strategy
	{
		public BasicSingleDeck()
		{
			// Player has a pair of cards
			//			Dealer Card	 A,2,3,4,5,6,7,8,9,10
			Pairs = new int[10,10] {{P,P,P,P,P,P,P,P,P,P}, // A   P
									{H,H,P,P,P,P,P,H,H,H}, // 2   l
									{H,H,H,P,P,P,P,H,H,H}, // 3   a
									{H,H,H,H,D,D,H,H,H,H}, // 4   y
									{H,D,D,D,D,D,D,D,D,H}, // 5   e
									{H,P,P,P,P,P,H,H,H,H}, // 6   r
									{H,P,P,P,P,P,P,H,H,H}, // 7   P
									{P,P,P,P,P,P,P,P,P,P}, // 8   a
									{S,P,P,P,P,P,S,P,P,S}, // 9   i
									{S,S,S,S,S,S,S,S,S,S}};// 10  r

			PairsLabels = new string[10] 
				{"A-A","2-2","3-3","4-4","5-5","6-6","7-7","8-8","9-9","10-10"};

			// Player has an ace and something else
			//		Dealer Card	     A,2,3,4,5,6,7,8,9,10
			Aces  = new int[9,10]  {{H,H,H,D,D,D,H,H,H,H}, // 2   P
									{H,H,H,D,D,D,H,H,H,H}, // 3   l
									{H,H,H,D,D,D,H,H,H,H}, // 4   a
									{H,H,H,D,D,D,H,H,H,H}, // 5   y
									{H,D,D,D,D,D,H,H,H,H}, // 6   e
									{S,D,D,D,D,S,S,H,H,H}, // 7   r
									{S,S,S,S,S,S,S,S,S,S}, // 8   
									{S,S,S,S,S,S,S,S,S,S}, // 9   
									{S,S,S,S,S,S,S,S,S,S}};// 10   

			AcesLabels = new string[9]
				{"A2","A3","A4","A5","A6","A7","A8","A9","A10"};

			// Player has a hard hand without an ace
			//      Dealer Card	     A,2,3,4,5,6,7,8,9,10
			Hand  = new int[14,10] {{H,H,H,H,H,H,H,H,H,H}, // 3   P
									{H,H,H,H,H,H,H,H,H,H}, // 4   l
									{H,H,H,H,H,H,H,H,H,H}, // 5   a
									{H,H,H,H,H,H,H,H,H,H}, // 6   y
									{H,H,H,H,H,H,H,H,H,H}, // 7   e
									{H,H,H,H,H,H,H,H,H,H}, // 8   r
									{H,D,D,D,D,D,H,H,H,H}, // 9   s
									{H,D,D,D,D,D,D,D,D,H}, // 10   
									{D,D,D,D,D,D,D,D,D,D}, // 11  T 
									{H,H,H,S,S,S,H,H,H,H}, // 12  o
									{H,S,S,S,S,S,H,H,H,H}, // 13  t
									{H,S,S,S,S,S,H,H,H,H}, // 14  a
									{H,S,S,S,S,S,H,H,H,H}, // 15  l
									{H,S,S,S,S,S,H,H,H,H}};// 16

			HandLabels = new string[14]
				{"3","4","5","6","7","8","9","10","11","12","13","14","15","16"};
		}

		private string name = "Basic Single Deck";
		public override string StrategyName{ get{ return name; }}
		public override bool GetInsuranceAdvice( double count, double cardCount, int decks )
		{
			return cardCount >= 3;
		}
		public override AdviceType GetAdvice( Hand playerHand, Card dealerCard, bool allowSplit, double cardCount )
		{
			AdviceType Advice = AdviceType.Null;

			try
			{
				if( dealerCard != null  && playerHand.Count > 0 )
				{
					if( playerHand.Count == 2 && allowSplit )
					{
						if( playerHand[0].FaceValue == playerHand[1].FaceValue )
						{
							Advice = (AdviceType)Pairs[ (int)playerHand[0].TrueValue, (int)dealerCard.TrueValue ];
						}
						else if( playerHand[0].FaceValue == Card.CardType.Ace )
						{
							Advice = (AdviceType)Aces[ (int)playerHand[1].TrueValue - 1, (int)dealerCard.TrueValue ];
						}
						else if( playerHand[1].FaceValue == Card.CardType.Ace )
						{
							Advice = (AdviceType)Aces[ (int)playerHand[0].TrueValue - 1, (int)dealerCard.TrueValue ];
						}
						else if( playerHand.Total() >= 17 )
						{
							Advice = AdviceType.Stand;
						}
						else
						{
							Advice = (AdviceType)Hand[ playerHand.Total() - 3, (int)dealerCard.TrueValue ];
						}
					}
					else if( playerHand.Total() >= 17 )
					{
						Advice = AdviceType.Stand;
					}
					else
					{
						Advice = (AdviceType)Hand[ playerHand.Total() - 3, (int)dealerCard.TrueValue ];
						if( Advice == AdviceType.DoubleDown )
							Advice = AdviceType.Hit;
					}
				}
			}
			catch (Exception Ex)
			{
				string foo = Ex.Message;
			}

			return Advice;
		}
	}

	[Serializable()]
	public class BasicMultiDeck : Strategy
	{
		public BasicMultiDeck()
		{
			// Player has a pair of cards
			//		Dealer Card	     A,2,3,4,5,6,7,8,9,10
			Pairs = new int[10,10] {{P,P,P,P,P,P,P,P,P,P}, // A   P
									{H,H,H,P,P,P,P,H,H,H}, // 2   l
									{H,H,H,P,P,P,P,H,H,H}, // 3   a
									{H,H,H,H,H,H,H,H,H,H}, // 4   y
									{H,D,D,D,D,D,D,D,D,H}, // 5   e
									{H,H,P,P,P,P,H,H,H,H}, // 6   r
									{H,P,P,P,P,P,P,H,H,H}, // 7   P
									{P,P,P,P,P,P,P,P,P,P}, // 8   a
									{S,P,P,P,P,P,S,P,P,S}, // 9   i
									{S,S,S,S,S,S,S,S,S,S}};// 10  r

			PairsLabels = new string[10] 
				{"A-A","2-2","3-3","4-4","5-5","6-6","7-7","8-8","9-9","10-10"};

			// Player has an ace and something else
			//		Dealer Card	     A,2,3,4,5,6,7,8,9,10
			Aces  = new int[9,10]  {{H,H,H,H,D,D,H,H,H,H}, // 2   P
									{H,H,H,H,D,D,H,H,H,H}, // 3   l
									{H,H,H,D,D,D,H,H,H,H}, // 4   a
									{H,H,H,D,D,D,H,H,H,H}, // 5   y
									{H,H,D,D,D,D,H,H,H,H}, // 6   e
									{H,S,D,D,D,D,S,S,H,H}, // 7   r
									{S,S,S,S,S,S,S,S,S,S}, // 8   
									{S,S,S,S,S,S,S,S,S,S}, // 9   
									{S,S,S,S,S,S,S,S,S,S}};// 10  
 
			AcesLabels = new string[9]
				{"A2","A3","A4","A5","A6","A7","A8","A9","A10"};

			// Player has a hard hand without an ace
			//      Dealer Card	     A,2,3,4,5,6,7,8,9,10
			Hand  = new int[14,10] {{H,H,H,H,H,H,H,H,H,H}, // 3   P
									{H,H,H,H,H,H,H,H,H,H}, // 4   l
									{H,H,H,H,H,H,H,H,H,H}, // 5   a
									{H,H,H,H,H,H,H,H,H,H}, // 6   y
									{H,H,H,H,H,H,H,H,H,H}, // 7   e
									{H,H,H,H,H,H,H,H,H,H}, // 8   r
									{H,H,D,D,D,D,H,H,H,H}, // 9   s
									{H,D,D,D,D,D,D,D,D,H}, // 10   
									{H,D,D,D,D,D,D,D,D,D}, // 11  T 
									{H,H,H,S,S,S,H,H,H,H}, // 12  o
									{H,S,S,S,S,S,H,H,H,H}, // 13  t
									{H,S,S,S,S,S,H,H,H,H}, // 14  a
									{H,S,S,S,S,S,H,H,H,H}, // 15  l
									{H,S,S,S,S,S,H,H,H,H}};// 16

			HandLabels = new string[14]
				{"3","4","5","6","7","8","9","10","11","12","13","14","15","16"};
		}

		private string name = "Basic Multi Deck";
		public override string StrategyName{ get{ return name; }}
		public override bool GetInsuranceAdvice( double count, double cardCount, int decks )
		{
			return cardCount >= 3;
		}
		public override AdviceType GetAdvice( Hand playerHand, Card dealerCard, bool allowSplit, double cardCount )
		{
			AdviceType Advice = AdviceType.Null;

			try
			{
				if( dealerCard != null  && playerHand.Count > 0 )
				{
					if( playerHand.Count == 2 && allowSplit )
					{
						if( playerHand[0].FaceValue == playerHand[1].FaceValue )
						{
							Advice = (AdviceType)Pairs[ (int)playerHand[0].TrueValue, (int)dealerCard.TrueValue ];
						}
						else if( playerHand[0].FaceValue == Card.CardType.Ace )
						{
							Advice = (AdviceType)Aces[ (int)playerHand[1].TrueValue - 1, (int)dealerCard.TrueValue ];
						}
						else if( playerHand[1].FaceValue == Card.CardType.Ace )
						{
							Advice = (AdviceType)Aces[ (int)playerHand[0].TrueValue - 1, (int)dealerCard.TrueValue ];
						}
						else if( playerHand.Total() >= 17 )
						{
							Advice = AdviceType.Stand;
						}
						else
						{
							Advice = (AdviceType)Hand[ playerHand.Total() - 3, (int)dealerCard.TrueValue ];
						}
					}
					else if( playerHand.Total() >= 17 )
					{
						Advice = AdviceType.Stand;
					}
					else
					{
						Advice = (AdviceType)Hand[ playerHand.Total() - 3, (int)dealerCard.TrueValue ];
						if( Advice == AdviceType.DoubleDown )
							Advice = AdviceType.Hit;
					}
				}
			}
			catch (Exception Ex)
			{
				string foo = Ex.Message;
			}

			return Advice;
		}
	}

	[Serializable()]
	public class AggresiveMultiDeck : Strategy
	{
		public AggresiveMultiDeck()
		{
			// Player has a pair of cards
			//			Dealer Card	 A,2,3,4,5,6,7,8,9,10
			Pairs = new int[10,10] {{P,P,P,P,P,P,P,P,P,P}, // A   P
									{P,P,P,P,P,P,P,P,P,P}, // 2   l
									{P,P,P,P,P,P,P,P,P,P}, // 3   a
									{H,H,P,P,P,P,H,H,H,H}, // 4   y
									{H,D,D,D,D,D,D,D,D,H}, // 5   e
									{H,P,P,P,P,P,H,H,H,H}, // 6   r
									{H,P,P,P,P,P,P,H,H,H}, // 7   P
									{P,P,P,P,P,P,P,P,P,P}, // 8   a
									{P,P,P,P,P,P,P,P,P,S}, // 9   i
									{P,P,P,P,P,P,P,P,P,S}};// 10  r

			PairsLabels = new string[10] 
			{"A-A","2-2","3-3","4-4","5-5","6-6","7-7","8-8","9-9","10-10"};

			// Player has an ace and something else
			//			Dealer Card	 A,2,3,4,5,6,7,8,9,10
			Aces  = new int[9,10]  {{H,H,H,H,D,D,H,H,H,H}, // 2   P
									{H,H,H,H,D,D,H,H,H,H}, // 3   l
									{H,H,H,D,D,D,H,H,H,H}, // 4   a
									{H,H,H,D,D,D,H,H,H,H}, // 5   y
									{H,H,D,D,D,D,H,H,H,H}, // 6   e
									{H,D,D,D,D,D,S,S,H,H}, // 7   r
									{S,D,D,D,D,D,S,S,S,S}, // 8   
									{S,D,D,D,D,D,S,S,S,S}, // 9   
									{S,D,D,D,D,D,S,S,S,S}};// 10  
 
			AcesLabels = new string[9]
				{"A2","A3","A4","A5","A6","A7","A8","A9","A10"};

			// Player has a hard hand without an ace
			//         Dealer Card	 A,2,3,4,5,6,7,8,9,10
			Hand  = new int[14,10] {{H,H,H,H,H,H,H,H,H,H}, // 3   P
									{H,H,H,H,H,H,H,H,H,H}, // 4   l
									{H,H,H,H,H,H,H,H,H,H}, // 5   a
									{H,H,H,H,H,H,H,H,H,H}, // 6   y
									{H,H,H,H,H,H,H,H,H,H}, // 7   e
									{H,H,H,H,H,H,H,H,H,H}, // 8   r
									{D,D,D,D,D,D,D,D,D,H}, // 9   s
									{D,D,D,D,D,D,D,D,D,D}, // 10   
									{D,D,D,D,D,D,D,D,D,D}, // 11  T 
									{H,H,H,S,S,S,H,H,H,H}, // 12  o
									{H,S,S,S,S,S,H,H,H,H}, // 13  t
									{H,S,S,S,S,S,H,H,H,H}, // 14  a
									{H,S,S,S,S,S,H,H,H,H}, // 15  l
									{H,S,S,S,S,S,H,H,H,H}};// 16

			HandLabels = new string[14]
				{"3","4","5","6","7","8","9","10","11","12","13","14","15","16"};
		}

		private string name = "Aggressive Multi Deck";
		public override string StrategyName{ get{ return name; }}
		public override bool GetInsuranceAdvice( double count, double cardCount, int decks )
		{
			return cardCount >= 3;
		}
		public override AdviceType GetAdvice( Hand playerHand, Card dealerCard, bool allowSplit, double cardCount )
		{
			AdviceType Advice = AdviceType.Null;

			try
			{
				if( dealerCard != null  && playerHand.Count > 0 )
				{
					if( playerHand.Count == 2 && allowSplit )
					{
						if( playerHand[0].FaceValue == playerHand[1].FaceValue )
						{
							Advice = (AdviceType)Pairs[ (int)playerHand[0].TrueValue, (int)dealerCard.TrueValue ];
						}
						else if( playerHand[0].FaceValue == Card.CardType.Ace )
						{
							Advice = (AdviceType)Aces[ (int)playerHand[1].TrueValue - 1, (int)dealerCard.TrueValue ];
						}
						else if( playerHand[1].FaceValue == Card.CardType.Ace )
						{
							Advice = (AdviceType)Aces[ (int)playerHand[0].TrueValue - 1, (int)dealerCard.TrueValue ];
						}
						else if( playerHand.Total() >= 17 )
						{
							Advice = AdviceType.Stand;
						}
						else
						{
							Advice = (AdviceType)Hand[ playerHand.Total() - 3, (int)dealerCard.TrueValue ];
						}
					}
					else if( playerHand.Total() >= 17 )
					{
						Advice = AdviceType.Stand;
					}
					else
					{
						Advice = (AdviceType)Hand[ playerHand.Total() - 3, (int)dealerCard.TrueValue ];
						if( Advice == AdviceType.DoubleDown )
							Advice = AdviceType.Hit;
					}
				}
			}
			catch (Exception Ex)
			{
				string foo = Ex.Message;
			}

			return Advice;
		}
	}

	[Serializable()]
	public class SmartMultiDeck : Strategy
	{
		public SmartMultiDeck()
		{
			// Player has a pair of cards
			//		Dealer Card	      A,2,3,4,5,6,7,8,9,10
			Pairs = new int[10,10]  {{P,P,P,P,P,P,P,P,P,P}, // A   P
									 {H,H,H,P,P,P,P,H,H,H}, // 2   l
									 {H,H,H,P,P,P,P,H,H,H}, // 3   a
									 {H,H,H,H,H,H,H,H,H,H}, // 4   y
									 {H,D,D,D,D,D,D,D,D,H}, // 5   e
									 {H,H,P,P,P,P,H,H,H,H}, // 6   r
									 {H,P,P,P,P,P,P,H,H,H}, // 7   P
									 {P,P,P,P,P,P,P,P,P,P}, // 8   a
									 {S,P,P,P,P,P,S,P,P,S}, // 9   i
									 {S,P,P,P,P,P,P,S,S,S}};// 10  r

			PairsLabels = new string[10] 
				{"A-A","2-2","3-3","4-4","5-5","6-6","7-7","8-8","9-9","10-10"};

			// Player has an ace and something else
			//		Dealer Card	      A,2,3,4,5,6,7,8,9,10
			Aces  = new int[9,10]   {{H,H,H,H,D,D,H,H,H,H}, // 2   P
									 {H,H,H,H,D,D,H,H,H,H}, // 3   l
									 {H,H,H,D,D,D,H,H,H,H}, // 4   a
									 {H,H,H,D,D,D,H,H,H,H}, // 5   y
									 {H,H,D,D,D,D,H,H,H,H}, // 6   e
									 {H,S,D,D,D,D,S,S,H,H}, // 7   r
									 {S,S,S,S,S,S,S,S,S,S}, // 8   
									 {S,S,S,S,S,S,S,S,S,S}, // 9   
									 {S,S,S,S,S,S,S,S,S,S}};// 10   

			AcesLabels = new string[9]
				{"A2","A3","A4","A5","A6","A7","A8","A9","A10"};

			// Player has a hard hand without an ace
			//      Dealer Card	      A,2,3,4,5,6,7,8,9,10
			Hand  = new int[14,10]  {{H,H,H,H,H,H,H,H,H,H}, // 3   P
									 {H,H,H,H,H,H,H,H,H,H}, // 4   l
									 {H,H,H,H,H,H,H,H,H,H}, // 5   a
									 {H,H,H,H,H,H,H,H,H,H}, // 6   y
									 {H,H,H,H,H,H,H,H,H,H}, // 7   e
									 {H,H,H,H,H,H,H,H,H,H}, // 8   r
									 {H,H,D,D,D,D,H,H,H,H}, // 9   s
									 {H,D,D,D,D,D,D,D,D,H}, // 10   
									 {H,D,D,D,D,D,D,D,D,D}, // 11  T 													  {H,H,H,S,S,S,H,H,H,H}, // SD  o
									 {H,S,S,S,S,H,H,H,H,H}, // 12  o
									 {H,S,S,S,S,S,H,H,H,H}, // 13  t
									 {H,S,S,S,S,S,H,H,H,H}, // 14  a
									 {H,S,S,S,S,S,H,H,H,H}, // 15  l
									 {H,S,S,S,S,S,H,H,H,H}};// 16

			HandLabels = new string[14]
				{"3","4","5","6","7","8","9","10","11","12","13","14","15","16"};

		}

		// Player has a hard hand without an ace, count is high (lots of tens)
		//                      Dealer Card	     A,2,3,4,5,6,7,8,9,10
		private int[,] Hand2  = new int[14,10] {{H,H,H,H,H,H,H,H,H,H}, // 3   P
												{H,H,H,H,H,H,H,H,H,H}, // 4   l
												{H,H,H,H,H,H,H,H,H,H}, // 5   a
												{H,H,H,H,H,H,H,H,H,H}, // 6   y
												{H,H,H,H,H,H,H,H,H,H}, // 7   e
												{H,D,D,D,D,D,D,H,H,H}, // 8   r
												{H,D,D,D,D,D,D,D,H,H}, // 9   s
												{H,D,D,D,D,D,D,D,D,H}, // 10   
												{H,D,D,D,D,D,D,D,D,D}, // 11  T 
												{H,S,S,S,S,H,H,H,H,H}, // 12  o
												{H,S,S,S,S,S,H,H,H,H}, // 13  t
												{H,S,S,S,S,S,S,H,H,H}, // 14  a
												{H,S,S,S,S,S,S,S,H,H}, // 15  l
												{H,S,S,S,S,S,S,S,S,H}};// 16

		private string name = "Smart Multi Deck";
		public override string StrategyName{ get{ return name; }}
		public override bool GetInsuranceAdvice( double count, double cardCount, int decks )
		{
			return cardCount >= 3;
		}
		public override AdviceType GetAdvice( Hand playerHand, Card dealerCard, bool allowSplit, double cardCount )
		{
			AdviceType Advice = AdviceType.Null;

			try
			{
				if( dealerCard != null  && playerHand.Count > 0 )
				{
					if( playerHand.Count == 2 && allowSplit )
					{
						if( playerHand[0].FaceValue == playerHand[1].FaceValue )
						{
							Advice = (AdviceType)Pairs[ (int)playerHand[0].TrueValue, (int)dealerCard.TrueValue ];
						}
						else if( playerHand[0].FaceValue == Card.CardType.Ace )
						{
							Advice = (AdviceType)Aces[ (int)playerHand[1].TrueValue - 1, (int)dealerCard.TrueValue ];
						}
						else if( playerHand[1].FaceValue == Card.CardType.Ace )
						{
							Advice = (AdviceType)Aces[ (int)playerHand[0].TrueValue - 1, (int)dealerCard.TrueValue ];
						}
						else if( playerHand.Total() >= 17 )
						{
							Advice = AdviceType.Stand;
						}
						else
						{
							if( cardCount > 0 )
								Advice = (AdviceType)Hand2[ playerHand.Total() - 3, (int)dealerCard.TrueValue ];
							else
								Advice = (AdviceType)Hand[ playerHand.Total() - 3, (int)dealerCard.TrueValue ];
						}
					}
					else if( playerHand.Total() >= 17 )
					{
						Advice = AdviceType.Stand;
					}
					else
					{
						if( cardCount > 0 )
							Advice = (AdviceType)Hand2[ playerHand.Total() - 3, (int)dealerCard.TrueValue ];
						else
							Advice = (AdviceType)Hand[ playerHand.Total() - 3, (int)dealerCard.TrueValue ];

						if( Advice == AdviceType.DoubleDown )
							Advice = AdviceType.Hit;
					}
				}
			}
			catch (Exception Ex)
			{
				string foo = Ex.Message;
			}

			return Advice;
		}
	}

	[Serializable()]
	public class HighLowMultiDeck : Strategy
	{

		public HighLowMultiDeck()
		{
			// Split, No Double After, S17 (p.38)
			//		Dealer Card	      A, 2, 3, 4, 5, 6, 7, 8, 9,10
			Pairs = new int[10,10] {{-3, P, P, P, P, P,-9,-8,-7,-8}, // A   P
									{ L,-3,-5,-7,-9, P, P, 5, L, L}, // 2   l
									{ L, 0,-4,-7,-9, P, P, 4, L, L}, // 3   a
									{ L, L, 8, 3,-1,-2, L, L, L, L}, // 4   y
									{ L, L, L, L, L, L, L, L, L, L}, // 5   e
									{ L,-1,-4,-6,-8,-10,L, L, L, L}, // 6   r
									{ L,-9, P, P, P, P, P, 5, L, L}, // 7   P
									{ P, P, P, P, P, P, P, P, P,98}, // 8   a
									{ 3,-2,-3,-5,-6,-6, 3,-8,-9, L}, // 9   i
									{ L, L, 8, 6, 5, 4, L, L, L, L}};// 10  r

			PairsLabels = new string[10] 
				{"A-A","2-2","3-3","4-4","5-5","6-6","7-7","8-8","9-9","10-10"};

			// Player has an ace and something else
			//		Dealer Card	      A, 2, 3, 4, 5, 6, 7, 8, 9,10
			Aces  = new int[19,10] {{ H, H, H, H, H, H, H, H, H, H}, // 2   P
									{ H, H, H, H, H, H, H, H, H, H}, // 3   l
									{ H, H, H, H, H, H, H, H, H, H}, // 4   a
									{ H, H, H, H, H, H, H, H, H, H}, // 5   y
									{ H, H, H, H, H, H, H, H, H, H}, // 6   e
									{ H, H, H, H, H, H, H, H, H, H}, // 7   r
									{ H, H, H, H, H, H, H, H, H, H}, // 8   s
									{ H, H, H, H, H, H, H, H, H, H}, // 9   
									{ H, H, H, H, H, H, H, H, H, H}, // 10  T
									{ H, H, H, H, H, H, H, H, H, H}, // 11  o
									{ H, H, H, H, H, H, H, H, H, H}, // 12  t
									{ H, H, H, H, H, H, H, H, H, H}, // 13  a
									{ H, H, H, H, H, H, H, H, H, H}, // 14  l
									{ H, H, H, H, H, H, H, H, H, H}, // 15   
									{ H, H, H, H, H, H, H, H, H, H}, // 16   
									{ H, H, H, H, H, H, H, H, H, H}, // 17   
									{ 1, S, S, S, S, S, S, S, H, H}, // 18   
									{ S, S, S, S, S, S, S, S, S, S}, // 19   
									{ S, S, S, S, S, S, S, S, S, S}};// 20
	 
			AcesLabels = new string[8]
				{"A2","A3","A4","A5","A6","A7","A8","A9"};

			// Double Down (hard hand), S17 (p.41)
			//         Dealer Card	   A, 2, 3, 4, 5, 6, 7, 8, 9,10
			DoubleH = new int[18,10]{{ L, L, L, L, L, L, L, L, L, L}, // 3   P
									 { L, L, L, L, L, L, L, L, L, L}, // 4   l
									 { L, L, L, L, L, L, L, L, L, L}, // 5   a
									 { L, L, L, L, L, L, L, L, L, L}, // 6   y
									 { L, L, L, L, 9, 9, L, L, L, L}, // 7   e
									 { L, L, 9, 5, 3, 1, L, L, L, L}, // 8   r
									 { L, 1, 0,-2,-4,-6, 3, 7, L, L}, // 9   s
									 { 4,-8,-9,-10,D, D,-6,-4,-1, 4}, // 10   
									 { 1, D, D, D, D, D,-9,-6,-4,-4}, // 11  T 													  {L, L, L, L, L, L, L, L, L, L},  // LL  o
									 { L, L, L, L, L, L, L, L, L, L}, // 12  o
									 { L, L, L, L, L, L, L, L, L, L}, // 13  t
									 { L, L, L, L, L, L, L, L, L, L}, // 14  a
									 { L, L, L, L, L, L, L, L, L, L}, // 15  l
									 { L, L, L, L, L, L, L, L, L, L}, // 16
									 { L, L, L, L, L, L, L, L, L, L}, // 17
									 { L, L, L, L, L, L, L, L, L, L}, // 18
									 { L, L, L, L, L, L, L, L, L, L}, // 19
									 { L, L, L, L, L, L, L, L, L, L}};// 20

			// Double Down (soft hand), S17 (p.41)
			//          Dealer Card	   A, 2, 3, 4, 5, 6, 7, 8, 9,10
			DoubleS = new int[8,10] {{ L, L, 7, 3, 0,-1, L, L, L, L}, // 2   P
									 { L, L, 7, 1,-1,-4, L, L, L, L}, // 3   l
									 { L, L, 7, 0,-4,-9, L, L, L, L}, // 4   a
									 { L, L, 4,-2,-6, D, L, L, L, L}, // 5   y
									 { L, 1,-3,-7,-10,D, L, L, L, L}, // 6   e
									 { L, 0,-2,-6,-8,-10,L, L, L, L}, // 7   r
									 { L, 8, 5, 3, 1, 1, L, L, L, L}, // 8
									 { L,10, 8, 6, 5, 4, L, L, L, L}};// 9   


			DoubleLabels = new string[18] 
				{"3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20"};

			// Player has a hard hand without an ace
			//            Dealer Card  A, 2, 3, 4, 5, 6, 7, 8, 9,10
			Hand  = new int[18,10] {{ H, H, H, H, H, H, H, H, H, H}, // 3   P
									{ H, H, H, H, H, H, H, H, H, H}, // 4   l
									{ H, H, H, H, H, H, H, H, H, H}, // 5   a
									{ H, H, H, H, H, H, H, H, H, H}, // 6   y
									{ H, H, H, H, H, H, H, H, H, H}, // 7   e
									{ H, H, H, H, H, H, H, H, H, H}, // 8   r
									{ H, H, H, H, H, H, H, H, H, H}, // 9   s
									{ H, H, H, H, H, H, H, H, H, H}, // 10   
									{ H, H, H, H, H, H, H, H, H, H}, // 11  T 													  {H,H,H,S,S,S,H,H,H,H}, // SD  o
									{ H, 3, 2, 0,-1, 0, H, H, H, H}, // 12  o
									{ H, 0,-1,-3,-4,-4, H, H, H, H}, // 13  t
									{ H,-3,-4,-6,-7,-7, H, H, H, H}, // 14  a
									{10,-5,-6,-7,-9,-9,10,10, 8, 4}, // 15  l
									{ 8,-8,-10,S, S, S, 9, 7, 5, 0}, // 16
									{-6, S, S, S, S, S, S, S, S, S}, // 17
									{ L, L, L, L, L, L, L, L, L, L}, // 18
									{ L, L, L, L, L, L, L, L, L, L}, // 19
									{ L, L, L, L, L, L, L, L, L, L}};// 20

			HandLabels = new string[13]
				{"5","6","7","8","9","10","11","12","13","14","15","16","17"};

		}

		private string name = "High Low Multi Deck";
		public override string StrategyName{ get{ return name; }}
		public override bool GetInsuranceAdvice( double count, double cardCount, int decks )
		{
			return count > decks * 4;
		}
		public override void Draw( Graphics drawingSurface, Point origin, Rectangle size, Rectangle space, Hand currentHand, Card dealerCard, double count )
		{
			SizeF labelSize;
			bool highlight = false;

			// Get largest string and move the table over to make room for the labels
			try
			{
				labelSize = drawingSurface.MeasureString("Seven",new Font(fontName,fontSize));
			}
			catch
			{
				labelSize = drawingSurface.MeasureString("Seven",new Font("Arial",fontSize));
			}

			StringFormat myFormat = new StringFormat();
			myFormat.Alignment = StringAlignment.Center;

			// Draw the Dealer's header row
			int dealerRowHeight = (int)labelSize.Height;
			drawingSurface.DrawString("Dealer's up card", new Font(fontName,fontSize),Brushes.Black, origin.X + labelSize.Width + (size.Width*10 + space.Width*10)/2, origin.Y, myFormat);

			// Draw the Dealer's up card labels
			for(int k=0; k<10; k++)
			{
				drawingSurface.DrawString(((Card.CardType)k).ToString(CultureInfo.InvariantCulture), new Font(fontName,fontSize), Brushes.Black, origin.X + k*space.Width + k*size.Width + (int)labelSize.Width + size.Width/2, origin.Y + dealerRowHeight, myFormat);
			}

			// Draw the Player's header column
			//			StringFormat myVerticalFormat = new StringFormat();
			//			myVerticalFormat.FormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft;
			//			myVerticalFormat.Alignment = StringAlignment.Center;
			//			g.DrawString("Player's Hand",new Font(fontName,fontSize),Brushes.Black,origin.X + (int)labelSize.Width+ (int)labelSize.Height + size.Width*10 + space.Width*10,origin.Y + dealerRowHeight + (size.Height*10 + space.Height*10)/2,myVerticalFormat);
			
			// Draw the actual Advice values, Hard Total | Soft Total | Aces | Pairs
			myFormat.Alignment = StringAlignment.Center;
			myFormat.LineAlignment = StringAlignment.Center;

			int[] tmpCard1 = new int[] {2,3,3,2,2,2,2,2,2,8,8,8,8};
			int[] tmpCard2 = new int[] {1,1,2,4,5,6,7,8,9,4,5,6,7};

			for(int j=0; j<=12; j++)
			{
				drawingSurface.DrawString(HandLabels[j].ToString(CultureInfo.InvariantCulture), new Font(fontName,fontSize), Brushes.Black, origin.X + (int)labelSize.Width/2, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + size.Height/2 + dealerRowHeight, myFormat);
				for(int i=0; i<10; i++)
				{
					Hand tmpHand = new Hand();
					tmpHand.Add( new Card((Card.CardType)tmpCard1[j], Card.Suits.Clubs));
					tmpHand.Add( new Card((Card.CardType)tmpCard2[j], Card.Suits.Clubs));
					
					highlight = false;
					if( currentHand != null && dealerCard != null && !currentHand.IsPair && currentHand.Total() == tmpHand.Total() && dealerCard.TrueValue == i && (!currentHand.IsSoft || currentHand.Count > 2) )
						highlight = true;

					AdviceType advice = GetAdvice(tmpHand, new Card((Card.CardType)i, Card.Suits.Clubs), false, count);

					DrawSquare( drawingSurface, advice, highlight, (currentHand != null && currentHand.Count == 2), new Rectangle(origin.X + i*space.Width + i*size.Width + (int)labelSize.Width, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + dealerRowHeight, size.Width, size.Height ) );
				}
			}

			origin.Y += size.Height * 13 + space.Height * 13;

			// Draw the 18+ row
			drawingSurface.DrawString("18+", new Font(fontName,fontSize), Brushes.Black, origin.X + (int)labelSize.Width/2, origin.Y + (int)labelSize.Height + size.Height/2 + dealerRowHeight, myFormat );
			for(int j=0; j<10; j++)
			{				
				highlight = false;
				if( currentHand != null && dealerCard != null && !currentHand.IsPair && currentHand.Total() >= 18 && dealerCard.TrueValue == j && (!currentHand.IsSoft || currentHand.Count > 2))
					highlight = true;

				DrawSquare( drawingSurface, AdviceType.Stand, highlight, (currentHand != null && currentHand.Count == 2), new Rectangle(origin.X + j*space.Width + j*size.Width + (int)labelSize.Width, origin.Y + (int)labelSize.Height + dealerRowHeight, size.Width, size.Height ) );
			}

			origin.Y += size.Height + space.Height;

			// Draw the soft totals S13 - S20
			string[] softLabels = new string[] {"S14","S15","S16","S17","S18","S19","S20"};

			for(int j=0; j<7; j++)
			{
				drawingSurface.DrawString(softLabels[j].ToString(CultureInfo.InvariantCulture), new Font(fontName,fontSize), Brushes.Black, origin.X + (int)labelSize.Width/2, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + size.Height/2 + dealerRowHeight, myFormat);
				for(int i=0; i<10; i++)
				{
					Hand tmpHand = new Hand();
					tmpHand.Add( new Card((Card.CardType)0, Card.Suits.Clubs));
					tmpHand.Add( new Card((Card.CardType)1, Card.Suits.Clubs));
					tmpHand.Add( new Card((Card.CardType)j, Card.Suits.Clubs));
					
					highlight = false;
					if( currentHand != null && dealerCard != null && !currentHand.IsPair && currentHand.Total() == tmpHand.Total() && dealerCard.TrueValue == i && currentHand.IsSoft && currentHand.Count > 2 )
						highlight = true;

					AdviceType advice = GetAdvice(tmpHand, new Card((Card.CardType)i, Card.Suits.Clubs), false, count);

					DrawSquare( drawingSurface, advice, highlight, false, new Rectangle(origin.X + i*space.Width + i*size.Width + (int)labelSize.Width, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + dealerRowHeight, size.Width, size.Height ) );
				}
			}

			origin.Y += size.Height * 7 + space.Height * 7;

			// Draw the strategies for soft hands
			for(int j=0; j<8; j++)
			{
				drawingSurface.DrawString(AcesLabels[j], new Font(fontName,fontSize), Brushes.Black, origin.X + (int)labelSize.Width/2, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + size.Height/2 + dealerRowHeight, myFormat);
				for(int i=0; i<10; i++)
				{
					Hand tmpHand = new Hand();
					tmpHand.Add( new Card((Card.CardType)0, Card.Suits.Clubs));
					tmpHand.Add( new Card((Card.CardType)j+1, Card.Suits.Clubs));

					highlight = false;
					if( currentHand != null && dealerCard != null && dealerCard.TrueValue == i && !currentHand.IsPair && currentHand.IsSoft && currentHand.Count == 2 && currentHand.Total() == tmpHand.Total() )
						highlight = true;

					DrawSquare( drawingSurface, GetAdvice(tmpHand, new Card((Card.CardType)i, Card.Suits.Clubs), (currentHand != null && currentHand.Count == 2), count), highlight, (currentHand != null && currentHand.Count == 2), new Rectangle(origin.X + i*space.Width + i*size.Width + (int)labelSize.Width, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + dealerRowHeight, size.Width, size.Height ) );
				}
			}

			origin.Y += size.Height * 8 + space.Height * 8;

			// Draw the strategies for pairs
			for(int j=0; j<10; j++)
			{
				drawingSurface.DrawString(PairsLabels[j], new Font(fontName,fontSize), Brushes.Black, origin.X + (int)labelSize.Width/2, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + size.Height/2 + dealerRowHeight, myFormat);
				for(int i=0; i<10; i++)
				{
					Hand tmpHand = new Hand();
					tmpHand.Add( new Card((Card.CardType)j, Card.Suits.Clubs));
					tmpHand.Add( new Card((Card.CardType)j, Card.Suits.Clubs));

					highlight = false;
					if( currentHand != null && dealerCard != null && currentHand.IsPair )
					{
						if( currentHand[0].TrueValue == tmpHand[0].TrueValue && dealerCard.TrueValue == i )
							highlight = true;
					}
					DrawSquare( drawingSurface, GetAdvice(tmpHand, dealerCard, true, count), highlight, (currentHand != null && currentHand.Count == 2), new Rectangle(origin.X + i*space.Width + i*size.Width + (int)labelSize.Width, origin.Y + j*space.Height + j*size.Height + (int)labelSize.Height + dealerRowHeight, size.Width, size.Height ) );
				}
			}
		}

		public override AdviceType GetAdvice( Hand playerHand, Card dealerCard, bool allowSplit, double cardCount )
		{
			AdviceType Advice = AdviceType.Null;
			AdviceType suggestedAdvice = AdviceType.Null;

			try
			{
				if( dealerCard != null && playerHand.Count > 0 )
				{
					if( playerHand.Count == 2 && allowSplit )
					{
						if( playerHand[0].FaceValue == playerHand[1].FaceValue )
						{
							Advice = (AdviceType)Pairs[ (int)playerHand[0].TrueValue, (int)dealerCard.TrueValue ];
							suggestedAdvice = AdviceType.Split;
						}
					}

					if( playerHand.Count == 2 )
					{
						if( Advice == AdviceType.Null && playerHand[0].FaceValue == Card.CardType.Ace )
						{
							suggestedAdvice = AdviceType.DoubleDown;
							Advice = (AdviceType)DoubleS[ playerHand[1].TrueValue - 1, (int)dealerCard.TrueValue ];
						}
					
						if( Advice == AdviceType.Null && playerHand[1].FaceValue == Card.CardType.Ace )
						{
							suggestedAdvice = AdviceType.DoubleDown;
							Advice = (AdviceType)DoubleS[ playerHand[0].TrueValue - 1, (int)dealerCard.TrueValue ];
						}
					
						if( Advice == AdviceType.Null )
						{
							suggestedAdvice = AdviceType.DoubleDown;
							Advice = (AdviceType)DoubleH[ playerHand.Total() - 3, (int)dealerCard.TrueValue ];
						}
					}

					if( Advice == AdviceType.Null && playerHand.IsSoft )
					{
						suggestedAdvice = AdviceType.Stand;
						Advice = (AdviceType)Aces[ playerHand.Total() - 2, (int)dealerCard.TrueValue ];
					}
					
					if( Advice == AdviceType.Null )
					{
						suggestedAdvice = AdviceType.Stand;
						Advice = (AdviceType)Hand[ playerHand.Total() - 3, (int)dealerCard.TrueValue ];
					}

					if( Math.Abs((int)Advice) < 100 )
					{
						if( (int)Advice > 90 )
						{
							if( cardCount < (int)Advice-90 )
								Advice = suggestedAdvice;
							else
								Advice = AdviceType.Null;
						}
						else
						{
							if( cardCount > (int)Advice )
								Advice = suggestedAdvice;
							else
								Advice = AdviceType.Null;
						}
					}

					if( Advice == AdviceType.Null && playerHand.Total() >= 18 )
						Advice = AdviceType.Stand;
					else if( Advice == AdviceType.Null )
						Advice = AdviceType.Hit;
				}
			}
			catch //(Exception Ex)
			{
				//string foo = Ex.Message;
			}

			return Advice;
		}
	}
}
