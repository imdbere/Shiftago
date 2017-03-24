using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftago
{
    public class Bot
    {
        Board ActualBoard;
        public PlayerColor OwnColor;
        Random Rand;

        List<int> PushableLines;
        public Bot(Board gameBoard, PlayerColor ownColor)
        {
            PushableLines = new List<int>();
            ActualBoard = gameBoard;
            OwnColor = ownColor;
            Rand = new Random();
            for (int i = 0; i < ActualBoard.GridSize * 4; i++)
                PushableLines.Add(i);
        }
        int? CheckWin(PlayerColor color)
        {
            return CheckWin(color, ActualBoard);
        }
        int? CheckWin(PlayerColor color, Board board)
        {
            Board virtualBoard;
            for (int i=0; i<ActualBoard.GridSize*4; i++)
            {
                virtualBoard = new Board(board);
                if (!virtualBoard.Push(i, color))
                    continue;

                if (virtualBoard.CheckForWin(color))
                    return i;
            }
            return null;
        }

        List<int> GetPushableLines()
        {
            for (int i = 0; i < ActualBoard.GridSize; i++)
            {
                if (ActualBoard.GetEmptyInColumn(i).Count == 0)
                {
                    PushableLines.Remove(i + ActualBoard.GridSize);
                    PushableLines.Remove(ActualBoard.GridSize * 4 - i -1);
                }
                if (ActualBoard.GetEmptyInRow(i).Count == 0)
                {
                    PushableLines.Remove(i);
                    PushableLines.Remove(ActualBoard.GridSize * 3 - i - 1);
                }

            }
            return new List<int>(PushableLines);
        }

        public int? NextMove()
        {
            List<int> possibleMoves = GetPushableLines();
            if (possibleMoves.Count == 0)
                return null;

            //Check if own Bot can Win
            int? ownWin = CheckWin(OwnColor);
            if (ownWin != null)
            {
                ActualBoard.Push((int)ownWin, OwnColor);
                return (int)ownWin;
            }
            //Check if next Player can Win
            {
                List<int> toRemove = new List<int>();
                foreach (int move in possibleMoves)
                {
                    Board virtualBoard = new Board(ActualBoard);
                    virtualBoard.Push(move, OwnColor);
                    //Board tmpBoard = new Board(virtualBoard);
                    int? nextPlayerStillWins = CheckWin(Players.GetNextPlayer(), virtualBoard);
                    if (nextPlayerStillWins != null)
                        toRemove.Add(move);
                    
                }
                if (toRemove.Count < possibleMoves.Count)
                    possibleMoves = possibleMoves.Except(toRemove).ToList();
            }

            {
                //Check if next Player after next Player can Win
                List<int> papWinMoves = new List<int>();
                foreach (int move in possibleMoves)
                {
                    //P0
                    Board virtualBoard = new Board(ActualBoard);
                    virtualBoard.Push(move, OwnColor);
                    bool allWin = true;
                    foreach (int move2 in PushableLines)
                    {
                        Board tmpBoard = new Board(virtualBoard);
                        tmpBoard.Push(move2, Players.GetNextPlayer());
                        if (CheckWin(Players.GetNextPlayer(Players.GetNextPlayer()), tmpBoard) == null)
                        {
                            allWin = false;
                            break;
                        }
                    }
                    if (allWin)
                        papWinMoves.Add(move);
                }
                if (Players.PlayerCount > 2)
                {
                    if (papWinMoves.Count < possibleMoves.Count)
                        possibleMoves = possibleMoves.Except(papWinMoves).ToList();
                }
                else if (papWinMoves.Count > 0)
                    possibleMoves = papWinMoves;

            }

            {
                //Check if next Player after next Player after next Player can Win
                if (Players.PlayerCount == 2)
                {
                    List<int> papapWinMoves = new List<int>();
                    foreach (int move in possibleMoves)
                    {
                        //P0
                        Board virtualBoard = new Board(ActualBoard);
                        virtualBoard.Push(move, OwnColor);
                        bool allWin = false;
                        foreach (int move2 in PushableLines)
                        {
                            Board tmpBoard = new Board(virtualBoard);
                            tmpBoard.Push(move2, Players.GetNextPlayer());
                            allWin = true;
                            //P1
                            foreach (int move3 in PushableLines)
                            {
                                //P0
                                Board tmpBoard1 = new Board(tmpBoard);
                                tmpBoard1.Push(move3, OwnColor);
                                if (CheckWin(Players.GetNextPlayer(), tmpBoard1) == null)
                                {
                                    allWin = false;
                                    break;
                                }
                            }
                            if (allWin)
                                break;
                        }
                        if (allWin)
                            papapWinMoves.Add(move);
                    }
                    if (papapWinMoves.Count < possibleMoves.Count)
                        possibleMoves = possibleMoves.Except(papapWinMoves).ToList();
                }
            }





            //Avoid unavoidable encounter from opposite side
            int? avoidingMove = AvoidStraightEncounter(Players.GetNextPlayer(), ActualBoard, Players.PlayerCount != 2);
            if (avoidingMove != null)
            {
                if (possibleMoves.Contains((int)avoidingMove))
                {
                    ActualBoard.Push((int)avoidingMove, OwnColor);
                    return (int)avoidingMove;
                }
            }
            if (Players.PlayerCount != 2)
            {
                int? avoidingMove1 = AvoidStraightEncounter(Players.GetNextPlayer(Players.GetNextPlayer()), ActualBoard, true);
                if (avoidingMove1 != null)
                {
                    if (possibleMoves.Contains((int)avoidingMove1))
                    {
                        ActualBoard.Push((int)avoidingMove1, OwnColor);
                        return (int)avoidingMove1;
                    }
                }
            }

            //Do random move if nothing else is possible
            int nextMove;
            if (possibleMoves.Count == 0)
            {
                nextMove = PushableLines[Rand.Next(0, PushableLines.Count - 1)];
                ActualBoard.Push(nextMove, OwnColor);
                return nextMove;
            } 

            // Rate remaining moves
            Dictionary<int, double> ratings = new Dictionary<int, double>();
            foreach (int move in possibleMoves)
            {
                Board virtualBoard = new Board(ActualBoard);
                virtualBoard.Push(move, OwnColor);
                ratings.Add(move, EvaluateOwnBoard(virtualBoard));
            }

            List<int> maxKeys = new List<int>();
            double max = ratings.Values.First();

            foreach (KeyValuePair<int, double> vals in ratings)
            {
                if (vals.Value > max)
                    max = vals.Value;
            }
            foreach (KeyValuePair<int, double> vals in ratings)
            {
                if (vals.Value >= max - BotConst.MoveRandomFactor)
                    maxKeys.Add(vals.Key);
            }

            //Choose random move from best rated moves
            nextMove = maxKeys[Rand.Next(0, maxKeys.Count - 1)];
            ActualBoard.Push(nextMove, OwnColor);

            return nextMove;
        }
        double EvaluateOwnBoard(Board board)
        {
            //List<double> PlayerValues = new List<double>();
            double totalValue = 0;
            totalValue += EvaluatePlayerBoard(board, OwnColor);
            

            List<double> otherPlayerValues = new List<double>();
            foreach (int move in PushableLines)
            {
                PlayerColor nextPlayer = Players.GetNextPlayer();
                Board virtualBoard = new Board(board);
                virtualBoard.Push(move, nextPlayer);
                double value = EvaluatePlayerBoard(virtualBoard, nextPlayer) * BotConst.NextPlayerRatio;
                for (int i=0; i<Players.PlayerCount - 2; i++)
                {
                    nextPlayer = Players.GetNextPlayer(nextPlayer);
                    value += EvaluatePlayerBoard(virtualBoard, nextPlayer) * BotConst.OtherPlayersRatio;
                }
                
                otherPlayerValues.Add(value);
            }
            totalValue -= otherPlayerValues.Max();
            return totalValue;
        }
        double EvaluatePlayerBoard(Board board, PlayerColor player)
        {
            int[,] valueGrid = (int[,])BotConst.GridValues.Clone();

            for (int x=0; x<ActualBoard.GridSize; x++)
            {
                for (int y = 0; y < ActualBoard.GridSize; y++)
                {
                    if (board.Grid[x,y] == player)
                        AddValueToSurrounding(x, y, valueGrid, player);
                }
            }

            double sum = 0;
            for (int x = 0; x < ActualBoard.GridSize; x++)
            {
                for (int y = 0; y < ActualBoard.GridSize; y++)
                {
                    if (board.Grid[x, y] == player)
                        sum += valueGrid[x, y];
                }
            }

            return sum;
        }
        public void AddValueToSurrounding(int x, int y, int[,] valueGrid, PlayerColor player)
        {
            int influence = BotConst.DirectNeighbourInfluence;
            if (player == OwnColor)
                influence = BotConst.DirectNeighbourInfluenceForEnemy;
            if (x > 0 && y > 0)
                valueGrid[x - 1, y - 1] += influence;
            if (x > 0)
                valueGrid[x - 1, y] += influence;
            if (y < ActualBoard.GridSize - 1 && x > 0)
                valueGrid[x - 1, y + 1] += influence;
            if (y < ActualBoard.GridSize - 1 )
                valueGrid[x, y + 1] += influence;
            if (x < ActualBoard.GridSize - 1 && y < ActualBoard.GridSize - 1)
                 valueGrid[x + 1, y + 1] += influence;
            if (x < ActualBoard.GridSize - 1)
                valueGrid[x + 1, y] += influence;
            if (x < ActualBoard.GridSize - 1 && y > 0 )
                valueGrid[x + 1, y - 1] += influence;
            if (y > 0)
                valueGrid[x, y - 1] += influence;

        }

        int? AvoidStraightEncounter(PlayerColor color, Board board, bool critialOnly)
        {
            for (int i=1; i<ActualBoard.GridSize - 1; i++)
            {
                int count1 = 0;
                for (int j = 0; j < ActualBoard.GridSize; j++)
                {
                    if (board.Grid[i, j] == color)
                        count1++;
                    else if (board.Grid[i, j] != PlayerColor.Empty)
                        count1 = 0;
                }
                if ((count1 == board.WinCount - 2) && (board.Grid[i, 0] == color || board.Grid[i, 0] == PlayerColor.Empty) && (board.Grid[i, board.GridSize - 1] == color || board.Grid[i, board.GridSize - 1] == PlayerColor.Empty) && !critialOnly)
                {
                    return ActualBoard.GridSize * 4 - 1 - i;
                }

                if ((count1 == board.WinCount - 1) && (board.Grid[i, 0] == color || board.Grid[i, board.GridSize - 1] == color))
                {
                    if (board.Grid[i, 0] == color)
                    {
                        return ActualBoard.GridSize*4 -1 - i;
                    }
                    else
                    {
                        return ActualBoard.GridSize + i;
                    }

                }

                int count2 = 0;
                for (int j = 0; j < ActualBoard.GridSize; j++)
                {
                    if (board.Grid[j, i] == color)
                        count2++;
                    else if (board.Grid[j, i] != PlayerColor.Empty)
                        count2 = 0;
                }
                if ((count2 == board.WinCount - 2) && (board.Grid[0, i] == color || board.Grid[0, i] == PlayerColor.Empty) && (board.Grid[board.GridSize - 1, i] == color || board.Grid[board.GridSize - 1, i] == PlayerColor.Empty) && !critialOnly)
                {
                    return i;
                }

                if ((count2 == board.WinCount - 1) && (board.Grid[0,i] == color || board.Grid[board.GridSize - 1, i] == color))
                {
                    if (board.Grid[0, i] == color)
                    {
                        return i;
                    }
                    else
                    {
                        return ActualBoard.GridSize * 3 - 1 - i;
                    }

                }
            }
            return null;
        }

    }
}
