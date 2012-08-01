using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Engine.DataStructures;
using Engine.Utility;

namespace RainbowMadness.Data
{
    public static class Parsers
    {
        private const String HEADER_PATTERN = @"\[(?<header>[^\]]*)\]";
        private static readonly Regex HEADER_REGEX = new Regex(HEADER_PATTERN);
        private static readonly Dictionary<Globals.SpecialCards, Func<Card>> CARD_FUNC_MAP = new Dictionary<Globals.SpecialCards, Func<Card>>
        {
            {Globals.SpecialCards.Draw2, () => {return new Draw2Card();}},
            {Globals.SpecialCards.Reverse, () => {return new ReverseCard();}},
            {Globals.SpecialCards.Skip, () => {return new SkipCard();}},
            {Globals.SpecialCards.Wild, () => {return new WildCard();}},
            {Globals.SpecialCards.WildDraw4, () => {return new WildDraw4Card();}}
        };
        
        public static ICollection<Card> ParseDeck(string filename)
        {
            ICollection<Card> deck = new CountedSet<Card>();
            
            
            string colorRaw = null;
            var color = Globals.CardColor.None;
            string valueRaw = null;
            var value = Globals.CardValue.None;
            Card card;
            string countRaw = null;
            int count = 0;
            foreach(var line in filename.ReadLines())
            {
                if (HEADER_REGEX.HasNamedCapture(line, "header"))
                {
                    // Header line, defining card color
                    color = Globals.CardColor.None; 
                    colorRaw = HEADER_REGEX.GetNamedCapture(line, "header");
                    if (Enum.IsDefined(typeof(Globals.CardColor), colorRaw))
                        color = (Globals.CardColor)Enum.Parse(typeof(Globals.CardColor), colorRaw, true);
                }
                else
                {
                    // Not a header line, defining card values

                    // Cards should have the format: CARD_NAME CARD_NUMBER
                    // With a single space separating the two
                    string[] lineParts = line.Split(' ');
                    if (lineParts.Length != 2)
                        continue;

                    value = Globals.CardValue.None;
                    valueRaw = lineParts[0];
                    if(Globals.CardValueMap.ContainsKey(valueRaw))
                        value = Globals.CardValueMap[valueRaw];

                    countRaw = lineParts[1];
                    try
                    {
                        count = int.Parse(countRaw);
                    }
                    catch (FormatException e)
                    {
                        continue;
                    }
                    
                    card = MakeCard(color, value, valueRaw);
                    if(card != null)
                        for (int i = 0; i < count; i++)
                            deck.Add(card.Copy());
                }

                

            }

            return deck;
        }

        private static Card MakeCard(Globals.CardColor color, Globals.CardValue value, string valueRaw)
        {
            Card card = null;

            if (value == Globals.CardValue.None)
            {
                // Special card value, not a regular card

                var special = Globals.SpecialCards.None;
                if (Enum.IsDefined(typeof(Globals.SpecialCards), valueRaw))
                    special = (Globals.SpecialCards)Enum.Parse(typeof(Globals.SpecialCards), valueRaw, true);

                if (special != Globals.SpecialCards.None)
                {
                    // Special cards have no value
                    card = CARD_FUNC_MAP[special]();
                    card.Color = color;
                }
            }
            else if (color != Globals.CardColor.None)
            {
                // Regular card
                card = new Card(color, value);
            }
            else
            {
                card = null;
            }

            return card;
        }
    }
}

