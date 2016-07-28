using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScrabbleCheat
{
    class Tile
    {
        protected VisualBrush brush;
        public Rectangle face;
        public char letter;
        public int col, row;
        public bool stretched = false;
        private static Dictionary<char, int> dict = new Dictionary<char,int>();
        public virtual void SetLetter(char letter){ }
        public virtual void WipeLetter() { }

        // Constructor
        public Tile(char letter)
        {
            brush = (VisualBrush)Application.Current.TryFindResource(letter.ToString());
            face = new Rectangle();
            face.Width = 50;
            face.Height = 50;
            face.Fill = brush;
            this.letter = letter;
            col = row = -1; 
        }

        // Static Constructor
        static Tile()
        {
            dict = new Dictionary<char, int>()
            {
                {'A', 1},
                {'B', 3},
                {'C', 3},
                {'D', 2},
                {'E', 1},
                {'F', 4},
                {'G', 2},
                {'H', 4},
                {'I', 1},
                {'J', 8},
                {'K', 5},
                {'L', 1},
                {'M', 3},
                {'N', 1},
                {'O', 1},
                {'P', 3},
                {'Q', 10},
                {'R', 1},
                {'S', 1},
                {'T', 1},
                {'U', 1},
                {'V', 4},
                {'W', 4},
                {'X', 8},
                {'Y', 4},
                {'Z', 10},
                {'#', 0}
            };
        }

        public static int GetScore(char letter)
        {
            int score = 0;
            dict.TryGetValue(letter, out score);
            return score;
        }

    } // class Tile


    class BlankTile : Tile
    {
        VisualBrush[] letterBrushes = new VisualBrush[26];
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public BlankTile()
            : base('#')
        {
            string key = "blank";
            for (int i = 0; i < 26; i++)
            {
                letterBrushes[i] = (VisualBrush)Application.Current.TryFindResource(key + letters[i].ToString());
            }
        }

        public override void SetLetter(char letter)
        {
            int i;
            for (i = 0; i < 26; i++)
            {
                if (letters[i] == char.ToUpper(letter)) break;
            }
            face.Fill = letterBrushes[i];
            this.letter = letter;
        }

        public override void WipeLetter()
        {
            face.Fill = brush;
            this.letter = '#';
        }


    } // class BlankTile

} // namespace Words_Plus
