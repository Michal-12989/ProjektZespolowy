using Microsoft.Win32;
using System.IO;
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
using PGNReader;
namespace ChessboardView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
   public partial class MainWindow : Window
{
    private Image[,] squares = new Image[8, 8];
    string pgn;
    string fen;
    int currentTurn = 0;
    int maxTurns = 0;
    public MainWindow()
    {
        InitializeComponent();
    GenerateBoard();
        }

        private void GenerateBoard()
        {
            BoardGrid.Children.Clear();
            BoardGrid.RowDefinitions.Clear();
            BoardGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < 8; i++)
            {
                BoardGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                BoardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Border square = new Border
                    {
                        Background = (row + col) % 2 == 0 ? new SolidColorBrush(Color.FromRgb(235, 236, 208)) : new SolidColorBrush(Color.FromRgb(115, 149, 82)),
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };

                    Image pieceImage = new Image { Stretch = Stretch.Uniform };
                    square.Child = pieceImage;

                    squares[row, col] = pieceImage;

                    Grid.SetRow(square, row);
                    Grid.SetColumn(square, col);
                    BoardGrid.Children.Add(square);
                }
            }
        }

        private void LoadFEN(string fen)
        {
            string[] ranks = fen.Split('/');
            if (ranks.Length != 8) throw new ArgumentException("FEN must have 8 ranks");

            for (int r = 0; r < 8; r++)
            {
                int c = 0;
                foreach (char ch in ranks[r])
                {
                    if (char.IsDigit(ch))
                    {
                        c += (int)char.GetNumericValue(ch);
                    }
                    else
                    {
                        if (c < 0 || c > 7) break;

                        string imageName = ch switch
                        {
                            'P' => "white-pawn.png",
                            'R' => "white-rook.png",
                            'N' => "white-knight.png",
                            'B' => "white-bishop.png",
                            'Q' => "white-queen.png",
                            'K' => "white-king.png",
                            'p' => "black-pawn.png",
                            'r' => "black-rook.png",
                            'n' => "black-knight.png",
                            'b' => "black-bishop.png",
                            'q' => "black-queen.png",
                            'k' => "black-king.png",
                            _ => null
                        };

                        if (imageName != null)  
                        {
                            int visualRow = 7 - r; // rank 1 at bottom

                            string uri = $"pack://application:,,,/ChessboardView;component/Assets/PieceImages/{imageName}";
                            squares[visualRow, c].Source = new BitmapImage(new Uri(uri));
                        }

                        c++;
                    }
                }
            }
        }

        private void ClearBoard()
        {
            for (int r = 0; r < 8; r++)
                for (int c = 0; c < 8; c++)
                    squares[r, c].Source = null;
        }
        private void UpdateBoard()
        {
            ClearBoard();
            var result = Reader.Read(pgn, currentTurn);
            fen = result.fen;
            maxTurns = result.maxTurns;
            GenerateBoard();
            LoadFEN(fen);
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a PGN file",
                Filter = "PGN files (*.pgn)|*.pgn"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                pgn = File.ReadAllText(filePath);
                currentTurn = 0;
                UpdateBoard();

            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (maxTurns > 0){
                currentTurn = 0;
                UpdateBoard();
            }
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (maxTurns > 0  && currentTurn>0)
            {
                currentTurn = currentTurn-1;
                UpdateBoard();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (maxTurns > 0 && maxTurns > currentTurn)
            {
                currentTurn = currentTurn + 1;
                UpdateBoard();
            }
        }

        private void EndButton_Click(object sender, RoutedEventArgs e)
        {
            if (maxTurns > 0)
            {
                currentTurn = maxTurns;
                UpdateBoard();
            }
        }
    }
}