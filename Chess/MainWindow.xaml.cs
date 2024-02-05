using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using Chess.ChessEngine;
using Point = System.Windows.Point;

namespace Chess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Board MainBoard;
        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetCachingHint(Board, CachingHint.Cache);
            RenderOptions.SetBitmapScalingMode(Board, BitmapScalingMode.NearestNeighbor);
            MainBoard = new(Board);
            MainBoard.OnFigureStep += OnFigureStep;
        }

        private void OnFigureStep(FigureStepEventArgs args)
        {
            if (!MainBoard.BlackKing.GoToNearestFreeSpace())
                MessageBox.Show("ПОЗДРАВЛЯЕМ! Вы выиграли этого бота!");
        }
       
    }

    
    public class FigureStepEventArgs
    {
        public ChessFigure Figure;
        public Point LastPosition;
        public Point MovePosition;
        public int XStep;
        public int YStep;
        public MoveType Move;
        public int StepLength;
        public FigureStepEventArgs(ChessFigure figure, Point lastPostion, int x, int y, MoveType move, int stepLength)
        {
            Figure = figure;
            LastPosition = lastPostion;
            MovePosition = new(x, y);
            XStep = (int)(MovePosition.X - LastPosition.X);
            YStep = (int)(MovePosition.Y - LastPosition.Y);
            Move = move;
            StepLength = stepLength;
        }
    }
}
