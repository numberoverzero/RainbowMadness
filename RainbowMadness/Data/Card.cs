using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainbowMadness.Data
{
    public class Card
    {
        public Color Color { get; protected set; }
        public Value Value { get; protected set; }

        public Card(){
            Color = Color.None;
            Value = Value.None;
        }

        public Card(Color color, Value value){
            Color = color;
            Value = value;
        }

        public virtual bool CanPlay(Game game)
        {
            if (Color == Color.None || Value == Value.None) 
                return false;
            
            Card top = game.Top;
            if (top.Color == this.Color)
                return true;
            if (top.Value == this.Value)
                return true;
            return false;
        }

        public virtual bool Play(Game game)
        {
            if (!CanPlay(game))
                return false;
            return game.SetTop(this);
        }

        static Card nullCard = new Card();
        public static Card NullCard
        {
            get { return nullCard; }
        }
    }

    public class WildCard : Card
    {
        public override bool CanPlay(Game game)
        {
            return true;
        }

        public override bool Play(Game game)
        {
            Color = game.CurrentPlayer.GetWildColor();
            return base.Play(game);
        }

    }

    public class WildDraw4Card : WildCard
    {
        public override bool Play(Game game)
        {
            bool played = base.Play(game);
            if (played)
                game.NextPlayer.DrawCard(game, 4);
            return played;
        }
    }

    public class SkipCard : Card
    {
        public SkipCard(Color color) : base(color, Value.None) { }

        public override bool CanPlay(Game game)
        {
            Card top = game.Top;
            if (Color == top.Color)
                return true;
            var topIsSkip = (top as SkipCard) != null;
            return topIsSkip;
        }
        
        public override bool Play(Game game)
        {
            bool played = base.Play(game);
            if (played)
            {
                game.AdvancePlayer();
                game.AdvancePlayer();
            }
            return played;
        }
    }

    public class ReverseCard : Card
    {
        public ReverseCard(Color color) : base(color, Value.None) { }

        public override bool CanPlay(Game game)
        {
            Card top = game.Top;
            if (Color == top.Color)
                return true;
            var topIsReverse = (top as ReverseCard) != null;
            return topIsReverse;
        }

        public override bool Play(Game game)
        {
            bool played = base.Play(game);
            if (played)
                game.ReversePlayDirection();
            return played;
        }
    }

    public class Draw2Card : Card
    {
        public Draw2Card(Color color) : base(color, Value.None) { }

        public override bool CanPlay(Game game)
        {
            Card top = game.Top;
            if (Color == top.Color)
                return true;
            var topIsDrawTwo = (top as Draw2Card) != null;
            return topIsDrawTwo;
        }

        public override bool Play(Game game)
        {
            bool played = base.Play(game);
            if (played)
                game.NextPlayer.DrawCard(game, 2);
            return played;
        }
    }
}
