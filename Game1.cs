using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDelta.Helpers;
using ProjectDelta.Objects;
using ProjectDelta.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static ProjectDelta.Helpers.InputHelper;

namespace ProjectDelta
{
    public class Game1 : Game
    {
        enum Itemtypes { Battery, Tower };
        Itemtypes currtype = Itemtypes.Battery;

        private bool Active = true;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player _player;
        private Screen _screen;
        private Basic2DCamera _camera;
        private InputHelper _inputHelper;
        private SettingsMenu _settingsMenu;

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
            _camera = new(_screen.VirtualWidth, _screen.VirtualHeight);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatchExtensions.Initialize(GraphicsDevice);
            GameData.LoadData(Content, GraphicsDevice);
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _inputHelper = new InputHelper();
            _settingsMenu = new SettingsMenu(_screen);

            _player = new Player(); 
            _player.LoadData();



            _settingsMenu.AssignButtonFunctions(_camera, _screen);
        }

        protected override void Update(GameTime gameTime)
        {
            if(this.IsActive)
            {
                Active = true;
                _inputHelper.Update(_screen, _camera);
                float deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;


                #region Main Input

                // Exit();
                #endregion

                if (_inputHelper.IsKeyPress(Keys.C))
                {
                    GC.Collect();
                }

                if (!GameData.IsPaused)
                {
                    if (_inputHelper.IsKeyDown(Keys.Right))
                        currtype = Itemtypes.Battery;
                    if (_inputHelper.IsKeyDown(Keys.Left))
                        currtype = Itemtypes.Tower;



                    if (_inputHelper.IsKeyPress(Keys.X))
                    {
                        GameObject newObject = new();

                        switch (currtype)
                        {
                            case Itemtypes.Tower:
                                {
                                    newObject = new Tower(_inputHelper);
                                    break;
                                }
                            case Itemtypes.Battery:
                                {
                                    newObject = new Battery(_inputHelper);
                                    break;
                                }
                        }
                        GameData.GameObjects.Add(newObject);
                    }



                    #region PowerLines

                    // Create Power Line Pair
                    if (_inputHelper.IsKeyPress(Keys.Z))
                    {
                        if (isFirstClick)
                        {
                            //Create First Tower
                            firstpoint = new Vector2((int)_inputHelper.WorldMousePosition.X, (int)_inputHelper.WorldMousePosition.Y);
                            firsttower = new Tower(_inputHelper);
                            GameData.GameObjects.Add(firsttower);
                            isFirstClick = false;
                        }
                        else
                        {
                            //Create Last Tower
                            lastpoint = new Vector2((int)_inputHelper.WorldMousePosition.X, (int)_inputHelper.WorldMousePosition.Y);
                            lasttower = new Tower(_inputHelper);
                            GameData.GameObjects.Add(lasttower);

                            // Create the power line and associate it with the towers.
                            PowerLine powerLine = new(firsttower, lasttower);
                            GameData.PowerLines.Add(powerLine);
                            powerLine.CreateLine();
                            isFirstClick = true;
                        }
                    }

                    // Remove Power Line Pair
                    if (_inputHelper.IsMouseButtonPress(MouseButtons.RightButton))
                    {
                        if (GameData.PowerLines.Count > 0)
                        {
                            // Get the first power line. (TEMP)
                            // We Remove Both Towers ATM.
                            PowerLine firstPowerLine = GameData.PowerLines[0];
                            firstPowerLine.RemoveTower(firsttower);
                            firstPowerLine.RemoveTower(lasttower);
                        }
                    }

                    #endregion

                    //Ensure camera is centered on player
                    _player.Update(gameTime, deltatime, _inputHelper);

                }


                _camera.Position = new Vector2(_player.Position.X + 8, _player.Position.Y + 8);
                _camera.CenterOrigin();
                _settingsMenu.Update(_inputHelper);

                for (int i = 0; i < GameData.ButtonList.Count; i++)
                {
                    Button button = GameData.ButtonList[i];
                    button.Update(_inputHelper.VirtualMousePosition, _inputHelper, deltatime);
                }

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
            _spriteBatch.Draw(GameData.Pixel, new Rectangle(0, 0, 800, 800), null, Color.SlateGray, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            //GameObjects
            for (int i = 0; i < GameData.GameObjects.Count; i++)
            {
                GameObject gameObject = GameData.GameObjects[i];
                _spriteBatch.Draw(GameData.TextureAtlas, new Vector2((int)gameObject.position.X, (int)gameObject.position.Y), gameObject.texture, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, gameObject.depth);


                if (GameData.IsDebug)
                {
                    _spriteBatch.Draw(GameData.Pixel, gameObject.origin, null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    _spriteBatch.DrawHollowRect(gameObject.collider, Color.Red);
                }
            }

            //Power Lines
            for (int i = 0; i < GameData.PowerLines.Count; i++)
            {
                PowerLine PowerLine = GameData.PowerLines[i];
                Vector2[] LineVerts = PowerLine.LineVertices;

                for (int j = 0; j < LineVerts.Length - 1; j++)
                {
                    _spriteBatch.Draw(GameData.Pixel, LineVerts[j], null, Color.Black, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 1f);
                }
            }

            //Player
            _player.Draw(_spriteBatch);

            _spriteBatch.End();

            #endregion

            #region UI / HUD
            // Draw UI elements (buttons and text)
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix: _screen.ScreenScaleMatrix);


            _settingsMenu.Draw(_spriteBatch);

            if (GameData.IsDebug)
            {
                _spriteBatch.DrawString(GameData.GameFont, "WorldMousePos: " + ((int)_inputHelper.WorldMousePosition.X).ToString() + " " + ((int)_inputHelper.WorldMousePosition.Y).ToString(), new Vector2((int)5, (int)5), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .98f);
                _spriteBatch.DrawString(GameData.GameFont, "VirtualMousePos: " + ((int)_inputHelper.VirtualMousePosition.X).ToString() + " " + ((int)_inputHelper.VirtualMousePosition.Y).ToString(), new Vector2((int)5, (int)15), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .98f);
            }
            

            string textToCenter = "Paused";
            float textSize = (int)GameData.GameFont.MeasureString(textToCenter).X * 2;

            for (int i = 0; i < GameData.ButtonList.Count; i++)
            {
                Button button = GameData.ButtonList[i];
                button.Draw(_spriteBatch);
            }

            // Pause Text
            if (!Active)
                _spriteBatch.DrawString(GameData.GameFont, textToCenter, new Vector2((int)(_screen.VirtualWidth / 2) - (textSize / 2) + 1, (int)_screen.VirtualHeight - 30), Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);

            _spriteBatch.End();

            #endregion

            base.Draw(gameTime);
        }
    }
}