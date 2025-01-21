using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeyAuth;
using ROYAL_REGEDIT___;
using static KeyAuth.api;

namespace ROYAL_REGEDIT___
{
    public partial class LoginFrom : Form
    {
        private const int ParticleCount = 80;
        private const int DrawCount = 80;
        private readonly Random _random = new Random();
        private readonly PointF[] _particlePositions = new PointF[ParticleCount];
        private readonly PointF[] _particleTargetPositions = new PointF[ParticleCount];
        private readonly float[] _particleSpeeds = new float[ParticleCount];
        private readonly float[] _particleSizes = new float[ParticleCount];
        private readonly float[] _particleRadii = new float[ParticleCount];
        private readonly float[] _particleRotations = new float[ParticleCount];
        private readonly PointF[] _vertices = new PointF[3];


        public LoginFrom()
        {
            InitializeComponent(); KeyAuthApp.init();

            DoubleBuffered = true;

            InitializeParticles();

            Timer timer = new Timer
            {
                Interval = 3
            };
            timer.Tick += (sender, args) =>
            {
                UpdateParticles();
                Invalidate();
            };
            timer.Start();


        }


        public static api KeyAuthApp = new api

             (name: "",
             ownerid: "",
             version: "");






        private void LoginFrom_Load(object sender, EventArgs e)
        {
            KeyAuthApp.init();
        }

    

        private void guna2Button1_Click_2(object sender, EventArgs e)
        {
            KeyAuthApp.login(user.Text, pass.Text);
            if (KeyAuthApp.response.success)
            {

                this.Hide();
                MainFrom HM = new MainFrom();
                HM.Show();

            }
            else
            {



            }
        }
        private void InitializeParticles()
        {
            Size screenSize = Screen.PrimaryScreen.Bounds.Size;
            for (int i = 0; i < ParticleCount; i++)
            {
                _particlePositions[i] = new PointF(0, 0);
                _particleTargetPositions[i] = new PointF(_random.Next(screenSize.Width), screenSize.Height * 2);
                _particleSpeeds[i] = 1 + _random.Next(25);
                _particleSizes[i] = _random.Next(8);
                _particleRadii[i] = _random.Next(4);
                _particleRotations[i] = 0;
            }
        }

        private void UpdateParticles()
        {
            Size screenSize = Screen.PrimaryScreen.Bounds.Size;
            for (int i = 0; i < ParticleCount; i++)
            {
                if (_particlePositions[i].X == 0 || _particlePositions[i].Y == 0)
                {
                    _particlePositions[i] = new PointF(_random.Next(screenSize.Width + 1), 15f);
                    _particleSpeeds[i] = 1 + _random.Next(25);
                    _particleRadii[i] = _random.Next(4);
                    _particleSizes[i] = _random.Next(8);
                    _particleTargetPositions[i] = new PointF(_random.Next(screenSize.Width), screenSize.Height * 2);
                }

                float deltaTime = 2.5f / 60;
                _particlePositions[i] = Lerp(_particlePositions[i], _particleTargetPositions[i], deltaTime * (_particleSpeeds[i] / 60));
                _particleRotations[i] += deltaTime;

                if (_particlePositions[i].Y > screenSize.Height)
                {
                    _particlePositions[i] = new PointF(0, 0);
                    _particleRotations[i] = 0;
                }
            }
        }

        private PointF Lerp(PointF start, PointF end, float t)
        {
            return new PointF(start.X + (end.X - start.X) * t, start.Y + (end.Y - start.Y) * t);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            for (int i = 0; i < DrawCount; i++)
            {
                DrawTriangleWithGlow(e.Graphics, _particlePositions[i], _particleSizes[i], _particleRotations[i]);
            }
        }

        private void DrawTriangleWithGlow(Graphics graphics, PointF position, float size, float rotation)
        {
            float angle = (float)(Math.PI * 2 / 3);
            PointF[] vertices = new PointF[3];

            for (int i = 0; i < 3; i++)
            {
                vertices[i] = new PointF(
                    position.X + size * (float)Math.Cos(rotation + i * angle),
                    position.Y + size * (float)Math.Sin(rotation + i * angle)
                );
            }

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            int maxGlowLayers = 10;
            for (int j = 0; j < maxGlowLayers; j++)
            {
                int alpha = 25 - 2 * j;
                using (Brush glowBrush = new SolidBrush(Color.FromArgb(alpha, 50, 205, 50)))
                {
                    float glowSize = size + j * 4;
                    graphics.FillEllipse(glowBrush, position.X - glowSize / 2, position.Y - glowSize / 2, glowSize, glowSize);
                }
            }


            using (Brush brush = new SolidBrush(Color.FromArgb(50, 205, 50)))
            {
                graphics.FillPolygon(brush, vertices);
            }
        }





        public class Particle
        {
            public PointF Position { get; set; }
            public PointF Velocity { get; set; }
            public int Radius { get; set; }
            public Color Color { get; set; }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateParticles();
            Invalidate();
        }
    }
}
