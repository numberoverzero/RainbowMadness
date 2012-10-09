using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Engine.DataStructures;
using Engine.Utility;

namespace RainbowMadness.Data
{
    public static class Parsers
    {
        private const String HeaderPattern = @"\[(?<header>[^\]]*)\]";
        private static readonly Regex HeaderRegex = new Regex(HeaderPattern);

        public static ICollection<Card> ParseDeck(string filename)
        {
            ICollection<Card> deck = new CountedCollection<Card>();


            var color = -1;
            foreach (var line in filename.ReadLines())
            {
                if (HeaderRegex.HasNamedCapture(line, "header"))
                {
                    // Header line, defining card color
                    var colorRaw = HeaderRegex.GetNamedCapture(line, "header");
                    color = Globals.ColorMap.GetValueOrDefault(colorRaw, -1);
                }
                else
                {
                    // Not a header line, defining card values

                    // Cards should have the format: CARD_NAME CARD_NUMBER
                    // With a single space separating the two
                    var lineParts = line.Split(' ');
                    if (lineParts.Length != 2)
                        continue;

                    // Type/Value | Count

                    var valueRaw = lineParts[0];
                    var type = Globals.TypeMap[valueRaw];
                    var value = Globals.ValueMap[valueRaw];

                    int count;
                    try
                    {
                        count = int.Parse(lineParts[1]);
                    }
                    catch
                    {
                        count = 0;
                    }

                    if (color < 0 && type != 4) continue; // Unknown color for non-wild card

                    for (; count > 0; count--)
                        deck.Add(new Card(color, type, value));
                }
            }

            return deck;
        }
    }
}