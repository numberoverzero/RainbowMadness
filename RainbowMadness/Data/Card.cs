using System;
using System.Collections.Generic;
using Engine.DataStructures;
using Engine.Serialization;
using Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace RainbowMadness.Data
{
    public class Card : ByteSerializeable, IEquatable<Card>
    {
        private const string FmtStr = "{{{0}{1}}}";
        private static Card _nullCard;
        public int Color; // 0=None, 1=Red, 2=Yellow, 3=Green, 4=Blue
        public int Type; // 0=Number, 1=Skip, 2=Reverse, 3=Draw, 4=Wild, 5=SwapHands
        public int Value; // Number: 0-9 Skip/Reverse: unused Draw: number Wild: draw count Swap: unused
        private static Texture2D _cardTexture;

        public static void LoadContent(ContentManager content)
        {
            _cardTexture = content.Load<Texture2D>(@"Cards\card");
        }

        public Card() : this(0, -1, -1)
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
            get { return _nullCard ?? (_nullCard = new Card(0, -1, -1)); }
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
            get { return Type == 3; }
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

        #region IByteSerializeable Members

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

        #endregion

        public Card Copy()
        {
            return new Card(Color, Type, Value);
        }

        public virtual bool CanPlay(Game game)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Card other)
        {
            return (Color == other.Color) && (Type == other.Type) && (Value == other.Value);
        }

        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash*31 + Color;
            hash = hash * 31 + Type;
            hash = hash * 31 + Value;
            return hash;
        }

        public override string ToString()
        {
            string color;
            switch (Color)
            {
                default:
                    color = "";
                    break;
                case 0:
                    color = "Red";
                    break;
                case 1:
                    color = "Yellow";
                    break;
                case 2:
                    color = "Green";
                    break;
                case 3:
                    color = "Blue";
                    break;
            }
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

        public static bool CanPlay(Game game, String player, Card card, out string response)
        {
            response = "This is a valid play.";
            var top = game.Top;

            if (!game.CurrentPlayer.Equals(player))
            {
                response = "It is not {0}'s turn.".format(player);
                return false;
            }

            if (card.IsWild)
                return true;

            if (card.Color == top.Color)
                return true;

            // From here on out, the card has a different color from the top
            if (card.Type == top.Type)
            {
                if (card.Value == top.Value)
                    return true;
                response = "Cards are the same type but have different value.";
                return false;
            }
            response = "Cards have different color and type.";
            return false;
        }

        /// <summary>
        /// Draw the card, centered at pos
        /// </summary>
        public void Draw(SpriteBatch batch, Vector2 pos, float scale)
        {
            batch.Draw(_cardTexture, pos, null, Microsoft.Xna.Framework.Color.White, 0, new Vector2(-0.5f), scale, SpriteEffects.None, 0);
        }

        public static Vector2 GraphicDimensions
        {
            get { return _cardTexture.Dimensions(); }
        }
    }
}