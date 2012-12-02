using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Engine.DataStructures;
using Engine.FileHandlers;
using Engine.Utility;

namespace RainbowMadnessShared
{
    public static class Parsers
    {
        public static CountedCollection<Card> ParseDeck(string filename)
        {
            var deck = new CountedCollection<Card>();
            var parser = new ConfigParser(filename, ' ');
            var colors = parser.Sections;

            foreach(var colorKey in colors)
            {
                var colorCards = parser.GetSection(colorKey);
                foreach(var card in colorCards)
                {
                    var color = Global.ColorMap.GetValueOrDefault(colorKey, -1);
                    var type = Global.TypeMap[card.Key];
                    var value = Global.ValueMap[card.Key];
                    var number = card.Value.ToInt();
                    deck.Add(new Card(color, type, value), number);
                }
            }
            return deck;
        }

        public static GameSettings ParseSettings(string filename)
        {
            var settings = new GameSettings();
            var parser = new ConfigParser(filename);
            
            const string game = "Game Settings";
            settings.MaxPlayers = parser.Get(game, "maxplayers", "7").ToInt();
            settings.DrawUntilPlayable = parser.Get(game, "drawuntilplayable", "true").ToBool();
            settings.CanPlayAfterDraw = parser.Get(game, "playafterdraw", "true").ToBool();
            settings.CardsPerStartingHand = parser.Get(game, "cardsinfirsthand", "7").ToInt();
            settings.DeckFilename = parser.Get(game, "deck", null);

            const string server = "Server Settings";
            settings.HostIP = parser.Get(server, "ip", "127.0.0.1");
            settings.LogFilename = parser.Get(server, "log", null);
            settings.Port = parser.Get(server, "port", "2012").ToInt();

            return settings;
        }

        /// <summary>
        /// Returns a mapping between name and host:port, all in lowercase.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseServerFavorites(string filename)
        {
            var favorites = new Dictionary<string, string>();
            var parser = new ConfigParser(filename);
            const string header = "Saved Hosts";
            var mixedCaseFavorites = parser.GetSection(header);
            foreach (var key in mixedCaseFavorites.Keys)
                favorites[key.ToLower()] = mixedCaseFavorites[key].ToLower();
            return favorites;
        }
    }

    public static class Global
    {
        public static readonly Dictionary<string, int>
            ColorMap = new Dictionary<string, int>
                           {
                               {"None", -1},
                               {"Red", 0},
                               {"Yellow", 1},
                               {"Green", 2},
                               {"Blue", 3},
                           };

        public static readonly Dictionary<int, string>
            ReverseColorMap = new Dictionary<int, string>
                           {
                               {-1, "None"},
                               {0, "Red"},
                               {1, "Yellow"},
                               {2, "Green"},
                               {3, "Blue"},
                           };

        public static readonly Dictionary<string, int>
            TypeMap = new Dictionary<string, int>
                          {
                              {"0", 0},
                              {"1", 0},
                              {"2", 0},
                              {"3", 0},
                              {"4", 0},
                              {"5", 0},
                              {"6", 0},
                              {"7", 0},
                              {"8", 0},
                              {"9", 0},
                              {"Skip", 1},
                              {"Reverse", 2},
                              {"Draw2", 3},
                              {"Wild", 4},
                              {"WildDraw4", 4},
                              {"Swap", 5},
                          };

        public static readonly Dictionary<string, int>
            ValueMap = new Dictionary<string, int>
                           {
                               {"0", 0},
                               {"1", 1},
                               {"2", 2},
                               {"3", 3},
                               {"4", 4},
                               {"5", 5},
                               {"6", 6},
                               {"7", 7},
                               {"8", 8},
                               {"9", 9},
                               {"Skip", -1},
                               {"Reverse", -1},
                               {"Draw2", 2},
                               {"Wild", 0},
                               {"WildDraw4", 4},
                               {"Swap", -1},
                           };
    }
}