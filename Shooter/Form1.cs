//#define My_Debug

using Shooter.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Shooter
{
    
    public partial class ZombieDeer : Form
    {

        const int FrameNum = 5;
        const int SplatNum = 3;

        bool splat = false;

        int _gameFrame = 0;
        int _splatTime = 0;

        int _hits = 0;
        int _misses = 0;
        int _totalShots = 0;
        double _averageHit = 0;

#if My_Debug
        int _cursX = 0;
        int _cursY = 0;
#endif
        CShooter _deer;
        CSplat _splat;
        CSign _sign;
        CScoreFrame _score;

        Random rnd = new Random();

        public ZombieDeer()
        {
            AmbienceSound();

            this.DoubleBuffered = true;             //      double buffer prevents from screen flicker
            this.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.ContainerControl |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.SupportsTransparentBackColor
                          , true);

            InitializeComponent();

            //timerGameLoop.Start(); 

            Bitmap b = new Bitmap(Resources.crosshair);
            this.Cursor = CustomCursor.CreateCursor(b, b.Height / 2, b.Width / 2);

            _score = new CScoreFrame() { Left = 20, Top = 20 };
            _sign = new CSign() { Left = 450, Top = 10 };
            _deer = new CShooter() { Left = 70, Top = 300 };
            _splat = new CSplat();        

        }

        private void timerGameLoop_Tick(object sender, EventArgs e)
        {
            if (_gameFrame >= FrameNum)
            {
                UpdateDeer();
                _gameFrame = 0;
            }

            if (splat)
            {
                if (_splatTime >= 3)
                {
                    splat = false;
                    _splatTime = 0;
                    UpdateDeer();
                }

                _splatTime++;
            }

            _gameFrame++;
            this.Refresh();
           
           
    }

        private void UpdateDeer()
        {
            _deer.Update(
                rnd.Next(Resources.deer.Width, this.Width - Resources.deer.Width),
                rnd.Next(this.Height / 2, this.Height - Resources.deer.Height * 2)
                );
           
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics dc = e.Graphics;

            _sign.DrawImage(dc);
            _score.DrawImage(dc);

            if (splat == true)
            {
                _splat.DrawImage(dc);
            }

            else
            {
                _deer.DrawImage(dc);
            }

            
            _sign.DrawImage(dc);
            _score.DrawImage(dc);

#if My_Debug
            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
            Font _font = new System.Drawing.Font("Stencil", 28, FontStyle.Regular);
            TextRenderer.DrawText(dc, "x=" + _cursX.ToString() + "|" + "y=" + _cursY.ToString(), _font,
                new Rectangle(0, 0, 300, 100), SystemColors.ControlText, flags);
#endif
            _deer.DrawImage(dc);

            //      wyniki na ekranie
            TextFormatFlags flags = TextFormatFlags.Left;
            Font _font = new System.Drawing.Font("Stencil", 24, FontStyle.Regular);
            TextRenderer.DrawText(e.Graphics, "Shots: " + _totalShots.ToString(), _font, new Rectangle(30, 30, 250, 60), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, "Hits: " + _hits.ToString(), _font, new Rectangle(30,70, 250, 60), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, "Misses: " + _misses.ToString(), _font, new Rectangle(30, 110, 250, 60), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, "Avg: " + _averageHit.ToString("F0") + "%", _font, new Rectangle(30, 150, 250, 60), SystemColors.ControlText, flags);
            TextRenderer.DrawText(e.Graphics, "/// © 2016 NACZ Games ///" , new System.Drawing.Font("Stencil", 14, FontStyle.Regular), new Rectangle(50, 210, 250, 60), SystemColors.ControlText, flags);

            
            base.OnPaint(e);
        }

        private void ZombieDeer_MouseMove(object sender, MouseEventArgs e)
        {
#if My_Debug
            _cursX = e.X;
            _cursY = e.Y;
#endif
            this.Refresh();
            
            

        }

        
        private async void ZombieDeer_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X > 375 && e.X < 840 && e.Y > 70 && e.Y <110) // START
            {
                timerGameLoop.Start();
                StartSound();
            }
            else if (e.X > 580 && e.X < 810 && e.Y > 120 && e.Y < 160) // STOP
            {
                timerGameLoop.Stop();
                StopSound();
            }
            else if (e.X > 595 && e.X < 855 && e.Y > 162 && e.Y < 195) // RESET
            {
                ResetSound();
                await Task.Delay(2300);
                Application.Restart();
            }
            else if (e.X > 592 && e.X < 830 && e.Y > 197 && e.Y < 232) // QUIT
            {
                GameOverSound();
                await Task.Delay(1600);
                Application.Exit();
            }
            else 
            {
                if (_deer.Hit(e.X, e.Y))
                {
                    splat = true;
                    _splat.Left = _deer.Left - Resources.blood.Width / 3;
                    _splat.Top = _deer.Top - Resources.blood.Height / 3;
                
                    _hits++;
                   
                }
                else _misses++;

                    
                    _totalShots = _hits + _misses;
                    _averageHit = (double)_hits / (double)_totalShots * 100.0;
            }

            FireGun();
            
        }


                //      dzwieki
        private void FireGun()
        {
            SoundPlayer simpleSound1 = new SoundPlayer(Resources.gun);

            simpleSound1.Play();
        }

        private void StopSound()
        {
            
            var player2 = new WMPLib.WindowsMediaPlayer();
            player2.URL = @"C:\Users\Piotr\Documents\Visual Studio 2015\Projects\Shooter\Shooter\Resources\pause.wav";
        }

        private void StartSound()
        {
            var player3 = new WMPLib.WindowsMediaPlayer();
            player3.URL = @"C:\Users\Piotr\Desktop\moje gry\GOT.midi";//@"C:\Users\Piotr\Documents\Visual Studio 2015\Projects\Shooter\Shooter\Resources\start.wav";
        }

        private void ResetSound()
        {
            var player4 = new WMPLib.WindowsMediaPlayer();
            player4.URL = @"C:\Users\Piotr\Documents\Visual Studio 2015\Projects\Shooter\Shooter\Resources\reset.wav";
        }

        private void GameOverSound()
        {
            var player5 = new WMPLib.WindowsMediaPlayer();
            player5.URL = @"C:\Users\Piotr\Desktop\moje gry\GOT.midi"; //"C:\Users\Piotr\Documents\Visual Studio 2015\Projects\Shooter\Shooter\Resources\gameOver.wav";
        }

        private void AmbienceSound()
        {
           
            var player1 = new WMPLib.WindowsMediaPlayer();
            player1.URL = @"C:\Users\Piotr\Documents\Visual Studio 2015\Projects\Shooter\Shooter\Resources\ambience.wav";
        }

        private void ZombieDeer_Load(object sender, EventArgs e)
        {

        }
    }
}
