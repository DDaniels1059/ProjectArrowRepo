using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectArrow.Helpers;
using ProjectArrow.Objects;
using ProjectArrow.System;
using ProjectArrow.UI;
using ProjectArrow.Utility;
using System;
using System.Diagnostics;
using System.Threading;

namespace ProjectArrow
{
    public class Game1 : Game
    {
        private FpsMonitor FPSM = new FpsMonitor();
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private InputManager inputHelper;
        private SettingsMenu settingsMenu;
        private Camera2d playerCamera = new Camera2d();
        private Player player;


        private bool testSwitch = false;
        private SpriteAnimation[] _testAnimations = new SpriteAnimation[2];
        private SpriteAnimation _testAnim;

        private Vector2 test = new Vector2(1100, 900);

        private int _testIndex = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            InactiveSleepTime = TimeSpan.Zero;
            graphics.ApplyChanges();
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

            //Not The Best Way
            //ScreenManager.SetHZ(GameData.CurrentHz);
            
            playerCamera.Zoom = GameData.CurrentZoom;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            settingsMenu = new SettingsMenu(playerCamera);
            inputHelper = new InputManager();
            player = new Player();

            ObjectManager.Initialize(spriteBatch);



            Battery batt1 = new Battery(new Vector2(700, 600));
            Battery batt2 = new Battery(new Vector2(500, 600));
            Battery batt3 = new Battery(new Vector2(600, 600));
            Battery batt4 = new Battery(new Vector2(800, 600));

            _testAnimations[0] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerLeft", 4, 5);
            _testAnimations[1] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerRight", 4, 5);
            _testAnim = _testAnimations[0];
        }

        protected override void Update(GameTime gameTime)
        {
            float deltatime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            inputHelper.Update(playerCamera);


            _testIndex += 1;

            if (inputHelper.IsKeyPress(Keys.C))
            {
                //GC.Collect();

                _testIndex = 0;
            }

            if (!GameData.IsPaused)
            {
                ObjectManager.UpdateObjects();
            }

            player.Update(gameTime, deltatime, inputHelper);

            float speed = 0.04f;

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


            //playerCamera.Follow(player.Position);

            FPSM.Update();
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            #region World Draw

            ScreenManager.WorldTargetBeginDraw();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix: playerCamera.get_transformation(GraphicsDevice));


                    player.Draw(spriteBatch);
                    Vector2 camPos = player.Position;
                    playerCamera._pos = camPos;


                ObjectManager.DrawObjects();
                
                _testAnim.Draw(spriteBatch, new Vector2(test.X, test.Y - 20), 1f, Color.DarkGreen);
                _testAnim.Draw(spriteBatch, new Vector2(test.X, test.Y + 120), 1f, Color.Crimson);
                _testAnim.Draw(spriteBatch, new Vector2(test.X, test.Y + 70), 1f, Color.CornflowerBlue);
                _testAnim.Draw(spriteBatch, new Vector2(test.X, test.Y + 35), 1f, Color.BlueViolet);



            //Nested for loops to draw the grid ONLY FOR TESTING
            for (int row = 0; row < 25; row++)
                {
                    for (int col = 0; col < 25; col++)
                    {
                        // Calculate the position for each tile
                        int x = col * GameData.ObjectTileSize;
                        int y = row * GameData.ObjectTileSize;

                        // Draw the Battery tile at the calculated position
                        spriteBatch.Draw(GameData.ObjectAtlas, new Vector2(x, y), GameData.ObjectMap["Grass"], Color.Gray, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }


            spriteBatch.End();

            ScreenManager.EndTargetDraws(spriteBatch);

            #endregion

            #region UI Draw

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

                Window.Title = "ProjectArrow" + " " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB";

                settingsMenu.Draw(spriteBatch);

                if (GameData.IsDebug)
                {
                    spriteBatch.DrawString(GameData.GameFont, "CameraPos: " + (playerCamera._pos.X).ToString() + " " + (playerCamera._pos.Y).ToString(), new Vector2(10, (int)1 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                    spriteBatch.DrawString(GameData.GameFont, "WorldPos: " + ((int)inputHelper.WorldMousePosition.X).ToString() + " " + ((int)inputHelper.WorldMousePosition.Y).ToString(), new Vector2(10, (int)10 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                    spriteBatch.DrawString(GameData.GameFont, "ScreenPos: " + ((int)inputHelper.ScreenMousePosition.X).ToString() + " " + ((int)inputHelper.ScreenMousePosition.Y).ToString(), new Vector2(10, (int)20 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                    FPSM.Draw(spriteBatch, GameData.GameFont, new Vector2(10, (int)30 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                    spriteBatch.DrawString(GameData.GameFont, "Test Increment: " + (_testIndex).ToString() , new Vector2(10, (int)40 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);              
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