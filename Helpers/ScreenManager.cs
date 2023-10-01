using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace ProjectArrow.Helpers
{
    public static class ScreenManager
    {
        public static GraphicsDeviceManager Graphics;
        public static GameWindow Window;
        public static int VirtualWidth = 640;
        public static int VirtualHeight = 360;
        public static float ScreenWidth;
        public static float ScreenHeight;

        private static float scale;
        private static float viewWidth;
        private static float viewHeight;
        private static bool isResizing;
        private static RenderTarget2D mainRenderTarget;
        private static Rectangle mainTargetDestination;

        public static Viewport WorldViewport { get; private set; }

        public static Viewport UiViewport { get; private set; }
        private static RenderTarget2D _uiRenderTarget;
        public static Rectangle _uiRenderTargetDestination;
        public static void Initialize()
        {
            mainRenderTarget = new RenderTarget2D(Graphics.GraphicsDevice, VirtualWidth, VirtualHeight);
            _uiRenderTarget = new RenderTarget2D(Graphics.GraphicsDevice, VirtualWidth, VirtualHeight);

            Graphics.DeviceCreated += OnGraphicsDeviceCreated;
            Graphics.DeviceReset += OnGraphicsDeviceReset;
            Window.ClientSizeChanged += OnWindowSizeChanged;
            Window.AllowUserResizing = true;

            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges();

            UpdateView();
        }

        private static void OnGraphicsDeviceCreated(object sender, EventArgs e)
        {
            UpdateView();
        }

        private static void OnGraphicsDeviceReset(object sender, EventArgs e)
        {
            UpdateView();
        }

        private static void OnWindowSizeChanged(object sender, EventArgs e)
        {
            if (!isResizing)
            {
                isResizing = true;
                ScreenWidth = /*Math.Max(1280, */Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth/*)*/;
                ScreenHeight = /*Math.Max(720, */Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight/*)*/;
                Graphics.PreferredBackBufferWidth = (int)ScreenWidth;
                Graphics.PreferredBackBufferHeight = (int)ScreenHeight;
                Graphics.ApplyChanges();
                UpdateView();
                isResizing = false;
            }
        }

        public static void SetFullscreen()
        {
            Graphics.PreferredBackBufferWidth = Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            Graphics.HardwareModeSwitch = true;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
        }

        public static void SetBorderless()
        {
            Graphics.PreferredBackBufferWidth = Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = Graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            Graphics.HardwareModeSwitch = false;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
        }

        public static void SetWindowed(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                Graphics.IsFullScreen = false;
                Graphics.PreferredBackBufferWidth = width;
                Graphics.PreferredBackBufferHeight = height;
                Graphics.IsFullScreen = false;
                Graphics.ApplyChanges();
            }
        }

        public static void UpdateView()
        {
            ScreenWidth = Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            ScreenHeight = Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            float scaleY = ScreenHeight / VirtualHeight;
            float scaleX = ScreenWidth / VirtualWidth;
            scale = Math.Min(scaleX, scaleY);

            // Calculate the scaled dimensions
            viewWidth = VirtualWidth * (int)Math.Ceiling(scale);
            viewHeight = VirtualHeight * (int)Math.Ceiling(scale);

            // Set the destination rectangle for rendering
            mainTargetDestination = new Rectangle(
                (int)((ScreenWidth - viewWidth) / 2f), // Center horizontally
                (int)((ScreenHeight - viewHeight) / 2f), // Center vertically
                (int)viewWidth,
                (int)viewHeight
            );

            WorldViewport = new Viewport(mainTargetDestination);

            _uiRenderTargetDestination = new Rectangle(
                (int)((ScreenWidth - viewWidth) / 2f), // Center horizontally
                (int)0, // Center vertically
                (int)viewWidth,
                (int)viewHeight
            );

            UiViewport = new Viewport(_uiRenderTargetDestination);
        }

        public static void WorldTargetBeginDraw()
        {
            Graphics.GraphicsDevice.SetRenderTarget(mainRenderTarget);
            Graphics.GraphicsDevice.Clear(Color.Black);
        }

        public static void UITargetBeginDraw()
        {
            Graphics.GraphicsDevice.SetRenderTarget(_uiRenderTarget);
            Graphics.GraphicsDevice.Clear(Color.Transparent);
        }

        public static void EndTargetDraws(SpriteBatch _spriteBatch)
        {
            Graphics.GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            _spriteBatch.Draw(mainRenderTarget, mainTargetDestination, Color.White);
            _spriteBatch.Draw(_uiRenderTarget, _uiRenderTargetDestination, Color.White);
            _spriteBatch.End();
        }
    }
}
