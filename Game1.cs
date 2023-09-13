using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDelta.Helpers;
using ProjectDelta.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static ProjectDelta.Helpers.InputHelper;

namespace ProjectDelta
{
    public class Game1 : Game
    {
        enum Itemtypes { gears, bricks };
        Itemtypes currtype = Itemtypes.gears;

        private bool Active = true;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player _player;
        private Screen _screen;
        private Basic2DCamera _camera;
        private InputHelper inputHelper;

        private List<Vector2[]> lines;

        private Rectangle _npcRect;
        private Button OptionsButton;

        private bool isFirstClick = true;


        Vector2 firstpoint;
        Vector2 lastpoint;
        Tower firsttower;
        Tower lasttower;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _screen = new Screen(_graphics, GraphicsDevice, Window);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteBatchExtensions.Initialize(GraphicsDevice);
            GameData.LoadData(Content, GraphicsDevice);
            inputHelper = new InputHelper();

            lines = new List<Vector2[]>();
            _player = new Player();
            _player.LoadData();
            _npcRect = new Rectangle((_screen.VirtualWidth / 2), (_screen.VirtualHeight / 2), 16, 16);


            // Create camera
            _camera = new(_screen.VirtualWidth, _screen.VirtualHeight);
            _camera.Zoom *= new Vector2(1f);

            OptionsButton = new Button();
            OptionsButton.CreateButton(GameData.TextureMap["Wrench"], GameData.TextureMap["Wrench"], false);
            OptionsButton.location = new Vector2(_screen.VirtualWidth - 24, 8);
            OptionsButton.buttonPress += SettingsPress;
        }

        protected override void Update(GameTime gameTime)
        {
            if(this.IsActive)
            {
                Active = true;
                GameData.UIScale = 1F;
                inputHelper.Update(_screen, _camera);
                float deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                //CreateLine(firsttower.linePosition, lasttower.linePosition);

                if (inputHelper.IsKeyDown(Keys.Escape))
                {
                    Exit();
                }
                if (inputHelper.IsKeyPress(Keys.D1))
                {
                    if (!_screen._isFullscreen)
                    {
                        _screen.SetFullscreen();
                    }
                    else
                    {
                        _screen.SetWindowed(1280, 720);
                    }
                }



                //if (inputHelper.IsKeyDown(Keys.Right))
                //    currtype = Itemtypes.gears;
                //if (inputHelper.IsKeyDown(Keys.Left))
                //    currtype = Itemtypes.bricks;


                //if (inputHelper.IsKeyPress(Keys.Z))
                //{
                //    GameObject newObject = new GameObject();

                //    switch (currtype)
                //    {
                //        case Itemtypes.gears:
                //            {
                //                newObject = new Tower(inputHelper);                                
                //                break;
                //            }
                //        case Itemtypes.bricks:
                //            {
                //                newObject = new Bricks();
                //                newObject.texture = GameData.TextureMap["Gear"];
                //                break;
                //            }
                //    }
                //    GameData.gameObjects.Add(newObject);
                //}

                #region PowerLines

                // Create Power Line Pair
                if (inputHelper.IsKeyPress(Keys.Z))
                {
                    if (isFirstClick)
                    {
                        //Create First Tower
                        firstpoint = new Vector2(inputHelper.WorldMousePosition.X, inputHelper.WorldMousePosition.Y);
                        firsttower = new Tower(inputHelper);
                        GameData.gameObjects.Add(firsttower);
                        isFirstClick = false;
                    }
                    else
                    {
                        //Create Last Tower
                        lastpoint = new Vector2(inputHelper.WorldMousePosition.X, inputHelper.WorldMousePosition.Y);
                        lasttower = new Tower(inputHelper);
                        GameData.gameObjects.Add(lasttower);

                        // Create the power line and associate it with the towers.
                        PowerLine powerLine = new PowerLine(firsttower, lasttower);
                        GameData.powerLines.Add(powerLine);
                        powerLine.CreateLine();
                        isFirstClick = true;
                    }
                }

                // Remove Power Line Pair
                if (inputHelper.IsMouseButtonPress(MouseButtons.RightButton))
                {
                    if (GameData.powerLines.Count > 0)
                    {
                        // Get the first power line. (TEMP)
                        // We Remove Both Towers ATM.
                        PowerLine firstPowerLine = GameData.powerLines[0];
                        firstPowerLine.RemoveTower(firsttower);
                        firstPowerLine.RemoveTower(lasttower);
                    }
                }

                #endregion

                for (int i = 0; i < GameData.ButtonList.Count; i++)
                {
                    Button button = GameData.ButtonList[i];
                    button.Update(inputHelper.VirtualMousePosition, inputHelper, deltatime, button.location);
                }

                //Ensure camera is centered on player
                _player.Update(gameTime, deltatime, inputHelper);
                _camera.Position = new Vector2(_player.Position.X + 8, _player.Position.Y + 8);
                _camera.CenterOrigin();

                base.Update(gameTime);
            }
            else
            {
                Active = false;
            }
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Viewport = _screen.Viewport;
            GraphicsDevice.Clear(Color.Black);

            #region World Draw

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix: _camera.TransformationMatrix * _screen.ScreenScaleMatrix);      

