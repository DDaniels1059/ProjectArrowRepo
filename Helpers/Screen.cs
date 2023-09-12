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

        public static bool _isResizing;

        public bool _isFullscreen { get; private set; }


        //  Screen scale matrix
        public Matrix ScreenScaleMatrix { get; private set; }

        //  Screen Viewport
        public Viewport Viewport { get; private set; }

        private GraphicsDeviceManager _graphics;
        private GraphicsDevice GraphicsDevice;
        private GameWindow Window;

        public Screen(GraphicsDeviceManager _graphics, GraphicsDevice GraphicsDevice, GameWindow Window)
        {
            _isFullscreen = false;

            this._graphics = _graphics;
            this.Window = Window;
            this.GraphicsDevice = GraphicsDevice;

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
            if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0 && !_isResizing)
            {
                _isResizing = true;
                _viewPadding = 0;
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                UpdateView();
                _isResizing = false;
            }
        }

        public void SetFullscreen()
        {
            _isFullscreen = true;
            //_isResizing = true;
            ViewPadding = _viewPadding;
            _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            Console.WriteLine("FULLSCREEN");
            //_isResizing = false;
        }


        public void SetWindowed()
        {
            _isFullscreen = false;
            //_isResizing = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
            //_isResizing = false;
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
                viewportPosition.X * VirtualWidth,
                viewportPosition.Y * VirtualHeight
            );

            return virtualResolutionPosition;
        }

        private void UpdateView()
        {
            float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // get View Size
            if (screenWidth / VirtualWidth > screenHeight / VirtualHeight)
            {
                ViewWidth = (int)(screenHeight / VirtualHeight * VirtualWidth);
                ViewHeight = (int)screenHeight;
            }
            else
            {
                ViewWidth = (int)screenWidth;
                ViewHeight = (int)(screenWidth / VirtualWidth * VirtualHeight);
            }

            if (_isFullscreen)
            {
                // apply View Padding
                var aspect = ViewHeight / (float)ViewWidth;
                ViewWidth -= ViewPadding * 2;
                ViewHeight -= (int)(aspect * ViewPadding * 2);
            }

            // update screen matrix
            var xscale = ViewWidth / (float)VirtualWidth;
            var yscale = ViewHeight / (float)VirtualHeight;

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
