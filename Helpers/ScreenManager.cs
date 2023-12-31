﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace ProjectArrow.Helpers
{
    public static class ScreenManager
    {
        public static GraphicsDeviceManager Graphics;
        public static GameWindow Window;
        public static Game Game;
        public static int VirtualWidth = 640;
        public static int VirtualHeight = 360;
        public static float ScreenWidth;
        public static float ScreenHeight;

        private static float viewWidth;
        private static float viewHeight;
        private static bool isResizing;

        public static Viewport WorldViewport;
        private static RenderTarget2D worldRenderTarget;
        private static Rectangle worldDestination;
        private static float scale;

        private static Color backGround;

        public static void Initialize(GraphicsDeviceManager graphics, GameWindow window, Game game)
        {
            Graphics = graphics;
            Window = window;
            Game = game;

            backGround = new Color(91, 110, 225);
            worldRenderTarget = new RenderTarget2D(Graphics.GraphicsDevice, VirtualWidth, VirtualHeight, false, Graphics.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.None);

            Graphics.DeviceCreated += OnGraphicsDeviceCreated;
            Graphics.DeviceReset += OnGraphicsDeviceReset;
            Window.ClientSizeChanged += OnWindowSizeChanged;
            Window.AllowUserResizing = true;


            Graphics.PreferredBackBufferWidth = 1280;   
            Graphics.PreferredBackBufferHeight = 720;
            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges();
        }

        private static void OnGraphicsDeviceCreated(object sender, EventArgs e)
        {
            worldRenderTarget.Dispose();
            worldRenderTarget = new RenderTarget2D(Graphics.GraphicsDevice, VirtualWidth, VirtualHeight);
            UpdateView();
        }

        private static void OnGraphicsDeviceReset(object sender, EventArgs e)
        {
            worldRenderTarget.Dispose();
            worldRenderTarget = new RenderTarget2D(Graphics.GraphicsDevice, VirtualWidth, VirtualHeight);
            UpdateView();
        }

        private static void OnWindowSizeChanged(object sender, EventArgs e)
        {
            if (!isResizing)
            {
                isResizing = true;
                ScreenWidth = Math.Max(640, Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth);
                ScreenHeight = Math.Max(360, Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight);
                Graphics.PreferredBackBufferWidth = (int)ScreenWidth;
                Graphics.PreferredBackBufferHeight = (int)ScreenHeight;
                Graphics.ApplyChanges();
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

            // Calculate the scale factors needed to fit the virtual resolution into the screen
            float scaleX = ScreenWidth / VirtualWidth;
            float scaleY = ScreenHeight / VirtualHeight;

            scale = Math.Max((int)Math.Ceiling(scaleX), (int)Math.Ceiling(scaleY));


            // Update the view dimensions based on the calculated scale
            viewWidth = VirtualWidth * scale;
            viewHeight = VirtualHeight * scale;

            // Set the destination rectangle for rendering
            worldDestination = new Rectangle(
                (int)((ScreenWidth - viewWidth) / 2f), // Center horizontally
                (int)((ScreenHeight - viewHeight) / 2f), // Center vertically
                (int)viewWidth,
                (int)viewHeight
            );

            WorldViewport = new Viewport(worldDestination);
        }

        public static void WorldTargetBeginDraw()
        {
            Graphics.GraphicsDevice.SetRenderTarget(worldRenderTarget);
            Graphics.GraphicsDevice.Clear(backGround);
        }

        public static void EndTargetDraws(SpriteBatch _spriteBatch)
        {
            Graphics.GraphicsDevice.SetRenderTarget(null);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, null);
            _spriteBatch.Draw(worldRenderTarget, worldDestination, Color.White);
            _spriteBatch.End();
        }
    }
}
