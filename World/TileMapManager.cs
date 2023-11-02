using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using TiledSharp;
using Microsoft.Xna.Framework;
using ProjectArrow.Helpers;
using System.Collections.Generic;
using ProjectArrow.World;
using ProjectArrow.Utility;
using System.Diagnostics;

namespace ProjectArrow
{

    class TileMapManager
    {
        private static TmxMap map;
        private static Texture2D tileset;

        private int tilesetTilesWide;
        private int tilesetTilesHigh;
        private int tileWidth;
        private int tileHeight;

        TmxLayer Ground;
        TmxLayer Colliders;
        TmxLayer Water;
        TmxObjectGroup ObjectLayer;

        private List<Tile> tiles = new List<Tile>();
        private List<AnimTile> animTiles = new List<AnimTile>();

        public void LoadContent(ContentManager Content)
        {
            map = new TmxMap(Content.RootDirectory + "\\OverWorld.tmx");
            tileset = Content.Load<Texture2D>($"Map/{map.Tilesets[0].Name}");

            Ground = map.Layers["Ground"];
            Colliders = map.Layers["Colliders"];
            Water = map.Layers["Water"];

            ObjectLayer = map.ObjectGroups["Objects"];

            GameData.playerStart.X = (int)(float)ObjectLayer.Objects["PlayerSpawn"].X;
            GameData.playerStart.Y = (int)(float)ObjectLayer.Objects["PlayerSpawn"].Y;

            tileWidth = map.Tilesets[0].TileWidth;
            tileHeight = map.Tilesets[0].TileHeight;

            tilesetTilesWide = tileset.Width / tileWidth;
            tilesetTilesHigh = tileset.Height / tileHeight;

            //Create Base Ground Map Data
            for (var i = 0; i < Ground.Tiles.Count; i++)
            {
                int gid = Ground.Tiles[i].Gid;
                TmxLayerTile tile = Ground.Tiles[i];

                if (gid != 0)
                {
                    int tileFrame = gid - 1;
                    int column = tileFrame % tilesetTilesWide;
                    int row = (int)Math.Floor(tileFrame / (double)tilesetTilesWide);

                    float x = tile.X * 16;
                    float y = tile.Y * 16;

                    Rectangle tilesetRect = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);
                    Rectangle destRect = new Rectangle((int)x, (int)y, tileWidth, tileHeight);
                    tiles.Add(new Tile
                    {
                        SourceRect = tilesetRect,
                        DestRect = destRect,
                        Position = new Vector2(x, y),
                    });
                }
            }

            //Create Base Water Map Data
            //Simplify The Animated Water Tile Set
            Dictionary<int, string> gidToAnimationName = new Dictionary<int, string>
            {
                { 25, "TopLeft" },
                { 33, "LeftCenter" },
                { 41, "BottomLeft" },
                { 49, "TopCenter" },
                { 57, "Corner" }
            };

            for (var i = 0; i < Water.Tiles.Count; i++)
            {
                int gid = Water.Tiles[i].Gid;
                TmxLayerTile tile = Water.Tiles[i];
                SpriteEffects effect = SpriteEffects.None;

                if (tile.HorizontalFlip)
                {
                    effect |= SpriteEffects.FlipHorizontally;
                }
                if (tile.VerticalFlip)
                {
                    effect |= SpriteEffects.FlipVertically;
                }

                if (gid > 0 )
                {
                    Debug.WriteLine(gid.ToString());
                }

                if (gidToAnimationName.ContainsKey(gid))
                {
                    float x = tile.X * 16;
                    float y = tile.Y * 16;
                    string animationName = gidToAnimationName[gid];

                    animTiles.Add(new AnimTile
                    {
                        Effect = effect,
                        Anim = new SpriteAnimation(GameData.WaterAnimAtlas, GameData.WaterAnimMap, animationName, 4, 2),
                        Position = new Vector2(x, y)
                    });
                }
            }

            //Create Collisions From Layer
            foreach (TmxLayerTile tile in Colliders.Tiles)
            {
                if (tile.Gid != 0)
                {
                    Rectangle rect = new Rectangle(tile.X * tileWidth, tile.Y * tileHeight, tileWidth, tileHeight);
                    GameData.CollisionBorders.Add(rect);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var animtile in animTiles)
            {
                animtile.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            foreach (var animtile in animTiles)
            {
                animtile.Draw(_spriteBatch);
            }

            foreach (var tile in tiles)
            {
                _spriteBatch.Draw(tileset, tile.DestRect, tile.SourceRect, Color.White);
            }
        }
    }
}
