using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainbowMadness.Data
{
    public struct Card
    {
        public Color Color;
        public readonly Value Value;

        public Card(Color color, Value value){
            Color = color;
            Value = value;
        }

        public bool CanPlay(Game game)
        {
            Card top = game.Top;
            if (top.Color == this.Color)
                return true;
            if (top.Value == this.Value)
                return true;
            return false;
        }

        public bool Play(Game game)
        {
            if (!CanPlay(game))
                return false;
            return game.SetTop(this);
        }

        static Card nullCard = new Card(Color.None, Value.None);
        public static Card NullCard
        {
            get { return nullCard; }
        }
    }
}
