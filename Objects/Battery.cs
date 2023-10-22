using Microsoft.Xna.Framework;
using ProjectArrow.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectArrow.Objects
{
    public class Battery : GameObject
    {
        public Vector2 linePosition;

        public Battery(/*InputHelper inputHelper,*/ Vector2 Position)
        {
            texture = GameData.ObjectMap["Battery"];
            position = new Vector2(Position.X, Position.Y); //new Vector2((inputHelper.WorldMousePosition.X - texture.Width / 2), inputHelper.WorldMousePosition.Y - 12);
            origin = new Vector2((int)(position.X + GameData.ObjectTileSize / 2), (int)(position.Y + 12));
            depth = Helper.GetDepth(origin);
            collider = new Rectangle((int)position.X, (int)position.Y + 8, texture.Width, texture.Width / 2);
        }
    }
}
