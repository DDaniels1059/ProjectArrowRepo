using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectDelta.Objects;
using System;
using System.Collections.Generic;

namespace ProjectDelta.Helpers
{
    public static class GameData
    {
        public static Dictionary<string, Rectangle> TextureMap;

        public static SpriteFont GameFont;
        public static Texture2D TextureAtlas;

        public static Texture2D PlayerDown;
        public static Texture2D PlayerUp;
        public static Texture2D PlayerRight;
        public static Texture2D PlayerLeft;

        public static Texture2D View;
        public static Texture2D Pixel;
        public static int TileSize = 16;
        public static float UIScale = 1f;
        public static bool IsDebug = false;

        public static List<Button> ButtonList;
        public static List<GameObject> GameObjects;
        public static List<PowerLine> PowerLines;

        public static void LoadData(ContentManager content, GraphicsDevice graphicsDevice)
        {
            View = content.Load<Texture2D>("Misc/View");
            TextureAtlas = content.Load<Texture2D>("Misc/TextureAtlas");
            TextureMap = new Dictionary<string, Rectangle>
            {                               //X  Y  Width     Height
                ["Player_Up"] = new Rectangle(0, 0, TileSize, TileSize),
                ["Player_Down"] = new Rectangle(16, 0, TileSize, TileSize),
                ["Player_Right"] = new Rectangle(32, 0, TileSize, TileSize),
                ["Player_Left"] = new Rectangle(48, 0, TileSize, TileSize),

                ["Wrench"] = new Rectangle(0, 16, TileSize, TileSize),
                ["Gear"] = new Rectangle(16, 16, TileSize, TileSize),
                ["Debug"] = new Rectangle(32, 16, TileSize, TileSize),
                ["Tower"] = new Rectangle(64, 0, TileSize, TileSize * 3),
                ["Battery"] = new Rectangle(80, 16, TileSize, TileSize * 2)
            };

            Pixel = new Texture2D(graphicsDevice, 1, 1);
            Pixel.SetData<Color>(new Color[] { Color.White });

            PlayerDown = content.Load<Texture2D>("Misc/PlayerWalkDown");
            PlayerUp = content.Load<Texture2D>("Misc/PlayerWalkUp");
            PlayerRight = content.Load<Texture2D>("Misc/PlayerWalkRight");
            PlayerLeft = content.Load<Texture2D>("Misc/PlayerWalkLeft");

            PowerLines = new List<PowerLine>();
            ButtonList = new List<Button>();
            GameObjects = new List<GameObject>();
            GameFont = content.Load<SpriteFont>("Misc/gameFont");
        }
    }
}
