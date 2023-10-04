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

            ScreenManager.EndTargetDraws(_spriteBatch);

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            Window.Title = "ProjectArrow" + " " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB";

            _settingsMenu.Draw(_spriteBatch);


            int width = (int)(64 * GameData.UIScale);
            int height = (int)(64 * GameData.UIScale);

            Rectangle rect = new Rectangle((int)(ScreenManager.ScreenWidth - (width) - 32), (int)(20 * (GameData.UIScale * 2) - height / 2), width, height);

            _spriteBatch.DrawFilledRect(rect, Color.Green);

            Rectangle rect2 = new Rectangle((int)(32), (int)(20 * (GameData.UIScale * 2) - height / 2), width, height);

            _spriteBatch.DrawFilledRect(rect2, Color.Green);

            Rectangle rect3 = new Rectangle((int)(32), (int)(ScreenManager.ScreenHeight - 20 * (GameData.UIScale * 2) - height / 2), width, height);

            _spriteBatch.DrawFilledRect(rect3, Color.Green);


            Rectangle rect4 = new Rectangle((int)(ScreenManager.ScreenWidth - (width) - 32), (int)(ScreenManager.ScreenHeight - 20 * (GameData.UIScale * 2) - height / 2), width, height);

            _spriteBatch.DrawFilledRect(rect4, Color.Green);

            string textToCenter = "Paused";
            float textSize = (int)GameData.GameFont.MeasureString(textToCenter).X * (GameData.UIScale * 2);
                
            // Pause Text
            if (!Active)
            {
                _spriteBatch.DrawString(GameData.GameFont, textToCenter, new Vector2((int)(_graphics.PreferredBackBufferWidth / 2) - (textSize / 2) + 6, (_graphics.PreferredBackBufferHeight) - 130), Color.Black, 0f, Vector2.Zero, GameData.UIScale * 2, SpriteEffects.None, 1f);
            }

            if (GameData.IsDebug)
            {
                _spriteBatch.DrawString(GameData.GameFont, "WorldPos: " + ((int)_inputHelper.WorldMousePosition.X).ToString() + " " + ((int)_inputHelper.WorldMousePosition.Y).ToString(), new Vector2(10, (int)10 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                _spriteBatch.DrawString(GameData.GameFont, "ScreenPos: " + ((int)_inputHelper.ScreenMousePosition.X).ToString() + " " + ((int)_inputHelper.ScreenMousePosition.Y).ToString(), new Vector2(10, (int)20 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }    
}