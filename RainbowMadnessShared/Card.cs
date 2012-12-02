using System;
using System.Collections.Generic;
using Engine.DataStructures;
using Engine.Rendering;
using Engine.Serialization;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RainbowMadnessShared
{
    public class Card : ByteSerializeable, IEquatable<Card>, IComparable<Card>
    {
        private const string FmtStr = "{{{0}{1}}}";
        private static Card _nullCard, _cardBack;
        private static Dictionary<string, Texture2D> _cardPieces;
        public int Color; // -1=None, 0=Red, 1=Yellow, 2=Green, 3=Blue, 4=Back of card
        public int Type; // 0=Number, 1=Skip, 2=Reverse, 3=Draw, 4=Wild
        public int Value; // Number: 0-9 Skip/Reverse: unused Draw: number Wild: draw count Swap: unused

        public Card() : this(-1, -1, -1)
        {
        }

        public Card(int color, int type, int value)
        {
            Color = color;
            Type = type;
            Value = value;
        }

        public static Card NullCard
        {
            get { return _nullCard ?? (_nullCard = new Card(-1, -1, -1)); }
        }

        public static Card BackCard
        {
            get { return _cardBack ?? (_cardBack = new Card(100, 100, 100)); }
        }

        public bool IsSkip
        {
            get { return Type == 1; }
        }

        public bool IsReverse
        {
            get { return Type == 2; }
        }

        public bool IsDraw
        {
            get
            {
                return Type == 3 
                    || (Type == 4 && Value > 0); // All wild are draws - normal wilds are draw 0
            }
        }

        public bool IsWild
        {
            get { return Type == 4; }
        }

        public bool IsSwap
        {
            get { return Type == 5; }
        }

        public bool IsNumber
        {
            get { return Type == 0; }
        }

        public bool IsRed
        {
            get { return Color == 1; }
        }

        public bool IsYellow
        {
            get { return Color == 2; }
        }

        public bool IsGreen
        {
            get { return Color == 3; }
        }

        public bool IsBlue
        {
            get { return Color == 4; }
        }

        public static Vector2 GraphicDimensions
        {
            get { return _cardPieces["Border"].Dimensions(); }
        }

        #region IEquatable<Card> Members

        public bool Equals(Card other)
        {
            return (Color == other.Color) && (Type == other.Type) && (Value == other.Value);
        }

        #endregion

        /// <summary>
        ///   Looks for textures in @"Cards\"
        /// </summary>
        /// <param name="content"> </param>
        public static void LoadContent(ContentManager content)
        {
            _cardPieces = new Dictionary<string, Texture2D>();
            Action<string, string> load = (file, name) => _cardPieces[name] = content.Load<Texture2D>(@"CardPieces\"+file);
            Action<string> loadExact = (filename) => load(filename, filename);
            
            // Pieces
            loadExact("Border");
            loadExact("Background");
            loadExact("WildBackground2");

            // Special cards
            loadExact("Empty");
            loadExact("WildBackgroundInvert");

            // Special text
            loadExact("Skip");
            loadExact("Reverse");
            loadExact("Draw2");
            loadExact("Wild");
            loadExact("WildDraw4");
            
            // Numbers
            for (int i = 0; i < 10; i++)
                load("Numbers\\{0}".format(i), i.ToString());
        }

        public override void BuildAsByteArray(ByteArrayBuilder builder)
        {
            builder.Add(Color);
            builder.Add(Type);
            builder.Add(Value);
        }

        protected override int ReadFromByteArray(ByteArrayReader reader)
        {
            Color = reader.ReadInt32();
            Type = reader.ReadInt32();
            Value = reader.ReadInt32();
            return reader.Index;
        }

        public Card Copy()
        {
            return new Card(Color, Type, Value);
        }

        public virtual bool CanPlay(Game game)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash*31 + Color;
            hash = hash*31 + Type;
            hash = hash*31 + Value;
            return hash;
        }

        public int CompareTo(Card other)
        {
            /*
             Less than zero     This object is less than the other parameter.
             Zero               This object is equal to other.
             Greater than zero  This object is greater than other.
             */
            if (other == null) return 1;
            if (IsWild) // Wilds always go to the left, WD4 to the right of regular wilds
            {
                if (other.IsWild)
                {
                    if (Value == other.Value) return 0;
                    return Value > other.Value ? 1 : -1;
                }
                return -1;
            }
            if (Color != other.Color) return Color > other.Color ? 1 : -1;

            //Not dealing with wilds, and they're the same color.  Should be an easy sort on type
            if(Type != other.Type) return Type > other.Type ? 1 : -1;

            if (Value == other.Value) return 0;
            return Value > other.Value ? 1 : -1;

        }

        public override string ToString()
        {
            string color = Global.ReverseColorMap[Color];
            string name;
            switch (Type)
            {
                default:
                    name = "UnknownCard";
                    break;
                case 0:
                    name = "" + Value;
                    break;
                case 1:
                    name = "Skip";
                    break;
                case 2:
                    name = "Reverse";
                    break;
                case 3:
                    name = "Draw" + Value;
                    break;
                case 4:
                    name = "Wild" + (Value > 0 ? "Draw" + Value : "");
                    break;
                case 5:
                    name = "SwapHand";
                    break;
            }
            if (color != "") color = color + " ";
            return FmtStr.format(color, name);
        }

        public bool CanPlay(Card other)
        {
            if (IsWild) return true; // Wild can play on anything

            if (Color == other.Color) return true; // Anything can play on its color

            // From here on out, the card has a different color from the other.
            return Type == other.Type && Value == other.Value;
        }

        /// <summary>
        ///   Draw the card, centered at pos
        /// </summary>
        public void Draw(SpriteBatch batch, SpriteFont font, Vector2 pos, float scale, bool showWildColor)
        {
            var center = pos + 0.5f*scale*GraphicDimensions;
            var centerColor = FromColor(Color, showWildColor);
            var backgroundTint = IsWild ? Microsoft.Xna.Framework.Color.White : centerColor;
            var borderColor = (IsWild && showWildColor)
                                      ? FromColor(Color, true)
                                      : Microsoft.Xna.Framework.Color.Black;
            if (this == NullCard)
            {
                batch.Draw(_cardPieces["Empty"], pos, null, Microsoft.Xna.Framework.Color.White, 0, new Vector2(-0.5f), scale, SpriteEffects.None, 0);
                batch.Draw(_cardPieces["Border"], pos, null, borderColor, 0, new Vector2(-0.5f), scale, SpriteEffects.None, 0);
                return;
            }

            if (this == BackCard)
            {
                batch.Draw(_cardPieces["WildBackgroundInvert"], pos, null, Microsoft.Xna.Framework.Color.White, 0, new Vector2(-0.5f), scale,
                           SpriteEffects.None, 0);
                batch.Draw(_cardPieces["Border"], pos, null, borderColor, 0, new Vector2(-0.5f), scale, SpriteEffects.None, 0);
                return;
            }

            var backgroundTexture = IsWild ? "WildBackground2" : "Background";
            
            // Draw Background
            batch.Draw(_cardPieces[backgroundTexture], pos, null, backgroundTint, 0, new Vector2(-0.5f), scale,
                       SpriteEffects.None, 0);

            // Draw card text
            var cardText = "";
            if (IsWild) cardText = Value == 4 ? "WildDraw4" : "Wild";
            if (IsDraw && Value == 2) cardText = "Draw2";
            if (IsReverse) cardText = "Reverse";
            if (IsSkip) cardText = "Skip";

            //should be a number at this point
            if (String.IsNullOrEmpty(cardText)) cardText = Value.ToString();

            batch.Draw(_cardPieces[cardText], pos, null, Microsoft.Xna.Framework.Color.White, 0, new Vector2(0, -0.5f), scale, SpriteEffects.None, 0);

            // Draw border
            batch.Draw(_cardPieces["Border"], pos, null, borderColor, 0, new Vector2(-0.5f), scale,
                       SpriteEffects.None, 0);
        }

        private Microsoft.Xna.Framework.Color FromColor(int color, bool showWildColor)
        {
            //public int Color; // -1=None, 0=Red, 1=Yellow, 2=Green, 3=Blue
            if (!showWildColor && IsWild) return Microsoft.Xna.Framework.Color.Transparent;
            switch(color)
            {
                default:
                    return Microsoft.Xna.Framework.Color.Transparent;
                case 0:
                    return Microsoft.Xna.Framework.Color.Red;
                case 1:
                    return Microsoft.Xna.Framework.Color.Yellow;
                case 2:
                    return Microsoft.Xna.Framework.Color.Green;
                case 3:
                    return Microsoft.Xna.Framework.Color.Blue;
            }
        }

        public static bool operator !=(Card a, Card b)
        {
            return !(a == b);
        }

        public static bool operator ==(Card a, Card b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        
    }
}