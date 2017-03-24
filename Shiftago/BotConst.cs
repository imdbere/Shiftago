using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftago
{
    public static class BotConst
    {
        public static int[,] GridValues = new int[7, 7]
        {
            /*{ 9,10,11,10,11,10, 9},
            {10,10,11,14,11,10,10},
            {11,11,15,12,15,11,11},
            {10,14,12,11,12,14,10},
            {11,11,15,12,15,11,11},
            {10,10,11,14,11,10,10},
            { 9,10,11,10,11,10, 9}*/

            { 8,9,11,10,11,9,8},
            {9,10,11,14,11,10,9},
            {11,11,15,12,15,11,11},
            {10,14,12,11,12,14,10},
            {11,11,15,12,15,11,11},
            {9,10,11,14,11,10,9},
            { 8,9,11,10,11,10, 8}


            //{ 5, 8,14,10,14, 8, 5},
            //{ 8,10,13,25,13,10, 8},
            //{14,14,24,24,24,14,14},
            //{10,25,24,20,24,25,10},
            //{14,14,24,24,24,14,14},
            //{ 8,10,13,25,13,10, 8},
            //{ 5, 8,14,10,14, 8, 5},
        };
        public static Random rand = new Random();

        public static int DirectNeighbourInfluence = 1;
        public static int DirectNeighbourInfluenceForEnemy = 2;
        public static double NextPlayerRatio = 4 / 5.0;
        public static double OtherPlayersRatio = 4 / 5.0;
        public static double MoveRandomFactor = 2;

    }
}
