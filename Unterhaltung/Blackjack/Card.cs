using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.Collections;
using System.Globalization;

namespace kioskplus.Unterhaltung.Blackjack
{
	/// <summary>
	/// Summary description for Card.
	/// </summary>
	[Serializable()]
	public class Card
	{
		private CardType cardType;
		private Suits cardSuit;
		private Image image;
		private int value;
		private int trueValue;
		public static SizeF cardSpacing;
		public static SizeF cardSize;

		public enum CardType
		{
			Ace = 0,
			Two = 1,
			Three = 2,
			Four = 3,
			Five = 4,
			Six = 5,
			Seven = 6,
			Eight = 7,
			Nine = 8,
			Ten = 9,
			Jack = 10,
			Queen = 11,
			King = 12
		}

		public enum Suits 
		{
			Clubs = 0,
			Diamonds = 1,
			Hearts = 2,
			Spades = 3
		}

		public Card( CardType type, Suits suit )
		{
			cardSuit = suit;
			cardType = type;
			value = ((int)suit * 13) + (int)cardType + 1;
            image = ResourcesBlackjack.GetImage("card"+value.ToString());
			trueValue = (int)cardType;
			if( trueValue > 9 )
				trueValue = 9;

			cardSize = image.PhysicalDimension;
			cardSpacing.Width = cardSize.Width / 5;
			cardSpacing.Height = cardSize.Height / 7;
		}

		public void Draw( Graphics drawingSurface, Point location, bool show, bool dim, bool doubledownCard )
		{
			float opaqueness = dim ? .5F : 1;
			float rotationAngle = doubledownCard ? 45 : 0;

			float[][] ptsArray ={	new float[] {1, 0, 0, 0, 0},
									new float[] {0, 1, 0, 0, 0},
									new float[] {0, 0, 1, 0, 0},
									new float[] {0, 0, 0, opaqueness, 0}, 
									new float[] {0, 0, 0, 0, 1}};
			ColorMatrix clrMatrix = new ColorMatrix(ptsArray);
			ImageAttributes imgAttributes = new ImageAttributes();
			imgAttributes.SetColorMatrix(clrMatrix,	ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

			GraphicsState previousState = drawingSurface.Save();

				Point Pcenter = new Point( location.X + image.Width / 2, location.Y + image.Height / 2 );
				drawingSurface.TranslateTransform( Pcenter.X, Pcenter.Y );
				drawingSurface.RotateTransform( rotationAngle );

				drawingSurface.DrawImage( show ? image : Shoe.BackImage, new Rectangle( -image.Width/2, -image.Height/2 , image.Width, image.Height ), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imgAttributes );

			drawingSurface.Restore( previousState );
		}
		public int Value 
		{
			get { return value; }
		}

		public CardType FaceValue
		{
			get { return cardType; }
		}

		public int TrueValue
		{
			get { return trueValue; }
		}

		public Image Image 
		{
			get { return image; }
		}

		public Suits Suit 
		{
			get { return cardSuit; }
		}	
	}

	//	A shared reference to access images and other resources.
	internal abstract class ResourcesBlackjack 
	{
		private static ResourceManager images;

        static ResourcesBlackjack() 
		{
			images = new ResourceManager("kioskplus.Unterhaltung.Blackjack.Images", Assembly.GetExecutingAssembly());
		}

		public static ResourceManager Images 
		{
			get { return images; }
		}

		public static Image GetImage(int imageId) 
		{
			return (Image)images.GetObject(imageId.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture );
		}

		public static Image GetImage(string imageId) 
		{
			return (Image)images.GetObject(imageId, CultureInfo.InvariantCulture);
		}
	} // Ende - Resources
}
