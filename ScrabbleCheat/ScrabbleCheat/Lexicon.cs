using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows;

// '#' = blank tile.  SPACE = space on board.
namespace Snotsoft.Words
{
    class Lexicon
    {
        #region Data fields

        private struct Section
        {
            public int start;
            public int finish;
            public int length;

            public Section(int start, int finish, int length)
            {
                this.start = start;
                this.finish = finish;
                this.length = length;
            }
        }

        private List<String> Words = new List<string>();
        private int WordCount;
        private const char BLANK = '#';
        private char SPACE = '0';
        private List<Section> sections = new List<Section>();

        #endregion // Data fields


        #region Constructor

        ///<Summary>
        /// Initialises a new instance of Lexicon
        /// Loads words from text file
        ///</Summary>
        public Lexicon()
        {
            int ch;
            string str = "";

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ScrabbleCheat.ScrabbleWords.txt"))
            {
                if (stream == null)
                {
                    MessageBox.Show("Can't load Words.txt");
                    return;
                }
                using (StreamReader reader = new StreamReader(stream))
                {
                    for (; ; )
                    {
                        ch = reader.Read(); // read a byte at a time
                        if (ch == -1) break;
                        if (ch != '\r') str = str + (char)ch;
                        else
                        {
                            Words.Add(str.ToUpper());
                            str = "";
                            reader.Read();
                        }
                    }
                }
            }
            WordCount = Words.Count;
            Words.Sort(SortByLength);
            int i = 0;
            int start = 0;
            int len = 2;
            do
            {
                if (Words[i].Length > len)
                {
                    sections.Add(new Section(start, i - 1, len));
                    start = i;
                    len++;
                }
                i++;
            } while (i < WordCount); // skip last word (28 letters)
        }

        #endregion // Constructor

        // compares string length for list.Sort method
        private int SortByLength(string a, string b)
        {
            if (a.Length > b.Length) return 1;
            if (a.Length < b.Length) return -1;
            return 0;
        }

        #region Private Helpers

// Check word can be made from letters
        private bool WordFromLetters(string word, string letters)
        {
            if (letters.Length < word.Length) return false;             // not enough letters
            for (int i = 0; i < letters.Length; i++)                    // move blank tokens # to end
            {
                if (letters[i] == BLANK)
                {
                    letters = letters.Remove(i, 1);
                    letters = letters.PadRight(letters.Length + 1, BLANK);
                }
            }

            bool match;
            for (int i = 0; i < word.Length; i++)   // for each letter in word
            {
                match = false;
                for (int j = 0; j < letters.Length; j++)    // for each tile in letters
                {
                    if ((word[i] == letters[j]) || (letters[j] == BLANK)) // if match
                    {
                        letters = letters.Remove(j, 1);                 // replace tile
                        letters.PadRight(letters.Length + 1, '~');      // with '~'
                        match = true;
                        break;
                    }
                }
                if (!match) return false;
            }
            return true;
        }

        // Checks string contains letter
        private bool ContainsLetter(string str)
        {
            bool rv = false;
            foreach (char ch in str)
            {
                if (char.IsLetter(ch))
                {
                    rv = true;
                    break;
                }
            }
            return rv;
        }

        private bool ContainsSpace(string str)
        {
            foreach (char ch in str)
            {
                if (ch == SPACE)  return true;
            }
            return false;
        }

        // Remove duplicate words from list
        private void RemoveDuplicates(List<string> list)
        {
            list.Sort();    // Sort alphabetically
            List<string> duplicates = new List<string>();
            for(int i=0; i<list.Count-1; i++)
            {
                if (list[i] == list[i + 1]) duplicates.Add(list[i]);
            }
            foreach (string word in duplicates)
            {
                list.Remove(word);
            }
        }

        #endregion // Private Helpers


        #region Public interface

// Set token to use as space
        public void SetSpaceToken(char token)
        {
            this.SPACE = token;
        }

// Check word is a real word
        public bool CheckWord(string word)
        {
            return Words.Contains(word);
        }

// Fills list with possible words from letters
        public void MakeList(string letters, List<String> list)
        {
            list.Clear();
            if (letters.Length == 0) return;    // return if no letters
            letters = letters.ToUpper();        // convert to upper case 
            foreach(string word in Words)
            {
                if (WordFromLetters(word, letters)) list.Add(word);
            }
        }

