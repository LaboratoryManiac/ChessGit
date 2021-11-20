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
using static Chess.CharFunc;
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
        //mult * distance = offset of drag-and-drop image - by default it may not be in the right place relative to cursor to look good
        double DRAG_OFFSET_X_MULT = -0.125;
        double DRAG_OFFSET_Y_MULT = -0.125;
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

        private void SetPieces()
        {
            for (int i = 0; i < 64; i++)
            {
                SetPieceAt(i, BoardMain.pos[i]);
            }
        }

        private void PaintSquares()
        {
            bool even = true;
            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                    even = !even;
                if (IsEven(i) == even)
                    PaintSquare(i, brushBlack);
                else
                    PaintSquare(i, brushWhite);
            }
        }

        private void PaintSquare(int pos, SolidColorBrush brush)
        {
            string name = "r" + pos.ToString();
            Rectangle r = (Rectangle)FindDescendant(GridBoard, name);
            if (r == null)
                MBNull(name);
            else
                r.Fill = brush;
        }

        // Find a descendant control by name.
        private static DependencyObject FindDescendant(
            DependencyObject parent, string name)
        {
            // See if this object has the target name.
            FrameworkElement element = parent as FrameworkElement;
            if ((element != null) && (element.Name == name)) return parent;

            // Recursively check the children.
            int num_children = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < num_children; i++)
            {
                // See if this child has the target name.
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                DependencyObject descendant = FindDescendant(child, name);
                if (descendant != null) return descendant;
            }

            // We didn't find a descendant with the target name.
            return null;
        }

        private void SetPieceImage(int pos, string colour, string piece)
        {
            string name = "i" + pos.ToString();
            Image i = (Image)FindDescendant(GridBoard, name);
            if (i == null)
                MBNull(name);
            else if (piece == "Null")
                i.Source = SourceBlank;
            else
                i.Source = conv.ConvertFromString("data/" + piece + colour + ".png") as ImageSource;
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
            SetPieceImage(i, colour.ToString(), piece.ToString());
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
            int pos = MouseGridPos((UIElement)e.Source);

            ImageDragTemp = (Image)FindDescendant(GridBoard, "i" + pos.ToString());
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
                msPos.X += DRAG_OFFSET_X_MULT * Width;
                msPos.Y += DRAG_OFFSET_Y_MULT * Width;
                ImageDrag.RenderTransform = new TranslateTransform(msPos.X, msPos.Y);
            }
        }

        private void GridBoard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (true)//move is in legalmove list
            {
                int pos = MouseGridPos((UIElement)e.Source);

                Image newImage = (Image)FindDescendant(GridBoard, "i" + pos.ToString());
                newImage.Source = SourceDragTemp;
            }
            else
            {
                ImageDragTemp.Source = SourceDragTemp;
            }


            Cursor = Cursors.Arrow;
            ImageDrag.Visibility = Visibility.Collapsed;
        }

        private void ImageDrag_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ImageDrag.IsMouseCaptured)
            {
                ImageDrag.ReleaseMouseCapture();
                GridBoard.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left) { RoutedEvent = Button.MouseUpEvent });
            }
        }

        private int MouseGridPos(UIElement element)
        {
            int row = Grid.GetRow(element);
            int column = Grid.GetColumn(element);

            return row * 8 + column;
        }

        private void r0_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ImageDrag.Height = r0.Height;
            ImageDrag.Width = r0.Width;
        }
    }
}
