using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrabbleCheat
{
    public class Turn
    {
        public int score;
        public int sPos;
        public List<Word> words;

        public Turn(Word word, int sPos = -1)
        {
            words = new List<Word>();
            words.Add(word);
            score = 0;
            this.sPos = sPos;
        }
    }
}
