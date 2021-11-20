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

        string StringBoardStart = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        Board BoardMain = new Board("8/8/8/8/8/8/8/8 w - - 0 1");
        SolidColorBrush brushBlack = new SolidColorBrush(Color.FromRgb(70, 0, 0));
        SolidColorBrush brushWhite = new SolidColorBrush(Color.FromRgb(214, 228, 223));

        ImageSourceConverter conv = new ImageSourceConverter();
        ImageSource SourceBlank;

        Image ImageDragTemp;
        ImageSource SourceDragTemp;
        //if mults are used for non-square images probably use an X and Y mult

        //mult * size = offset of drag-and-drop image - by default it may not be in the right place relative to cursor to look good
        double DRAG_OFFSET_MULT = -0.027;
        //mult * size = size change of drag-and-drop image
        double DRAG_SIZE_MULT = -0.889;
        public MainWindow()
        {
            SourceBlank = conv.ConvertFromString("data/Blank.png") as ImageSource;

            InitializeComponent();

            DrawBoard();
            LoadFen(BoardMain, StringBoardStart);
            LoadBoardState(BoardMain);
        }

        private void DrawBoard()
        {
            PaintSquares();
        }

        private void LoadBoardState(Board board)
        {
            SetPieces();
        }

        private void PaintSquares()
        {
            bool even = true;
            for (int i = 0; i < 64; i++)
            {
                int[] pos = IntPos(i);

                if (pos[1] == 0) //switch on start of each row (i % 8 == 0) otherwise you have columns of the same colour
                    even = !even;
                if (IsEven(i) == even)
                    PaintSquare(pos, brushBlack);
                else
                    PaintSquare(pos, brushWhite);
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
            Rectangle r = new Rectangle();
            GridBoard.Children.Add(r);
            Grid.SetRow(r, pos[0]);
            Grid.SetColumn(r, pos[1]);
        }
        private void SetPieces()
        {
            for (int i = 0; i < 64; i++)
            {
                SetPieceAt(i, BoardMain.pos[i]);
            }
        }

        private void SetPieceAt(int i, char c)
        {
            ECOLOUR colour;
            if (IsUpper(c))
                colour = ECOLOUR.White;
            else
                colour = ECOLOUR.Black;
            EPIECE piece = ToUpper(c) switch
            {
                'K' => EPIECE.King,
                'Q' => EPIECE.Queen,
                'R' => EPIECE.Rook,
                'B' => EPIECE.Bishop,
                'N' => EPIECE.Knight,
                'P' => EPIECE.Pawn,
                _ => EPIECE.Null
            };

            int[] pos = IntPos(i);

            SetPieceImage(pos, colour.ToString(), piece.ToString());
        }

        private void SetPieceImage(int[] pos, string colour, string piece)
        {
            try
            {
                Image i = ImageAtCell(pos[0], pos[1], GridBoard);
                if (piece == "Null")
                    i.Source = SourceBlank;
                else
                    i.Source = conv.ConvertFromString("data/" + piece + colour + ".png") as ImageSource;
            }
            catch
            {
                DrawPieceImage(pos);
                SetPieceImage(pos, colour, piece);
            }
        }

        private void DrawPieceImage(int[] pos)
        {
            Image i = new Image();
            GridBoard.Children.Add(i);
            Grid.SetRow(i, pos[0]);
            Grid.SetColumn(i, pos[1]);
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
            LoadBoardState(BoardMain);
        }

        private void GridBoard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ImageDragTemp = (Image)e.MouseDevice.DirectlyOver;
            int[] pos = MouseGridPos(ImageDragTemp);
            SourceDragTemp = ImageDragTemp.Source;

            if (SourceDragTemp != SourceBlank) //if square not empty
            { //start dragging
                ImageDrag.Source = SourceDragTemp;
                SetPieceImage(pos, "", "Null");

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
                msPos.X += DRAG_OFFSET_MULT * GridBoard.ActualWidth;
                msPos.Y += DRAG_OFFSET_MULT * GridBoard.ActualHeight;
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

                    if (parent == GridBoard)
                    {
                        if (true)//TODO move is in legalmove list
                        {
                            Image newImage = (Image)e.MouseDevice.DirectlyOver;
                            int[] pos = MouseGridPos(newImage);
                            newImage.Source = SourceDragTemp;
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

        private void ResetDragged(Image im, ImageSource so)
        {
            im.Source = so;
        }

        private int[] MouseGridPos(Image element)
        {
            int[] pos = new int[2];

            pos[0] = Grid.GetRow(element);
            pos[1] = Grid.GetColumn(element);

            return pos;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double d = GetLower(this.ActualHeight, this.ActualWidth);
            ImageDrag.MaxHeight = d + DRAG_SIZE_MULT * d;
            ImageDrag.MaxWidth = d + DRAG_SIZE_MULT * d;
        }
    }
}
