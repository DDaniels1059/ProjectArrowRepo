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
            texture = GameData.ObjectMap["Box"];
            position = Position; //new Vector2((inputHelper.WorldMousePosition.X - texture.Width / 2), inputHelper.WorldMousePosition.Y - 12);
            origin = new Vector2((position.X + GameData.ObjectTileSize / 2), (position.Y + 24));
            depth = Helper.GetDepth(origin);
            collider = new Rectangle((int)position.X, (int)position.Y + 8, texture.Width, texture.Width / 2);
        }
    }
}
