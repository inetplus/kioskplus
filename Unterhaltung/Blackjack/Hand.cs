using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;

namespace kioskplus.Unterhaltung.Blackjack
{
	/// <summary>
	/// Summary description for Hand.
	/// </summary>
	[Serializable()]
	public class Hand : IList, IEnumerable, ICollection
	{
		// A hand consists of an array of cards
		private Card[] cards;
		// Indicates the next position in the cards array
		private int cardPosition;
		// Where is this hand to be drawn on the screen
		private Point location;
		// How much is wagered on this hand of Blackjack
		private double wager;
		// Indicates the player doubled this hand so we can draw the last card on an angle
		private bool doubled;
		// The most cards allowed in a single hand of Blackjack is 12 so we can hard code that
		protected const int maxCardPosition = 12;
		//	How much to increase our array in response to Adds
		private const int RESIZEAMOUNT = 5;

		public enum OutcomeType
		{
			None = 0,
			Won = 1,
			Lost = 2,
			Push = 3,
			Blackjack = 4
		}

		public Hand()
		{
			// Setup an array of cards
			cards = new Card[RESIZEAMOUNT];
			// Flag indicates double down wager.  Last card is drawn tilted at an angle.
			doubled = false;
			// Reset the position within the array
			cardPosition=0;
			// Reset the wager for this hand
			wager = 0;
		}

		public Hand( int size )
		{
			//	Acceptable sizes are 1 - 12
			if (size > 0 && size < 13)
				cards = new Card[size];
			else
				cards = new Card[RESIZEAMOUNT];
			// Flag indicates double down wager.  Last card is drawn tilted at an angle.
			doubled = false;
			// Reset the position within the array
			cardPosition=0;
			// Reset the wager for this hand
			wager = 0;
		}

		public Hand( Point pntLocation )
		{
			// Store the location to draw the hand on the surface
			location = pntLocation;
			// Setup an array of cards
			cards = new Card[RESIZEAMOUNT];
			// Flag indicates double down wager.  Last card is drawn tilted at an angle.
			doubled = false;
			// Reset the position within the array
			cardPosition=0;
			// Reset the wager for this hand
			wager = 0;
		}

		public Point HandLocation
		{
			get{ return location; }
			set{ location = value; }
		}

		public double Wager
		{
			get{ return wager; }
			set{ wager = value; }
		}

		public bool Doubled
		{
			get{ return doubled; }
			set{ doubled = value; }
		}

