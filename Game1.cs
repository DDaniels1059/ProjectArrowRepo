using MGResolutionExample;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectDelta
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private int speed = 100;
        private KeyboardState kStateOld = Keyboard.GetState();
        private MouseState mStateOld = Mouse.GetState();
        public SpriteFont _gameFont;


        private Screen screen;
        private Vector2 VirtualMousePosition;
        private Vector2 WorldMousePosition;


        //  Just a rectangle to represent a flat surface, or floor in our world
        private Rectangle _screenRect;
        private Rectangle _playerRect;
        private Rectangle _npcRect;
        private Rectangle _buttonRect;

        //  A 1x1 pixel that will be used to draw the screen and player texture.
        private Texture2D _pixel;
        private Texture2D _player;
        private float rotation = 0.01f;

        //  The camera
        private Basic2DCamera _camera;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            screen = new Screen(_graphics, GraphicsDevice, Window);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _gameFont = Content.Load<SpriteFont>("Misc/gameFont");
            _player = Content.Load<Texture2D>("Misc/Player");

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData<Color>(new Color[] { Color.White });

            _screenRect = new Rectangle(0, 0, screen.Width, screen.Height);

            //  Setting the player to a 32x32 sprite, but setting the position to be in the center of the screen rect
            //  which is why width and height are halved and then 16 (half the player size) subtracted
            _playerRect = new Rectangle((screen.Width / 2) - 8, (screen.Height / 2) - 8, 16, 16);


            _npcRect = new Rectangle((screen.Width / 2), (screen.Height / 2), 16, 16);


            //  Create camera
            _camera = new(screen.Width, screen.Height);
            _camera.Zoom *= new Vector2(1f);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState kState = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            // Convert the screen mouse position to virtual resolution *For HUD*
            Vector2 MousePosition = new Vector2(mState.X, mState.Y);
            VirtualMousePosition = screen.ConvertScreenToVirtualResolution(MousePosition);

            // Transform mouse position to camera space
            WorldMousePosition = _camera.ScreenToCamera(VirtualMousePosition);

            // Temporary Button For Testing Zoom and VirtualMousePosition
            _buttonRect = new Rectangle(screen.Width - 22, 5, 16, 16);


            //  Move player up/down/left/right
            if (kState.IsKeyDown(Keys.A))
            {
                _playerRect.X -= (int)(speed * deltatime);
            }
            else if (kState.IsKeyDown(Keys.D))
            {
                _playerRect.X += (int)(speed * deltatime);
            }
            else if (kState.IsKeyDown(Keys.W))
            {
                _playerRect.Y -= (int)(speed * deltatime);
            }
            else if (kState.IsKeyDown(Keys.S))
            {
                _playerRect.Y += (int)(speed * deltatime);
            }

            if (kState.IsKeyDown(Keys.D1) && kStateOld.IsKeyUp(Keys.D1))
            {
                screen.SetFullscreen();
            }
            if (kState.IsKeyDown(Keys.D2) && kStateOld.IsKeyUp(Keys.D2))
            {
                screen.SetWindowed(screen.Width * 2, screen.Height * 2);
            }
            if (kState.IsKeyDown(Keys.D4) && kStateOld.IsKeyUp(Keys.D4))
            {
                screen.SetWindowed(screen.Width * 4, screen.Height * 4);
            }
            if (kState.IsKeyDown(Keys.D6) && kStateOld.IsKeyUp(Keys.D6))
            {
                screen.SetWindowed(screen.Width * 6, screen.Height * 6);
            }
            if (kState.IsKeyDown(Keys.D8) && kStateOld.IsKeyUp(Keys.D8))
            {
                screen.SetWindowed(screen.Width * 8, screen.Height * 8);
            }
            if (kState.IsKeyDown(Keys.D0) && kStateOld.IsKeyUp(Keys.D0))
            {
                screen.SetWindowed(screen.Width * 10, screen.Height * 10);
            }
            // ToDo Scale *Works*? but gets reset by the ClientSizeChanged when the screen is Maximized..

            if (kState.IsKeyDown(Keys.Right) && kStateOld.IsKeyUp(Keys.Right))
            {
                screen.ViewPadding += 10;
            }

            if (kState.IsKeyDown(Keys.Left) && kStateOld.IsKeyUp(Keys.Left))
            {
                screen.ViewPadding -= 10;
            }


            if (_buttonRect.Contains(VirtualMousePosition))
            {
                if (mState.LeftButton == ButtonState.Pressed && mStateOld.LeftButton == ButtonState.Released)
                {
                    _camera.Zoom /= new Vector2(2f);
                }

                if (mState.RightButton == ButtonState.Pressed && mStateOld.RightButton == ButtonState.Released)
                {
                    _camera.Zoom *= new Vector2(2f);
                }
            }


            //  Ensure camera is centered on player
            //_camera.Rotation += 0.01f;
            _camera.Position = _playerRect.Location.ToVector2() + (new Vector2(_playerRect.Size.X, _playerRect.Size.Y) * 0.5f);
            _camera.CenterOrigin();

            rotation += 0.01f;

            mStateOld = mState;
            kStateOld = kState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Viewport = screen.Viewport;


            GraphicsDevice.Clear(Color.Red);

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix: _camera.TransformationMatrix * screen.ScreenScaleMatrix);      

            _spriteBatch.Draw(_pixel, _screenRect, null, Color.Orange, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            //Dont Normally Do Calculations in Draw, this is just for testing.
            Vector2 origin1 = new Vector2 (_playerRect.X - 8, _playerRect.Y - 8);
            float depth1 = origin1.Y / screen.Viewport.Width;
            depth1 = depth1 * 0.01f;

            Vector2 origin2 = new Vector2(_npcRect.X - 8, _npcRect.Y - 14);
            float depth2 = origin2.Y / screen.Viewport.Width;
            depth2 = depth2 * 0.01f;

            _spriteBatch.Draw(_player, _playerRect, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth1);
            _spriteBatch.Draw(_pixel, _npcRect, null, Color.Green, rotation, new Vector2(0.5f, 0.5f), SpriteEffects.None, depth2);

            _spriteBatch.End();

            // Draw UI elements (buttons and text)
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix: screen.ScreenScaleMatrix);
            _spriteBatch.Draw(_pixel, _buttonRect, null, Color.Gray);


            // Any SpriteBatch calls (those above as well) will not show up Visible unless Winow Size is Equal to or below the Virtual Resolution, 320x180 unless
            // the below criteria are met. (Removing The Screen Scale Matrix seems to fix this, though not really a fix since it takes out the whole point of this Test Space)

            // With a virtual resolution of 320x180 and My Physical Display being 1920x1080, the largest LayerDepth that can be used until the Batch Call goes invisible is .170f (Unless in Fullscreen *PRESS D1*, then it is around .160f)
            // If the virtual resolution is lower, that will directly cause the LayerDepth needing to be lowered

            // I tested with a 2k monitor, and at a Virtual resolution of 320x180 the LayerDepth at .170f is too high, and the SpriteBatch call will go 
            // invisible once the backbuffer width / height goes above the known working res of 1920x1080. I assume this has to do with the viewport, so I am trying to look into it now.
            // It is odd though, since you would think layer depth would be seperate from this since in the viewport you set it to 0-1
            // I have tried making the Layer max in the viewport Higher, to no success.


            // FIX: When Creating the matrix scale in the screen class, we need to use the 3 overloads, x scale, y scale, and z scale. x&y will just be what we already use, but we want Z index to stay the same
            // so it doesnt affect the scaling. So put that to 1.

            _spriteBatch.DrawString(_gameFont, "WorldMousePos:" + ((int)WorldMousePosition.X).ToString() + " " + ((int)WorldMousePosition.Y).ToString(), new Vector2((int)5, (int)5), Color.White, 0f, Vector2.Zero, .02f, SpriteEffects.None, 1f);
            _spriteBatch.DrawString(_gameFont, "VirtualMousePos:" + ((int)VirtualMousePosition.X).ToString() + " " + ((int)VirtualMousePosition.Y).ToString(), new Vector2((int)5, (int)20), Color.White, 0f, Vector2.Zero, .02f, SpriteEffects.None, 1f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}