using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;

namespace ProjectDelta.Helpers
{
    public class Screen
    {
        public static bool IsResizing;
        public int VirtualWidth { get; private set; }
        public int VirtualHeight { get; private set; }
        public int ViewWidth { get; private set; }
        public int ViewHeight { get; private set; }

        private int _viewPadding;  //  View padding, amount to apply for letter/pillar boxing

        public int ViewPadding
        {
            get => _viewPadding;
            set
            {
                //  Only perform view update if the value is changed
                if (_viewPadding != value)
                {
                    _viewPadding = value;
                    UpdateView();
                }
            }
        }


        public bool IsFullscreen { get; private set; }


        //  Screen scale matrix
        public Matrix ScreenScaleMatrix { get; private set; }

        //  Screen Viewport
        public Viewport Viewport { get; private set; }

        private GraphicsDeviceManager _graphics;
        private GraphicsDevice _graphicsDevice;
        private GameWindow _window;

        public Screen(GraphicsDeviceManager _graphics, GraphicsDevice GraphicsDevice, GameWindow Window)
        {
            IsFullscreen = false;

            this._graphics = _graphics;
            this._window = Window;
            this._graphicsDevice = GraphicsDevice;

            Window.AllowUserResizing = true;

            VirtualWidth = 320;
            VirtualHeight = 180;

            _graphics.DeviceCreated += OnGraphicsDeviceCreated;
            _graphics.DeviceReset += OnGraphicsDeviceReset;
            Window.ClientSizeChanged += OnWindowSizeChanged;


            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();


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
                _viewPadding = 0;
                _graphics.PreferredBackBufferWidth = _window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = _window.ClientBounds.Height;
                UpdateView();
                IsResizing = false;
            }
        }

        public void SetFullscreen()
        {
            IsFullscreen = true;
            IsResizing = true;
            ViewPadding = _viewPadding;
            _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            _graphics.HardwareModeSwitch = true;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            Console.WriteLine("FULLSCREEN");
            IsResizing = false;
        }

        public void SetBorderless()
        {
            IsFullscreen = true;
            IsResizing = true;
            ViewPadding = _viewPadding;
            _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.HardwareModeSwitch = false;
            _graphics.ApplyChanges();
            Console.WriteLine("FULLSCREEN");
            IsResizing = false;
        }

        public void SetWindowed(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                IsFullscreen = false;
                IsResizing = true;
                _graphics.PreferredBackBufferWidth = width;
                _graphics.PreferredBackBufferHeight = height;
                _graphics.IsFullScreen = false;
                _graphics.ApplyChanges();
                Console.WriteLine("WINDOW-" + width + "x" + height);
                IsResizing = false;
            }
        }

        private void UpdateView()
        {
            float ScreenWidth = _graphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = _graphicsDevice.PresentationParameters.BackBufferHeight;

            // get View Size
            if (ScreenWidth / (float)VirtualWidth > screenHeight / (float)VirtualHeight)
            {
                ViewWidth = (int)(screenHeight / (float)VirtualHeight * (float)VirtualWidth);
                ViewHeight = (int)screenHeight;
            }
            else
            {
                ViewWidth = (int)ScreenWidth;
                ViewHeight = (int)(ScreenWidth / (float)VirtualWidth * (float)VirtualHeight);
            }

            // apply View Padding
            float Aspect = (float)ViewHeight / (float)ViewWidth;
            ViewWidth -= ViewPadding * 2;
            ViewHeight -= (int)(Aspect * (float)ViewPadding * 2f);


            // update screen matrix
            var XScale = (float)ViewWidth / (float)VirtualWidth;
            var YScale = (float)ViewHeight / (float)VirtualHeight;

            ScreenScaleMatrix = Matrix.CreateScale((float)XScale, (float)YScale, 1);

            // update viewport
            Viewport = new Viewport
            {
                X = (int)(ScreenWidth / 2f - (float)(ViewWidth / 2)),
                Y = (int)(screenHeight / 2f - (float)(ViewHeight / 2)),
                Width = ViewWidth,
                Height = ViewHeight,
                MinDepth = 0f,
                MaxDepth = 1f,
            };
        }
    }
}
