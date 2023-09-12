using Microsoft.Xna.Framework;
using ProjectDelta.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDelta
{
    public class Tower : GameObject
    {
        public Vector2 linePosition;

        public Tower(InputHelper inputHelper)
        {
            texture = GameData.TextureMap["Tower"];
            linePosition = new Vector2(texture.Width / 2, 16);
            position = new Vector2((int)(inputHelper.WorldMousePosition.X - (texture.Width / 2)), (int)(inputHelper.WorldMousePosition.Y - 48));
            origin = new Vector2((int)(position.X + (GameData.TileSize / 2)), (int)(position.Y + (43)));
            depth = Helper.GetDepth(origin);
            collider = new Rectangle((int)position.X, (int)position.Y + 40, texture.Width, (int)(texture.Width / 1));
        }

    }
}
