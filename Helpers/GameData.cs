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
        public static Dictionary<string, Rectangle> WaterAnimMap { get; private set; }
        public static Dictionary<string, Rectangle> UIMap { get; private set; }
        public static Dictionary<string, Rectangle> PlayerMap { get; private set; }
        public static Dictionary<string, Rectangle> PlayerFishMap { get; private set; }



        public static SpriteFont GameFont { get; private set; }
        public static Texture2D ObjectAtlas { get; private set; }
        public static Texture2D WaterAnimAtlas { get; private set; }
        public static Texture2D UIAtlas { get; private set; }
        public static Texture2D PlayerAtlas { get; private set; }
        public static Texture2D PlayerFishAtlas { get; private set; }

        public static Texture2D Pixel { get; private set; }
        public static List<Rectangle> CollisionBorders { get; private set; }
        public static Vector2 playerStart;

        public static int ObjectTileSize { get; private set; }
        public static int UITileSize { get; private set; }
        public static int PlayerSize { get; private set; }


        public static bool IsDebug { get; set; }
        public static bool IsPaused { get; set; }
        public static bool ExitGame { get; set; }

        public static int UIScale { get; set; }
        public static int CurrentZoom { get; set; }
        public static bool AllowVysnc { get; set; }


        public static void LoadData(ContentManager content, GraphicsDevice graphicsDevice)
        {
            CollisionBorders = new List<Rectangle>();

            ObjectTileSize = 16;
            UITileSize = 16;
            PlayerSize = 16;

            IsDebug = false;
            IsPaused = false;
            ExitGame = false;

            UIScale = FileManager.CurrentSettings.UIScale;
            AllowVysnc = FileManager.CurrentSettings.Vsync;
            CurrentZoom = FileManager.CurrentSettings.Zoom;

            UIAtlas = content.Load<Texture2D>("Misc/UIAtlas");
            UIMap = new Dictionary<string, Rectangle>
            {                                 //X  Y  Width       Height
                ["DebugButton"] = new Rectangle(0, 0, UITileSize, UITileSize),
                ["DebugButtonPressed"] = new Rectangle(16, 0, UITileSize, UITileSize),
                ["LeftArrow"] = new Rectangle(32, 0, UITileSize, UITileSize),
                ["LeftArrowPressed"] = new Rectangle(48, 0, UITileSize, UITileSize),
                ["NonPressedButton"] = new Rectangle(0, 16, UITileSize, UITileSize),
                ["PressedButton"] = new Rectangle(16, 16, UITileSize, UITileSize),
                ["ExitButton"] = new Rectangle(32, 16, UITileSize, UITileSize)
            };

            WaterAnimAtlas = content.Load<Texture2D>("Misc/WaterAnimAtlas");
            WaterAnimMap = new Dictionary<string, Rectangle>
            {                           //X  Y  Width           Height
                ["TopLeft"] = new Rectangle(0, 0, ObjectTileSize * 4, ObjectTileSize),
                ["LeftCenter"] = new Rectangle(0, 16, ObjectTileSize * 4, ObjectTileSize),
                ["BottomLeft"] = new Rectangle(0, 32, ObjectTileSize * 4, ObjectTileSize),
                ["TopCenter"] = new Rectangle(0, 48, ObjectTileSize * 4, ObjectTileSize),
                ["Corner"] = new Rectangle(0, 64, ObjectTileSize * 4, ObjectTileSize),
            };

            PlayerAtlas = content.Load<Texture2D>("Misc/PlayerAtlas");
            PlayerMap = new Dictionary<string, Rectangle>
            {                              //X  Y  Width         Height
                ["PlayerUp"] = new Rectangle(0, 0, PlayerSize * 4, PlayerSize),
                ["PlayerDown"] = new Rectangle(0, 16, PlayerSize * 4, PlayerSize),
                ["PlayerRight"] = new Rectangle(0, 32, PlayerSize * 4, PlayerSize),
                ["PlayerLeft"] = new Rectangle(0, 48, PlayerSize * 4, PlayerSize),
            };

            PlayerFishAtlas = content.Load<Texture2D>("Misc/PlayerFishAtlas");
            PlayerFishMap = new Dictionary<string, Rectangle>
            {                              //X  Y  Width         Height
                ["FishRight"] = new Rectangle(0, 0, PlayerSize * 6 , PlayerSize * 2),
                ["FishLeft"] = new Rectangle(0, 32, PlayerSize * 6, PlayerSize * 2),
                ["FishDown"] = new Rectangle(0, 64, PlayerSize * 6, PlayerSize * 2),
                ["FishUp"] = new Rectangle(0, 96, PlayerSize * 6, PlayerSize * 2),
            };

            ObjectAtlas = content.Load<Texture2D>("Misc/ObjectAtlas");
            ObjectMap = new Dictionary<string, Rectangle>
            {                           //X  Y  Width           Height
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
