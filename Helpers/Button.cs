using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDelta.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static ProjectDelta.Helpers.InputHelper;

namespace ProjectDelta
{
    public class Button
    {
        public Rectangle bounds;
        private Rectangle _defaultSprite;
        private Rectangle _pressedSprite;
        public bool canDraw = false;
        private bool _isPressed = false;
        private bool _isToggle = false;
        private bool _isAnchoredRight = false;
        private bool _isAnchoredBottom = false;
        private bool _toggled = false;
        private float _timer = 100;

        public delegate void onPress();
        public onPress buttonPress;

        private int _scaledWidth;
        private int _scaledHeight;
        private int _sideAnchorOffset;
        private int _verticalAnchorOffset;
        private int _buttonSetCount; 

        public Vector2 location;
        private Vector2 _offset;
        private SpriteEffects _flip;

        //Default Press Action
        private void DefaultPress()
        {
            Debug.WriteLine("Button Pressed: ADD New Function");

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

        public Button(Rectangle defaultSprite, Rectangle pressedSprite, bool isToggle, bool isFlippedHorizontal, bool isAnchoredRight, bool isAnchoredBottom, int buttonSetCount = 1)
        {
            this._isToggle = isToggle;
            this._defaultSprite = defaultSprite;
            this._pressedSprite = pressedSprite;
            this._isAnchoredRight = isAnchoredRight;
            this._isAnchoredBottom = isAnchoredBottom;
            this._buttonSetCount = buttonSetCount;
            if (isFlippedHorizontal)
            {
                _flip = SpriteEffects.FlipHorizontally;
            }
            else
            {
                _flip = SpriteEffects.None;
            }
            buttonPress = DefaultPress;
            GameData.ButtonList.Add(this);
        }

        public bool GetToggleState()
        {
            return _toggled;
        }

        //Update Each Button In Main Call By Using For Loop
        public void Update(Vector2 VirtualMousePositon, InputHelper InputHelper, float DeltaTime)
        {
            if (canDraw)
            {
                _scaledWidth = (int)(_defaultSprite.Width * GameData.UIScale);
                _scaledHeight = (int)(_defaultSprite.Height * GameData.UIScale);
                _offset.X = (int)(_defaultSprite.Width - _scaledWidth);
                _offset.Y = (int)(_defaultSprite.Width - _scaledWidth) / 2;



                if (GameData.UIScale > 1)
                {
                    if (!_isAnchoredRight)
                    {
                        _sideAnchorOffset = (int)(-10 * GameData.UIScale) * (int)(GameData.UIScale);
                    }
                    else
                    {
                        _sideAnchorOffset = (int)(10 * GameData.UIScale) * (int)(GameData.UIScale);
                    }

                    if (!_isAnchoredBottom)
                    {
                        _verticalAnchorOffset = 20;
                    }
                    else
                    {
                        _verticalAnchorOffset = (int)((_buttonSetCount)) * (int)(10 * GameData.UIScale);
                    }

                    _offset.X = (int)((_defaultSprite.Width - _scaledWidth) / 2) + _sideAnchorOffset;
                    _offset.Y = (int)((_defaultSprite.Width - _scaledWidth) / 2) + _verticalAnchorOffset;
                }

                bounds.X = ((int)location.X + (int)_offset.X);
                bounds.Y = ((int)location.Y + (int)_offset.Y);
                bounds.Width = _scaledWidth;
                bounds.Height = _scaledHeight;

                if (bounds.Contains(VirtualMousePositon) && InputHelper.IsMouseButtonPress(MouseButtons.LeftButton))
                {
                    _isPressed = true;
                    buttonPress?.Invoke();
                }

                if (_isPressed)
                {
                    _timer -= 400 * DeltaTime;
                    if (_timer <= 0)
                    {
                        _isPressed = false;
                        _timer = 100;
                        Debug.WriteLine("Button Released");
                    }
                }
            }
        }

        //Draw Each Button In Main Call By Using For Loop
        public void Draw(SpriteBatch _spriteBatch)
        {
            if (!_isToggle && canDraw)
            {
                if (_isPressed)
                {
                    _spriteBatch.Draw(GameData.TextureAtlas, bounds, _pressedSprite, Color.White, 0f, Vector2.Zero, _flip, 1f);
                }
                else
                {
                    _spriteBatch.Draw(GameData.TextureAtlas, bounds, _defaultSprite, Color.White, 0f, Vector2.Zero, _flip, 1f);
                }
            }
            else if (_isToggle && canDraw)
            {
                if (_toggled)
                {
                    _spriteBatch.Draw(GameData.TextureAtlas, bounds, _pressedSprite, Color.White, 0f, Vector2.Zero, _flip, 1f);
                }
                else
                {
                    _spriteBatch.Draw(GameData.TextureAtlas, bounds, _defaultSprite, Color.White, 0f, Vector2.Zero, _flip, 1f);
                }
            }

            if (GameData.IsDebug && canDraw)
                _spriteBatch.DrawHollowRect(bounds, Color.Red);
        }
    }   
}
