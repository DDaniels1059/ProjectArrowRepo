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
        public static float viewWidth;
        public static float viewHeight;
        private static float viewHeightOffset;
        public static float viewWidthOffset;
        private static bool isResizing;

        private static RenderTarget2D worldRenderTarget;
        private static Rectangle worldDestination;
        public static Viewport WorldViewport;

        public static void Initialize()
        {
            worldRenderTarget = new RenderTarget2D(Graphics.GraphicsDevice, VirtualWidth, VirtualHeight);

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
                ScreenWidth = Math.Max(1280, Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth);
                ScreenHeight = Math.Max(720, Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
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
            scale = Math.Max(scaleX, scaleY);
         
            // Calculate the scaled dimensions
            viewWidth = VirtualWidth * (float)Math.Ceiling(scale);
            viewHeight = VirtualHeight * (float)Math.Ceiling(scale);

            // Set the destination rectangle for rendering
            worldDestination = new Rectangle(
                (int)((ScreenWidth - viewWidth) / 2f), // Center horizontally
                (int)((ScreenHeight - viewHeight) / 2f), // Center vertically
                (int)viewWidth,
                (int)viewHeight
            );
            WorldViewport = new Viewport(worldDestination);

        

            //viewHeightOffset = (ScreenHeight - viewHeight) / 2;
            //if (viewHeightOffset < 0)
            //    viewHeightOffset = 0;

            //uiCenterDestination = new Rectangle(
            //    (int)((ScreenWidth - viewWidth) / 2f), // Center horizontally
            //    (int)viewHeightOffset, // Center vertically
            //    (int)viewWidth,
            //    (int)viewHeight
            //);
            //uiViewport = new Viewport(uiCenterDestination);


            //uiCenterDestination = new Rectangle(
            //    (int)0, // Center horizontally
            //    (int)viewHeightOffset, // Center vertically
            //    (int)viewWidth,
            //    (int)viewHeight
            //);
        }

        public static void WorldTargetBeginDraw()
        {
            Graphics.GraphicsDevice.SetRenderTarget(worldRenderTarget);
            Graphics.GraphicsDevice.Clear(Color.Black);
        }

        public static void EndTargetDraws(SpriteBatch _spriteBatch)
        {
            Graphics.GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
            _spriteBatch.Draw(worldRenderTarget, worldDestination, Color.White);
            _spriteBatch.End();
        }
    }
}
