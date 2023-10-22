using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectArrow.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectArrow.Objects
{
    public static class ObjectManager
    {

        private static SpriteBatch spriteBatch;
        public static List<GameObject> GameObjects;


        public static void Initialize(SpriteBatch SpriteBatch)
        {
            GameObjects = new();
            spriteBatch = SpriteBatch;
        }

        public static void UpdateObjects()
        {
            for(int i = 0; i < GameObjects.Count; i++)
            {
                GameObject obj = GameObjects[i];
                if (obj != null)
                {
                    obj.Update();
                }
            }
        }

        public static void DrawObjects()
        {
            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObject obj = GameObjects[i];
                obj.Draw(spriteBatch);
            }
        }
    }
}
