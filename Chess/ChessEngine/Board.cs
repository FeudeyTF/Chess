using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chess.ChessEngine
{
    public delegate void FigureStep(FigureStepEventArgs args);
    public class Board
    {
        public event FigureStep OnFigureStep;
        public Team UserTeam = Team.White;
        public Team LastMove = Team.Black;
        public ChessFigure BlackKing { get; private set; }
        public ChessFigure WhiteKing { get; private set; }
        public ChessFigure WhiteRook { get; private set; }
        public List<ChessFigure> Figures { get; private set; }
        public Grid BoardGrid { get; }
        public Board(Grid boardGrid)
        {
            BoardGrid = boardGrid;
            Figures = new();
            BlackKing = new(this, Team.Black, FigureType.King, 0, 1, 1, MoveType.Everywhere);
            WhiteKing = new(this, Team.White, FigureType.King, 1, 0, 1, MoveType.Everywhere);
            WhiteRook = new(this, Team.White, FigureType.Rook, 7, 1, 7, MoveType.Vertical, MoveType.Horisontal);

            BoardGrid.PreviewMouseUp += Board_PreviewMouseUp;
            BoardGrid.PreviewMouseMove += Board_PreviewMouseMove;
        }
        public void Board_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            foreach (var fig in Figures)
            {
                if (fig.MouseDown)
                {
                    var pos = e.GetPosition(BoardGrid);
                    fig.ChessIcon.Margin = new(pos.X - fig.Offset.X, pos.Y - fig.Offset.Y, 0, 0);
                }
            }
        }

        public void Board_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var fig in Figures)
            {

                if (fig.MouseDown)
                {
                    fig.MouseDown = false;
                    fig.OnMouseUp();
                }
            }
            BoardGrid.ReleaseMouseCapture();
        }
        public void Move(FigureStepEventArgs args)
        {
            OnFigureStep(args);
        }
    }
}
