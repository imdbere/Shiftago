using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Threading;

namespace Shiftago
{
    public partial class GameScreen : Form
    {
        Board GameBoard;

        Graphics GRaster;
        Graphics GFields;
        Graphics GBuffer;
        Graphics GArrow;
        Bitmap RasterImage;
        Bitmap ArrowImage;
        Bitmap FieldImage;
        Bitmap DoubleBufferImage;

        Bitmap ArrowUP;
        Bitmap ArrowDOWN;
        Bitmap ArrowLEFT;
        Bitmap ArrowRIGHT;
        Bitmap BG;

        Bitmap BGreen;
        Bitmap BOrange;
        Bitmap BWhite;
        Bitmap BBlue;

        Bot[] Bots;
        Stack<PlayerColor[,]> MovesList;
        
        Color BackgroundColor = SystemColors.Control;
        Stopwatch sw = new Stopwatch();

        const int ArrowSize = 40;
        const int ArrowSpacing = 5;

        System.Windows.Forms.Timer nextMoveTimer = new System.Windows.Forms.Timer();
        const int BorderOffset = 95;
        int[,] WinAnalyse = new int[7, 7];

        int TileSize;
        int GameCount = 0;

        bool GameRunning = false;
        bool AutoGameMode = false;

        public GameScreen(int gridSize, int playerCount, int winCount, int botCount)
        {
            GameBoard = new Board(gridSize, winCount);
            Bots = new Bot[botCount];
            for (int i= 0; i < botCount; i++)
            {
                Bots[i] = new Bot(GameBoard, (PlayerColor)i);
            }
            MovesList = new Stack<PlayerColor[,]>();
            Players.PlayerCount = playerCount;
            InitializeComponent();

            RasterImage = new Bitmap(pictureBoxMain.Width, pictureBoxMain.Width);
            ArrowImage = new Bitmap(pictureBoxMain.Width, pictureBoxMain.Width);
            FieldImage = new Bitmap(pictureBoxMain.Width, pictureBoxMain.Width);
            DoubleBufferImage = new Bitmap(pictureBoxMain.Width, pictureBoxMain.Width);

            ArrowUP = new Bitmap(new Bitmap("Arrow.png"));
            ArrowDOWN = new Bitmap(ArrowUP);
            ArrowLEFT = new Bitmap(ArrowUP);
            ArrowRIGHT = new Bitmap(ArrowUP);
            ArrowRIGHT.RotateFlip(RotateFlipType.Rotate180FlipNone);
            ArrowDOWN.RotateFlip(RotateFlipType.Rotate90FlipNone);
            ArrowUP.RotateFlip(RotateFlipType.Rotate270FlipNone);
            BG = new Bitmap("Shiftago.jpg");

            GRaster = Graphics.FromImage(RasterImage);
            GArrow = Graphics.FromImage(ArrowImage);
            GFields = Graphics.FromImage(FieldImage);
            GFields.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            GBuffer = Graphics.FromImage(DoubleBufferImage);

            BBlue = new Bitmap("blau.png");
            BWhite = new Bitmap("weiss.png");
            BOrange = new Bitmap("orange.png");
            BGreen = new Bitmap("gruen.png");

            Players.SetRandomPlayer();
            nextMoveTimer.Interval = 1000;
            
            nextMoveTimer.Tick += delegate
            {
                nextMoveTimer.Stop();
                NextPlayer();
            };

            TileSize = (pictureBoxMain.Width - BorderOffset * 2) / gridSize;
            DrawRaster();
            DrawArrows();

            labelPlayer.Text = Players.GetColorDesc();
            labelPlayer.ForeColor = Players.GetColor();

            labelNextPlayer.Text = Players.GetColorDesc(Players.GetNextPlayer());
            labelNextPlayer.ForeColor = Players.GetRealColor(Players.GetNextPlayer());

            UpdateGraphics();
        }

        private void pictureBoxMain_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(DoubleBufferImage, Point.Empty);
        }

