﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System;
using System.Drawing;
using Point = System.Windows.Point;

namespace Chess.ChessEngine
{
    public class ChessFigure
    {

        public const int EdgeX = 7 * 6;
        public const int EdgeY = 7 * 6;
        public const int Square = 16 * 6;

        public List<MoveType> MoveTypes;
        public Team FigureTeam;
        public FigureType Type;

        public Label ChessIcon;
        public bool MouseDown;

        public Point Position;
        public Point Offset;
        private Board Board;
        public int MaxStep;
        public ChessFigure(Board board, Team team, FigureType type, int x, int y, int maxStep, params MoveType[] moveTypes)
        {
            Board = board;
            MaxStep = maxStep;
            FigureTeam = team;
            Type = type;
            MoveTypes = moveTypes.ToList();
            ChessIcon = new Label()
            {
                TabIndex = 99,
                Width = Square,
                Height = Square,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Wheat,
                Background = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(team.ToString()[0] + "_" + type.ToString() + ".png", UriKind.Relative)),
                    Stretch = Stretch.Uniform,
                },
                HorizontalAlignment = HorizontalAlignment.Left,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                AllowDrop = true,
                Cursor = Board.UserTeam == team ? Cursors.Hand : Cursors.No

            };
            RenderOptions.SetCachingHint(ChessIcon, CachingHint.Cache);
            RenderOptions.SetBitmapScalingMode(ChessIcon, BitmapScalingMode.NearestNeighbor);
            Set(x, y);
            ChessIcon.MouseDown += OnMouseDown;
            Board.BoardGrid.Children.Add(ChessIcon);
            Board.Figures.Add(this);
        }
        public void OnMouseUp()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Position.X != i || Position.Y != j)
                    {
                        Rectangle rect = new(i * Square, j * Square, Square, Square);
                        if (rect.Contains((int)ChessIcon.Margin.Left, (int)ChessIcon.Margin.Top))
                        {
                            MoveType moveType = CountMoveType(i, j);
                            int step = CountStep(i, j, moveType);
                            if ((MoveTypes.Contains(moveType) || MoveTypes.Contains(MoveType.Everywhere)) && moveType != MoveType.Unknown && (step <= MaxStep))
                            {
                                var args = new FigureStepEventArgs(this, Position, i, j, moveType, step);
                                Move(i, j);
                                Board.Move(args);
                                return;
                            }
                        }
                    }

                }
            }
            Return();
        }
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Board.UserTeam == FigureTeam && Board.LastMove != FigureTeam)
            {
                Position = new((int)(ChessIcon.Margin.Left / Square), (int)(ChessIcon.Margin.Top / Square));
                MouseDown = true;
                Offset = e.GetPosition(Board.BoardGrid);
                Offset.X -= ChessIcon.Margin.Left;
                Offset.Y -= ChessIcon.Margin.Top;
                Board.BoardGrid.CaptureMouse();
            }
        }
        public bool CanGoTo(int x, int y)
        {
            var moveType = CountMoveType(x, y);
            var steps = CountStep(x, y, moveType);
            if ((Position.X == x && Position.Y == y) || moveType == MoveType.Unknown || (!MoveTypes.Contains(moveType) && !MoveTypes.Contains(MoveType.Everywhere)) || steps > MaxStep)
                return false;
            foreach (var figure in Board.Figures)
            {
                if (figure != this)
                    if ((figure.Position.X == x && figure.Position.Y == y) || IsObjectOnLine(moveType, Position, new Point(x, y), figure.Position))
                        return false;
            }
            return true;
        }
        public bool IsObjectOnLine(MoveType moveType, Point start, Point end, Point obj)
        {
            int x1 = (int)Math.Min(start.X, end.X);
            int y1 = (int)Math.Min(start.Y, end.Y);
            int x2 = (int)Math.Max(start.X, end.X);
            int y2 = (int)Math.Max(start.Y, end.Y);
            switch (moveType)
            {
                case MoveType.Vertical:
                    return obj.Y <= y1 && obj.Y >= y2;
                case MoveType.Horisontal:
                    return obj.X <= x1 && obj.X >= x2;
                case MoveType.Diagonal:
                    return obj.Y <= y1 && obj.Y >= y2 && obj.X <= x1 && obj.X >= x2;
                default:
                    return false;
            }
        }
        public bool GoToNearestFreeSpace()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (CanGoTo(i, j) && FindCautions(i, j).Count == 0)
                    {
                        Move(i, j);
                        return true;
                    }

                }
            }
            return false;
        }
        public List<ChessFigure> FindCautions() => FindCautions((int)Position.X, (int)Position.Y);
        public List<ChessFigure> FindCautions(int x, int y)
        {
            List<ChessFigure> result = new();
            foreach (var figure in Board.Figures)
                if (figure != this && figure.CanGoTo(x, y))
                {
                    result.Add(figure);
                }
            return result;
        }
        public void Move(int x, int y)
        {
            Set(x, y);
            Board.LastMove = FigureTeam;
        }
        public void Set(int x, int y)
        {
            ChessIcon.Margin = new Thickness(EdgeX + x * Square, EdgeY + y * Square, 0, 0);
            Position = new(x, y);
        }
        public void Return() => Set((int)Position.X, (int)Position.Y);
        public MoveType CountMoveType(int x, int y)
        {
            if (Position.X == x && Position.Y != y)
                return MoveType.Vertical;
            else if (Position.X != x && Position.Y == y)
                return MoveType.Horisontal;
            else if (Math.Abs(Position.X - x) == Math.Abs(Position.Y - y))
                return MoveType.Diagonal;
            else
                return MoveType.Unknown;
        }
        public int CountStep(int x, int y, MoveType moveType)
        {
            return moveType switch
            {
                MoveType.Unknown => 0,
                MoveType.Everywhere => 0,
                MoveType.Vertical => (int)Math.Abs(Position.Y - y),
                MoveType.Horisontal => (int)Math.Abs(Position.X - x),
                MoveType.Diagonal => (int)Math.Abs(Position.X - x),
                _ => 0,
            };
        }
    }

    public enum Team
    {
        White,
        Black
    }
    public enum FigureType
    {
        Pawn,
        King,
        Rook,
        Knight,
        Bishop
    }
    public enum MoveType
    {
        Unknown,
        Horisontal,
        Vertical,
        Diagonal,
        Everywhere
    }
}
