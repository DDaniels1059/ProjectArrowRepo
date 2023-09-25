using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using static System.Formats.Asn1.AsnWriter;
using System.Linq;

namespace ProjectArrow.Helpers
{
    public class Screen
    {
        public static bool IsResizing { get; private set; }
        public int GameWidth { get; private set; }
        public int GameHeight { get; private set; }
        public float ViewWidth { get; private set; }
        public float ViewHeight { get; private set; }
        public bool IsFullscreen { get; private set; }
        public Viewport Viewport { get; private set; }

        private GraphicsDeviceManager _graphics;
        private GraphicsDevice _graphicsDevice;
        private GameWindow _window;

        private RenderTarget2D _mainRenderTarget;
        public Rectangle _renderTargetDestination;
        public float scale { get; private set; }
        private bool isScalingAllowed;

        public Screen(GraphicsDeviceManager _graphics, GraphicsDevice GraphicsDevice, GameWindow Window, int GameWidth, int GameHeight)
        {
            IsFullscreen = false;

            this._graphics = _graphics;
            this._window = Window;
            this._graphicsDevice = GraphicsDevice;
            this.GameWidth = GameWidth;
            this.GameHeight = GameHeight;

            isScalingAllowed = true;

            _graphics.DeviceCreated += OnGraphicsDeviceCreated;
            _graphics.DeviceReset += OnGraphicsDeviceReset;
            Window.ClientSizeChanged += OnWindowSizeChanged;
            Window.AllowUserResizing = false;

            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            _mainRenderTarget = new RenderTarget2D(_graphicsDevice, GameWidth, GameHeight);

            UpdateView();
        }

        private void OnGraphicsDeviceCreated(object sender, EventArgs e)
        {
            //  When graphics device is created, call UpdateView
            UpdateView();
        }

        private void OnGraphicsDeviceReset(object sender, EventArgs e)
        {
            //  When graphics device is reset, call UpdateView
            UpdateView();
        }

        private void OnWindowSizeChanged(object sender, EventArgs e)
        {
            if (!IsResizing)
            {
                IsResizing = true;
                _graphics.PreferredBackBufferWidth = _window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = _window.ClientBounds.Height;
                _graphics.ApplyChanges();
                UpdateView();
                IsResizing = false;
            }
        }

        public void SetFullscreen()
        {
            IsFullscreen = true;
            _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            _graphics.HardwareModeSwitch = true;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

        public void SetBorderless()
        {
            IsFullscreen = true;
            _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.HardwareModeSwitch = false;
            _graphics.ApplyChanges();
        }

        public void SetWindowed(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                IsFullscreen = false;
                _graphics.PreferredBackBufferWidth = width;
                _graphics.PreferredBackBufferHeight = height;
                _graphics.IsFullScreen = false;
                _graphics.ApplyChanges();
            }
        }

        public void UpdateView()
        {
            float screenWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;

            float scaleY = screenHeight / GameHeight;
            float scaleX = screenWidth / GameWidth;
            scale = Math.Min(scaleX, scaleY);


            //Calculate the size of the scaled render target
            //Without Floor Or Ceiling, There Will Be Multiple Pixels Copied and Scale Will Look Bad On UI As Well.
            //Any combination of Round, Ceiling, or Max (Math.Max for the scale ^) will cause the Render Target to go outside the view area, and impede UI
            float scaledWidth = GameWidth * (float)Math.Floor(scale);
            float scaledHeight = GameHeight * (float)Math.Floor(scale);

            // Calculate the position to center the render target
            float offsetX = (screenWidth - scaledWidth) / 2f;
            float offsetY = (screenHeight - scaledHeight) / 2f;

            // Set the destination rectangle for rendering
            _renderTargetDestination = new Rectangle(
                (int)offsetX,
                (int)offsetY,
                (int)scaledWidth,
                (int)scaledHeight
            );

            Viewport = new Viewport(_renderTargetDestination);
        }

        public void TargetBeginDraw()
        {
            _graphicsDevice.SetRenderTarget(_mainRenderTarget);
            _graphicsDevice.Clear(Color.Black);
        }

        public void TargetEndDraw(SpriteBatch _spriteBatch)
        {
            _graphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            _spriteBatch.Draw(_mainRenderTarget, _renderTargetDestination, Color.White);
            _spriteBatch.End();
        }      
    }
}
