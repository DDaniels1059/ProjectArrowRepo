using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectArrow.Objects;
using ProjectArrow.UI;
using System;
using System.Collections.Generic;

namespace ProjectArrow.Helpers
{
    public static class GameData
    {
        public static Dictionary<string, Rectangle> TextureMap { get; private set; }
        public static Dictionary<string, Rectangle> PlayerMap { get; private set; }

        public static SpriteFont GameFont { get; private set; }
        public static Texture2D TextureAtlas { get; private set; }
        public static Texture2D PlayerAtlas { get; private set; }

        public static Texture2D View { get; private set; }
        public static Texture2D Pixel { get; private set; }

        public static int TileSize { get; private set; }
        public static float UIScale { get; set; }
        public static bool IsDebug { get; set; }
        public static bool IsPaused { get; set; }

        public static List<Button> ButtonList { get; set; }
        public static List<Objects.GameObject> GameObjects { get; set; }
        public static List<PowerLine> PowerLines { get; set; }

        public static void LoadData(ContentManager content, GraphicsDevice graphicsDevice)
        {
            TileSize = 16;
            UIScale = 1f;
            IsDebug = false;
            IsPaused = false;

            View = content.Load<Texture2D>("Misc/View");

            TextureAtlas = content.Load<Texture2D>("Misc/TextureAtlas");
            TextureMap = new Dictionary<string, Rectangle>
            {                            //X  Y  Width     Height
                ["DebugButton"] = new Rectangle(0, 0, TileSize, TileSize),
                ["DebugButtonPressed"] = new Rectangle(16, 0, TileSize, TileSize),
                ["LeftArrow"] = new Rectangle(32, 0, TileSize, TileSize),
                ["LeftArrowPressed"] = new Rectangle(48, 0, TileSize, TileSize),
                ["Tower"] = new Rectangle(0, 32, TileSize, TileSize * 3),
                ["PinkDebug"] = new Rectangle(0, 16, TileSize, TileSize),
                ["Battery"] = new Rectangle(16, 16, TileSize, TileSize),
                ["Wrench"] = new Rectangle(32, 16, TileSize, TileSize),
                ["Gear"] = new Rectangle(48, 16, TileSize, TileSize)
            };

            PlayerAtlas = content.Load<Texture2D>("Misc/PlayerAtlas");
            PlayerMap = new Dictionary<string, Rectangle>
            {                              //X  Y  Width     Height
                ["PlayerUp"] = new Rectangle(0, 0, TileSize * 4, TileSize),
                ["PlayerDown"] = new Rectangle(0, 16, TileSize * 4, TileSize),
                ["PlayerRight"] = new Rectangle(0, 32, TileSize * 4, TileSize),
                ["PlayerLeft"] = new Rectangle(0, 48, TileSize * 4, TileSize),
            };

            Pixel = new Texture2D(graphicsDevice, 1, 1);
            Pixel.SetData<Color>(new Color[] { Color.White });
            GameFont = content.Load<SpriteFont>("Misc/gameFont");

            PowerLines = new List<PowerLine>();
            ButtonList = new List<Button>();
            GameObjects = new List<Objects.GameObject>();
        }
    }
}
