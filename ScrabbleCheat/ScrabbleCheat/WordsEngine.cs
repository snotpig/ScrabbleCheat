using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snotsoft.Words;

namespace ScrabbleCheat
{
    partial class WordsEngine
    {
        private const int HOR = 0;
        private const int VER = 1;
        private const int BONUS = 50;
        private const char BLANK = '#';
        private char SPACE = '?';
        private Lexicon lex = new Lexicon();
        private List<Turn> turnList = new List<Turn>();
        private List<Turn> singletList = new List<Turn>();
        private char[,] board;
        private string[][] boardStrings = new string[2][];
        private string rack = "";
        private int numResults = 5;
        private bool sorted = true;//false;// 

        public WordsEngine()
        {
            boardStrings[0] = new string[15];
            boardStrings[1] = new string[15];
        }

        public void SetSpaceToken(char ch)
        {
            SPACE = ch;
            lex.SetSpaceToken(SPACE);
        }

        // Comparison method for List.Sort
        private int CompareScores(Turn a, Turn b)
        {
            if (a.score > b.score) return -1;
            if (a.score < b.score) return 1;
            return 0;
        }

        // Comparison method for List.Sort
        private int CompareWordLength(Word a, Word b)
        {
            int aPos = CheckForSinglet(boardStrings[a.vert][a.rank].Substring(a.file, a.text.Length));
            int bPos = CheckForSinglet(boardStrings[b.vert][b.rank].Substring(b.file, b.text.Length));
            if (aPos > bPos) return 1;      // prefer non-singlet
            if (aPos < bPos) return -1;
            if (a.text.Length > b.text.Length) return -1;   // prefer longer word
            if (a.text.Length < b.text.Length) return 1;
            if (a.vert < b.vert) return -1;                 // prefer horizontal
            if (a.vert > b.vert) return 1;
            if (a.file < b.file) return -1;                 // prefer top left
            if (a.file > b.file) return 1;
            return 0;
        }

        // Comparison method for List.Sort
        private int CompareTilesUsed(Word a, Word b)
        {
            int numA = GetNumTilesUsed(a);
            int numB = GetNumTilesUsed(b);
            if (numA > numB) return -1;
            if (numA < numB) return 1;
            return 0;
        }

        private int GetNumTilesUsed(Word w)
        {
            int num = 0;
            int file = w.file;
            for (int i = 0; i < w.text.Length; i++)
            {
                if (boardStrings[w.vert][w.rank][file] == SPACE) num++;
                file++;
            }
            return num;
        }

        private void RemoveDuplicates()
        {
            turnList.Sort(CompareScores);
            bool match = false;
            int i, j, k;

            for (i = 0; i < (turnList.Count - 1); i++ )
            {
                for (j = i + 1; j < turnList.Count; j++)
                {
                    match = false;
                    if ((turnList[i].score == turnList[j].score) && (turnList[i].words.Count == turnList[j].words.Count)) // same score and word count
                    {
                        match = true;
                        for (k = 0; k < turnList[j].words.Count; k++)
                        {
                            if (!turnList[i].words.Contains(turnList[j].words[k]))
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                    if (match)
                    {
                        turnList.RemoveAt(j);
                        j--;
                    }
                }
            }
        }

        private void ReturnResults(List<Turn> list)
        {
            list.Clear();
            for(int i = 0; i < numResults; i++)
            {
                if (turnList.Count > i)
                {
                    if(sorted)turnList[i].words.Sort(CompareWordLength);
                    list.Add(turnList[i]);
                }
            }
        }

        private bool CheckForEmptyBoard()
        {
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (board[i, j] != SPACE) return false;
                }
            }
            return true;
        }

        #region Scoring methods

