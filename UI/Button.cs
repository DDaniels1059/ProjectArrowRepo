using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectArrow.Helpers;
using ProjectArrow.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static ProjectArrow.Helpers.InputManager;

namespace ProjectArrow.UI
{
    public class Button
    {
        public Rectangle Bounds;
        private Rectangle _defaultSprite;
        private Rectangle _pressedSprite;
        private bool _isPressed = false;
        private bool _isToggle = false;
        private bool _isAnchoredRight = false;
        private bool _toggled = false;
        private float _timer = 100;

        public delegate void onPress();
        public onPress buttonPress;

        private int _scaledWidth;
        private int _scaledHeight;
        private int _sideAnchorOffset;
        private int _verticalOffset;

        public Vector2 position;
        private Vector2 _offset;
        private SpriteEffects _flip;

        //Default Press Action
        //If using TOGGLE button use this function as well when assigning function for button. 
        //Example: DebugToggle.buttonPress += NewDebugPressFunction;
        //Will allow the default toggle function to exist along side the new one. You could just use the below code,
        //but this allows it to be a bit cleaner.
        private void DefaultPress()
        {
            if (_isToggle)
            {
                if (_toggled == true)
                {
                    _toggled = false;
                }
                else
                {
                    _toggled = true;
                }
            }
        }

        public Button(Rectangle defaultSprite, Rectangle pressedSprite, bool isToggle, bool isFlippedHorizontal, bool isAnchoredRight)
        {
            _isToggle = isToggle;
            _defaultSprite = defaultSprite;
            _pressedSprite = pressedSprite;
            _isAnchoredRight = isAnchoredRight;
            if (isFlippedHorizontal)
            {
                _flip = SpriteEffects.FlipHorizontally;
            }
            else
            {
                _flip = SpriteEffects.None;
            }
            buttonPress = DefaultPress;
        }

        // Returns Current Toggle Bool
        public bool GetToggleState()
        {
            return _toggled;
        }

        // Sets Toggle Bool
        public void SetToggleState(bool toggle)
        {
            _toggled = toggle;
        }

        //Update Each Button In Main Call By Using For Loop
        public void Update(Vector2 VirtualMousePositon, InputManager InputHelper, float DeltaTime)
        {
            //This Will Scale The Buttons Evenly By The Current UIScale
            //We Subtract Anything That Is Not Anchored Right By TileSize So It Will Be Offset Correctly
            //
            _scaledWidth = (int)(_defaultSprite.Width * GameData.UIScale);
            _scaledHeight = (int)(_defaultSprite.Height * GameData.UIScale);
            if (!_isAnchoredRight)
            {
                _offset.X = _defaultSprite.Width - _scaledWidth - GameData.UITileSize;
            }
            else
            {
                _offset.X = _defaultSprite.Width - _scaledWidth;
            }

            _offset.Y = -1 * (GameData.UIScale);

            if (GameData.UIScale > 1)
            {
                //This Will Scale The Button To The LEFT OR RIGHT Based On The isAnchoredBool
                //The  2 && -2 Are Just For The Scale Of The Offset. The Larger It Is, The Bigger Gap Between The Button
                //And Whatever It's X position is Tied To, When Scaled.
                if (!_isAnchoredRight)
                {
                    _sideAnchorOffset = (int)(-2 * GameData.UIScale) * (int)GameData.UIScale - GameData.UITileSize;
                }
                else
                {
                    _sideAnchorOffset = (int)(2 * GameData.UIScale) * (int)GameData.UIScale;
                }

                // This Will Scale The Buttons Vertical Position :
                // In This Case Down By An Amount that will Center It To The Text Rectangles
                // Look In Settings Menu For Those Examples, *Please* Forgive The Messy Code
                _verticalOffset = (int)((2 * GameData.UIScale) * (int)GameData.UIScale) - (int)GameData.UIScale * (int)GameData.UIScale + 2;

                _offset.X = (_defaultSprite.Width - _scaledWidth) / 2 + _sideAnchorOffset;
                _offset.Y =  -1 * (GameData.UIScale);
            }


            //This Scales Up The Button Bounds, Used For Input Detection
            Bounds.X = (int)position.X + (int)_offset.X;
            Bounds.Y = (int)position.Y + (int)_offset.Y;
            Bounds.Width = (int)_scaledWidth;
            Bounds.Height = (int)_scaledHeight;               


            if (Bounds.Contains(VirtualMousePositon) && InputHelper.IsMouseButtonPress(MouseButtons.LeftButton))
            {
                _isPressed = true;
                buttonPress?.Invoke();
            }

            if (_isPressed)
            {
                _timer -= 0.5f * DeltaTime;
                if (_timer <= 0)
                {
                    _isPressed = false;
                    _timer = 100;
                    //Button Released
                }
            }
        }

        //Draw Each Button In Main Call By Using For Loop
        public void Draw(SpriteBatch _spriteBatch)
        {
            if (!_isToggle)
            {
                if (_isPressed)
                {
                    _spriteBatch.Draw(GameData.UIAtlas, Bounds, _pressedSprite, Color.White, 0f, Vector2.Zero, _flip, 0.5f);
                }
                else
                {
                    _spriteBatch.Draw(GameData.UIAtlas, Bounds, _defaultSprite, Color.White, 0f, Vector2.Zero, _flip, 0.5f);
                }
            }
            else if (_isToggle)
            {
                if (_toggled)
                {
                    _spriteBatch.Draw(GameData.UIAtlas, Bounds, _pressedSprite, Color.White, 0f, Vector2.Zero, _flip, 0.5f);
                }
                else
                {
                    _spriteBatch.Draw(GameData.UIAtlas, Bounds, _defaultSprite, Color.White, 0f, Vector2.Zero, _flip, 0.5f);
                }
            }

            if (GameData.IsDebug)
                _spriteBatch.DrawHollowRect(Bounds, Color.Red);
        }
    }
}
