using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectArrow.System;
using System.Collections.Generic;

namespace ProjectArrow.Helpers
{
    public static class GameData
    {
        public static Dictionary<string, Rectangle> ObjectMap { get; private set; }
        public static Dictionary<string, Rectangle> UIMap { get; private set; }
        public static Dictionary<string, Rectangle> PlayerMap { get; private set; }

        public static SpriteFont GameFont { get; private set; }
        public static Texture2D ObjectAtlas { get; private set; }
        public static Texture2D UIAtlas { get; private set; }
        public static Texture2D PlayerAtlas { get; private set; }
        public static Texture2D Pixel { get; private set; }


        public static int ObjectTileSize { get; private set; }
        public static int UITileSize { get; private set; }
        public static int PlayerSize { get; private set; }


        public static bool IsDebug { get; set; }
        public static bool IsPaused { get; set; }


        public static int UIScale { get; set; }
        public static int CurrentZoom { get; set; }
        public static bool AllowVysnc { get; set; }
        public static int CurrentHz { get; set; }


        public static void LoadData(ContentManager content, GraphicsDevice graphicsDevice)
        {
            UITileSize = 16;
            PlayerSize = 16;
            ObjectTileSize = 16;

            IsDebug = false;
            IsPaused = false;

            UIScale = FileManager.CurrentSettings.UIScale;
            AllowVysnc = FileManager.CurrentSettings.Vsync;
            CurrentHz = FileManager.CurrentSettings.Hz;
            CurrentZoom = FileManager.CurrentSettings.Zoom;

            UIAtlas = content.Load<Texture2D>("Misc/UIAtlas");
            UIMap = new Dictionary<string, Rectangle>
            {                                 //X  Y  Width       Height
                ["DebugButton"] = new Rectangle(0, 0, UITileSize, UITileSize),
                ["DebugButtonPressed"] = new Rectangle(16, 0, UITileSize, UITileSize),
                ["LeftArrow"] = new Rectangle(32, 0, UITileSize, UITileSize),
                ["LeftArrowPressed"] = new Rectangle(48, 0, UITileSize, UITileSize),
                ["NonPressedButton"] = new Rectangle(0, 16, UITileSize, UITileSize),
                ["PressedButton"] = new Rectangle(16, 16, UITileSize, UITileSize)
            };

            PlayerAtlas = content.Load<Texture2D>("Misc/PlayerAtlas");
            PlayerMap = new Dictionary<string, Rectangle>
            {                                 //X  Y  Width         Height
                ["PlayerUp"] = new Rectangle(0, 0, PlayerSize * 4, PlayerSize),
                ["PlayerDown"] = new Rectangle(0, 16, PlayerSize * 4, PlayerSize),
                ["PlayerRight"] = new Rectangle(0, 32, PlayerSize * 4, PlayerSize),
                ["PlayerLeft"] = new Rectangle(0, 48, PlayerSize * 4, PlayerSize),
            };

            ObjectAtlas = content.Load<Texture2D>("Misc/ObjectAtlas");
            ObjectMap = new Dictionary<string, Rectangle>
            {                             //X  Y  Width           Height
                ["Grass"] = new Rectangle(0, 0, ObjectTileSize, ObjectTileSize),
                ["Box"] =  new Rectangle(16, 0, ObjectTileSize, ObjectTileSize),
                ["Gear"] =    new Rectangle(32, 0, ObjectTileSize, ObjectTileSize)
            };

            Pixel = new Texture2D(graphicsDevice, 1, 1);
            Pixel.SetData<Color>(new Color[] { Color.White });
            GameFont = content.Load<SpriteFont>("Misc/gameFont");
        }
    }
}
