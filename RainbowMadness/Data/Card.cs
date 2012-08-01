using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainbowMadness.Data
{
    public class Card
    {
        private const string FMT_STR = "{{{0}: {1}, {2}}}";
        public virtual Globals.CardColor Color { get; set; }
        public virtual Globals.CardValue Value { get; set; }

        public Card(){
            Color = Globals.CardColor.None;
            Value = Globals.CardValue.None;
        }

        public Card(Globals.CardColor color, Globals.CardValue value)
        {
            Color = color;
            Value = value;
        }

        public virtual bool CanPlay(Game game)
        {
            if (Color == Globals.CardColor.None || Value == Globals.CardValue.None) 
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
            game.Top = this;
            return true;
        }

        public virtual Card Copy()
        {
            Card card = _copy();
            CopyDataInto(card);
            return card;
        }

        protected virtual void CopyDataInto(Card card)
        {
            card.Color = this.Color;
            card.Value = this.Value;
        }

        protected virtual Card _copy()
        {
            return new Card();
        }

        static Card nullCard = new Card();
        public static Card NullCard
        {
            get { return nullCard; }
        }

        public override string ToString()
        {
            return String.Format(FMT_STR, this.GetType(), Color, Value);
        }
    }

    public class WildCard : Card
    {
        bool _choosingColor = false;
        public override Globals.CardColor Color
        {
            get { return base.Color; }
            set 
            {
                if (_choosingColor) base.Color = value;
                else base.Color = Globals.CardColor.None;
            }
        }
        public override bool CanPlay(Game game)
        {
            return true;
        }

        public override bool Play(Game game)
        {
            _choosingColor = true;
            Color = game.CurrentPlayer.GetWildColor();
            _choosingColor = false;
            return base.Play(game);
        }

        protected override Card _copy()
        {
            return new WildCard();
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

        protected override Card _copy()
        {
            return new WildDraw4Card();
        }
    }

    public class SkipCard : Card
    {
        public SkipCard() : base() { }
        public SkipCard(Globals.CardColor color) : base(color, Globals.CardValue.None) { }

        public override bool CanPlay(Game game)
        {
            if (Color == Globals.CardColor.None)
                return false;

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

        protected override Card _copy()
        {
            return new SkipCard();
        }
    }

    public class ReverseCard : Card
    {
        public ReverseCard() : base() { }
        public ReverseCard(Globals.CardColor color) : base(color, Globals.CardValue.None) { }

        public override bool CanPlay(Game game)
        {
            if (Color == Globals.CardColor.None)
                return false;

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

        protected override Card _copy()
        {
            return new SkipCard();
        }
    }

    public class Draw2Card : Card
    {
        public Draw2Card() : base() { }
        public Draw2Card(Globals.CardColor color) : base(color, Globals.CardValue.None) { }

        public override bool CanPlay(Game game)
        {
            if (Color == Globals.CardColor.None)
                return false;

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

        protected override Card _copy()
        {
            return new Draw2Card();
        }
    }
}
