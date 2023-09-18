using Microsoft.Xna.Framework;
using ProjectDelta.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDelta.Objects
{
    public class Battery : GameObject
    {
        public Vector2 linePosition;

        public Battery(InputHelper inputHelper)
        {
            texture = GameData.TextureMap["Battery"];
            position = new Vector2((int)(inputHelper.WorldMousePosition.X - texture.Width / 2), (int)(inputHelper.WorldMousePosition.Y - 12));
            linePosition = new Vector2(position.X + (texture.Width / 2), position.Y);
            origin = new Vector2((int)(position.X + GameData.TileSize / 2), (int)(position.Y + 12));
            depth = Helper.GetDepth(origin);
            collider = new Rectangle((int)position.X + 1, (int)position.Y + 8, texture.Width - 2, texture.Width / 2);
        }
    }
}
