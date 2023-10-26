using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectArrow.Helpers;
using System;
using System.Diagnostics;


namespace ProjectArrow.Utility
{
    public class FpsMonitor
    {
        public float Value { get; private set; }
        public TimeSpan Sample { get; set; }
        private Stopwatch sw;
        private int Frames;
        public FpsMonitor()
        {
            this.Sample = TimeSpan.FromSeconds(1);
            this.Value = 0;
            this.Frames = 0;
            this.sw = Stopwatch.StartNew();
        }
        public void Update()
        {
            if (sw.Elapsed > Sample)
            {
                this.Value = (float)(Frames / sw.Elapsed.TotalSeconds);
                this.sw.Reset();
                this.sw.Start();
                this.Frames = 0;
            }
        }
        public void Draw(SpriteBatch SpriteBatch, SpriteFont Font, Vector2 Location, Color Color, float Rotation, Vector2 Origin, float Scale, SpriteEffects SpriteEffect, float Depth)
        {
            this.Frames++;
            SpriteBatch.DrawString(Font, "FPS: " + this.Value.ToString(), Location, Color, Rotation, Origin, Scale, SpriteEffect, Depth);
        }
    }
}
