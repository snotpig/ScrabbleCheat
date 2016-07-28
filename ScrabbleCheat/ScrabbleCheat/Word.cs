using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrabbleCheat
{
    public struct Word : IEquatable<Word>
    {
        public int rank, file;
        public string text;
        public int vert;

        public Word(string text, int rank, int file, int vert)
        {
            this.text = text;
            this.rank = rank;
            this.file = file;
            this.vert = vert;
        }

        public bool Equals(Word w)
        {
            if (this.text != w.text) return false;
            if (this.rank != w.rank) return false;
            if (this.file != w.file) return false;
            if (this.vert != w.vert) return false;
            return true;
        }

    }
}