        void DrawRaster()
        {
            
            for (int x = 0; x < GameBoard.GridSize; x++)
            {
                for (int y = 0; y < GameBoard.GridSize; y++)
                {
                    GRaster.DrawRectangle(Pens.Black, new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize));

                }
            }
        }
        void DrawArrows ()
        {
            for (int x = 0; x < GameBoard.GridSize; x++)
            {
                GArrow.DrawImage(ArrowDOWN, x*TileSize + BorderOffset + (TileSize - ArrowSize)/2, BorderOffset - ArrowSize - ArrowSpacing, ArrowSize, ArrowSize);
            }
            for (int y = 0; y < GameBoard.GridSize; y++)
            {
                GArrow.DrawImage(ArrowLEFT, BorderOffset - ArrowSize - ArrowSpacing, y * TileSize + BorderOffset + (TileSize - ArrowSize) / 2, ArrowSize, ArrowSize);
            }
            for (int x = 0; x < GameBoard.GridSize; x++)
            {
                GArrow.DrawImage(ArrowUP, x * TileSize + BorderOffset + (TileSize - ArrowSize) / 2, pictureBoxMain.Width - BorderOffset + ArrowSpacing, ArrowSize, ArrowSize);
            }
            for (int y = 0; y < GameBoard.GridSize; y++)
            {
                GArrow.DrawImage(ArrowRIGHT, pictureBoxMain.Height - BorderOffset + ArrowSpacing, y * TileSize + BorderOffset + (TileSize - ArrowSize) / 2, ArrowSize, ArrowSize);
            }
        }

        void DrawFields()
        {
            GFields.Clear(Color.Transparent);
            for (int x = 0; x < GameBoard.GridSize; x++)
            {
                for (int y = 0; y < GameBoard.GridSize; y++)
                {
                    if (GameBoard.Grid[x, y] != PlayerColor.Empty)
                    {
                        Bitmap toDraw = BBlue;
                        if (GameBoard.Grid[x, y] == PlayerColor.Blue)
                            toDraw = BBlue;
                        if (GameBoard.Grid[x, y] == PlayerColor.White)
                            toDraw = BWhite;
                        if (GameBoard.Grid[x, y] == PlayerColor.Green)
                            toDraw = BGreen;
                        if (GameBoard.Grid[x, y] == PlayerColor.Orange)
                            toDraw = BOrange;

                        //GFields.FillEllipse(BBlue, x * TileSize + (TileSize - 52)/2, y * TileSize + (TileSize - 52) / 2, 52, 52);
                        GFields.DrawImage(toDraw, x * TileSize + (TileSize - 52) / 2, (GameBoard.GridSize - 1 - y) * TileSize + (TileSize - 52) / 2, 52, 52);
                    }
                }
            }
        }

        void UpdateGraphics()
        {
            GBuffer.Clear(BackgroundColor);
            if (GameBoard.GridSize == 7)
                GBuffer.DrawImage(BG, BorderOffset - 26, BorderOffset - 23, TileSize * GameBoard.GridSize + 54, TileSize * GameBoard.GridSize + 48);
            else
                GBuffer.DrawImage(RasterImage, new Point(BorderOffset, BorderOffset));

            DrawFields();

            GBuffer.DrawImage(FieldImage, new Point(BorderOffset, BorderOffset));
            GBuffer.DrawImage(ArrowImage, Point.Empty);
            pictureBoxMain.Invalidate();
        }

        void NextPlayer()
        {
            if (!GameRunning)
                return;

            MovesList.Push(GameBoard.GridCopy());

            if (GameBoard.CheckForWin(Players.CurrentPlayer))
            {
                PlayerWon(Players.CurrentPlayer);
                return;
            }
                

            Players.NextPlayer();

            labelPlayer.Text = Players.GetColorDesc();
            labelPlayer.ForeColor = Players.GetColor();

            labelNextPlayer.Text = Players.GetColorDesc(Players.GetNextPlayer());
            labelNextPlayer.ForeColor = Players.GetRealColor(Players.GetNextPlayer());

            if (GameBoard.CheckForWin(Players.CurrentPlayer))
            {
                PlayerWon(Players.CurrentPlayer);
                return;
            }

            foreach (Bot bot in Bots)
            {
                if (Players.CurrentPlayer == bot.OwnColor)
                {
                    sw.Restart();
                    if (bot.NextMove() == null)
                        PlayerPatt(bot.OwnColor);
                        
                    labelCalcTime.Text = sw.ElapsedMilliseconds + "ms";
                    UpdateGraphics();
                    nextMoveTimer.Start();
                }
            }
        }

        private void GameScreen_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        void EndGame()
        {
            UpdateGraphics();
            GameCount++;
            labelGame.Text = GameCount.ToString();
            if (AutoGameMode)
            {
                ResetGame();
                GameRunning = true;
                NextPlayer();
            }
            else
                GameRunning = false;
        }


        void ResetGame()
        {
            GameBoard.Reset();
            MovesList.Clear();
            for (int i = 0; i < Bots.Count(); i++)
            {
                Bots[i] = new Bot(GameBoard, (PlayerColor)i);
            }
            UpdateGraphics();
        }

        void PlayerPatt(PlayerColor color)
        {
            if (!AutoGameMode)
                MessageBox.Show("Player " + Enum.GetName(typeof(PlayerColor), color) + " made patt!");
            EndGame();
        }

        void PlayerWon(PlayerColor color)
        {
            UpdateGraphics();
            List<Tuple<int, int>> winSituation = GameBoard.SearchLine(GameBoard.WinCount, color);
            foreach (Tuple<int, int> tuple in winSituation)
            {
                Debug.WriteLine("X: " + tuple.Item1 + " Y: " + tuple.Item2);
                WinAnalyse[tuple.Item1, tuple.Item2]++;
            }
            String s = "";
            for (int x = 0; x < GameBoard.GridSize; x++)
            {
                for (int y = 0; y < GameBoard.GridSize; y++)
                {
                    s += WinAnalyse[x, y].ToString("000") + " ";
                }
                s += Environment.NewLine;
            }
            textBoxDebug.Text = s;
            if (!AutoGameMode)
                MessageBox.Show("Player " + Enum.GetName(typeof(PlayerColor), color) + " won!");
            EndGame();
        }
        private void pictureBoxMain_MouseClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine(e.Location);
            if (nextMoveTimer.Enabled)
                return;

            if (!GameRunning) GameRunning = true;
            if (e.X < BorderOffset && e.Y > BorderOffset && e.Y < pictureBoxMain.Width - BorderOffset)
            {
                int index = (e.Y - BorderOffset) / TileSize;
                if (GameBoard.Push(Direction.Right, GameBoard.GridSize - 1 - index, Players.CurrentPlayer))
                    nextMoveTimer.Start();
            }
            if (e.X > pictureBoxMain.Width - BorderOffset && e.Y > BorderOffset && e.Y < pictureBoxMain.Width - BorderOffset)
            {
                int index = (e.Y - BorderOffset) / TileSize;
                if (GameBoard.Push(Direction.Left, GameBoard.GridSize - 1 - index, Players.CurrentPlayer))
                    nextMoveTimer.Start();
            }
            if (e.Y < BorderOffset && e.X > BorderOffset && e.X < pictureBoxMain.Width - BorderOffset)
            {
                int index = (e.X - BorderOffset) / TileSize;
                if (GameBoard.Push(Direction.Down, index, Players.CurrentPlayer))
                    nextMoveTimer.Start();
            }
            if (e.Y > pictureBoxMain.Width - BorderOffset && e.X > BorderOffset && e.X < pictureBoxMain.Width - BorderOffset)
            {
                int index = (e.X - BorderOffset) / TileSize;
                if (GameBoard.Push(Direction.Up, index, Players.CurrentPlayer))
                    NextPlayer();
            }
            UpdateGraphics();

        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            GameRunning = true;
            NextPlayer();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            nextMoveTimer.Interval = trackBar1.Value;
            labelSpeed.Text = trackBar1.Value + "ms";
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            //if (!GameRunning)
            {
                if (MovesList.Count > 0)
                {
                    GameBoard.Grid = MovesList.Pop();
                    Players.PreviousPlayer();
                    labelPlayer.Text = Players.GetColorDesc();
                    labelPlayer.ForeColor = Players.GetColor();

                    labelNextPlayer.Text = Players.GetColorDesc(Players.GetNextPlayer());
                    labelNextPlayer.ForeColor = Players.GetRealColor(Players.GetNextPlayer());
                    UpdateGraphics();
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Enabled && Players.PlayerCount == Bots.Count())
            {
                AutoGameMode = true;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DoubleBufferImage.Save(saveFileDialog1.FileName);
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            GameRunning = false;
        }
    }
}
