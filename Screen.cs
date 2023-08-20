using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDelta
{
    internal class Screen
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int ViewWidth { get; private set; }
        public int ViewHeight { get; private set; }

        private bool _isResizing;

        //  Screen scale matrix
        public Matrix ScreenScaleMatrix { get; private set; }

        //  Screen Viewport
        public Viewport Viewport { get; private set; }

        //  View padding, amount to apply for letter/pillar boxing
        private int _viewPadding;

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

        private GraphicsDeviceManager _graphics;
        private GraphicsDevice GraphicsDevice;
        private GameWindow Window;

        public Screen(GraphicsDeviceManager _graphics, GraphicsDevice GraphicsDevice, GameWindow Window)
        {
            this._graphics = _graphics;
            this.Window = Window;
            this.GraphicsDevice = GraphicsDevice;

            Window.AllowUserResizing = true;

            Width = 320;
            Height = 180;

            _graphics.DeviceCreated += OnGraphicsDeviceCreated;
            _graphics.DeviceReset += OnGraphicsDeviceReset;
            Window.ClientSizeChanged += OnWindowSizeChanged;

            //  Applying my graphics settings, we'll start with a 1280x720 window (this is 2x our width and height resolution)
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
            if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0 && !_isResizing)
            {
                _isResizing = true;
                //  Set the backbuffer width and height to the window bounds
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                //  Now update the view
                UpdateView();
                _isResizing = false;    
            }
        }

        public void SetFullscreen()
        {
            _isResizing = true;
            ViewPadding = _viewPadding;
            _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            Console.WriteLine("FULLSCREEN");
            _isResizing = false;
        }

        public void SetWindowed(int width, int height)
        {
            if (width > 0 && height > 0)
            {
                _isResizing = true;
                _graphics.PreferredBackBufferWidth = width;
                _graphics.PreferredBackBufferHeight = height;
                _graphics.IsFullScreen = false;
                _graphics.ApplyChanges();
                Console.WriteLine("WINDOW-" + width + "x" + height);
                _isResizing = false;
            }
        }

        public Vector2 ConvertScreenToVirtualResolution(Vector2 mousePosition)
        {
            // Calculate the position within the viewport
            Vector2 viewportPosition = new Vector2(
                (mousePosition.X - Viewport.X) / Viewport.Width,
                (mousePosition.Y - Viewport.Y) / Viewport.Height
            );

            // Convert to the virtual resolution coords
            Vector2 virtualResolutionPosition = new Vector2(
                viewportPosition.X * Width,
                viewportPosition.Y * Height
            );

            return virtualResolutionPosition;
        }

        private void UpdateView()
        {
            float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // get View Size
            if (screenWidth / Width > screenHeight / Height)
            {
                ViewWidth = (int)(screenHeight / Height * Width);
                ViewHeight = (int)screenHeight;
            }
            else
            {
                ViewWidth = (int)screenWidth;
                ViewHeight = (int)(screenWidth / Width * Height);
            }

            // apply View Padding
            var aspect = ViewHeight / (float)ViewWidth;
            ViewWidth -= ViewPadding * 2;
            ViewHeight -= (int)(aspect * ViewPadding * 2);

            // update screen matrix
            var xscale = ViewWidth / (float)Width;
            var yscale = ViewHeight / (float)Height;

            ScreenScaleMatrix = Matrix.CreateScale(xscale, yscale, 1);

            // update viewport
            Viewport = new Viewport
            {
                X = (int)(screenWidth / 2f - ViewWidth / 2),
                Y = (int)(screenHeight / 2f - ViewHeight / 2),
                Width = ViewWidth,
                Height = ViewHeight,
                MinDepth = 0f,
                MaxDepth = 1f,
            };
        }
    }
}
