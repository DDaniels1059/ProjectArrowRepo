using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ProjectArrow.Helpers
{
    public class SpriteManager
    {
        public Texture2D TextureAtlas;
        public Rectangle SourceRectangle;
        public Vector2 Position = Vector2.Zero;
        public Color Color = Color.White;
        public Vector2 Origin;
        public float Rotation = 0f;
        public float Scale = 1f;
        public SpriteEffects SpriteEffect;
        protected Rectangle[] Rectangles;
        protected int FrameIndex = 0;
        public string SpriteName { get; }


        public SpriteManager(Texture2D TextureAtlas, Dictionary<string, Rectangle> TextureMap, string SpriteName, int frames)
        {
            this.TextureAtlas = TextureAtlas;
            this.SpriteName = SpriteName;

            if (TextureMap.TryGetValue(SpriteName, out var sourceRect))
            {
                this.SourceRectangle = sourceRect;
            }

            int width = SourceRectangle.Width / frames;
            Rectangles = new Rectangle[frames];

            for (int i = 0; i < frames; i++)
            {
                Rectangles[i] = new Rectangle(i * width, SourceRectangle.Top, width, SourceRectangle.Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch, float depth)
        {
            if (SpriteName != null && Rectangles.Length > 0)
            {
                spriteBatch.Draw(TextureAtlas, Position, Rectangles[FrameIndex], Color, Rotation, Origin, Scale, SpriteEffect, depth);
            }
        }

        public void Draw(SpriteBatch spriteBatch, float depth, Color color)
        {
            if (SpriteName != null && Rectangles.Length > 0)
            {
                spriteBatch.Draw(TextureAtlas, Position, Rectangles[FrameIndex], color, Rotation, Origin, Scale, SpriteEffect, depth);
            }
        }

        public void Draw(SpriteBatch spriteBatch, float depth, float rotation)
        {
            if (SpriteName != null && Rectangles.Length > 0)
            {
                spriteBatch.Draw(TextureAtlas, Position, Rectangles[FrameIndex], Color, rotation, Origin, Scale, SpriteEffect, depth);
            }
        }
    }

    public class SpriteAnimation : SpriteManager
    {
        private float _timeElapsed;
        private float _timeToUpdate;
        public bool IsLooping = true;

        public int FramesPerSecond { set { _timeToUpdate = 1f / value; } }

        public SpriteAnimation(Texture2D TextureAtlas, Dictionary<string, Rectangle> TextureMap, string SpriteName, int frames, int fps) : base(TextureAtlas, TextureMap, SpriteName, frames)
        {
            FramesPerSecond = fps;
        }

        public void Update(GameTime gameTime)
        {
            _timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeElapsed > _timeToUpdate)
            {
                _timeElapsed -= _timeToUpdate;

                if (FrameIndex < Rectangles.Length - 1)
                    FrameIndex++;

                else if (IsLooping)
                    FrameIndex = 0;
            }
        }

        public void SetFrame(int frame)
        {
            FrameIndex = frame;
        }
    }
}