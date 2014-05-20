using System;
using System.Drawing;
using System.Windows.Forms;

namespace kioskplus.Unterhaltung.Blackjack
{
	/// <summary>
	/// The dealer object is fairly simple since he has only one hand of cards
	/// and no bank.  The bank in this game is unlimited and money comes out of 
	/// thin air to pay the players (if they win).
	/// </summary>
	[Serializable()]
	public class Dealer
	{
		private Hand dealerHand;
		private Point location;

		public Dealer(Point pntlocation)
		{
			// Tell the dealer where his origin point is
			location = pntlocation;

			// Create a hand for the dealer and locate its origin at the dealer's origin
			dealerHand = new Hand(new Point(0,0));
		}

		public void Reset()
		{
			// Get a new hand for each round
			dealerHand = new Hand(new Point(0,0));
		}

		public void AddCard( Card card )
		{
			// Take a card
			dealerHand.Add(card);
		}

		public int Total()
		{
			// The dealer's total is his hand's total.  This is just a shortcut to it.
			return dealerHand.Total();
		}
		
		public Hand Hand
		{
			get { return dealerHand; }
		}

		public void DrawHand(Graphics drawingSurface, bool displayDownCard)
		{
			// Draw the dealer's hand on the drawing surface
			bool firstCard = true;

			// Not really necessary for the dealer since he only has one hand, but...
			int x = this.location.X + dealerHand.HandLocation.X;
			int y = this.location.Y + dealerHand.HandLocation.Y;

			// We have to check to see if the dealer has any cards because this routine
			// gets called every time the screen needs to repaint.
			if( dealerHand.Count > 0 )
			{
				// We don't draw the dealer's label until it's time to show their down card
				if( displayDownCard )
					drawingSurface.DrawString( dealerHand.Label(true), new Font("Arial",14,FontStyle.Bold), new SolidBrush(Color.Yellow), x-10, y-10 );
				
				// Move down and to the right
				x += (int)Card.cardSpacing.Width;
				y += (int)Card.cardSpacing.Height;

				// Now draw each of the dealer's cards
				foreach( Card card in dealerHand )
				{
					if( card != null )
					{
						card.Draw( drawingSurface, new Point(x, y), !firstCard || displayDownCard, false, false );
						firstCard = false;
						x += (int)Card.cardSpacing.Width;
						y += (int)Card.cardSpacing.Height;
					}
				}
			}
		}
	}
}
