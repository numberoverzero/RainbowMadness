using System;
using Engine.DataStructures;
using Engine.Serialization;
using Engine.Utility;
namespace RainbowMadness.Data
{
    public class Card : IByteSerializeable
    {
        private const string FmtStr = "{{{0}{1}}}";
        private static Card _nullCard;
        public int Color; // 0=None, 1=Red, 2=Yellow, 3=Green, 4=Blue
        private int _type; // 0=Number, 1=Skip, 2=Reverse, 3=Draw, 4=Wild, 5=SwapHands
        public int Value; // Number: 0-9 Skip/Reverse: unused Draw: number Wild: draw count Swap: unused

        public Card() : this(0, -1, -1)
        {
        }

        public Card(int color, int type, int value)
        {
            Color = color;
            _type = type;
            Value = value;
        }

        public static Card NullCard
        {
            get { return _nullCard ?? (_nullCard = new Card(0, -1, -1)); }
        }


        public bool IsSkip
        {
            get { return _type == 1; }
        }

        public bool IsReverse
        {
            get { return _type == 2; }
        }

        public bool IsDraw
        {
            get { return _type == 3; }
        }

        public bool IsWild
        {
            get { return _type == 4; }
        }

        public bool IsSwap
        {
            get { return _type == 5; }
        }

        public bool IsNumber
        {
            get { return _type == 0; }
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

        public byte[] AsByteArray()
        {
            var b = new ByteArrayBuilder();
            b.Add(Color);
            b.Add(_type);
            b.Add(Value);
            return b.GetByteArray();
        }

        public int FromByteArray(byte[] bytes, int startIndex)
        {
            var reader = new ByteArrayReader(bytes, startIndex);
            Color = reader.ReadInt32();
            _type = reader.ReadInt32();
            Value = reader.ReadInt32();
            return reader.Index;
        }

        #endregion

        public Card Copy()
        {
            return new Card(Color, _type, Value);
        }

        public virtual bool CanPlay(Game game)
        {
            throw new NotImplementedException();
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
            switch (_type)
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
    }
}