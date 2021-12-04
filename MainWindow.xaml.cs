using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Chess.Board;
using static Chess.CharFunc;
using static Chess.DoubleFunc;
using static Chess.IntFunc;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private void MBNull(string s)
        {
            MessageBox.Show(s + "is null");
        }

        private readonly string StringBoardStart = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        private bool OpponentAI = false;
        private Board BoardMain = new();
        private readonly SolidColorBrush brushBlack = new(Color.FromRgb(70, 0, 0));
        private readonly SolidColorBrush brushWhite = new(Color.FromRgb(214, 228, 223));
        private readonly ImageSourceConverter conv = new();
        private readonly ImageSource SourceBlank;
        private Image ImageDragTemp;
        private ImageSource SourceDragTemp;

        //if mults are used for non-square images probably use an X and Y mult

        //mult * size = offset of drag-and-drop image - by default it may not be in the right place relative to cursor to look good
        private readonly double DRAG_OFFSET_MULT = -0.053;

        //mult * size = size change of drag-and-drop image
        private readonly double DRAG_SIZE_MULT = -0.889;
        private double LowerWindowLength = 0;
        public MainWindow()
        {
            SourceBlank = conv.ConvertFromString("data/NullNull.png") as ImageSource;

            InitializeComponent();

            DrawBoard();
            LoadFen(BoardMain, StringBoardStart); //StringBoardStart when finished debugging
        }

        private void DrawBoard()
        {
            PaintSquares();
        }

        private void PaintSquares()
        {
            bool even = true;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int[] pos = new int[] { i, j };

                    if (pos[1] == 0) //switch on start of each row otherwise you have columns of the same colour
                        even = !even;
                    if (IsEven(j) == even)
                        PaintSquare(pos, brushBlack);
                    else
                        PaintSquare(pos, brushWhite);
                }
            }
        }

        private void PaintSquare(int[] pos, SolidColorBrush brush)
        {
            try
            {
                Rectangle r = RectangleAtCell(pos[0], pos[1], GridBoard);
                r.Fill = brush;
            }
            catch
            {
                DrawSquare(pos, brush);
                PaintSquare(pos, brush);
            }
        }

        private void DrawSquare(int[] pos, SolidColorBrush brush)
        {
            Rectangle r = new();
            GridBoard.Children.Add(r);
            Grid.SetRow(r, pos[0]);
            Grid.SetColumn(r, pos[1]);
        }
        private void SetPieces(Piece[,] pieces)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Pos pos = new(i, j);
                    Piece p = pieces[i, j];
                    SetPieceImage(pos, p.Colour.ToString(), p.Type.ToString());
                }
            }
        }

        private void SetPieceImage(Pos pos, string colour, string piece)
        {
            try
            {
                Image i = ImageAtCell(pos.Row, pos.Column, GridBoard);
                i.Source = conv.ConvertFromString("data/" + piece + colour + ".png") as ImageSource;
            }
            catch
            {
                DrawPieceImage(pos);
                SetPieceImage(pos, colour, piece);
            }
        }

        private void DrawPieceImage(Pos pos)
        {
            Image i = new();
            GridBoard.Children.Add(i);
            Grid.SetRow(i, pos.Row);
            Grid.SetColumn(i, pos.Column);
        }

        private void SetTurn(ECOLOUR colour)
        {
            ImageTurn.Source = conv.ConvertFromString("data/" + "King" + colour + ".png") as ImageSource;
        }

        private void SetHalfM(int halfm)
        {
            LabelHalfM.Content = "Moves since capture/pawn move: " + halfm.ToString();
        }

        private void SetFullM(int fullm)
        {
            LabelFullM.Content = "Moves: " + fullm.ToString();
        }
        private void SetWinner(EWINNER winner)
        {
            LabelWinner.Content = winner switch
            {
                EWINNER.Null => "Ongoing",
                EWINNER.Black => "Black wins",
                EWINNER.White => "White wins",
                EWINNER.Draw => "Draw",
            };
        }

        private static Rectangle RectangleAtCell(int row, int column, Grid grid)
        {
            return ElementsAtCell(row, column, grid).OfType<Rectangle>().First();
        }
        private static Image ImageAtCell(int row, int column, Grid grid)
        {
            return ElementsAtCell(row, column, grid).OfType<Image>().First();
        }

        private static IEnumerable<UIElement> ElementsAtCell(int row, int column, Grid grid)
        {
            return grid.Children.Cast<UIElement>().Where(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == column);
        }

        private void ButtonFen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadFen(BoardMain, TextboxFen.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Invalid FEN");
            }
        }

        private void LoadFen(Board board, string fen)
        {
            BoardMain = new Board(fen);
            BoardVisualUpdate(BoardMain);
        }

        private void GridBoard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ImageDragTemp = (Image)e.MouseDevice.DirectlyOver;
            Pos pos = MouseGridPos(ImageDragTemp);
            SourceDragTemp = ImageDragTemp.Source;

            if (SourceDragTemp.ToString() != SourceBlank.ToString()) //if square not empty
            { //start dragging
                ImageDrag.Source = SourceDragTemp;
                SetPieceImage(pos, "Null", "Null");

                Cursor = Cursors.Hand;
                ImageDrag.Visibility = Visibility.Visible;
                ImageDrag.CaptureMouse();
            }
        }

        private void ImageDrag_MouseMove(object sender, MouseEventArgs e)
        {
            if (ImageDrag.IsMouseCaptured)
            {
                Point msPos = e.GetPosition(CanvasDrag);
                msPos.X += DRAG_OFFSET_MULT * LowerWindowLength;
                msPos.Y += DRAG_OFFSET_MULT * LowerWindowLength;
                ImageDrag.RenderTransform = new TranslateTransform(msPos.X, msPos.Y);
            }
        }

        private void ImageDrag_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ImageDrag.IsMouseCaptured) //should be unneccessary
            {
                Cursor = Cursors.Arrow;
                ImageDrag.Visibility = Visibility.Collapsed;
                ImageDrag.ReleaseMouseCapture();

                try //if not in try catch dropping piece out of window will cause crash when it should just reset piece
                {
                    UIElement parent = VisualTreeHelper.GetParent((UIElement)e.MouseDevice.DirectlyOver) as UIElement;

                    if (parent == GridBoard && BoardMain.winner == EWINNER.Null) //if dropped on board and game is ongoing
                    {
                        Image newImage = (Image)e.MouseDevice.DirectlyOver;

                        Pos start = MouseGridPos(ImageDragTemp);
                        Pos end = MouseGridPos(newImage);

                        if (BoardMain.PlayerMove(start, end))//TODO move is in legalmove list
                        {
                            newImage.Source = SourceDragTemp;
                            if (OpponentAI)
                                BoardMain.AIMove();
                            BoardVisualUpdate(BoardMain);
                        }
                        else
                        {
                            ResetDragged(ImageDragTemp, SourceDragTemp); //illegal move, reset
                        }
                    }
                    else
                    {
                        ResetDragged(ImageDragTemp, SourceDragTemp); //not on board, reset
                    }
                }
                catch
                {
                    ResetDragged(ImageDragTemp, SourceDragTemp); //not on board, reset
                }
            }
        }

        private void BoardVisualUpdate(Board board)
        {
            SetPieces(board.pieces);
            SetTurn(board.turn);
            SetHalfM(board.halfm);
            SetFullM(board.fullm);
            SetWinner(board.winner);
        }

        private void ResetDragged(Image im, ImageSource so)
        {
            im.Source = so;
        }

        private Pos MouseGridPos(Image element)
        {
            Pos pos = new()
            {
                Row = Grid.GetRow(element),
                Column = Grid.GetColumn(element),
            };

            return pos;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LowerWindowLength = GetLower(this.ActualHeight, this.ActualWidth);
            ImageDrag.MaxHeight = LowerWindowLength + DRAG_SIZE_MULT * LowerWindowLength;
            ImageDrag.MaxWidth = LowerWindowLength + DRAG_SIZE_MULT * LowerWindowLength;
        }

        private void ButtonOpponentAI_Click(object sender, RoutedEventArgs e)
        {
            OpponentAI = !OpponentAI;
            if (OpponentAI)
                ButtonOpponentAI.Content = "AI: Off";
            else
                ButtonOpponentAI.Content = "AI: On";
        }
    }
}
