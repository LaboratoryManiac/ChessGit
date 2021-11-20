﻿using System;
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
            Rectangle r = RectangleAtCell(pos[0], pos[1], GridBoard);

            if (r == null)
                MBNull(pos.ToString() + "has no rectangle to paint");
            else
                r.Fill = brush;
        }

        private void SetPieceImage(int[] pos, string colour, string piece)
        {

            Image i = ImageAtCell(pos[0], pos[1], GridBoard);

            if (i == null)
                MBNull(pos.ToString() + "has no image to set source");
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

            int[] pos = IntPos(i);

            SetPieceImage(pos, colour.ToString(), piece.ToString());
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
                msPos.X += DRAG_OFFSET_X_MULT * Width;
                msPos.Y += DRAG_OFFSET_Y_MULT * Width;
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
            ImageDrag.MaxHeight = r0.ActualHeight;
            ImageDrag.MaxWidth = r0.ActualWidth;
        }
    }
}
