using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftago
{
    public static class Players
    {
        public static PlayerColor CurrentPlayer;
        public static int PlayerCount;
        public static Random Rand = new Random();
        public static void NextPlayer()
        {
            CurrentPlayer++;
            if ((int)CurrentPlayer == PlayerCount)
                CurrentPlayer = 0;
        }

        public static void PreviousPlayer()
        {
            CurrentPlayer--;
            if ((int)CurrentPlayer < 0)
                CurrentPlayer = (PlayerColor)(PlayerCount - 1);
        }
        public static PlayerColor GetNextPlayer()
        {
            PlayerColor nextPlayer = CurrentPlayer + 1;
            if ((int)nextPlayer == PlayerCount)
                nextPlayer = 0;

            return nextPlayer;
        }
        public static PlayerColor GetNextPlayer(PlayerColor player)
        {
            PlayerColor nextPlayer = player + 1;
            if ((int)nextPlayer == PlayerCount)
                nextPlayer = 0;

            return nextPlayer;
        }
        public static void SetRandomPlayer()
        {
            CurrentPlayer = (PlayerColor)Rand.Next(0, PlayerCount);
        }
        public static Color GetRealColor(PlayerColor color)
        {
            switch (color)
            {
                case PlayerColor.White:
                    return Color.White;
                case PlayerColor.Green:
                    return Color.Green;
                case PlayerColor.Blue:
                    return Color.Blue;
                case PlayerColor.Orange:
                    return Color.Orange;
                default:
                    return Color.Transparent;
            }
        }
        public static Color GetColor()
        {
            return GetRealColor(CurrentPlayer);
        }

        public static String GetColorDesc(PlayerColor color)
        {
            return Enum.GetName(typeof(PlayerColor), color);
        }
        public static String GetColorDesc()
        {
            return Enum.GetName(typeof(PlayerColor), CurrentPlayer);
        }
    }
}
