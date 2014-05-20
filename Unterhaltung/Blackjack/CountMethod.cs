using System;
using System.Collections;

namespace kioskplus.Unterhaltung.Blackjack
{
	/// <summary>
	/// Summary description for CountMethod.
	/// </summary>
	[Serializable()]
	public abstract class CountMethod
	{
		protected CountMethod( int numberOfDecks )
		{
			Decks = numberOfDecks;
		}

		protected int Decks;
		protected double runningTotal;
		protected int cardCount;
		protected int acesCount;
		protected int twosCount;
		protected int threesCount;
		protected int foursCount;
		protected int fivesCount;
		protected int sixesCount;
		protected int sevensCount;
		protected int eightsCount;
		protected int ninesCount;
		protected int tensCount;

		protected virtual double[] GetCounts() { return null; }
		protected int SideCount( Card.CardType cardType )
		{
			int count = 0;

			switch( cardType )
			{
				case Card.CardType.Ace:
					count = acesCount;
					break;
				case Card.CardType.Two:
					count = twosCount;
					break;
				case Card.CardType.Three:
					count = threesCount;
					break;
				case Card.CardType.Four:
					count = foursCount;
					break;
				case Card.CardType.Five:
					count = fivesCount;
					break;
				case Card.CardType.Six:
					count = sixesCount;
					break;
				case Card.CardType.Seven:
					count = sevensCount;
					break;
				case Card.CardType.Eight:
					count = eightsCount;
					break;
				case Card.CardType.Nine:
					count = ninesCount;
					break;
				case Card.CardType.Ten:
					count = tensCount;
					break;
			}

			return count;
		}
		public virtual double Insurance10Count()
		{
			double count = acesCount +
							twosCount +
							threesCount +
							foursCount +
							fivesCount +
							sixesCount +
							sevensCount +
							eightsCount +
							ninesCount +
							(tensCount * -2);
			return count;
		}

		public virtual double GetWager( double normalBet )
		{
			double wager = 0;
			double trueCount = Count;

			if( trueCount > 0 )
				wager = normalBet * trueCount;
			else if( trueCount == 0 )
				wager = normalBet;
			else if( trueCount < 0 )
				wager = normalBet * trueCount;

			// $10 table minimum :)  Also, round to nearest integer
			wager = (int)Math.Max( 5, wager );

			return wager;
		}

		public virtual string MethodName{ get{ return "";}}
		public virtual int MethodLevel{ get{ return 0; }}
		public virtual void CountCard( Card card )
		{
			runningTotal += GetCounts()[ (int)card.TrueValue ];
			cardCount++;

			// Now keep side counts for all cards
			switch( card.FaceValue )
			{
				case Card.CardType.Ace:
					acesCount++;
					break;
				case Card.CardType.Two:
					twosCount++;
					break;
				case Card.CardType.Three:
					threesCount++;
					break;
				case Card.CardType.Four:
					foursCount++;
					break;
				case Card.CardType.Five:
					fivesCount++;
					break;
				case Card.CardType.Six:
					sixesCount++;
					break;
				case Card.CardType.Seven:
					sevensCount++;
					break;
				case Card.CardType.Eight:
					eightsCount++;
					break;
				case Card.CardType.Nine:
					ninesCount++;
					break;
				case Card.CardType.Ten:
					tensCount++;
					break;
				case Card.CardType.Jack:
					tensCount++;
					break;
				case Card.CardType.Queen:
					tensCount++;
					break;
				case Card.CardType.King:
					tensCount++;
					break;
			}
		}

		public virtual void Reset( int decks )
		{
			cardCount = 0;
			runningTotal = 0;
			acesCount = 0;
			twosCount = 0;
			threesCount = 0;
			foursCount = 0;
			fivesCount = 0;
			sixesCount = 0;
			sevensCount = 0;
			eightsCount = 0;
			ninesCount = 0;
			tensCount = 0;
			Decks = decks;
		}

		public virtual double Count
		{
			get{ return runningTotal / (( Decks*52 - cardCount ) / 52 ); }
		}

