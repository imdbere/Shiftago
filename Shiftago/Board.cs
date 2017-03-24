using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Shiftago
{
    public enum PlayerColor { Empty=-1, White=0, Green=1, Blue=2, Orange=3}
    public enum Direction { Up, Down, Left, Right}
    public class Board
    {
        public PlayerColor[,] Grid;
        public int WinCount;
        public int GridSize;

        public Board (int gridSize, int winCount)
        {
            Grid = new PlayerColor[gridSize, gridSize];
            WinCount = winCount;
            GridSize = gridSize;

            Reset();
        }

        public Board (Board oldBoard)
        {
            WinCount = oldBoard.WinCount;
            GridSize = oldBoard.GridSize;

            Grid = oldBoard.GridCopy();
        }

        public PlayerColor[,] GridCopy()
        {
            return (PlayerColor[,])Grid.Clone();
        }

        public bool Push (int index, PlayerColor newColor)
        {
            if (index < GridSize)
                return Push(Direction.Right, index, newColor);
            else if (index < GridSize*2)
                return Push(Direction.Down, index-GridSize, newColor);
            else if (index < GridSize*3)
                return Push(Direction.Left, GridSize*3 - 1 - index, newColor);
            else if (index < GridSize*4)
                return Push(Direction.Up, GridSize * 4 - 1 - index, newColor);

            return false;
        }
        public bool Push (Direction dir, int index, PlayerColor newColor)
        {
            List<int> emptyList;
            switch (dir)
            {
                case Direction.Up:
                    emptyList = GetEmptyInColumn(index);
                    if (emptyList.Count == 0)
                        return false;

                    for (int y= (int)emptyList.First(); y>0; y--)
                    {
                        Grid[index, y] = Grid[index, y - 1];
                    }
                    Grid[index, 0] = newColor;
                    return true;


                case Direction.Down:
                    emptyList = GetEmptyInColumn(index);
                    if (emptyList.Count == 0)
                        return false;

                    for (int y = emptyList.Last(); y < GridSize-1; y++)
                    {
                        Grid[index, y] = Grid[index, y + 1];
                    }
                    Grid[index, GridSize-1] = newColor;
                    return true;


                case Direction.Right:
                    emptyList = GetEmptyInRow(index);
                    if (emptyList.Count == 0)
                        return false;

                    for (int x = emptyList.First(); x > 0; x--)
                    {
                        Grid[x,index] = Grid[x - 1, index];
                    }
                    Grid[0, index] = newColor;
                    return true;


                case Direction.Left:
                    emptyList = GetEmptyInRow(index);
                    if (emptyList.Count == 0)
                        return false;

                    for (int x = emptyList.Last(); x < GridSize -1; x++)
                    {
                        Grid[x, index] = Grid[x + 1, index];
                    }
                    Grid[GridSize -1 , index] = newColor;
                    return true;

            }
            return false;
        }

        public bool CheckForWin (PlayerColor color)
        {
            return SearchLine(WinCount, color) != null;
        }

        public List<Tuple<int, int>> SearchLine(int lineLength, PlayerColor color)
        {
            // Vertical
            for (int x = 0; x < GridSize; x++)
            {
                List<Tuple<int, int>> list = new List<Tuple<int, int>>();
                for (int y = 0; y < GridSize; y++)
                {
                    if (Grid[x, y] == color)
                    {
                        list.Add(new Tuple<int, int>(x, y));
                        if (list.Count >= lineLength)
                            return list;
                    }
                    else
                        list.Clear();
                }
            }

            //Horizontal
            for (int y = 0; y < GridSize; y++)
            {
                List<Tuple<int, int>> list = new List<Tuple<int, int>>();
                for (int x = 0; x < GridSize; x++)
                {
                    if (Grid[x, y] == color)
                    {
                        list.Add(new Tuple<int, int>(x, y));
                        if (list.Count >= lineLength)
                            return list;
                    }
                    else
                        list.Clear();
                }
            }

            // Diagonal
            for (int i = 0; i <= GridSize - lineLength; i++)
            {

                List<Tuple<int, int>> list1 = new List<Tuple<int, int>>();
                List<Tuple<int, int>> list2 = new List<Tuple<int, int>>();
                List<Tuple<int, int>> list3 = new List<Tuple<int, int>>();
                List<Tuple<int, int>> list4 = new List<Tuple<int, int>>();

                for (int a = 0; i + a < GridSize; a++)
                {
                    if (Grid[a, a + i] == color)
                    {
                        list1.Add(new Tuple<int, int>(a, a + i));
                        if (list1.Count >= lineLength)
                            return list1;
                    }
                    else
                        list1.Clear();

                    if (Grid[a + i, a] == color)
                    {
                        list2.Add(new Tuple<int, int>(a + i, a));
                        if (list2.Count >= lineLength)
                            return list2;
                    }
                    else
                        list2.Clear();

                    if (Grid[GridSize - 1 - a, a + i] == color)
                    {
                        list3.Add(new Tuple<int, int>(GridSize - 1 - a, a + i));
                        if (list3.Count >= lineLength)
                            return list3;
                    }
                    else
                        list3.Clear();

                    if (Grid[GridSize - 1 - (a + i), a] == color)
                    {
                        list4.Add(new Tuple<int, int>(GridSize - 1 - (a + i), a));
                        if (list4.Count >= lineLength)
                            return list4;
                    }
                    else
                        list4.Clear();
                }
                list1.Clear();
                list2.Clear();
                list3.Clear();
                list4.Clear();
            }

            return null;
        }

        /*public List<Tuple<int, int>> SearchOnOppositeSide (int lineLength, PlayerColor color)
        {
            // Vertical
            for (int x = 0; x < GridSize; x++)
            {
                List<Tuple<int, int>> list = new List<Tuple<int, int>>();
                for (int y = 0; y < GridSize; y++)
                {
                    if (Grid[x, y] == color)
                    {
                        list.Add(new Tuple<int, int>(x, y));
                        if (list.Count >= lineLength)
                            return list;
                    }
                    else
                        list.Clear();
                }
            }

            //Horizontal
            for (int y = 0; y < GridSize; y++)
            {
                List<Tuple<int, int>> list = new List<Tuple<int, int>>();
                for (int x = 0; x < GridSize; x++)
                {
                    if (Grid[x, y] == color)
                    {
                        list.Add(new Tuple<int, int>(x, y));
                        if (list.Count >= lineLength)
                            return list;
                    }
                    else
                        list.Clear();
                }
            }
        }
        */

        public List<int> GetEmptyInRow(int index)
        {
            List<int> EmptyList = new List<int>();
            for (int x=0; x<GridSize; x++)
            {
                if (Grid[x, index] == PlayerColor.Empty)
                    EmptyList.Add(x);
            }
            return EmptyList;
        }

        public List<int> GetEmptyInColumn(int index)
        {
            List<int> EmptyList = new List<int>();
            for (int y = 0; y < GridSize; y++)
            {
                if (Grid[index, y] == PlayerColor.Empty)
                    EmptyList.Add(y);
            }
            return EmptyList;
        }

        /*public int CheckSurrounding(int i, int j, PlayerColor color)
        {
            int counter = 0;
            int row_limit = Grid.GetLength(0);
            if (row_limit > 0)
            {
               int column_limit = Grid.GetLength(1);
                for (int x = Math.Max(0, i - 1); x <= Math.Min(i + 1, row_limit); x++)
                {
                    for (int y = Math.Max(0, j - 1); y <= Math.Min(j + 1, column_limit); y++)
                    {
                        if (x != i || y != j)
                        {
                            if (Grid[x, y] == color)
                                counter++;
                         }
                    }
                 }
            }
            return 0;
        }*/

        public bool CheckSurrounding (int x, int y, PlayerColor color)
        {
            if (x > 0 && y > 0 && Grid[x - 1, y - 1] == color)
                return true;
            if (x > 0 && Grid[x - 1, y] == color)
                return true;
            if (y < GridSize - 1 && x > 0 && Grid[x - 1, y + 1] == color)
                return true;
            if (y < GridSize - 1 && Grid[x, y + 1] == color)
                return true;
            if (x < GridSize - 1 && y < GridSize - 1 && Grid[x + 1, y + 1] == color)
                return true;
            if (x < GridSize - 1 && Grid[x + 1, y] == color)
                return true;
            if (x < GridSize - 1 && y > 0 && Grid[x + 1, y - 1] == color)
                return true;
            if (y > 0 && Grid[x, y - 1] == color)
                return true;
            return false;
        }

        public void Reset()
        {
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    Grid[x, y] = PlayerColor.Empty;
                }
            }
        }

    }
}
