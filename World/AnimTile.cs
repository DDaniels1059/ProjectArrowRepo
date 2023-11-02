using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectArrow.Helpers;
using ProjectArrow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectArrow.World
{
    public class AnimTile
    {
        public SpriteAnimation Anim;
        public SpriteEffects Effect;
        public Vector2 Position { get; set; }

        public void Update(GameTime gameTime)
        {
            Anim.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Anim.Draw(spriteBatch, Position, 0.001f, Effect);
        }
    }
}
