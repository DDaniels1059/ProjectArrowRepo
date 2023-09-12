using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectDelta.Helpers
{
    public class SpriteManager
    {
        public Texture2D Texture;
        public Vector2 Position = Vector2.Zero;
        public Color Color = Color.White;
        public Vector2 Origin;
        public float Rotation = 0f;
        public float Scale = 1f;
        public SpriteEffects SpriteEffect;
        protected Rectangle[] Rectangles;
        protected int FrameIndex = 0;
        public string SpriteName { get; set; }


        public SpriteManager(Texture2D Texture, int frames)
        {
            this.Texture = Texture;
            int width = Texture.Width / frames;
            Rectangles = new Rectangle[frames];

            for (int i = 0; i < frames; i++)
                Rectangles[i] = new Rectangle(i * width, 0, width, Texture.Height);
        }
    
        public void Draw(SpriteBatch spriteBatch, float depth)
        {
            spriteBatch.Draw(Texture, Position, Rectangles[FrameIndex], Color, Rotation, Origin, Scale, SpriteEffect, depth);
        }

        public void Draw(SpriteBatch spriteBatch, float depth, Color Color)
        {
            spriteBatch.Draw(Texture, Position, Rectangles[FrameIndex], Color, Rotation, Origin, Scale, SpriteEffect, depth);
        }

        public void Draw(SpriteBatch spriteBatch, float depth, float Rotation)
        {
            spriteBatch.Draw(Texture, Position, Rectangles[FrameIndex], Color, Rotation, Origin, Scale, SpriteEffect, depth);
        }

        //Specific For Using the Texture Atlas
        public void DrawFromAtlas(SpriteBatch spriteBatch, float depth)
        {
            if (GameData.TextureMap.TryGetValue(SpriteName, out var sourceRect))
            {
                spriteBatch.Draw(Texture, Position, sourceRect, Color, Rotation, Origin, Scale, SpriteEffect, depth);
            }
        }
    }

    public class SpriteAnimation : SpriteManager
    {
        private float timeElapsed;
        public bool IsLooping = true;
        private float timeToUpdate;
        public int FramesPerSecond { set { timeToUpdate = 1f / value; } }

        public SpriteAnimation(Texture2D Texture, int frames, int fps, string spriteName) : base(Texture, frames)
        {
            FramesPerSecond = fps;
            SpriteName = spriteName; // Store the sprite's name

        }

        public void Update(GameTime gameTime)
        {
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeElapsed > timeToUpdate)
            {
                timeElapsed -= timeToUpdate;

                if (FrameIndex < Rectangles.Length - 1)
                    FrameIndex++;

                else if (IsLooping)
                    FrameIndex = 0;
            }
        }

        public void setFrame(int frame)
        {
            FrameIndex = frame;
        }
    }
}