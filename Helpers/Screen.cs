using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace ProjectArrow.Helpers
{
    public class Screen
    {
        public static bool IsResizing { get; private set; }
        public Point VirtualResolution { get; private set; }
        public int ViewWidth { get; private set; }
        public int ViewHeight { get; private set; }
        public bool IsFullscreen { get; private set; }

        public Viewport Viewport { get; private set; }

        private GraphicsDeviceManager _graphics;
        private GraphicsDevice _graphicsDevice;
        private GameWindow _window;

        private RenderTarget2D _renderTarget;
        private Rectangle _renderTargetDestination;

        public Screen(GraphicsDeviceManager _graphics, GraphicsDevice GraphicsDevice, GameWindow Window, Point VirtualResolution)
        {
            IsFullscreen = false;

            this._graphics = _graphics;
            this._window = Window;
            this._graphicsDevice = GraphicsDevice;
            this.VirtualResolution = VirtualResolution;

            _graphics.DeviceCreated += OnGraphicsDeviceCreated;
            _graphics.DeviceReset += OnGraphicsDeviceReset;
            Window.ClientSizeChanged += OnWindowSizeChanged;
            Window.AllowUserResizing = true;


            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            _renderTarget = new RenderTarget2D(_graphicsDevice, VirtualResolution.X, VirtualResolution.Y);
            UpdateView();
        }

        private void OnGraphicsDeviceCreated(object sender, EventArgs e)
        {
            //  When graphics device is created, call UpdateView to recalculate the screen scale matrix
            UpdateView();
        }

        private void OnGraphicsDeviceReset(object sender, EventArgs e)
        {
            //  When graphics device is reset, call UpdateView to recalculate the screen scale matrix
            UpdateView();
        }

        private void OnWindowSizeChanged(object sender, EventArgs e)
        {
            if (_window.ClientBounds.Width > 0 && _window.ClientBounds.Height > 0 && !IsResizing)
            {
                IsResizing = true;
                _graphics.PreferredBackBufferWidth = _window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = _window.ClientBounds.Height;
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
            float ScreenWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;

            if (ScreenWidth / VirtualResolution.X > screenHeight / VirtualResolution.Y)
            {
                ViewWidth = (int)(screenHeight / VirtualResolution.Y * VirtualResolution.X);
                ViewHeight = (int)screenHeight;
            }
            else
            {
                ViewWidth = (int)ScreenWidth;
                ViewHeight = (int)(ScreenWidth / VirtualResolution.X * VirtualResolution.Y);
            }

            _renderTargetDestination = new Rectangle
           (
              (int)(ScreenWidth / 2f - (ViewWidth / 2)), 
              (int)(screenHeight / 2f - (ViewHeight / 2)), 
              ViewWidth, 
              ViewHeight
           );

            Viewport = new Viewport(_renderTargetDestination);
        }

        public void TargetBeginDraw()
        {
            _graphicsDevice.SetRenderTarget(_renderTarget);
            _graphicsDevice.Clear(Color.Black);
        }

        public void TargetEndDraw(SpriteBatch _spriteBatch)
        {
            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            _spriteBatch.Draw(_renderTarget, _renderTargetDestination, Color.White);
            _spriteBatch.End();
        }

        private void UpdateView2()
        {
            //float ScreenWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
            //float screenHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;

            //// get View Size
            //if (ScreenWidth / VirtualWidth > screenHeight / VirtualHeight)
            //{
            //    ViewWidth = (int)(screenHeight / VirtualHeight * VirtualWidth);
            //    ViewHeight = (int)screenHeight;
            //}
            //else
            //{
            //    ViewWidth = (int)ScreenWidth;
            //    ViewHeight = (int)(ScreenWidth / VirtualWidth * VirtualHeight);
            //}

            //// apply View Padding
            //var Aspect = ViewHeight / ViewWidth;
            //ViewWidth -= ViewPadding * 2;
            //ViewHeight -= (int)(Aspect * ViewPadding * 2f);

            //ScreenScaleMatrix = Matrix.CreateScale(ViewWidth / (float)VirtualWidth, ViewWidth / (float)VirtualWidth, 1);

            //// update viewport
            //Viewport = new Viewport
            //{
            //    X = (int)(ScreenWidth / 2f - (ViewWidth / 2)),
            //    Y = (int)(screenHeight / 2f - (ViewHeight / 2)),
            //    Width = ViewWidth,
            //    Height = ViewHeight,
            //    MinDepth = 0f,
            //    MaxDepth = 1f,
            //};
        }
    }
}
