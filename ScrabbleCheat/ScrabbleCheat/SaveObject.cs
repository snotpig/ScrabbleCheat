using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ScrabbleCheat
{
    [Serializable()]
    public class SaveObject : ISerializable
    {
        public char[,] board = new char[15, 15];
        public string rack;

        public SaveObject()
        {
            board = null;
            rack = null;
        }

        public SaveObject(char[,] board, string rack)
        {
            this.board = board;
            this.rack = rack;
        }

        #region ISerializable Members

        public SaveObject(SerializationInfo info, StreamingContext ctxt)
        {
            string name = string.Empty;
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    name = "|" + i.ToString() + "-" + j.ToString();
                    char ch = (char)info.GetValue(name, typeof(char));
                    board[i, j] = FilterChar(ch);
                }
            }
            rack = FilterString((string)info.GetValue("r", typeof(string)));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            string name = string.Empty;
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    name = "|" + i.ToString() + "-" + j.ToString();
                    info.AddValue(name, board[i, j]);
                }
            }
            info.AddValue("r", rack);
        }

        public string CopyData(char[,] board)
        {
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    board[i, j] = this.board[i, j];
                }
            }
            return rack;
        }

        private char FilterChar(char ch)
        {
            if(MainWindow.ALPHABET.Contains(ch.ToString())) return ch;
            if (MainWindow.ALPHABET.Contains(char.ToUpper(ch).ToString())) return ch;
            return MainWindow.SPACE;
        }

        private string FilterString(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in str)
            {
                sb.Append(FilterChar(ch));
            }
            return sb.ToString();
        }

        #endregion
    }
}
