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
        private Rectangle defaultSprite;
        private Rectangle pressedSprite;
        public Rectangle bounds;
        public bool canDraw = false;
        private bool isPressed = false;
        private bool isToggle = false;
        private bool isAnchoredRight = false;
        private bool isAnchoredBottom = false;
        private bool toggled = false;
        private float timer = 100;

        public delegate void onPress();
        public onPress buttonPress;
        private Color debugColor = Color.Red;

        private int scaledWidth;
        private int scaledHeight;
        private int SideAnchorOffset;
        private int VerticalAnchorOffset;
        private int buttonSetCount; 

        public Vector2 location;
        private Vector2 offset;
        private SpriteEffects Flip = SpriteEffects.None;

        //Default Press Action
        private void DefaultPress()
        {
            Debug.WriteLine("Button Pressed: ADD New Function");

            if (isToggle)
            {
                if (toggled == true)
                {
                    toggled = false;
                }
                else
                {
                    toggled = true;
                }
            }
        }

        public Button(Rectangle defaultSprite, Rectangle pressedSprite, bool isToggle, bool isFlippedHorizontal, bool isAnchoredRight, bool isAnchoredBottom, int buttonSetCount = 1)
        {
            this.isToggle = isToggle;
            this.defaultSprite = defaultSprite;
            this.pressedSprite = pressedSprite;
            this.isAnchoredRight = isAnchoredRight;
            this.isAnchoredBottom = isAnchoredBottom;
            this.buttonSetCount = buttonSetCount; 

            buttonPress = DefaultPress;
            GameData.ButtonList.Add(this);

            if (isFlippedHorizontal)
            {
                Flip = SpriteEffects.FlipHorizontally;
            }
        }

        public bool GetToggleState()
        {
            return toggled;
        }

        //Update Each Button In Main Call By Using For Loop
        public void Update(Vector2 VirtualMousePositon, InputHelper InputHelper, float DeltaTime, Vector2 location)
        {
            if (canDraw)
            {
                this.location = location;
                scaledWidth = (int)(defaultSprite.Width * GameData.UIScale);
                scaledHeight = (int)(defaultSprite.Height * GameData.UIScale);
                offset = new Vector2((defaultSprite.Width - scaledWidth), (defaultSprite.Width - scaledWidth) / 2);

                

                if (GameData.UIScale > 1)
                {
                    if (!isAnchoredRight)
                    {
                        SideAnchorOffset = (int)(-5 * GameData.UIScale) * (int)(GameData.UIScale);
                    }
                    else
                    {
                        SideAnchorOffset = (int)(5 * GameData.UIScale) * (int)(GameData.UIScale);
                    }

                    if (!isAnchoredBottom)
                    {
                        VerticalAnchorOffset = 10;
                    }
                    else
                    {
                        VerticalAnchorOffset = (int)((1 * buttonSetCount)) * (int)(10 * GameData.UIScale);
                    }

                    offset = new Vector2(((defaultSprite.Width - scaledWidth) / 2) + SideAnchorOffset, ((defaultSprite.Width - scaledWidth) / 2) + VerticalAnchorOffset);
                }

                bounds = new Rectangle((int)location.X + (int)offset.X, (int)location.Y + (int)offset.Y, scaledWidth, scaledHeight);

                if (bounds.Contains(VirtualMousePositon) && InputHelper.IsMouseButtonPress(MouseButtons.LeftButton))
                {
                    isPressed = true;

                    //If Button press != Null //buttonPress?.Invoke();
                    if (buttonPress != null)
                    {
                        buttonPress.Invoke();
                    }
                }

                if (isPressed)
                {
                    timer -= 400 * DeltaTime;

                    if (timer <= 0)
                    {
                        isPressed = false;
                        timer = 100;
                        Debug.WriteLine("Button Released");
                    }
                }
            }
        }




        //Draw Each Button In Main Call By Using For Loop
        public void Draw(SpriteBatch _spriteBatch)
        {
            if (!isToggle && canDraw)
            {
                if (isPressed)
                {
                    _spriteBatch.Draw(GameData.TextureAtlas, new Rectangle((int)location.X + (int)offset.X, (int)location.Y + (int)offset.Y, scaledWidth, scaledHeight), new Rectangle(pressedSprite.X, pressedSprite.Y, GameData.TileSize, GameData.TileSize), Color.White, 0f, Vector2.Zero, Flip, 1f);
                }
                else
                {
                    _spriteBatch.Draw(GameData.TextureAtlas, new Rectangle((int)location.X + (int)offset.X, (int)location.Y + (int)offset.Y, scaledWidth, scaledHeight), new Rectangle(defaultSprite.X, defaultSprite.Y, GameData.TileSize, GameData.TileSize), Color.White, 0f, Vector2.Zero, Flip, 1f);
                }
            }
            else if (isToggle && canDraw)
            {
                if (toggled)
                {
                    _spriteBatch.Draw(GameData.TextureAtlas, new Rectangle((int)location.X + (int)offset.X, (int)location.Y + (int)offset.Y, scaledWidth, scaledHeight), new Rectangle(pressedSprite.X, pressedSprite.Y, GameData.TileSize, GameData.TileSize), Color.White, 0f, Vector2.Zero, Flip, 1f);
                }
                else
                {
                    _spriteBatch.Draw(GameData.TextureAtlas, new Rectangle((int)location.X + (int)offset.X, (int)location.Y + (int)offset.Y, scaledWidth, scaledHeight), new Rectangle(defaultSprite.X, defaultSprite.Y, GameData.TileSize, GameData.TileSize), Color.White, 0f, Vector2.Zero, Flip, 1f);
                }
            }

            if (GameData.IsDebug && canDraw)
                _spriteBatch.DrawHollowRect(bounds, Color.Red);
        }
    }   
}
