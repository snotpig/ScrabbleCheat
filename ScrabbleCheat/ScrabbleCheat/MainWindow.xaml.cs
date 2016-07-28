using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Snotsoft.Controls;
using UserControls;
using System.Threading;
using System.ComponentModel;
using ScrabbleCheat;
using Snotsoft.Validation;

namespace ScrabbleCheat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum dirs { Right, Down, RedRight, RedDown, None, Rack };

        #region Data fields

        private const string windowTitle = "Scrabble Cheat - ";
        private const string LETTERS = "AAAAAAAAABBCCDDDDEEEEEEEEEEEEFFGGGHHIIIIIIIIIJKLLLLMMNNNNNNOOOOOOOOPPQRRRRRRSSSSTTTTTTUUUUVVWWXYYZ##";
        public const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ#";
        private const string SAVEFILE = "Cheat.sav";
        private const string SAVEFOLDER = "Snotsoft\\Scrabble Cheat";
        private const string NEWGAME = "New Game";
        public const char SPACE = '_';
        //        private const char BLANK = '#';
        private const int NUMRESULTS = 6;
        private const int RACK_TOP = 195;
        private const int RACK_LEFT = 65;
        public string name = NEWGAME;
        private List<Tile> tiles;
        private Rectangle[] arrowShapes = new Rectangle[4];
        private char[,] board = new char[15, 15];
        private string rack;
        private List<Turn> results = new List<Turn>();
        private Point originalLocation = new Point();
        private CircularProgressBar progress;
        private SolidColorBrush wordHighlight;
        private SolidColorBrush transparent;
        private SolidColorBrush black;
        private SolidColorBrush red;
        private Select select;
        private BackgroundWorker backgroundWorker;
        private WordsEngine engine;
        private bool isDirty = false;
        private dirs arrow = dirs.None;
        private _square arrowSquare = new _square(15,15);

        #endregion // Data fields

        #region Constructor

        public MainWindow()
        {
//            if (!Validated()) Environment.Exit(0);
//            if (Expired()) Environment.Exit(0);
            InitializeComponent();
            Setup setup = new Setup();
            setup.CreateFolder(SAVEFOLDER);
            engine = new WordsEngine();
            engine.SetSpaceToken(SPACE);
            canvas.AllowDragOutOfView = true;
            CreateTiles();
            CreateArrows();
            backgroundWorker = ((BackgroundWorker)this.FindResource("backgroundWorker"));
            wordHighlight = new SolidColorBrush(Color.FromArgb(30, 50, 50, 100));
            transparent = new SolidColorBrush(Colors.Transparent);
            black = new SolidColorBrush(Colors.Black);
            red = new SolidColorBrush(Color.FromRgb(150, 0, 0));
            NewGame();
        }

        #endregion // Constructor

        #region Event Handlers

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            engine.GetWords(results, rack, board, NUMRESULTS);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StopProgress();
            ShowWords();
            cmdStart.IsEnabled = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            PromptSave();
            base.OnClosing(e);
            Environment.Exit(0);
        }

        private void canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LeftClick(sender, e.GetPosition(canvas));
        }

        private void rectRack_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RackClicked();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            KeyPressed(e.Key);
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void cmdLoad_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void cmdNewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void cmdStart_Click(object sender, RoutedEventArgs e)
        {
            cmdStart.IsEnabled = false;
            backgroundWorker.RunWorkerAsync();
            StartProgress();
        }

        private void txt0_MouseEnter(object sender, MouseEventArgs e)
        {
            txt0.Background = wordHighlight;
            s0.Background = wordHighlight;
            ShowOnBoard(0);
            cmdPlay0.Visibility = Visibility.Visible;
        }

        private void txt0_MouseLeave(object sender, MouseEventArgs e)
        {
            txt0.Background = transparent;
            s0.Background = transparent;
            UnShowOnBoard();
            cmdPlay0.Visibility = Visibility.Hidden;
        }

        private void txt1_MouseEnter(object sender, MouseEventArgs e)
        {
            txt1.Background = wordHighlight;
            s1.Background = wordHighlight;
            ShowOnBoard(1);
            cmdPlay1.Visibility = Visibility.Visible;
        }

        private void txt1_MouseLeave(object sender, MouseEventArgs e)
        {
            txt1.Background = transparent;
            s1.Background = transparent;
            UnShowOnBoard();
            cmdPlay1.Visibility = Visibility.Hidden;
        }

        private void txt2_MouseEnter(object sender, MouseEventArgs e)
        {
            txt2.Background = wordHighlight;
            s2.Background = wordHighlight;
            ShowOnBoard(2);
            cmdPlay2.Visibility = Visibility.Visible;
        }

        private void txt2_MouseLeave(object sender, MouseEventArgs e)
        {
            txt2.Background = transparent;
            s2.Background = transparent;
            UnShowOnBoard();
            cmdPlay2.Visibility = Visibility.Hidden;
        }

        private void txt3_MouseEnter(object sender, MouseEventArgs e)
        {
            txt3.Background = wordHighlight;
            s3.Background = wordHighlight;
            ShowOnBoard(3);
            cmdPlay3.Visibility = Visibility.Visible;
        }

        private void txt3_MouseLeave(object sender, MouseEventArgs e)
        {
            txt3.Background = transparent;
            s3.Background = transparent;
            UnShowOnBoard();
            cmdPlay3.Visibility = Visibility.Hidden;
        }

        private void txt4_MouseEnter(object sender, MouseEventArgs e)
        {
            txt4.Background = wordHighlight;
            s4.Background = wordHighlight;
            ShowOnBoard(4);
            cmdPlay4.Visibility = Visibility.Visible;
        }

        private void txt4_MouseLeave(object sender, MouseEventArgs e)
        {
            txt4.Background = transparent;
            s4.Background = transparent;
            UnShowOnBoard();
            cmdPlay4.Visibility = Visibility.Hidden;
        }

        private void txt5_MouseEnter(object sender, MouseEventArgs e)
        {
            txt5.Background = wordHighlight;
            s5.Background = wordHighlight;
            ShowOnBoard(5);
            cmdPlay5.Visibility = Visibility.Visible;
        }

        private void txt5_MouseLeave(object sender, MouseEventArgs e)
        {
            txt5.Background = transparent;
            s5.Background = transparent;
            UnShowOnBoard();
            cmdPlay5.Visibility = Visibility.Hidden;
        }

        private void cmdPlay0_Click(object sender, RoutedEventArgs e)
        {
            PlayOnBoard(0);
        }

        private void cmdPlay1_Click(object sender, RoutedEventArgs e)
        {
            PlayOnBoard(1);
        }

        private void cmdPlay2_Click(object sender, RoutedEventArgs e)
        {
            PlayOnBoard(2);
        }

        private void cmdPlay3_Click(object sender, RoutedEventArgs e)
        {
            PlayOnBoard(3);
        }

        private void cmdPlay4_Click(object sender, RoutedEventArgs e)
        {
            PlayOnBoard(4);
        }

        private void cmdPlay5_Click(object sender, RoutedEventArgs e)
        {
            PlayOnBoard(5);
        }

        private void cmdAbout_Click(object sender, RoutedEventArgs e)
        {
            LaunchAboutBox();
        }

        #endregion // Event Handlers


        private bool Expired()
        {
            DateTime dt = new DateTime(2014, 7, 30);
            if (DateTime.Compare(dt, DateTime.Now) < 0)
            {
                return true;
            }
            return false;
        }

        private void CreateTiles()
        {
            tiles = new List<Tile>();

            for (int i = 0; i < LETTERS.Length; i++)
            {
                if (LETTERS[i] == '#')
                {
                    tiles.Add(new BlankTile());
                }
                else tiles.Add(new Tile(LETTERS[i]));
            }
        }

        private void CreateArrows()
        {
            arrowShapes[(int)dirs.Right] = new Rectangle();
            arrowShapes[(int)dirs.Right].Width = 50;
            arrowShapes[(int)dirs.Right].Height = 50;
            arrowShapes[(int)dirs.Right].Fill = (VisualBrush)Application.Current.TryFindResource("rightArrowBlack");
            arrowShapes[(int)dirs.Down] = new Rectangle();
            arrowShapes[(int)dirs.Down].Width = 50;
            arrowShapes[(int)dirs.Down].Height = 50;
            arrowShapes[(int)dirs.Down].Fill = (VisualBrush)Application.Current.TryFindResource("downArrowBlack");
            arrowShapes[(int)dirs.RedRight] = new Rectangle();
            arrowShapes[(int)dirs.RedRight].Width = 50;
            arrowShapes[(int)dirs.RedRight].Height = 50;
            arrowShapes[(int)dirs.RedRight].Fill = (VisualBrush)Application.Current.TryFindResource("rightArrowRed");
            arrowShapes[(int)dirs.RedDown] = new Rectangle();
            arrowShapes[(int)dirs.RedDown].Width = 50;
            arrowShapes[(int)dirs.RedDown].Height = 50;
            arrowShapes[(int)dirs.RedDown].Fill = (VisualBrush)Application.Current.TryFindResource("downArrowRed");
        }

        private void Reset()
        {
            HideResults();
            rack = new string(SPACE, 7);
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    board[i, j] = SPACE;
                }
            }
            LayoutTiles();
            name = NEWGAME;
            Title = windowTitle + name;
            RemoveAllArrows();
        }

        private void NewGame()
        {
            EnableButtons(false);
            PromptSave();
            cmdNewGame.IsEnabled = false;
            Reset();
            MarkDirty(false);
            cmdNewGame.IsEnabled = true;
            EnableButtons();
            Keyboard.Focus(this);
        }

        private void LayoutTiles()
        {
            canvas.Children.Clear();
            foreach (Tile t in tiles)
            {
                if (t is BlankTile) t.WipeLetter();
            }

            int j = 0;
            for (int i = 0; i < LETTERS.Length; i++)
            {
                canvas.Children.Add(tiles[i].face);
                Canvas.SetLeft(tiles[i].face, (j % 9) * 56 + 0);
                Canvas.SetTop(tiles[i].face, 60 * (int)(j / 9));
                tiles[i].col = -1;
                if (i < LETTERS.Length - 1)
                {
                    if (tiles[i + 1].letter != tiles[i].letter) j++;
                }
            }
        }

        private void PopulateBoard()
        {
            bool isLower;

            foreach (Tile t in tiles)
            {
                t.col = -1;
            }

            for (int col = 0; col < 15; col++)
            {
                for (int row = 0; row < 15; row++)
                {
                    if (board[col, row] != SPACE)
                    {
                        isLower = char.IsLower(board[col, row]);
                        for (int i = 0; i < tiles.Count; i++)
                        {
                            if (isLower && (tiles[i] is BlankTile) && (tiles[i].col == -1))
                            {
                                tiles[i].SetLetter(char.ToUpper(board[col, row]));
                                tiles[i].col = col;
                                tiles[i].row = row;
                                Canvas.SetLeft(tiles[i].face, 500 + 50 * col);
                                Canvas.SetTop(tiles[i].face, 50 * row);
                                break;
                            }
                            else if ((tiles[i].letter == (char)board[col, row]) && (tiles[i].col == -1))
                            {
                                tiles[i].col = col;
                                tiles[i].row = row;
                                Canvas.SetLeft(tiles[i].face, 500 + 50 * col);
                                Canvas.SetTop(tiles[i].face, 50 * row);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void PopulateRack()
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < tiles.Count; j++)
                {
                    if ((tiles[j].letter == rack[i]) && (tiles[j].col < 0))               // tile from box
                    {
                        tiles[j].col = 100 + i;
                        Canvas.SetLeft(tiles[j].face, 50 * i + RACK_LEFT);
                        Canvas.SetTop(tiles[j].face, RACK_TOP);
                        break;
                    }
                }
            }
        }

        private void ClearRack()
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].col > 99)               // tile in rack
                {
                    int j = ALPHABET.IndexOf(tiles[i].letter);
                    Canvas.SetLeft(tiles[i].face, (j % 9) * 56 + 0);
                    Canvas.SetTop(tiles[i].face, 60 * (int)(j / 9));
                    tiles[i].col = -1;
                    if (tiles[i] is BlankTile) tiles[i].WipeLetter();
                }
            }
            rack = new string(SPACE, 7);
        }

        private void Save()
        {
            EnableButtons(false);
            SaveObject obj = new SaveObject(board, rack);
            SaveDialog dlg = new SaveDialog(name);
            dlg.Owner = this;
            dlg.Left = this.Left + 33;
            dlg.Top = this.Top + 325;
            dlg.SetName(name);
            dlg.saveObject = obj;
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                name = dlg.name;
                Title = windowTitle + name;
                MarkDirty(false);
            }
            EnableButtons();
            Keyboard.Focus(this);
        }

        private void PromptSave()
        {
            if (!isDirty) return;
            EnableButtons(false);
            string message = "";
            if (name == NEWGAME)
            {
                message = "Save current game?";
            }
            else
            {
                message = "Save " + name + "?";
            }
            PromptSave dlg = new PromptSave(message);
            dlg.Owner = this;
            dlg.Top = this.Top + 260;
            dlg.Left = this.Left + 35;
            Nullable<bool> result = dlg.ShowDialog();
            if(result == true)
            {
                Save();
            }
            EnableButtons();
            Keyboard.Focus(this);
        }

        private void Load()
        {
            PromptSave();
            EnableButtons(false);
            LoadDialog dlg = new LoadDialog();
            dlg.Owner = this;
            dlg.Left = this.Left + 33;
            dlg.Top = this.Top;
            Nullable<bool> result = dlg.ShowDlg();

            if (result == true)
            {
                SaveObject obj = dlg.saveObject;
                Reset();
                if (obj.rack != null)
                {
                    rack = obj.CopyData(board);
                }
                PopulateBoard();
                PopulateRack();
                name = dlg.name;
                Title = windowTitle + name;
                MarkDirty(false);
            }
            EnableButtons();
            Keyboard.Focus(this);
        }

        private void EnableButtons(bool enable=true)
        {
            cmdAbout.IsEnabled = enable;
            cmdLoad.IsEnabled = enable;
            cmdNewGame.IsEnabled = enable;
            cmdSave.IsEnabled = enable;
            cmdStart.IsEnabled = enable;
        }

        private void StartProgress()
        {
            canvas.AllowDragging = false;
            progress = new CircularProgressBar();
            canvas.Children.Add(progress);
            Canvas.SetLeft(progress, 815);
            Canvas.SetTop(progress, 316);
            canvas.BringToFront(progress);
            HideResults();
        }

        private void StopProgress()
        {
            canvas.Children.Remove(progress);
            canvas.AllowDragging = true;
            ShowResults();
        }

        private void ShowResults()
        {
            int numResults = results.Count;
            if (numResults > 0)
            {
                txt0.Visibility = Visibility.Visible;
                s0.Visibility = Visibility.Visible;
            }
            if (numResults > 1)
            {
                txt1.Visibility = Visibility.Visible;
                s1.Visibility = Visibility.Visible;
            }
            if (numResults > 2)
            {
                txt2.Visibility = Visibility.Visible;
                s2.Visibility = Visibility.Visible;
            }
            if (numResults > 3)
            {
                txt3.Visibility = Visibility.Visible;
                s3.Visibility = Visibility.Visible;
            }
            if (numResults > 4)
            {
                txt4.Visibility = Visibility.Visible;
                s4.Visibility = Visibility.Visible;
            }
            if (numResults > 5)
            {
                txt5.Visibility = Visibility.Visible;
                s5.Visibility = Visibility.Visible;
            }
        }

        private void HideResults()
        {
            txt0.Visibility = Visibility.Hidden;
            txt1.Visibility = Visibility.Hidden;
            txt2.Visibility = Visibility.Hidden;
            txt3.Visibility = Visibility.Hidden;
            txt4.Visibility = Visibility.Hidden;
            txt5.Visibility = Visibility.Hidden;
            s0.Visibility = Visibility.Hidden;
            s1.Visibility = Visibility.Hidden;
            s2.Visibility = Visibility.Hidden;
            s3.Visibility = Visibility.Hidden;
            s4.Visibility = Visibility.Hidden;
            s5.Visibility = Visibility.Hidden;
            cmdPlay0.Visibility = Visibility.Hidden;
            cmdPlay1.Visibility = Visibility.Hidden;
            cmdPlay2.Visibility = Visibility.Hidden;
            cmdPlay3.Visibility = Visibility.Hidden;
            cmdPlay4.Visibility = Visibility.Hidden;
            cmdPlay5.Visibility = Visibility.Hidden;
        }

        private void ReturnTile(int i)
        {
            Canvas.SetLeft(tiles[i].face, originalLocation.X);
            Canvas.SetTop(tiles[i].face, originalLocation.Y);
            if (tiles[i].col > 99)
            {
                rack = rack.Remove(tiles[i].col - 100, 1);
                rack = rack.Insert(tiles[i].col - 100, tiles[i].letter.ToString());
            }
            else if (tiles[i].col >= 0)
            {
                board[tiles[i].col, tiles[i].row] = tiles[i].letter;
            }
        }

        private void LeftClick(object sender, Point pt)
        {
            if (sender.ToString() == "System.Windows.Controls.Primitives.UniformGrid")
            {
                UpdateArrow(new _square((int)(pt.X - 500) / 50, (int)(pt.Y) / 50));
                return;
            }
            UIElement face = canvas.GetLastDragged();
            if ((face != null) && (face != arrowShapes[0]) && (face != arrowShapes[1]) && (face != arrowShapes[2]) && (face != arrowShapes[3]))
            {
                RemoveAllArrows();
                PlaceTile(face);
            }
            else
            {
                if (pt.X > 499) UpdateArrow(new _square((int)(pt.X - 500) / 50, (int)(pt.Y) / 50));
            }
        }

        private void PlaceTile(UIElement face)
        {
            originalLocation = canvas.GetLastLocation();

            double left = Canvas.GetLeft(face);
            double top = Canvas.GetTop(face);
            int i = -1;
            for (i = 0; i < tiles.Count; i++)               // Find matching tile
            {
                if (Object.ReferenceEquals(tiles[i].face, face)) break; // << match
            }

            if ((left > 475) && (left < 1275) && (top > -25) && (top < 775))       // on board
            {
                AddToBoard(left, top, i);
                HideResults();
                MarkDirty();
            }
            else if ((left >= RACK_LEFT - 45) && (left < RACK_LEFT + 345) && (top >= RACK_TOP - 30) && (top < RACK_TOP + 30))  // on rack
            {
                AddToRack(left, i);
                HideResults();
                MarkDirty();
            }
            else if ((left >= 0) && (left < 500) && (top >= -25) && (top < 171))  // in box
            {
                ReturnToBox(i);
                HideResults();
                MarkDirty();
            }
            else ReturnTile(i); // return to old location
        }

        void AddToBoard(double left, double top, int i)
        {
            if (tiles[i].col > 99) RemoveFromRack(i);           // From rack
            else if (tiles[i].col >= 0) RemoveFromBoard(i);      // Already on Board
            _square sq = FindSquare(left, top, i);          // Get col & row for nearest free square if available
            if ((sq.col >= 0) && (sq.col < 15) && (sq.row >= 0) && (sq.row < 15))
            {
                Canvas.SetLeft(tiles[i].face, 50 * sq.col + 500);
                Canvas.SetTop(tiles[i].face, 50 * sq.row);
                if ((tiles[i].col > 0) && (tiles[i].col < 100))
                {
                    board[tiles[i].col, tiles[i].row] = SPACE;
                }
                if (tiles[i] is BlankTile)          // if Blank tile show select dialog
                {
                    ChooseBlankLetter(tiles[i], sq);
                }
                board[sq.col, sq.row] = tiles[i].letter;
                tiles[i].col = sq.col;
                tiles[i].row = sq.row;
            }
            else ReturnTile(i);
        }

        private void ChooseBlankLetter(Tile tile, _square square)
        {
                    tile.WipeLetter();
                    select = new Select();
                    select.Owner = this;
                    select.Top = this.Top + 21 * square.row;
                    select.Left = this.Left + 27 * square.col + 100;
                    select.ShowDialog();
                    tile.SetLetter(select.letter);
        }

        private _square FindSquare(double left, double top, int i)
        {
            _square sq = new _square();
            sq.col = (int)Math.Floor((left - 475) / 50);       // nearest square
            sq.row = (int)Math.Floor((top + 25) / 50);
            if (sq.row < 0) sq.row++;
            if (sq.row > 14) sq.row--;
            if (sq.col < 0) sq.col++;
            if (sq.col > 14) sq.col--;
            if (board[sq.col, sq.row] != SPACE)
            {
                if ((tiles[i].col == sq.col) && (tiles[i].row == sq.row)) return sq;
                double xDir = ((left + 25) % 50) - 25;
                double yDir = ((top + 25) % 50) - 25;
                if (Math.Abs(xDir) > Math.Abs(yDir))
                {
                    if (xDir > 0) sq.col++;
                    else sq.col--;
                }
                else
                {
                    if (yDir > 0) sq.row++;
                    else sq.row--;
                }
            }
            if ((sq.col < 0) || (sq.col > 14) || (sq.row < 0) || (sq.row > 14)) // out of bounds
            {
                sq.col = -1;
                return sq;
            }
            if (board[sq.col, sq.row] != SPACE) sq.col = -1; // no free square
            return sq;
        }

        private void AddToRack(double left, int i)
        {
            if (tiles[i].col > 99) RemoveFromRack(i);           // Already on rack
            else if (tiles[i].col >= 0) RemoveFromBoard(i);      // From Board
            int col = FindRackSpace(left, i);
            if (col >= 0)                       // rack not full
            {
                if (tiles[i] is BlankTile) tiles[i].WipeLetter();
                Canvas.SetLeft(tiles[i].face, 50 * col + RACK_LEFT);
                Canvas.SetTop(tiles[i].face, RACK_TOP);
                rack = rack.Remove(col, 1);
                rack = rack.Insert(col, tiles[i].letter.ToString());
                tiles[i].col = 100 + col;
            }
            else ReturnTile(i);
        }

        private int FindRackSpace(double left, int i)
        {
            int col = (int)Math.Floor((left - 60) / 50);        // nearest square
            if (col < 0) col = 0;                               // include
            if (col > 6) col = 6;                               // sides
            if (rack[col] != SPACE)
            {
                if (tiles[i].col - 100 == col) return col;
                if (!rack.Contains(SPACE)) return -1;              // rack full
                for (int j = col; j < col + 7; j++)
                {
                    if (rack[j % 7] == SPACE)
                    {
                        return (j % 7);
                    }
                }
            }
            return col;
        }

        private void ReturnToBox(int i)
        {
            if (tiles[i].col > 99) RemoveFromRack(i);
            else if (tiles[i].col >= 0) RemoveFromBoard(i);
            if (tiles[i] is BlankTile) tiles[i].WipeLetter();
            int j = ALPHABET.IndexOf(tiles[i].letter);
            Canvas.SetLeft(tiles[i].face, (j % 9) * 56 + 0);
            Canvas.SetTop(tiles[i].face, 60 * (int)(j / 9));
            tiles[i].col = -1;
        }

        private void ReturnToRack(int i)
        {
            tiles[i].col = FindRackSpace(0, i) + 100;
            Canvas.SetLeft(tiles[i].face, RACK_LEFT + (tiles[i].col - 100) * 50);
            Canvas.SetTop(tiles[i].face, RACK_TOP);
            tiles[i].WipeLetter();
            rack = rack.Remove(tiles[i].col - 100, 1).Insert(tiles[i].col - 100, tiles[i].letter.ToString());
        }

        private void RemoveFromBoard(int i)
        {
            board[tiles[i].col, tiles[i].row] = SPACE;
        }

        private void RemoveFromRack(int i)
        {
            rack = rack.Remove(tiles[i].col - 100, 1);
            rack = rack.Insert(tiles[i].col - 100, SPACE.ToString());
        }

        private void ShowWords()
        {
            int numResults = results.Count;
            if (numResults > 0)
            {
                txt0.Content = results[0].words[0].text.ToUpper();
                s0.Content = results[0].score.ToString();
            }
            else
            {
                txt0.Content = "";
                s0.Content = "";
            }
            if (numResults > 1)
            {
                txt1.Content = results[1].words[0].text.ToUpper();
                s1.Content = results[1].score.ToString();
            }
            else
            {
                txt1.Content = "";
                s1.Content = "";
            }
            if (numResults > 2)
            {
                txt2.Content = results[2].words[0].text.ToUpper();
                s2.Content = results[2].score.ToString();
            }
            else
            {
                txt2.Content = "";
                s2.Content = "";
            }
            if (numResults > 3)
            {
                txt3.Content = results[3].words[0].text.ToUpper();
                s3.Content = results[3].score.ToString();
            }
            else
            {
                txt3.Content = "";
                s3.Content = "";
            }
            if (numResults > 4)
            {
                txt4.Content = results[4].words[0].text.ToUpper();
                s4.Content = results[4].score.ToString();
            }
            else
            {
                txt4.Content = "";
                s4.Content = "";
            }
            if (numResults > 5)
            {
                txt5.Content = results[5].words[0].text.ToUpper();
                s5.Content = results[5].score.ToString();
            }
            else
            {
                txt5.Content = "";
                s5.Content = "";
            }
        }

        private void StretchToBoard(Tile tile, int col, int row)
        {
            Canvas.SetLeft(tile.face, 500 + col * 50);
            Canvas.SetTop(tile.face, row * 50);
            board[col, row] = '*';
            tile.stretched = true;
        }

        private void ReboundToRack(Tile tile)
        {
            Canvas.SetLeft(tile.face, RACK_LEFT + (tile.col - 100) * 50);
            Canvas.SetTop(tile.face, RACK_TOP);
            tile.stretched = false;
            tile.WipeLetter();
        }

        private void MoveToBoard(Tile tile, int col, int row)
        {
            Canvas.SetLeft(tile.face, 500 + col * 50);
            Canvas.SetTop(tile.face, row * 50);
            if (tile is BlankTile)
            {
                board[col, row] = char.ToLower(tile.letter);
            }
            else board[col, row] = tile.letter;
            tile.col = col;
            tile.row = row;
        }

        private void MoveToRack(Tile tile, int col)
        {
            Canvas.SetLeft(tile.face, RACK_LEFT + 50 * col);
            Canvas.SetTop(tile.face, RACK_TOP);
            tile.col = 100 + col;
            rack = rack.Remove(col, 1).Insert(col, tile.letter.ToString());
        }

        private void ShowOnBoard(int i)
        {
            char blankLetter = '?';  //
            int col, row;
            bool isHor;

            if (results.Count <= i) return;
            for (int j = 0; j < results[i].words.Count; j++)  // run through words in list
            {
                string word = results[i].words[j].text;
                if (results[i].words[j].vert == 0) // horizontal
                {
                    col = results[i].words[j].file;
                    row = results[i].words[j].rank;
                    isHor = true;
                }
                else
                {
                    col = results[i].words[j].rank;
                    row = results[i].words[j].file;
                    isHor = false;
                }

                for (int k = 0; k < word.Length; k++) // run through letters in word
                {
                    if (board[col, row] == SPACE)       // if board square unoccupied
                    {
                        if (Char.IsLower(word[k])) // lower case denotes Blank tile
                        {
                            blankLetter = Char.ToUpper(word[k]);
                            word = word.Remove(k, 1);   // replace with 
                            word = word.Insert(k, "#"); // '#' token
                        }
                        foreach (Tile tile in tiles)    // run through tiles to find match
                        {
                            if ((tile.col > 99) && (tile.letter == word[k]) && !tile.stretched)
                            {
                                tile.SetLetter(blankLetter);
                                StretchToBoard(tile, col, row);
                                break;
                            }
                        }
                    }
                    if (isHor) col++;
                    else row++;
                }
            }
        }

        private void UnShowOnBoard()
        {
            foreach (Tile tile in tiles)
            {
                if (tile.col > 99)
                {
                    ReboundToRack(tile);
                }
            }
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (board[i, j] == '*')
                    {
                        board[i, j] = SPACE;
                    }
                }
            }

        }

        private void PlayOnBoard(int i)
        {
            UnShowOnBoard();
            HideResults();
            char blankLetter = '?';  //
            int col, row;
            bool isHor;

            if (results.Count <= i) return;
            for (int j = 0; j < results[i].words.Count; j++)  // run through words in list
            {
                string word = results[i].words[j].text;
                if (results[i].words[j].vert == 0) // horizontal
                {
                    col = results[i].words[j].file;
                    row = results[i].words[j].rank;
                    isHor = true;
                }
                else
                {
                    col = results[i].words[j].rank;
                    row = results[i].words[j].file;
                    isHor = false;
                }

                for (int k = 0; k < word.Length; k++) // run through letters in word
                {
                    if (board[col, row] == SPACE)       // if board square unoccupied
                    {
                        if (Char.IsLower(word[k])) // lower case denotes Blank tile
                        {
                            blankLetter = Char.ToUpper(word[k]);
                            word = word.Remove(k, 1);   // replace with 
                            word = word.Insert(k, "#"); // '#' token
                        }
                        foreach (Tile tile in tiles)    // run through tiles to find match
                        {
                            if ((tile.col > 99) && (tile.letter == word[k]))
                            {
                                tile.SetLetter(blankLetter);
                                rack = rack.Remove(tile.col - 100, 1);
                                rack = rack.Insert(tile.col - 100, SPACE.ToString());
                                MoveToBoard(tile, col, row);
                                break;
                            }
                        }
                    }
                    if (isHor) col++;
                    else row++;
                }
            }

            TidyRack();
            MarkDirty();
        }

        private int TidyRack()
        {
            string _rack = rack;
            ClearRack();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _rack.Length; i++)
            {
                if (_rack[i] != SPACE) sb.Append(_rack[i]);
            }
            int rv = sb.Length;
            sb.Insert(sb.Length, SPACE.ToString(), _rack.Length - sb.Length);
            rack = sb.ToString();
            PopulateRack();
            return rv;
        }

        private bool ContainsTile(_square square)
        {
            if (board[square.col, square.row] == SPACE) return false;
            return true;
        }

        void LaunchAboutBox()
        {
            AboutBox about = new AboutBox();
            about.Left = this.Left;
            about.Top = this.Top;
            about.ShowDialog();
            Keyboard.Focus(this);
        }

        private void KeyPressed(Key key)
        {
            if(arrow != dirs.None) // arrow on board
            {
                if (key == Key.Space) UpdateArrow(arrowSquare);
                if (key == Key.Back) BackSpace();
            }
            char ch = key.ToString()[0];
            if (key == Key.Oem7) ch = '#';
            else if (key.ToString().Length > 1) return;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].letter == ch)
                {
                    MarkDirty();
                    if ((arrow == dirs.Right) && (tiles[i].col < 0))
                    {
                        if (arrowSquare.col > 14) return;
                        MoveToBoard(tiles[i], arrowSquare.col, arrowSquare.row);
                        if (ch == '#')          // if Blank tile show select dialog
                        {
                            ChooseBlankLetter(tiles[i], arrowSquare);
                        }
                        do arrowSquare.col++;
                        while ((arrowSquare.col < 15) && (ContainsTile(arrowSquare))); // skip occupied squares
                        if (arrowSquare.col < 15)
                        {
                            PositionArrow();
                        }
                        else
                        {
                            canvas.Children.Remove(arrowShapes[0]);
                        }
                        break;
                    }
                    else if ((arrow == dirs.Down) && (tiles[i].col < 0))
                    {
                        if (arrowSquare.row > 14) return;
                        MoveToBoard(tiles[i], arrowSquare.col, arrowSquare.row);
                        if (ch == '#')          // if Blank tile show select dialog
                        {
                            ChooseBlankLetter(tiles[i], arrowSquare);
                        }
                        do arrowSquare.row++;
                        while ((arrowSquare.row < 15) && (ContainsTile(arrowSquare))); // skip occupied squares
                        if (arrowSquare.row < 15)
                        {
                            PositionArrow();
                        }
                        else
                        {
                            canvas.Children.Remove(arrowShapes[1]);
                        }
                        break;
                    }
                    else if ((arrow == dirs.RedRight) && (tiles[i].col > 99))
                    {
                        if (arrowSquare.col > 14) return;
                        RemoveFromRack(i);
                        MoveToBoard(tiles[i], arrowSquare.col, arrowSquare.row);
                        if (ch == '#')          // if Blank tile show select dialog
                        {
                            ChooseBlankLetter(tiles[i], arrowSquare);
                        }
                        do arrowSquare.col++;
                        while ((arrowSquare.col < 15) && (ContainsTile(arrowSquare))); // skip occupied squares
                        if (arrowSquare.col < 15)
                        {
                            PositionArrow();
                        }
                        else
                        {
                            canvas.Children.Remove(arrowShapes[2]);
                        }
                        break;
                    }
                    else if ((arrow == dirs.RedDown) && (tiles[i].col > 99))
                    {
                        if (arrowSquare.row > 14) return;
                        RemoveFromRack(i);
                        MoveToBoard(tiles[i], arrowSquare.col, arrowSquare.row);
                        if (ch == '#')          // if Blank tile show select dialog
                        {
                            ChooseBlankLetter(tiles[i], arrowSquare);
                        }
                        do arrowSquare.row++;
                        while ((arrowSquare.row < 15) && (ContainsTile(arrowSquare))); // skip occupied squares
                        if (arrowSquare.row < 15)
                        {
                            PositionArrow();
                        }
                        else
                        {
                            canvas.Children.Remove(arrowShapes[3]);
                        }
                        break;
                    }
                    else if ((arrow == dirs.Rack) && (tiles[i].col < 0))
                    {
                        if (arrowSquare.col > 6) return;
                        MoveToRack(tiles[i], arrowSquare.col);
                        arrowSquare.col++;
                        if(arrowSquare.col < 7)  PositionArrow();
                        else canvas.Children.Remove(arrowShapes[0]);
                        break;
                    }
                }
            }
        }

        private void BackSpace()
        {
            if (arrow == dirs.Rack)
            {
                BackSpaceRack();
                return;
            }
            if ((arrow == dirs.Right) || (arrow == dirs.RedRight))
            {
                if (arrowSquare.col == 0) return;
                arrowSquare.col--;
                if (arrowSquare.col == 14)
                {
                    canvas.Children.Add(arrowShapes[(int)arrow]);
                }
            }
            else
            {
                if (arrowSquare.row == 0) return;
                arrowSquare.row--;
                if (arrowSquare.row == 14)
                {
                    canvas.Children.Add(arrowShapes[(int)arrow]);
                }
            }

            for(int i=0; i<tiles.Count; i++)
            {
                if ((tiles[i].col == arrowSquare.col) && (tiles[i].row == arrowSquare.row))
                {
                    PositionArrow();
                    if ((arrow == dirs.Right) || (arrow == dirs.Down))
                    {
                        ReturnToBox(i);
                        break;
                    }
                    else
                    {
                        ReturnToRack(i);
                        break;
                    }
                }
            }
        }

        private void BackSpaceRack()
        {
            if (arrowSquare.col == 0) return;
            arrowSquare.col--;
            if (arrowSquare.col == 6)
            {
                canvas.Children.Add(arrowShapes[0]);
            }
            PositionArrow();
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].col - 100 == arrowSquare.col)
                {
                    ReturnToBox(i);
                    rack = rack.Remove(arrowSquare.col, 1).Insert(arrowSquare.col, SPACE.ToString());
                    break;
                }
            }
        }

        private void UpdateArrow(_square sq)
        {
            if (sq.col < 0) sq.col = 0;
            if (sq.col > 14) sq.col = 14;
            if (sq.row < 0) sq.row = 0;
            if (sq.row > 14) sq.row = 14;
            if (sq != arrowSquare)          // new square
            {
                if ((arrow == dirs.Right) || (arrow == dirs.Rack))
                {
                    canvas.Children.Remove(arrowShapes[0]);
                }
                else if (arrow == dirs.Down)
                {
                    canvas.Children.Remove(arrowShapes[1]);
                }
                else if (arrow == dirs.RedRight)
                {
                    canvas.Children.Remove(arrowShapes[2]);
                }
                else if (arrow == dirs.RedDown)
                {
                    canvas.Children.Remove(arrowShapes[3]);
                }
                canvas.Children.Add(arrowShapes[0]);
                Canvas.SetLeft(arrowShapes[0], 500 + 50 * sq.col);
                Canvas.SetTop(arrowShapes[0], sq.row * 50);
                arrow = dirs.Right;
                arrowSquare = sq;
                rectRack.Stroke = transparent;
                box.Stroke = black;
            }
            else                                    // same square
            {
                if (arrow == dirs.None)
                {
                    canvas.Children.Add(arrowShapes[0]);
                    Canvas.SetLeft(arrowShapes[0], 500 + 50 * sq.col);
                    Canvas.SetTop(arrowShapes[0], sq.row * 50);
                    arrow = dirs.Right;
                    box.Stroke = black;
                }
                else if (arrow == dirs.Right)
                {
                    canvas.Children.Remove(arrowShapes[0]);
                    canvas.Children.Add(arrowShapes[1]);
                    Canvas.SetLeft(arrowShapes[1], 500 + 50 * sq.col);
                    Canvas.SetTop(arrowShapes[1], sq.row * 50);
                    arrow = dirs.Down;
                }
                else if (arrow == dirs.Down)
                {
                    canvas.Children.Remove(arrowShapes[1]);
                    canvas.Children.Add(arrowShapes[2]);
                    Canvas.SetLeft(arrowShapes[2], 500 + 50 * sq.col);
                    Canvas.SetTop(arrowShapes[2], sq.row * 50);
                    arrow = dirs.RedRight;
                    box.Stroke = transparent;
                    rectRack.Stroke = red;
                }
                else if (arrow == dirs.RedRight)
                {
                    canvas.Children.Remove(arrowShapes[2]);
                    canvas.Children.Add(arrowShapes[3]);
                    Canvas.SetLeft(arrowShapes[3], 500 + 50 * sq.col);
                    Canvas.SetTop(arrowShapes[3], sq.row * 50);
                    arrow = dirs.RedDown;
                }
                else if (arrow == dirs.RedDown)
                {
                    canvas.Children.Remove(arrowShapes[3]);
                    arrow = dirs.None;
                    rectRack.Stroke = transparent;
                }
            }
        }

        private void PositionArrow()
        {
            int i = (int)arrow;
            if (i < 5)
            {
                Canvas.SetLeft(arrowShapes[i], 500 + 50 * arrowSquare.col);
                Canvas.SetTop(arrowShapes[i], 50 * arrowSquare.row);
            }
            else
            {
                Canvas.SetLeft(arrowShapes[0], RACK_LEFT + 50 * arrowSquare.col);
                Canvas.SetTop(arrowShapes[0], RACK_TOP);
            }
        }

        private void RemoveAllArrows()
        {
            foreach (Rectangle _arrow in arrowShapes)
            {
                canvas.Children.Remove(_arrow);
            }
            box.Stroke = transparent;
            rectRack.Stroke = transparent;
            arrow = dirs.None;
            arrowSquare.col = arrowSquare.row = 15;
        }

        #region Test

        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = Mouse.GetPosition(canvas);
            int col = (int)(pt.X - 500) / 50;
            int row = (int)pt.Y / 50;
            engine.FindWords(col, row, results);
            ShowWords();
        }

        private void cmdPause_Click(object sender, RoutedEventArgs e)
        {
            engine.Pause();
        }

        private void cmdTest_Click(object sender, RoutedEventArgs e)
        {
            //            engine.Test();
            //            ShowOnBoard(0, true);
            engine.GetWords(results, rack, board, 6);
        }

        #endregion // Test

        private bool Validated()
        {
            Validate val = new Validate("ScrabbleCheat");
            return val.CheckValid();
        }

        private void RackClicked()
        {
            RemoveAllArrows();
            int pos = TidyRack();
            canvas.Children.Add(arrowShapes[0]);
            Canvas.SetLeft(arrowShapes[0], RACK_LEFT + 50 * pos);
            Canvas.SetTop(arrowShapes[0], RACK_TOP);
            arrow = dirs.Rack;
            arrowSquare.col = pos;
            arrowSquare.row = 100;
        }

        private void MarkDirty(bool isDirty=true)
        {
            this.isDirty = isDirty;
            if (isDirty) Title = windowTitle + name + "*";
            else Title = windowTitle + name;
        }
    }
}
