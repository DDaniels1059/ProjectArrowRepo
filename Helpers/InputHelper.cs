using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDelta.Helpers
{
    public class InputHelper
    {
        private KeyboardState currentKeyboardState = new();
        private MouseState currentMouseState = new();
        private KeyboardState lastKeyboardState = new();
        private MouseState lastMouseState = new();
        private Vector2 cursorPos = new(0, 0);

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

        public void Update(Screen _screen, Basic2DCamera _camera)
        {
            lastKeyboardState = currentKeyboardState;
            lastMouseState = currentMouseState;

            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            //track cursor position
            cursorPos.X = currentMouseState.X;
            cursorPos.Y = currentMouseState.Y;

            // Transform Mouse Position to Our Virtual 320x180 Resolution
            virtualMousePosition = _screen.ConvertScreenToVirtualResolution(cursorPos);
            // Transform Mouse Position to World Space
            worldMousePosition = _camera.ScreenToCamera(VirtualMousePosition);
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