		public static ArrayList GetMethods()
		{
			ArrayList methods = new ArrayList();

			methods.Add( "Hi-Lo" );
			methods.Add( "High-Low" );
			methods.Add( "Hi Opt I" );
			methods.Add( "Hi Opt II" );
			methods.Add( "Silver Fox" );
			methods.Add( "Brh I" );
			methods.Add( "Brh II" );
			methods.Add( "Canfield Expert" );
			methods.Add( "Canfield Master" );
			methods.Add( "KO" );
			methods.Add( "Omega II" );
			methods.Add( "Red Seven" );
			methods.Add( "Revere Adv. Plus Minus" );
			methods.Add( "Revere Point Count" );
			methods.Add( "Unb. Zen 11" );
			methods.Add( "Uston Adv. Plus Minus" );
			methods.Add( "Uston APC" );
			methods.Add( "Uston SS" );
			methods.Add( "Wong Halves" );
			methods.Add( "Zen Count" );
			methods.Add( "Hi-Lo Ace Side Count" );
			methods.Add( "HiOptI Ace-Seven Side Count" );

			return methods;
		}
		public static CountMethod NewMethod( string methodName, int n )
		{
			CountMethod returnValue = null;

			switch( methodName.ToLower() )
			{
				case( "hi-lo" ):
					returnValue = new HiLo( n );
					break;
				case( "high-low" ):
					returnValue = new HighLow( n );
					break;
				case( "hi opt i" ):
					returnValue = new HiOptI( n );
					break;
				case( "hi opt ii" ):
					returnValue = new HiOptII( n );
					break;
				case( "silver fox" ):
					returnValue = new SilverFox( n );
					break;
				case( "brh i" ):
					returnValue = new BrhI( n );
					break;
				case( "brh ii" ):
					returnValue = new BrhII( n );
					break;
				case( "canfield expert" ):
					returnValue = new CanfieldExpert( n );
					break;
				case( "canfield master" ):
					returnValue = new CanfieldMaster( n );
					break;
				case( "ko" ):
					returnValue = new KO( n );
					break;
				case( "omega ii" ):
					returnValue = new OmegaII( n );
					break;
				case( "red seven" ):
					returnValue = new RedSeven( n );
					break;
				case( "revere adv. plus minus" ):
					returnValue = new RevereAdvPlusMinus( n );
					break;
				case( "revere point count" ):
					returnValue = new ReverePointCount( n );
					break;
				case( "unb. zen 11" ):
					returnValue = new UnbZen11( n );
					break;
				case( "uston adv. plus minus" ):
					returnValue = new UstonAdvPlusMinus( n );
					break;
				case( "uston apc" ):
					returnValue = new UstonApc( n );
					break;
				case( "uston ss" ):
					returnValue = new UstonSS( n );
					break;
				case( "wong halves" ):
					returnValue = new WongHalves( n );
					break;
				case( "zen count" ):
					returnValue = new ZenCount( n );
					break;
				case( "hi-lo ace side count" ):
					returnValue = new HiLoA( n );
					break;
				case( "hiopti ace-seven side count" ):
					returnValue = new HiOptIA7( n );
					break;
				default:
					returnValue = null;
					break;
			}

			return returnValue;
		}
	}

