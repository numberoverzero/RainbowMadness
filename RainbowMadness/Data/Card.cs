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
        private int _color; // 0=None, 1=Red, 2=Yellow, 3=Green, 4=Blue
        private int _type; // 0=Number, 1=Skip, 2=Reverse, 3=Draw, 4=Wild, 5=SwapHands
        private int _value; // Number: 0-9 Skip/Reverse: unused Draw: number Wild: draw count Swap: unused

        public Card():this(0,-1,-1){ }

        public Card(int color, int type, int value)
        {
            _color = color;
            _type = type;
            _value = value;
        }

        public static Card NullCard
        {
            get { return _nullCard ?? (_nullCard = new Card(0, -1, -1)); }
        }

        public byte[] AsByteArray()
        {
            var b = new ByteArrayBuilder();
            b.Add(_color);
            b.Add(_type);
            b.Add(_value);
            return b.GetByteArray();
        }

        public int FromByteArray(byte[] bytes, int startIndex)
        {
            var reader = new ByteArrayReader(bytes, startIndex);
            _color = reader.ReadInt32();
            _type = reader.ReadInt32();
            _value = reader.ReadInt32();
            return reader.Index;
        }

        public Card Copy()
        {
            return new Card(_color, _type, _value);
        }

        public virtual bool CanPlay(Game game)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            string color;
            switch(_color)
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
            switch(_type)
            {
                default:
                    name = "UnknownCard";
                    break;
                case 0:
                    name = ""+_value;
                    break;
                case 1:
                    name = "Skip";
                    break;
                case 2:
                    name = "Reverse";
                    break;
                case 3:
                    name = "Draw"+_value;
                    break;
                case 4:
                    name = "Wild" + (_value > 0 ? "Draw"+_value : "");
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