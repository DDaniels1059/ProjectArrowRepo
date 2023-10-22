using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectArrow.Helpers;
using ProjectArrow.Objects;
using ProjectArrow.System;
using ProjectArrow.UI;
using ProjectArrow.Utility;
using System;

namespace ProjectArrow
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private InputManager inputHelper;
        private SettingsMenu settingsMenu;
        private Camera2d playerCamera = new Camera2d();
        private Player player;


        private float _frameRate;
        private Vector2 test = new Vector2(1100,900);
        private bool testSwitch = false;
        private SpriteAnimation[] _testAnimations = new SpriteAnimation[2];
        private SpriteAnimation _testAnim;
        private Texture2D _testTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        { 
            base.Initialize();
        }

        protected override void LoadContent()
        {
            FileManager.Initialize();
            ScreenManager.Initialize(graphics, Window, this);
            SpriteBatchExtensions.Initialize(GraphicsDevice);
            GameData.LoadData(Content, GraphicsDevice);
            ScreenManager.SetHZ(GameData.CurrentHz);

            playerCamera.Zoom = GameData.CurrentZoom;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            settingsMenu = new SettingsMenu(playerCamera);
            inputHelper = new InputManager();
            player = new Player();

            _testTexture = Content.Load<Texture2D>("Misc/Test");

            ObjectManager.Initialize(spriteBatch);

            Battery Batt1 = new (new Vector2(300, 320));
            Battery Batt2 = new (new Vector2(330, 320));
            Battery Batt3 = new(new Vector2(20, 320));
            Battery Batt4 = new(new Vector2(610, 320));


            _testAnimations[0] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerLeft", 4, 11);
            _testAnimations[1] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerRight", 4, 11);
            _testAnim = _testAnimations[0];
        }

        protected override void Update(GameTime gameTime)
        {
            float deltatime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            inputHelper.Update(playerCamera);


            if (inputHelper.IsKeyPress(Keys.C))
            {
                GC.Collect();
            }

            if (!GameData.IsPaused)
            {
                ObjectManager.UpdateObjects();
                player.Update(gameTime, deltatime, inputHelper);
            }


            float speed = 0.06f;

            if (!testSwitch)
            {
                _testAnim = _testAnimations[1];

                if (test.X > 980)
                {
                    testSwitch = true;
                }
                else
                {
                    test.X += speed * deltatime;
                }
            }

            if (testSwitch)
            {
                _testAnim = _testAnimations[0];

                if (test.X < 535)
                {
                    testSwitch = false;
                }
                else
                {
                    test.X -= speed * deltatime;
                }
            }

                
            _testAnim.Position.X = test.X;
            _testAnim.Position.Y = test.Y;
            _testAnim.Update(gameTime);

            settingsMenu.Update(inputHelper, deltatime);


            _frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            #region World Draw

            ScreenManager.WorldTargetBeginDraw();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix: playerCamera.get_transformation(GraphicsDevice));


                spriteBatch.Draw(_testTexture, new Vector2(0,0),null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.000001f);
                
                player.Draw(spriteBatch, playerCamera);
                spriteBatch.Draw(GameData.Pixel, new Rectangle(-1500, 0, 3000, 3000), null, Color.DimGray, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                ObjectManager.DrawObjects();
                
                _testAnim.Draw(spriteBatch, test, 1f, Color.DarkGreen);
                _testAnim.Draw(spriteBatch, new Vector2(test.X, test.Y + 120), 1f, Color.Crimson);
                _testAnim.Draw(spriteBatch, new Vector2(test.X, test.Y + 70), 1f, Color.CornflowerBlue);
                _testAnim.Draw(spriteBatch, new Vector2(test.X, test.Y + 35), 1f, Color.BlueViolet);

            spriteBatch.End();

            ScreenManager.EndTargetDraws(spriteBatch);

            #endregion

            #region UI Draw

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

                Window.Title = "ProjectArrow" + " " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB";

                settingsMenu.Draw(spriteBatch);

                if (GameData.IsDebug)
                {
                    spriteBatch.DrawString(GameData.GameFont, "WorldPos: " + ((int)inputHelper.WorldMousePosition.X).ToString() + " " + ((int)inputHelper.WorldMousePosition.Y).ToString(), new Vector2(10, (int)10 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                    spriteBatch.DrawString(GameData.GameFont, "ScreenPos: " + ((int)inputHelper.ScreenMousePosition.X).ToString() + " " + ((int)inputHelper.ScreenMousePosition.Y).ToString(), new Vector2(10, (int)20 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                    spriteBatch.DrawString(GameData.GameFont, "FPS: " + _frameRate.ToString(), new Vector2(10, (int)30 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                }


            spriteBatch.End();

            //int width = (int)(64 * GameData.UIScale);
            //int height = (int)(64 * GameData.UIScale);

            //Rectangle rect = new Rectangle((int)(ScreenManager.ScreenWidth - (width) - 32), (int)(20 * (GameData.UIScale * 2) - height / 2), width, height);

            //spriteBatch.DrawHollowRect(rect, Color.Red);


            //spriteBatch.DrawFilledRect(rect, Color.Green);

            //Rectangle rect2 = new Rectangle((int)(32), (int)(20 * (GameData.UIScale * 2) - height / 2), width, height);

            //spriteBatch.DrawFilledRect(rect2, Color.Green);

            //Rectangle rect3 = new Rectangle((int)(32), (int)(ScreenManager.ScreenHeight - 20 * (GameData.UIScale * 2) - height / 2), width, height);

            //spriteBatch.DrawFilledRect(rect3, Color.Green);


            //Rectangle rect4 = new Rectangle((int)(ScreenManager.ScreenWidth - (width) - 32), (int)(ScreenManager.ScreenHeight - 20 * (GameData.UIScale * 2) - height / 2), width, height);

            //spriteBatch.DrawFilledRect(rect4, Color.Green);

            //string textToCenter = "Paused";
            //float textSize = (int)GameData.GameFont.MeasureString(textToCenter).X * (GameData.UIScale * 2);

            //// Pause Text
            //if (GameData.IsPaused || !Active)
            //{
            //    spriteBatch.DrawString(GameData.GameFont, textToCenter, new Vector2((int)(graphics.PreferredBackBufferWidth / 2) - (textSize / 2) + 6, (graphics.PreferredBackBufferHeight) - 130), Color.Black, 0f, Vector2.Zero, GameData.UIScale * 2, SpriteEffects.None, 1f);
            //}

            #endregion

            base.Draw(gameTime);

        }
    }    
}