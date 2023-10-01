using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectArrow.Helpers;
using ProjectArrow.Objects;
using ProjectArrow.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;

namespace ProjectArrow
{
    public class Game1 : Game
    {
        private bool Active = true;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Basic2DCamera _camera;
        private InputHelper _inputHelper;
        private SettingsMenu _settingsMenu;
        private Player _player;

        private List<MonitorResolution> _availableResolutions;
        private int currentResolutionIndex = 0;


        #region Temporary Resolution Change

        public struct MonitorResolution
        {
            public int Width;
            public int Height;

            public MonitorResolution(int width, int height)
            {
                Width = width;
                Height = height;
            }
        }

        private List<MonitorResolution> GetAvailableResolutions()
        {
            List<MonitorResolution> resolutions = new List<MonitorResolution>();

            foreach (var displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                resolutions.Add(new MonitorResolution(displayMode.Width, displayMode.Height));
            }

            return resolutions;
        }

        public void CycleResolutions(int direction)
        {
            if (_availableResolutions.Count == 0)
                return;

            currentResolutionIndex += direction;
            if (currentResolutionIndex < 0)
            {
                currentResolutionIndex = _availableResolutions.Count - 1;
            }
            else if (currentResolutionIndex >= _availableResolutions.Count)
            {
                currentResolutionIndex = 0;
            }

            ScreenManager.SetWindowed(_availableResolutions[currentResolutionIndex].Width, _availableResolutions[currentResolutionIndex].Height);
        }

        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            ScreenManager.Graphics = _graphics;
            ScreenManager.Window = Window;
        }

        protected override void Initialize()
        {
            _camera = new(ScreenManager.VirtualWidth, ScreenManager.VirtualHeight);
            _availableResolutions = GetAvailableResolutions();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ScreenManager.Initialize();
            SpriteBatchExtensions.Initialize(GraphicsDevice);
            GameData.LoadData(Content, GraphicsDevice);
            _player = new Player();
            _inputHelper = new InputHelper();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _settingsMenu = new SettingsMenu();
            _settingsMenu.AssignButtonFunctions(_camera);


            Battery Batt1 = new (new Vector2(300, 320));
            Battery Batt2 = new (new Vector2(330, 320));
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                Active = true;
                float deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;


                _inputHelper.Update(_camera);


                if (_inputHelper.IsKeyRelease(Keys.Right))
                    CycleResolutions(1);

                if (_inputHelper.IsKeyRelease(Keys.Left))
                    CycleResolutions(-1);

                if (_inputHelper.IsKeyPress(Keys.C))
                {
                    GC.Collect();
                }
               
                if (!GameData.IsPaused)
                {
                    _player.Update(gameTime, deltatime, _inputHelper);
                }

                for (int i = 0; i < GameData.ButtonList.Count; i++)
                {
                    Button button = GameData.ButtonList[i]; 
                    button.Update(_inputHelper.ScreenMousePosition, _inputHelper, deltatime);
                }

                _camera.Position = new Vector2((int)_player.Position.X + 8, (int)_player.Position.Y + 8);
                _camera.CenterOrigin();

                _settingsMenu.Update(_inputHelper);
                base.Update(gameTime);
            }
            else
            {
                Active = false;
            }
        }

        protected override void Draw(GameTime gameTime)
        {

            ScreenManager.WorldTargetBeginDraw();

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix: _camera.TransformationMatrix);

                _spriteBatch.Draw(GameData.Pixel, new Rectangle(0, 0, 800, 800), null, Color.SlateGray, 0f, Vector2.Zero, SpriteEffects.None, 0f);

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

                _player.Draw(_spriteBatch);

            _spriteBatch.End();


            ScreenManager.UITargetBeginDraw();

            //Draw UI
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

                Window.Title = "ProjectArrow" + " " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB";

                _settingsMenu.Draw(_spriteBatch);

                if (GameData.IsDebug)
                {
                    _spriteBatch.DrawString(GameData.GameFont, "WorldPos: " + ((int)_inputHelper.WorldMousePosition.X).ToString() + " " + ((int)_inputHelper.WorldMousePosition.Y).ToString(), new Vector2(20, (int)50), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                    _spriteBatch.DrawString(GameData.GameFont, "ScreenPos: " + ((int)_inputHelper.ScreenMousePosition.X).ToString() + " " + ((int)_inputHelper.ScreenMousePosition.Y).ToString(), new Vector2(20, (int)100), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                }

                string textToCenter = "Paused";
                float textSize = (int)GameData.GameFont.MeasureString(textToCenter).X * (GameData.UIScale * 2);


                // Pause Text
                if (!Active)
                {
                    _spriteBatch.DrawString(GameData.GameFont, textToCenter, new Vector2((int)(_graphics.PreferredBackBufferWidth / 2) - (textSize / 2) + 6, (_graphics.PreferredBackBufferHeight) - 148), Color.Black, 0f, Vector2.Zero, GameData.UIScale * 2, SpriteEffects.None, 1f);
                }

            _spriteBatch.End();

            ScreenManager.EndTargetDraws(_spriteBatch);


            base.Draw(gameTime);
        }
    }    
}