        bool GivesBonus(string str)
        {
            int s = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == SPACE) s++;
                if (s == 7) return true;
            }
            return false;
        }

        #region ScoreWord

        /// <summary>
        /// returns score for word positioned on board
        /// </summary>
        /// <param name="word">Contains the word to be scored</param>
        /// <returns></returns>
        private int ScoreWord(Word word)
        {
            int col, row;
            if (word.vert == 0)
            {
                col = word.file;
                row = word.rank;
            }
            else
            {
                col = word.rank;
                row = word.file;
            }
            int score = 0;
            int multiplier = 1;
            for (int i = 0; i < word.text.Length; i++)
            {
                int ls = Tile.GetScore(word.text[i]);

                if (board[col, row] == SPACE)
                {
                    if (scrabbleBoard[col, row] == 'd') ls *= 2;
                    else if (scrabbleBoard[col, row] == 't') ls *= 3;
                    else if (scrabbleBoard[col, row] == 'D') multiplier *= 2;
                    else if (scrabbleBoard[col, row] == 'T') multiplier *= 3;
                }
                score += ls;
                if (word.vert == 0) col++;
                else row++;
            }
            score *= multiplier;
            return score;
        }

        #endregion // Scoreword

        #region CalculateScores

        /// <summary>
        /// Fill turnList with secondary words and scores
        /// </summary>
        private void CalculateScores()
        {
            string board = string.Empty;

            for (int i = 0; i < turnList.Count; i++)
            {
                turnList[i].score = 0;
                for (int j = 0; j < turnList[i].words.Count; j++)
                {
                    turnList[i].score += ScoreWord(turnList[i].words[j]);
                    board = boardStrings[turnList[i].words[j].vert][turnList[i].words[j].rank].Substring(turnList[i].words[j].file,
                                                                                turnList[i].words[j].text.Length);
                    if (board.Length > 6)
                    {
                        if (GivesBonus(board)) turnList[i].score += BONUS;
                    }
                }
            }
        }

        #endregion // CalculateScores

        #endregion // Scoring methods

        public void GetWords(List<Turn> list, string rack, char[,] board, int numResults)
        {
            this.rack = rack;
            this.board = board;
            this.numResults = numResults;
            MakeBoardStrings();
            turnList.Clear();
            if (CheckForEmptyBoard())
            {
                GetFirstWord();
            }
            else
            {
                GetPrimaryWords();
                GetSecondaryWords();
                GetSingletWords();
                GetSingletCrossWords();
            }
                CalculateScores();
                RemoveDuplicates();
                ReturnResults(list);
        }

        private void GetFirstWord()
        {
            const int vert = 0;
            const int rank = 7;
            List<string> list = new List<string>();

            for (int file = 1; file < 8; file++)                    // Create slots: file = start
            {
                for (int len = 8 - file; len < 8; len++)            // len = length
                {
                    if (len == 1) continue;
                    list.Clear();
                    lex.CheckEmptySlot(len, rack, list);
                    foreach (string word in list)
                    {
                        turnList.Add(new Turn(new Word(word, rank, file, vert)));
                    }
                }
            }
        }

        private void MakeBoardStrings()
        {
            for (int i = 0; i < 15; i++)
            {
                boardStrings[HOR][i] = "";
                boardStrings[VER][i] = "";

                for (int j = 0; j < 15; j++)
                {
                    boardStrings[HOR][i] += board[j, i].ToString();
                    boardStrings[VER][i] += board[i, j].ToString();
                }
            }
        }

        private void GetPrimaryWords()
        {
            List<string> words = new List<string>();
            string slot = "";
            int sPos = -1;

            for (int v = 0; v < 2; v++)
            {
                for (int rank = 0; rank < 15; rank++)       // for each column/row
                {
                    for (int file = 0; file < 15; file++)                     // Create slots: file = start
                    {
                        if ((file > 0) && (boardStrings[v][rank][file - 1] != SPACE)) continue; // left border condition
                        for (int len = 2; len < (16 - file); len++)          // len = length
                        {
                            if ((file + len < 15) && (boardStrings[v][rank][file+len] != SPACE)) continue; // right border condition
                            slot = boardStrings[v][rank].Substring(file, len);
                            sPos = CheckForSinglet(slot);
                            words.Clear();
                            lex.CheckSlot(slot, rack, words);
                            foreach (string word in words)
                            {
                                turnList.Add(new Turn(new Word(word, rank, file, v), sPos));
                            }
                        }
                    }
                }
            }
        }

        int CheckForSinglet(string slot)
        {
            int pos = 0;
            int count = 0;
            for (int i = 0; i < slot.Count(); i++)
            {
                if (slot[i] == SPACE)
                {
                    count++;
                    pos = i;
                }
                if (count > 1) return -1;
            }
            if (count == 1) return pos;
            return -1;
        }

        private void GetSecondaryWords()
        {
            int v;
            _word _w;
            StringBuilder line;

            for (int i = 0; i < turnList.Count; i++) // for each primary word
            {
                if (turnList[i].sPos >= 0) continue;             // ignore singlets
                v = ((turnList[i].words[0].vert + 1) & 1);                  // orthogonal
                for (int j = 0; j < turnList[i].words[0].text.Length; j++)  // for each letter in primary word
                {
                    if (boardStrings[v][turnList[i].words[0].file + j][turnList[i].words[0].rank] == SPACE)//<<<
                    {
                        line = new StringBuilder(boardStrings[v][turnList[i].words[0].file + j]);
                        line[turnList[i].words[0].rank] = turnList[i].words[0].text[j]; // insert letter
                        _w = FindWord(line.ToString(), turnList[i].words[0].rank);//
                        if (_w.start != -1)
                        {
                            if (lex.CheckWord(_w.text)) // if valid word add to Turn
                            {
                                turnList[i].words.Add(new Word(_w.text, turnList[i].words[0].file + j, _w.start, v));//
                            }
                            else // else remove Turn
                            {
                                turnList.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private _word FindWord(string line, int pos)
        {
            int s = pos;
            while ((s >= 0) && (line[s] != SPACE))
            {
                s--;
            }
            s++;
            while ((pos < 15) && (line[pos] != SPACE))
            {
                pos++;
            }
            if (pos > (s + 1)) return new _word(line.Substring(s, pos - s), s);
            return new _word("", -1);
        }

        private void GetSingletWords()
        {
            List<string> words = new List<string>();
            int v, sPos, rank;
            StringBuilder line;
            string _rack;
            bool attached;

            singletList.Clear();
            for (int i = 0; i < turnList.Count; i++) // for each primary word
            {
                if (turnList[i].sPos < 0) continue;             // only singlets
                v = ((turnList[i].words[0].vert + 1) & 1);       // orthogonal
                sPos = turnList[i].sPos;
                rank = turnList[i].words[0].file + sPos;
                attached = false;

                line = new StringBuilder(boardStrings[v][rank]);
                line[turnList[i].words[0].rank] = turnList[i].words[0].text[sPos]; // insert letter in line
                if(char.IsLower(turnList[i].words[0].text[sPos]))           // if lower case
                {
                    _rack = rack.Remove(rack.IndexOf(BLANK.ToString()), 1); // remove blank from rack
                }
                else _rack = rack.Remove(rack.IndexOf(turnList[i].words[0].text[sPos]) , 1); //else remove letter from rack

                for (int file = 0; file <= turnList[i].words[0].rank; file++) // start of slot
                {
                    if ((file > 0) && (line[file - 1] != SPACE)) continue; // Left boundary condition
                    for (int len = turnList[i].words[0].rank - file + 1; len < (16 - file); len++)// length of slot
                    {
                        if (len == 1) continue;     // ignore single letter slot 
                        if ((file + len < 15) && (line[file + len] != SPACE)) continue; // Right boundary condition
                        words.Clear();
                        lex.CheckFullSlot(line.ToString().Substring(file, len), _rack, words);
                        if (words.Count > 0)
                        {
                            foreach (string word in words)
                            {
                                Turn t = new Turn(turnList[i].words[0], sPos);
                                t.words.Add(new Word(word, rank, file, v));
                                singletList.Add(t);
                                attached = true;
                            }
                        }
                    } // length of slot len
                }  // start position of slot file

                if (!attached) // no children: check if attached
                {
                    if (turnList[i].words[0].rank > 0)
                    {
                        if (line[turnList[i].words[0].rank - 1] != SPACE)
                        {
                            attached = true;
                        }
                    }
                    if (turnList[i].words[0].rank < 14)
                    {
                        if (line[turnList[i].words[0].rank + 1] != SPACE)
                        {
                            attached = true;
                        }
                    }
                }

                if (attached)
                {
                    turnList.RemoveAt(i);
                    i--;
                }

            } // Turn i from turnList
        }

        private void GetSingletCrossWords()
        {
            StringBuilder line;
            _word _w;
            int v;

            for (int i = 0; i < singletList.Count; i++) // for each singlet in singletList
            {
                v = (singletList[i].words[1].vert + 1) & 1; // orientation

                for (int j = 0; j < singletList[i].words[1].text.Length; j++) // for each letter in word
                {
                    if (singletList[i].words[1].file + j == singletList[i].words[0].rank) continue; // ignore singlet position
                    if (boardStrings[v][singletList[i].words[1].file + j][singletList[i].words[1].rank] == SPACE)
                    {
                        line = new StringBuilder(boardStrings[v][singletList[i].words[1].file + j]);
                        line[singletList[i].words[1].rank] = singletList[i].words[1].text[j]; // insert letter
                        _w = FindWord(line.ToString(), singletList[i].words[1].rank);
                        if (_w.start != -1)
                        {
                            if (lex.CheckWord(_w.text)) // if valid word add to Turn
                            {
                                singletList[i].words.Add(new Word(_w.text, singletList[i].words[1].file + j, _w.start, v));
                            }
                            else // else remove Turn
                            {
                                singletList.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }
                }
            } // singlet word i
            foreach(Turn t in singletList)
            {
                turnList.Add(t);
            }
        }


        #region Test

        public void Pause()
        {
        }

        public void Test()
        {
            for (int i = 0; i < turnList.Count; i++)
            {
                for (int j = 0; j < turnList[i].words.Count; j++)
                {
                    if ((turnList[i].words[j].text == "JUT"))// && (turnList[i].score == 41))
                    {
                    }
                }
            }
            for (int i = 0; i < singletList.Count; i++)
            {
                for (int j = 0; j < singletList[i].words.Count; j++)
                {
                    if ((singletList[i].words[0].text == "AGED") && (singletList[i].words[1].text == "GO"))
                    {
                    }
                }
            }
        }

        public void ReturnSinglets(List<Turn> list)
        {
            int count = singletList.Count;
            list.Clear();
            int i = 5;
            while ((count > 0) && (i > 0))
            {
                count--; i--;
                singletList[count].words.Sort(CompareWordLength);
                list.Add(singletList[count]);
            }
        }

        public void FindWords(int col, int row, List<Turn> list)
        {
            list.Clear();
            for (int i = 0; i < turnList.Count; i++)
            {
                for (int j = 0; j < turnList[i].words.Count; j++ )
                {
                    Word w = turnList[i].words[j];
                    if ((w.vert == 0) && (w.rank == row) && (w.file == col))
                    {
                        turnList[i].score = i;
                        list.Add(turnList[i]);
                    }
                    if ((w.vert == 1) && (w.rank == col) && (w.file == row))
                    {
                        turnList[i].score = i;
                        list.Add(turnList[i]);
                    }
                }
            }
        }

        #endregion // Test
    }
}
