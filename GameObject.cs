using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectDelta.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDelta
{
    public class GameObject
    {
        public Rectangle texture;
        public Rectangle collider;
        public Vector2 position;
        public Vector2 origin;
        public float depth;

        public virtual void Update(GameTime gameTime)
        {
            //Vector2 origin = new Vector2(position.X + (GameData.TileSize / 2), position.Y + (GameData.TileSize - 2));
            //depth = Helper.GetDepth(origin);
        }
    }
}