            //Background
            _spriteBatch.Draw(GameData._pixel, new Rectangle(0, 0, 800, 800), null, Color.Orange, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            //GameObjects
            for (int i = 0; i < GameData.gameObjects.Count; i++)
            {
                GameObject gameObject = GameData.gameObjects[i];
                _spriteBatch.Draw(GameData.TextureAtlas, new Vector2((int)gameObject.position.X, (int)gameObject.position.Y), gameObject.texture, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, gameObject.depth);
                //Draw Origin
                _spriteBatch.Draw(GameData._pixel, gameObject.origin, null, Color.Red,0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);


                // Check if the GameObject is of type Tower
                if (gameObject is Tower tower)
                {
                    _spriteBatch.DrawHollowRect(tower.collider, Color.Red);
                }
            }


            //foreach (var powerLine in GameData.powerLines)
            //{
            //    // Check if the power line has vertices and draw it
            //    if (powerLine.LineVertices.Count > 0)
            //    {
            //        for (int j = 0; j < powerLine.LineVertices.Count - 1; j++)
            //        {
            //            _spriteBatch.Draw(GameData._pixel, powerLine.LineVertices[j], null, Color.Black, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 1f);
            //        }
            //    }
            //}


            for (int i = 0; i < GameData.powerLines.Count; i++)
            {
                PowerLine PowerLine = GameData.powerLines[i];
                Vector2[] LineVerts = PowerLine.LineVertices;

                for (int j = 0; j < LineVerts.Length - 1; j++)
                {
                    _spriteBatch.Draw(GameData._pixel, LineVerts[j], null, Color.Black, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 1f);
                }
            }


            ////Power Lines
            //for (int i = 0; i < lines.Count; i++)
            //{
            //    Vector2[] line = lines[i];

            //    for (int j = 0; j < line.Length - 1; j++)
            //    {
            //        _spriteBatch.Draw(GameData._pixel, line[j], null, Color.Black, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 1f);
            //    }
            //}

            //Player
            _player.Draw(_spriteBatch);


            //NPC
            _spriteBatch.Draw(GameData.TextureAtlas, _npcRect, GameData.TextureMap["Gear"], Color.Green, 0f, Vector2.Zero, SpriteEffects.None, Helper.GetDepth(new Vector2(_npcRect.X + 8, _npcRect.Y + 14)));


            _spriteBatch.End();

            #endregion

            #region UI / HUD
            // Draw UI elements (buttons and text)
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix: _screen.ScreenScaleMatrix);

            //_spriteBatch.Draw(GameData.TextureAtlas, new Rectangle(_buttonRect.X + 8, _buttonRect.Y + 8, GameData.TileSize, GameData.TileSize), GameData.TextureMap["Wrench"], Color.White, rotation, _buttonRect.Size.ToVector2() / 2f, SpriteEffects.None, 1f);
            //_spriteBatch.Draw(_pixel, _buttonRect, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, .9f);
            //This allows us to see the padding view change, much easier...
            //Show this only when the user wants to change padding?
            //_spriteBatch.Draw(_view, Vector2.Zero, null, Color.White);

            _spriteBatch.DrawString(GameData.GameFont, "WorldMousePos: " + ((int)inputHelper.WorldMousePosition.X).ToString() + " " + ((int)inputHelper.WorldMousePosition.Y).ToString(), new Vector2((int)5, (int)5), Color.White, 0f, Vector2.Zero, .35f, SpriteEffects.None, 1f);
            _spriteBatch.DrawString(GameData.GameFont, "VirtualMousePos: " + ((int)inputHelper.VirtualMousePosition.X).ToString() + " " + ((int)inputHelper.VirtualMousePosition.Y).ToString(), new Vector2((int)5, (int)15), Color.White, 0f, Vector2.Zero, .35f, SpriteEffects.None, 1f);
            //_spriteBatch.DrawString(GameData.GameFont, "LEFT And RIGHT Arrows To Change View Padding", new Vector2((int)5, (int)15), Color.White, 0f, Vector2.Zero, .25f, SpriteEffects.None, 1f);

            string textToCenter = "Paused";
            float textSize = GameData.GameFont.MeasureString(textToCenter).X * 1f;

            for (int i = 0; i < GameData.ButtonList.Count; i++)
            {
                Button button = GameData.ButtonList[i];
                button.Draw(_spriteBatch);
            }

            // Pause Text
            if (!Active)
                _spriteBatch.DrawString(GameData.GameFont, textToCenter, new Vector2((int)(324 - textSize) / 2, (int)(180 / 3)), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);


            _spriteBatch.End();

            #endregion

            base.Draw(gameTime);
        }

        public void SettingsPress()
        {
            if (inputHelper.IsMouseButtonPress(MouseButtons.RightButton))
            {
                _camera.Zoom /= new Vector2(2f);
            }

            if (inputHelper.IsMouseButtonPress(MouseButtons.LeftButton))
            {
                _camera.Zoom *= new Vector2(2f);
            }
        }
    }
}