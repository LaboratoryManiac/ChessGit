﻿using System;
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
            PaintSquares();
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
                    case "Pieces":
                        ClearPieces(grid, intCounter);
                        break;
                }
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
                MessageBox.Show(name);
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
