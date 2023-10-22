using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectArrow.Helpers;
using ProjectArrow.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectArrow.Objects
{
    public class GameObject
    {
        public Rectangle texture;
        public Rectangle collider;
        public Vector2 position;
        public Vector2 origin;
        public float depth;

        public GameObject()
        {
            ObjectManager.GameObjects.Add(this);
        }

        public virtual void Update()
        {
            //Vector2 origin = new Vector2(position.X + (GameData.TileSize / 2), position.Y + (GameData.TileSize - 2));
            //depth = Helper.GetDepth(origin);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GameData.ObjectAtlas, new Vector2(position.X, position.Y), texture, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);

            if (GameData.IsDebug)
            {
                spriteBatch.Draw(GameData.Pixel, origin, null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                spriteBatch.DrawHollowRect(collider, Color.Red);
            }
        }
    }
}
