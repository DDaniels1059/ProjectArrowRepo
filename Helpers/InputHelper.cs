using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectArrow.Helpers
{
    public class InputHelper
    {
        private KeyboardState currentKeyboardState = new();
        private MouseState currentMouseState = new();
        private KeyboardState lastKeyboardState = new();
        private MouseState lastMouseState = new();
        private Vector2 cursorPos = new(0, 0);

        private int previousScrollWheelValue;
        public int ScrollWheelValue { get; private set; }

        private Vector2 virtualMousePosition;
        public Vector2 VirtualMousePosition
        {
            get { return virtualMousePosition; }
            private set { virtualMousePosition = value; }
        }
        private Vector2 worldMousePosition;
        public Vector2 WorldMousePosition
        {
            get { return worldMousePosition; }
            private set { worldMousePosition = value; }
        }

        public enum MouseButtons { LeftButton, RightButton }

        public void Update(Basic2DCamera _camera, Screen _screen)
        {
            lastKeyboardState = currentKeyboardState;
            lastMouseState = currentMouseState;

            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            //track cursor position
            cursorPos.X = currentMouseState.X;
            cursorPos.Y = currentMouseState.Y;

            previousScrollWheelValue = ScrollWheelValue;
            ScrollWheelValue = (int)(currentMouseState.ScrollWheelValue);

            // Transform Mouse Position to Our Virtual 320x180 Resolution
            virtualMousePosition = ConvertScreenToVirtualResolution(cursorPos, _screen);
            // Transform Mouse Position to World Space
            worldMousePosition = _camera.ScreenToCamera(VirtualMousePosition);
        }

        public bool IsScrollingUp()
        {
            return ScrollWheelValue > previousScrollWheelValue;
        }

        public bool IsScrollingDown()
        {
            return ScrollWheelValue < previousScrollWheelValue;
        }

        public static Vector2 ConvertScreenToVirtualResolution(Vector2 mousePosition, Screen _screen)
        {
            // Calculate the position within the viewport
            Vector2 viewportPosition = new(
               (mousePosition.X - _screen.Viewport.X) / _screen.Viewport.Width,
                (mousePosition.Y - _screen.Viewport.Y) / _screen.Viewport.Height
            );

            //Convert to the virtual resolution coords
            Vector2 virtualResolutionPosition = new(
                viewportPosition.X * _screen.VirtualResolution.X,
                viewportPosition.Y * _screen.VirtualResolution.Y
            );

            return virtualResolutionPosition;
        }

        //check for keyboard key press, hold, and release
        public bool IsKeyPress(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) &&
                lastKeyboardState.IsKeyUp(key);
        }

        public bool IsKeyDown(Keys key)
        { return currentKeyboardState.IsKeyDown(key); }

        public bool IsKeyRelease(Keys key)
        {
            return lastKeyboardState.IsKeyDown(key) &&
                currentKeyboardState.IsKeyUp(key);
        }

        public bool IsMouseButtonPress(MouseButtons button)
        {   //check to see the mouse button was pressed
            if (button == MouseButtons.LeftButton)
            {
                return currentMouseState.LeftButton == ButtonState.Pressed &&
                    lastMouseState.LeftButton == ButtonState.Released;
            }
            else if (button == MouseButtons.RightButton)
            {
                return currentMouseState.RightButton == ButtonState.Pressed &&
                    lastMouseState.RightButton == ButtonState.Released;
            }
            else { return false; }
        }

        public bool IsMouseButtonRelease(MouseButtons button)
        {   //check to see the mouse button was released
            if (button == MouseButtons.LeftButton)
            {
                return lastMouseState.LeftButton == ButtonState.Pressed &&
                    currentMouseState.LeftButton == ButtonState.Released;
            }
            else if (button == MouseButtons.RightButton)
            {
                return lastMouseState.RightButton == ButtonState.Pressed &&
                    currentMouseState.RightButton == ButtonState.Released;
            }
            else { return false; }
        }

        public bool IsMouseButtonDown(MouseButtons button)
        {   //check to see if the mouse button is being held down
            if (button == MouseButtons.LeftButton)
            { return currentMouseState.LeftButton == ButtonState.Pressed; }
            else if (button == MouseButtons.RightButton)
            { return currentMouseState.RightButton == ButtonState.Pressed; }
            else { return false; }
        }
    }
}
