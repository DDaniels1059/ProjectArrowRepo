using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectArrow.Helpers
{
    public static class Helper
    {
        public static float GetDepth(Vector2 origin)
        {
            float depth;
            depth = origin.Y / 1280;
            depth *= 0.01f;
            return depth;
        }
    }
}