        // Create list of words from slot and letters
        public void CheckSlot(string slot, string tiles, List<string> list)
        {
            if (!ContainsLetter(slot)) return;
            if (!ContainsSpace(slot)) return;
            bool match;
            tiles = tiles.ToUpper();
            for (int i = 0; i < tiles.Length; i++)                    // move blank tokens # to end
            {
                if (tiles[i] == BLANK)
                {
                    tiles = tiles.Remove(i, 1);
                    tiles = tiles.PadRight(tiles.Length + 1, BLANK);
                }
            }
            Section section = sections[slot.Length - 2];

            for (int i = section.start; i < section.finish; i++)
            {
                StringBuilder word = new StringBuilder(Words[i]);
                if (slot.Length != word.Length) continue; // word and slot different length
                string _tiles = tiles.ToString();
                match = true;

                for (int j = 0; j < word.Length; j++)        // check each letter of word in turn
                {
                    if (char.ToUpper(slot[j]) == word[j])  // letters match
                    {
                        word[j] = slot[j];
                        continue;
                    }
                    if (slot[j] == SPACE)     // space in position i
                    {
                        match = false;
                        for (int k = 0; k < _tiles.Length; k++)
                        {
                            if ((_tiles[k] == word[j]) || (_tiles[k] == BLANK))      // match from tiles
                            {
                                if(_tiles[k] == BLANK)                                  // if blank tile
                                {                                                       // replace letter 
                                    word.Replace(word[j], char.ToLower(word[j]), j, 1); // with lower case
                                }
                                _tiles = _tiles.Remove(k, 1);
                                match = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        match = false;// if letters don't match and 
                        break;      // position not blank match = false
                    }
                    if (!match) break; //  if no match go to next word
                }
                if (match) list.Add(word.ToString());
            }
        }

        public void CheckEmptySlot(int slotLength, string tiles, List<string> list)
        {
            string slot = new string(SPACE, slotLength);
            bool match;
            tiles = tiles.ToUpper();
            slot = slot.ToUpper();
            for (int i = 0; i < tiles.Length; i++)                    // move blank tokens # to end
            {
                if (tiles[i] == BLANK)
                {
                    tiles = tiles.Remove(i, 1);
                    tiles = tiles.PadRight(tiles.Length + 1, BLANK);
                }
            }
            Section section = sections[slot.Length - 2];

            for (int i = section.start; i < section.finish; i++)
            {
                if (slot.Length != Words[i].Length) continue; // word and slot different length
                StringBuilder word = new StringBuilder(Words[i]);
                string _tiles = tiles.ToString();
                match = true;

                for (int j = 0; j < word.Length; j++)        // check each letter of word in turn
                {
                    match = false;
                    for (int k = 0; k < _tiles.Length; k++)
                    {
                        if ((_tiles[k] == word[j]) || (_tiles[k] == BLANK))      // match from tiles
                        {
                            if (_tiles[k] == BLANK)                                  // if blank tile
                            {                                                       // replace letter 
                                word.Replace(word[j], char.ToLower(word[j]), j, 1); // with lower case
                            }
                            _tiles = _tiles.Remove(k, 1);
                            match = true;
                            break;
                        }
                    }
                    if (!match) break; //  if no match go to next word
                }
                if (match) list.Add(word.ToString());
            }
        }

        public void CheckFullSlot(string slot, string tiles, List<string> list)
        {
            if (!ContainsLetter(slot)) return;
            bool match;
            tiles = tiles.ToUpper();
            for (int i = 0; i < tiles.Length; i++)                    // move blank tokens # to end
            {
                if (tiles[i] == BLANK)
                {
                    tiles = tiles.Remove(i, 1);
                    tiles = tiles.PadRight(tiles.Length + 1, BLANK);
                }
            }
            Section section = sections[slot.Length - 2];

            for (int i = section.start; i < section.finish; i++)
            {
                StringBuilder word = new StringBuilder(Words[i]);
                if (slot.Length != word.Length) continue; // word and slot different length
                string _tiles = tiles.ToString();
                match = true;

                for (int j = 0; j < word.Length; j++)        // check each letter of word in turn
                {
                    if (char.ToUpper(slot[j]) == word[j])  // letters match
                    {
                        word[j] = slot[j];
                        continue;
                    }
                    if (slot[j] == SPACE)     // space in position i
                    {
                        match = false;
                        for (int k = 0; k < _tiles.Length; k++)
                        {
                            if ((_tiles[k] == word[j]) || (_tiles[k] == BLANK))      // match from tiles
                            {
                                if (_tiles[k] == BLANK)                                  // if blank tile
                                {                                                       // replace letter 
                                    word.Replace(word[j], char.ToLower(word[j]), j, 1); // with lower case
                                }
                                _tiles = _tiles.Remove(k, 1);
                                match = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        match = false;// if letters don't match and 
                        break;      // position not blank match = false
                    }
                    if (!match) break; //  if no match go to next word
                }
                if (match) list.Add(word.ToString());
            }
        }

        #endregion // Public interface


    }   // class WordList
}   // namespace Snotsoft.Words