	[Serializable()]
	public class HiLo : CountMethod
	{
		public HiLo( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] { 0, 0, 1, 1, 1, 1, 0, 0, 0,-1 };
		private string name = "Hi-Lo";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class HighLow : CountMethod
	{
		public HighLow( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] {-1, 1, 1, 1, 1, 1, 0, 0, 0,-1 };
		private string name = "High-Low";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class HiOptI : CountMethod
	{
		public HiOptI( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] { 0, 0, 1, 1, 1, 1, 0, 0, 0,-1 };
		private string name = "HiOptI";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class HiOptII : CountMethod
	{
		public HiOptII( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] { 0, 1, 1, 2, 2, 1, 1, 0, 0,-2 };
		private string name = "HiOptII";
		private int level = 2;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class SilverFox : CountMethod
	{
		public SilverFox( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] {-1, 1, 1, 1, 1, 1, 1, 0,-1,-1 };
		private string name = "Silver Fox";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}
	
	[Serializable()]
	public class BrhI : CountMethod
	{
		public BrhI( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] {-2, 1, 2, 2, 3, 2, 1, 0, 0,-2 };
		private string name = "BrhI";
		private int level = 3;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}
	
	[Serializable()]
	public class BrhII : CountMethod
	{
		public BrhII( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] {-2, 1, 1, 2, 2, 2, 1, 0, 0,-2 };
		private string name = "BrhII";
		private int level = 2;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class CanfieldExpert : CountMethod
	{
		public CanfieldExpert( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] { 0, 0, 1, 1, 1, 1, 1, 0,-1,-1 };
		private string name = "Canfield Expert";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}
	
	[Serializable()]
	public class CanfieldMaster : CountMethod
	{
		public CanfieldMaster( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] { 0, 1, 1, 2, 2, 2, 1, 0,-1,-2 };
		private string name = "Canfield Master";
		private int level = 2;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}
	
	[Serializable()]
	public class KO : CountMethod
	{
		public KO( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] {-1, 1, 1, 1, 1, 1, 1, 0, 0,-1 };
		private string name = "KO";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}
	
	[Serializable()]
	public class OmegaII : CountMethod
	{
		public OmegaII( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] { 0, 0, 1, 1, 1, 1, 1, 0,-1,-1 };
		private string name = "Omega II";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}
	
	[Serializable()]
	public class RedSeven : CountMethod
	{
		public RedSeven( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] {-1, 1, 1, 1, 1, 1,.5, 0, 0,-1 };
		private string name = "Red Seven";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}	

	[Serializable()]
	public class RevereAdvPlusMinus : CountMethod
	{
		public RevereAdvPlusMinus( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] { 0, 1, 1, 1, 1, 1, 0, 0,-1,-1 };
		private string name = "Revere Adv. Plus Minus";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class ReverePointCount : CountMethod
	{
		public ReverePointCount( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] {-2, 1, 2, 2, 2, 2, 1, 0, 0,-2 };
		private string name = "Revere Point Count";
		private int level = 2;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class UnbZen11 : CountMethod
	{
		public UnbZen11( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] {-1, 1, 2, 2, 2, 2, 1, 0, 0,-2 };
		private string name = "Unb. Zen 11";
		private int level = 2;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class UstonAdvPlusMinus : CountMethod
	{
		public UstonAdvPlusMinus( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] {-1, 0, 1, 1, 1, 1, 1, 0, 0,-1 };
		private string name = "Uston Adv. Plus Minus";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class UstonApc : CountMethod
	{
		public UstonApc( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] { 0, 1, 2, 2, 3, 2, 2, 1,-1,-3 };
		private string name = "Uston APC";
		private int level = 3;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class UstonSS : CountMethod
	{
		public UstonSS( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] {-2, 2, 2, 2, 3, 2, 1, 0,-1,-2 };
		private string name = "Uston SS";
		private int level = 2;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class WongHalves : CountMethod
	{
		public WongHalves( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] {-1,.5, 1, 1,1.5,1,.5, 0,-.5,-1 };
		private string name = "Wong Halves";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class ZenCount : CountMethod
	{
		public ZenCount( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] {-1, 1, 1, 2, 2, 2, 1, 0, 0,-2 };
		private string name = "Zen Count";
		private int level = 2;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class HiLoA : CountMethod
	{
		// The standard HiLo method with an Ace side count
		public HiLoA( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] { 0, 0, 1, 1, 1, 1, 0, 0, 0,-1 };
		public override double GetWager( double normalBet )
		{
			double wager = 0;
			double trueCount = Count;
			int aceCount = base.SideCount( Card.CardType.Ace );

			double aceRatio = ((cardCount / 13.0) - aceCount) / (( Decks*52 - cardCount ) / 52.0 );
			trueCount += aceRatio;

			if( trueCount > 0 )
				wager = normalBet * trueCount;
			else if( trueCount == 0 )
				wager = normalBet;
			else if( trueCount < 0 )
				wager = normalBet * trueCount;

			// $10 table minimum :)  Also, round to nearest integer
			wager = (int)Math.Max( 10, wager );

			return wager;
		}

		private string name = "Hi-Lo Ace Side Count";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

	[Serializable()]
	public class HiOptIA7 : CountMethod
	{
		public HiOptIA7( int n ) : base( n )
		{
		}

		protected override double[] GetCounts() { return counts; }
		// Card counts                             A  2  3  4  5  6  7  8  9  T
		protected double[] counts = new double[] { 0, 0, 1, 1, 1, 1, 0, 0, 0,-1 };
		public override double GetWager( double normalBet )
		{
			double wager = 0;
			double trueCount = Count;
			int aceCount = base.SideCount( Card.CardType.Ace );
			int sevenCount = base.SideCount( Card.CardType.Seven );

			double aceRatio = ((cardCount / 13.0) - aceCount) / (( Decks*52 - cardCount ) / 52.0 );
			double sevenRatio = ((cardCount / 13.0) - sevenCount) / (( Decks*52 - cardCount ) / 52.0 );
			trueCount += aceRatio;
			trueCount += sevenRatio;

			if( trueCount > 0 )
				wager = normalBet * trueCount;
			else if( trueCount == 0 )
				wager = normalBet;
			else if( trueCount < 0 )
				wager = normalBet * trueCount;

			// $10 table minimum :)  Also, round to nearest integer
			wager = (int)Math.Max( 10, wager );

			return wager;
		}

		private string name = "Hi Opt I Ace Seven side counts";
		private int level = 1;
		public override string MethodName{ get{ return name; }}
		public override int MethodLevel{ get{ return level; }}
	}

}