		/// <summary>
		/// Determine if the hand is a pair
		/// </summary>
		public bool IsPair
		{
			get
			{
				if( cardPosition == 2 )
				{
					if( cards[0].FaceValue == cards[1].FaceValue )
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Determines if the hand has aces that, when valued at 11,
		/// do not make the hand exceed 21
		/// </summary>
		public bool IsSoft
		{
			get
			{
                //int total = 0;
                //int aceCount = 0;
                //int cardCount = 0;

                //foreach (Card card in cards)
                //{
                //    if (card != null)
                //    {
                //        cardCount++;
                //        switch (card.FaceValue)
                //        {
                //            case Card.CardType.Ace:
                //                total += 11;
                //                aceCount++;
                //                break;
                //            case Card.CardType.Ten:
                //                total += 10;
                //                break;
                //            default:
                //                total += card.TrueValue + 1;
                //                break;
                //        }
                //    }
                //}

                //if (total > 21)
                //{
                //    foreach (Card card in cards)
                //    {
                //        if (card != null)
                //        {
                //            if (card.FaceValue == Card.CardType.Ace)
                //            {
                //                aceCount--;
                //                total -= 10;
                //            }

                //            if (total <= 21)
                //                break;

                //        }
                //    }
                //}
                //return aceCount > 0;
                return false;
			}
		}

		public bool IsBlackjack()
		{
			return this.Total() == 21 && cardPosition == 2;
		}

		public int Total()
		{
			int total = 0;
			int aceCount = 0;
			int cardCount = 0;

			foreach( Card card in cards )
			{
				if( card != null )
				{
					cardCount++;
					switch( card.FaceValue )
					{
						case Card.CardType.Ace:
							total += 11;
							aceCount++;
							break;
						case Card.CardType.Ten:
							total += 10;
							break;
						default:
							total += card.TrueValue + 1;
							break;
					}
				}
			}

			if( total > 21 )
			{
				foreach( Card card in cards )
				{
					if( card != null )
					{
						if( card.FaceValue == Card.CardType.Ace )
						{
							aceCount --;
							total -= 10;
						}
						
						if( total <= 21 )
							break;
						
					}	
				}
			}
			return total;
		}

		public string Label(bool possibleBJ)
		{
			int total = 0;
			int aceCount = 0;
			int cardCount = 0;

			foreach( Card card in cards )
			{
				if( card != null )
				{
					cardCount++;
					switch( card.FaceValue )
					{
						case Card.CardType.Ace:
							total += 11;
							aceCount ++;
							break;
						case Card.CardType.Ten:
							total += 10;
							break;
						default:
							total += card.TrueValue + 1;
							break;
					}
				}
			}

			if( total > 21 )
			{
				foreach( Card card in cards )
				{
					if( card != null )
					{
						if( card.FaceValue == Card.CardType.Ace )
						{
							aceCount --;
							total -= 10;
						}
						
						if( total <= 21 )
							break;
						
					}	
				}
			}

			string returnValue = "";
			if( total == 21 && cardCount == 2 && possibleBJ )
				returnValue = "Blackjack";
			else if( aceCount > 0 && cardCount > 1 )
				returnValue = /*"soft " + */total.ToString(CultureInfo.InvariantCulture);
			else if( total > 21 )
                returnValue = Program.getMyLangString("blackKVerloren");//"BUST";
			else
				returnValue = total.ToString(CultureInfo.InvariantCulture);

			return returnValue;
		}

		public OutcomeType Outcome( Hand dealerHand, int numberOfHands )
		{
			OutcomeType returnValue = OutcomeType.None;

			bool dealerBlackjack = dealerHand.Total() == 21 && dealerHand.Count == 2;
			if( this.Total() > 0 )
			{
				if( Total() > 21 )
					returnValue = OutcomeType.Lost;
				else if( IsBlackjack() && !dealerBlackjack && numberOfHands == 1 )
					returnValue = OutcomeType.Blackjack;
				else if( dealerHand.Total() > 21 )
					returnValue = OutcomeType.Won;
				else if( Total() < dealerHand.Total() )
					returnValue = OutcomeType.Lost;
				else if( Total() > dealerHand.Total() )
					returnValue = OutcomeType.Won;
				else if( Total() == dealerHand.Total() )
					returnValue = OutcomeType.Push;
			}

			return returnValue;
		}

		public Strategy.AdviceType GetAdvice( Card dealerCard, Strategy playerStrategy, bool allowSplit, double cardCount )
		{
			if( playerStrategy != null )
				return playerStrategy.GetAdvice( this, dealerCard, allowSplit, cardCount );
			else
				return Strategy.AdviceType.None;
		}

		//	IList
		Object IList.this[int index] 
		{
			get 
			{ 
				if (index < 0 || index > cardPosition) 
					throw new System.IndexOutOfRangeException("index");
				else 
					return cards[index];
			}

			set 
			{
				if (index < 0 || index > cardPosition) 
					throw new System.IndexOutOfRangeException("index");
				else
					cards[index] = (Card)value;
			}
		}

		public Card this[int index] 
		{
			get 
			{ 
				if (index < 0 || index > cardPosition) 
					throw new System.IndexOutOfRangeException("index");
				else 
					return cards[index];
			}

			set 
			{
				if (index < 0 || index > cardPosition) 
					throw new System.IndexOutOfRangeException("index");
				else
					cards[index] = (Card)value;
			}
		}

		public bool IsFixedSize 
		{
			get { return false; }
		}

		public bool IsReadOnly 
		{
			get { return false; }
		}

		int IList.Add(Object card) 
		{
			Card crd = card as Card;

			if (crd == null)
				throw new InvalidCastException("You can only add Cards to a hand.");

			return Add(crd);
		}
		
		public int Add(Card card) 
		{
			if (cardPosition == cards.Length - 1) 
			{
				//	Create a new larger array and copy the 
				//	elements into it
				int newSize = cardPosition + RESIZEAMOUNT;
				Card[] tmpCards = new Card[newSize];

				Array.Copy(cards, 0, tmpCards, 0, cards.Length);
				cards = tmpCards;
			}

			//	Add the card to the array
			cards[cardPosition++] = card;

			//	Return the new cards index
			return cardPosition - 1;
		}
	
		bool IList.Contains(Object card) 
		{
			Card crd = card as Card;

			if (crd == null)
				return false;
			else
				return Contains(crd);
		}
		
		public bool Contains(Card card) 
		{
			for (int x = 0; x < cards.Length; x++) 
			{
				if (cards[x].Value == card.Value) 
					return true;
			}

			return false;
		}

		int IList.IndexOf(Object card) 
		{
			Card crd = card as Card;

			if (crd == null)
				return -1;
			else
				return IndexOf(crd);
		}
		
		public int IndexOf(Card card) 
		{
			for (int x = 0; x < cards.Length; x++) 
			{
				if (cards[x].Value == card.Value ) 
					return x;
			}
			
			return -1;
		}

		void IList.Insert(int index, Object card) 
		{
			Card crd = card as Card;

			if (crd == null)
				throw new InvalidCastException("You can only add Cards to a hand.");
				
			Insert(index, crd);
		}

		void Insert(int index, Card card) 
		{
			if (index < 0 || index > cardPosition) 
				throw new System.IndexOutOfRangeException("index");

			int newSize;

			if (cardPosition == cards.Length - 1) 
				newSize = cardPosition + RESIZEAMOUNT;
			else
				newSize = cardPosition;

			Card[] tmpCards = new Card[newSize];

			Array.Copy(cards, 0, tmpCards, 0, index);
			tmpCards[index] = card;
			index++;
			Array.Copy(cards, index, tmpCards, index, cards.Length - index);
				
			cards = tmpCards;
			cardPosition++;
		}

		void IList.Remove(Object card) 
		{
			Card crd = card as Card;

			if (crd != null)
				Remove(crd);
		}

		public void Remove(Card card) 
		{
			int idx = this.IndexOf(card);

			if (idx >= 0) 
			{
				this.RemoveAt(idx);
			}
		}

		public void RemoveAt(int index) 
		{
			if (index < 0 || index > cardPosition)
				throw new System.IndexOutOfRangeException("You have specified an invalid index");
			else 
			{				
				Card[] aTmp = new Card[cards.Length];
				
				if (index > 0)
					Array.Copy(cards, 0, aTmp, 0, index);
				
				int newStart = index + 1;
				if (newStart < cards.Length)
					Array.Copy(cards, newStart, aTmp, index, cards.Length - newStart);

				cardPosition--;

				cards = aTmp;
			}
		}

		public void Clear() 
		{
			cards = new Card[RESIZEAMOUNT];
			cardPosition = 0;
		}

		//	ICollection
		void ICollection.CopyTo(Array array, int index) 
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException("Index cannot be negative.");

			if (array == null) 
				throw new ArgumentNullException();		

			if (array.GetLowerBound(0) != 0 || array.Rank > 1)
				throw new ArgumentException("Only zero-based, single-dimensioned arrays permitted.");

			if (array.Length >= index || array.Length - index < cards.Length)
				throw new ArgumentException();

			Array.Copy(cards, 0, array, index, cards.Length);
		}

		public void CopyTo(Card[] array, int index) 
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException("Index cannot be negative.");

			if (array == null) 
				throw new ArgumentNullException();

			if (array.GetLowerBound(0) != 0 || array.Rank > 1)
				throw new ArgumentException("Only zero-based, single-dimensioned arrays permitted.");

			if (array.Length >= index || array.Length - index < cards.Length)
				throw new ArgumentException();

			Array.Copy(cards, 0, array, index, cards.Length);
		}
		
		public bool IsSynchronized 
		{ 
			get { return true; }
		}

		public Object SyncRoot 
		{
			get { return cards.SyncRoot; }
		}

		public int Count 
		{
			get { return cardPosition; }
		}

		//	IEnumerable
		IEnumerator IEnumerable.GetEnumerator() 
		{
			return cards.GetEnumerator();
		}

		public CardEnumerator GetEnumerator() 
		{
			return new CardEnumerator(cards);
		}
	}

	public class CardEnumerator 
	{
		private IEnumerator enumerator;

		//	No default construction
		private CardEnumerator() { }

		//	Can be created only inside this assembly
		internal CardEnumerator(IEnumerable parent) 
		{
			enumerator = parent.GetEnumerator();
		}

		public void Reset() 
		{
			enumerator.Reset();
		}

		public Card Current 
		{
			get { return (Card)enumerator.Current; }
		}

		public bool MoveNext() 
		{
            return enumerator.MoveNext();
		}
	}
}
