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
        private FpsMonitor FPSM = new FpsMonitor();
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private InputManager inputHelper;
        private SettingsMenu settingsMenu;
        private Camera2d playerCamera;
        private Player player;

        private TileMapManager tileMapManager = new TileMapManager();

        private bool testSwitch = false;
        private SpriteAnimation _testAnim;
        private SpriteAnimation[] _testAnimArray;
        private Vector2 test = new Vector2(1440, 870);
        private float Timer = 30;

        private Color testColor = Color.White;

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
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            FileManager.Initialize();
            ScreenManager.Initialize(graphics, Window, this);
            SpriteBatchExtensions.Initialize(GraphicsDevice);
            GameData.LoadData(Content, GraphicsDevice);
            tileMapManager.LoadContent(Content);


            inputHelper = new InputManager();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            playerCamera = new Camera2d();
            settingsMenu = new SettingsMenu(playerCamera);
            player = new Player();

            ObjectManager.Initialize(spriteBatch);

            Battery batt1 = new Battery(new Vector2(750, 600));
            Battery batt2 = new Battery(new Vector2(800, 600));
            Battery batt3 = new Battery(new Vector2(850, 600));
            Battery batt4 = new Battery(new Vector2(900, 600));

            _testAnimArray = new SpriteAnimation[2];
            _testAnimArray[0] = new SpriteAnimation(GameData.PlayerFishAtlas, GameData.PlayerFishMap, "FishLeft", 3, 6, false);
            _testAnimArray[1] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerRight", 4, 5);
            _testAnim = _testAnimArray[1];
        }

        protected override void Update(GameTime gameTime)
        {
            float deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            inputHelper.Update(playerCamera);

            Timer -= deltatime * 10f;

            if (inputHelper.IsKeyPress(Keys.C))
            {
                GC.Collect();
            }

            if (!GameData.IsPaused)
            {
                ObjectManager.UpdateObjects();
            }

            player.Update(gameTime, deltatime, inputHelper);



            tileMapManager.Update(gameTime);

            if (inputHelper.IsKeyPress(Keys.F))
            {
                player.state = Player.State.Fishing;
            }

            _testAnim.Position.X = test.X;
            _testAnim.Position.Y = test.Y;
            _testAnim.Update(gameTime);


            settingsMenu.Update(inputHelper, deltatime);

            if (gameTime.IsRunningSlowly)
            {
                testColor = Color.Red;
            }
            else
            {
                testColor = Color.White;
            }


            Window.Title = "ProjectArrow" + " " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB";

            if (GameData.ExitGame)
            {
                this.Exit();
            }

            FPSM.Update();
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
            #region World Draw
            playerCamera.Follow(player.Position);

            ScreenManager.WorldTargetBeginDraw();

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix: playerCamera.get_transformation(GraphicsDevice));

            player.Draw(spriteBatch);
            ObjectManager.DrawObjects();
                
            _testAnim.Draw(spriteBatch, new Vector2(test.X, test.Y), 1f, Color.White);

            tileMapManager.Draw(spriteBatch);


            spriteBatch.End();

            ScreenManager.EndTargetDraws(spriteBatch);

            #endregion

            #region UI Draw

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);


            settingsMenu.Draw(spriteBatch);

            if (GameData.IsDebug)
            {
                spriteBatch.DrawString(GameData.GameFont, "CameraPos: " + (playerCamera._pos.X).ToString() + " " + (playerCamera._pos.Y).ToString(), new Vector2(10, (int)1 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                spriteBatch.DrawString(GameData.GameFont, "PlayerPos: " + (player.Position.X).ToString() + " " + (player.Position.Y).ToString(), new Vector2(10, (int)20 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                spriteBatch.DrawString(GameData.GameFont, "WorldPos: " + ((int)inputHelper.WorldMousePosition.X).ToString() + " " + ((int)inputHelper.WorldMousePosition.Y).ToString(), new Vector2(10, (int)40 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                spriteBatch.DrawString(GameData.GameFont, "ScreenPos: " + ((int)inputHelper.ScreenMousePosition.X).ToString() + " " + ((int)inputHelper.ScreenMousePosition.Y).ToString(), new Vector2(10, (int)60 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
                FPSM.Draw(spriteBatch, GameData.GameFont, new Vector2(10, (int)80 * (GameData.UIScale)), Color.White, 0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, .4f);
            }

            spriteBatch.End();
            
            #endregion

            base.Draw(gameTime);

        }
    }    
}