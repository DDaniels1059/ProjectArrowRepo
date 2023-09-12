using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ProjectDelta.Helpers
{
    public static class GameData
    {
        //public static Dictionary<int, Rectangle> TextureCoords;
        public static Dictionary<string, Rectangle> TextureMap;
        public static Texture2D TextureAtlas;
        public static Texture2D View;
        public static Texture2D _pixel;
        public static int TileSize = 16;
        public static float UIScale = 1f;
        public static SpriteFont GameFont;
        public static List<Button> ButtonList;
        public static List<GameObject> gameObjects;


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
                ["Tower"] = new Rectangle(64, 0, TileSize, TileSize * 3)

            //["Grass1"] = new Rectangle(96, 32, GameData.TileSize, GameData.TileSize),
        };

            //For Creating Randomness In A Map without Noise. Look At Your MonoSnake Project at the Map.cs
            //TextureCoords = new Dictionary<int, Rectangle>
            //{
            //    { 1, TextureMap["cactus"] },
            //    { 2, TextureMap["cactusflower"] },
            //    { 3, TextureMap["cactussingle"] },
            //    { 4, TextureMap["Grass1"] },
            //    { 5, TextureMap["Grass2"] }
            //};
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData<Color>(new Color[] { Color.White });

            ButtonList = new List<Button>();
            gameObjects = new List<GameObject>();
            GameFont = content.Load<SpriteFont>("Misc/gameFont");
        }
    }
}
