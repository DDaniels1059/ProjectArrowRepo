using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectArrow.Helpers;
using ProjectArrow.Objects;
using ProjectArrow.UI;
using System;
using System.Diagnostics;

namespace ProjectArrow
{
    public class Game1 : Game
    {
        enum Itemtypes { Battery, Tower };
        Itemtypes currtype = Itemtypes.Battery;

        private bool Active = true;
        private GraphicsDeviceManager _graphics;
        private FpsMonitor _fpsMonitor;
        private SpriteBatch _spriteBatch;
        private Screen _screen;
        private Basic2DCamera _camera;
        private InputHelper _inputHelper;
        private SettingsMenu _settingsMenu;
        private Player _player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _screen = new Screen(_graphics, GraphicsDevice, Window, 640, 360);
            _camera = new(_screen.GameWidth, _screen.GameHeight);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatchExtensions.Initialize(GraphicsDevice);
            GameData.LoadData(Content, GraphicsDevice);
            _fpsMonitor = new FpsMonitor();
            _inputHelper = new InputHelper();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _settingsMenu = new SettingsMenu(_screen);
            _settingsMenu.AssignButtonFunctions(_camera, _screen);
            _player = new Player();
        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                Active = true;
                _inputHelper.Update(_camera, _screen);
                float deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;

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
                        Objects.GameObject newObject = new();

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


                    _player.Update(gameTime, deltatime, _inputHelper);

                }


                for (int i = 0; i < GameData.ButtonList.Count; i++)
                {
                    Button button = GameData.ButtonList[i];
                    button.Update(_inputHelper.VirtualMousePosition, _inputHelper, deltatime);
                }

                _camera.Position = new Vector2((int)_player.Position.X + 8, (int)_player.Position.Y + 8);
                _camera.CenterOrigin();
                _settingsMenu.Update(_inputHelper);
                _fpsMonitor.Update();
                base.Update(gameTime);
            }
            else
            {
                Active = false;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            _screen.TargetBeginDraw();

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix: _camera.TransformationMatrix);

            _spriteBatch.Draw(GameData.Pixel, new Rectangle(0, 0, 800, 800), null, Color.SlateGray, 0f, Vector2.Zero, SpriteEffects.None, 0f);

                //GameObjects
                for (int i = 0; i < GameData.GameObjects.Count; i++)
                {
                    Objects.GameObject gameObject = GameData.GameObjects[i];
                    _spriteBatch.Draw(GameData.TextureAtlas, new Vector2((int)gameObject.position.X, (int)gameObject.position.Y), gameObject.texture, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, gameObject.depth);


                    if (GameData.IsDebug)
                    {
                        _spriteBatch.Draw(GameData.Pixel, gameObject.origin, null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                        _spriteBatch.DrawHollowRect(gameObject.collider, Color.Red);
                    }
                }

                _player.Draw(_spriteBatch);

            _spriteBatch.End();

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);

                _settingsMenu.Draw(_spriteBatch);

                if (GameData.IsDebug)
                {
                    _fpsMonitor.Draw(_spriteBatch, GameData.GameFont, new Vector2((int)5, (int)25), Color.White);
                    _spriteBatch.DrawString(GameData.GameFont, "WorldMousePos: " + ((int)_inputHelper.WorldMousePosition.X).ToString() + " " + ((int)_inputHelper.WorldMousePosition.Y).ToString(), new Vector2((int)5, (int)5), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .5f);
                    _spriteBatch.DrawString(GameData.GameFont, "VirtualMousePos: " + ((int)_inputHelper.VirtualMousePosition.X).ToString() + " " + ((int)_inputHelper.VirtualMousePosition.Y).ToString(), new Vector2((int)5, (int)15), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, .5f);
                }



                string textToCenter = "Paused";
                float textSize = (int)GameData.GameFont.MeasureString(textToCenter).X * 4;

                for (int i = 0; i < GameData.ButtonList.Count; i++)
                {
                    Button button = GameData.ButtonList[i];
                    button.Draw(_spriteBatch);
                }

                // Pause Text
                if (!Active)
                    _spriteBatch.DrawString(GameData.GameFont, textToCenter, new Vector2((int)(_screen.GameWidth / 2) - (textSize / 2) + 1, (int)_screen.GameHeight - 40), Color.Black, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);

                Window.Title = "ProjectArrow" + " " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB";


            _spriteBatch.End();

            _screen.TargetEndDraw(_spriteBatch);

            base.Draw(gameTime);
        }

    }
    public class FpsMonitor
    {
        public float Value { get; private set; }
        public TimeSpan Sample { get; set; }
        private Stopwatch sw;
        private int Frames;
        public FpsMonitor()
        {
            this.Sample = TimeSpan.FromSeconds(1);
            this.Value = 0;
            this.Frames = 0;
            this.sw = Stopwatch.StartNew();
        }
        public void Update()
        {
            if (sw.Elapsed > Sample)
            {
                this.Value = (float)(Frames / sw.Elapsed.TotalSeconds);
                this.sw.Reset();
                this.sw.Start();
                this.Frames = 0;
            }
        }
        public void Draw(SpriteBatch SpriteBatch, SpriteFont Font, Vector2 Location, Color Color)
        {
            this.Frames++;
            SpriteBatch.DrawString(Font, "FPS: " + this.Value.ToString(), Location, Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.5f);
        }
    }
}