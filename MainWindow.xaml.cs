using System;
using System.Collections.Generic;
using System.Linq;
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
        string StringBoardStart = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        Board BoardMain = new Board("8/8/8/8/8/8/8/8 w - - 0 1");
        SolidColorBrush brushBlack = new SolidColorBrush(Color.FromRgb(70, 0, 0));
        SolidColorBrush brushWhite = new SolidColorBrush(Color.FromRgb(214, 228, 223));
        public MainWindow()
        {
            InitializeComponent();

            DrawBoard();
            LoadFen(BoardMain, StringBoardStart);
            LoadBoardState(BoardMain);
        }

        private void DrawBoard()
        {
            ClearChildren(GridBoard, "Squares");
            DrawSquares();
        }

        private void LoadBoardState(Board board)
        {
            ClearChildren(GridBoard, "Pieces");
            DrawPieces();
        }

        private void ClearChildren(Grid grid, string type)
        {
            int intTotalChildren = grid.Children.Count - 1;
            for (int intCounter = intTotalChildren; intCounter > 0; intCounter--)
            {
                switch (type)
                {
                    case "Squares":
                        ClearSquares(grid, intCounter);
                        break;
                    case "Pieces":
                        ClearPieces(grid, intCounter);
                        break;
                }
            }
        }
        private void ClearSquares(Grid grid, int intCounter)
        {
            if (grid.Children[intCounter].GetType() == typeof(Rectangle))
            {
                Rectangle ucCurrentChild = (Rectangle)grid.Children[intCounter];
                grid.Children.Remove(ucCurrentChild);
            }
        }
        private void ClearPieces(Grid grid, int intCounter)
        {
            if (grid.Children[intCounter].GetType() == typeof(Image))
            {
                Image ucCurrentChild = (Image)grid.Children[intCounter];
                grid.Children.Remove(ucCurrentChild);
            }
        }

        private void DrawPieces()
        {
            for (int i = 0; i < 64; i++)
            {
                if (!(BoardMain.pos[i] == 0)) //if commented out, will put blank images in unoccupied squares to stop columns/rows getting removed
                {
                    DrawPieceAt(i, BoardMain.pos[i]);
                }
            }
        }

        private void DrawSquares()
        {
            bool even = true;
            for (int i = 0; i < 64; i++)
            {
                if (i % 8 == 0)
                    even = !even;
                if (IsEven(i) == even)
                    DrawSquare(i, brushBlack);
                else
                    DrawSquare(i, brushWhite);
            }
        }

        private void DrawSquare(int i, SolidColorBrush brush)
        {
            Rectangle r = new Rectangle()
            {
                Stretch = Stretch.Fill,
                Fill = brush
            };
            GridBoard.Children.Add(r);
            Grid.SetColumn(r, i % 8);
            Grid.SetRow(r, i / 8);
        }

        private void DrawPieceImage(int i, string colour, string piece)
        {
            Image im = new Image
            {
                Stretch = Stretch.UniformToFill,
                Source = new ImageSourceConverter().ConvertFromString("data/" + piece + colour + ".png") as ImageSource
            };
            GridBoard.Children.Add(im);
            Grid.SetColumn(im, i % 8);
            Grid.SetRow(im, i / 8);
        }

        private void DrawPieceAt(int i, char c)
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
            DrawPieceImage(i, colour.ToString(), piece.ToString());
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
    }
}
