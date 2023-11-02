using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectArrow.World
{
    public class Tile
    {
        public Rectangle SourceRect { get; set; }
        public Rectangle DestRect { get; set; }
        public Vector2 Position { get; set; }

    }
}
