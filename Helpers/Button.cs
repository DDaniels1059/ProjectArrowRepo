using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDelta.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static ProjectDelta.Helpers.InputHelper;

namespace ProjectDelta
{
    public class Button
    {
        private Rectangle defaultSprite;
        private Rectangle pressedSprite;
        public Rectangle bounds;
        private bool isPressed = false;
        private bool isToggle = false;
        private bool toggled = false;
        private float timer = 100;

        public delegate void onPress();
        public onPress buttonPress;
        public Vector2 location;
        private Color debugColor = Color.Red;

        int scaledWidth;
        int scaledHeight;
        Vector2 offset;


        //Default Press Action
        private void DefaultPress()
        {
            Debug.WriteLine("Button Pressed: ADD New Function");

            if(isToggle)
            {
                if(toggled == true)
                {
                    toggled = false;
                }
                else
                {
                    toggled = true;
                }
            }
        }


        //Creates Button, Adds to List, Sets Default Button Press
        //No Manual Width Or Height, Redundant since I want Textures To be a set size
        //I Can Just Scale Them Up From That Size If Needed
        public void CreateButton(Rectangle defaultSprite, Rectangle pressedSprite, bool isToggle)
        {
            this.isToggle = isToggle;
            this.defaultSprite = defaultSprite;
            this.pressedSprite = pressedSprite;
            //this.UIScale = scale;
            buttonPress = DefaultPress;
            GameData.ButtonList.Add(this);
        }

        public bool GetToggleState()
        {
            return toggled;
        }

        //Update Each Button In Main Call By Using For Loop
        public void Update(Vector2 VirtualMousePositon, InputHelper InputHelper, float DeltaTime, Vector2 location)
        {
            this.location = location;

            scaledWidth = (int)(defaultSprite.Width * GameData.UIScale);
            scaledHeight = (int)(defaultSprite.Height * GameData.UIScale);
            offset = new Vector2((defaultSprite.Width - scaledWidth), (int)location.Y);
            bounds = new Rectangle((int)location.X + (int)offset.X, (int)offset.Y, scaledWidth, scaledHeight);

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




        //Draw Each Button In Main Call By Using For Loop
        public void Draw(SpriteBatch _spriteBatch)
        {
            if (!isToggle)
            {
                if (isPressed)
                {
                    //_spriteBatch.Draw(GameData.TextureAtlas, new Vector2((int)location.X, (int)location.Y) + offset, new Rectangle(pressedSprite.X, pressedSprite.Y, scaledWidth, scaledHeight), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
                    _spriteBatch.Draw(GameData.TextureAtlas, new Rectangle((int)location.X + (int)offset.X, (int)offset.Y, scaledWidth, scaledHeight), new Rectangle(pressedSprite.X, pressedSprite.Y, GameData.TileSize, GameData.TileSize), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                }
                else
                {
                    //_spriteBatch.Draw(GameData.TextureAtlas, new Vector2((int)location.X, (int)location.Y) + offset, new Rectangle(defaultSprite.X, defaultSprite.Y, scaledWidth, scaledHeight), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
                    _spriteBatch.Draw(GameData.TextureAtlas, new Rectangle((int)location.X + (int)offset.X, (int)offset.Y, scaledWidth, scaledHeight), new Rectangle(defaultSprite.X, defaultSprite.Y, GameData.TileSize, GameData.TileSize), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                }
            }
            else
            {
                if (toggled)
                {
                    // Calculate the scaled dimensions
                    int scaledWidth = (int)(pressedSprite.Width * GameData.UIScale);
                    int scaledHeight = (int)(pressedSprite.Height * GameData.UIScale);

                    // Calculate the offset to center the scaled rectangle
                    Vector2 offset = new Vector2((pressedSprite.Width - scaledWidth) / 2, (pressedSprite.Height - scaledHeight) / 2);

                    // Draw the scaled rectangle with the centering offset
                    _spriteBatch.Draw(GameData.TextureAtlas, new Rectangle((int)location.X + (int)offset.X, (int)location.Y + (int)offset.Y, scaledWidth, scaledHeight), new Rectangle(pressedSprite.X, pressedSprite.Y, GameData.TileSize, GameData.TileSize), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

                }
                else
                {
                    int scaledWidth = (int)(defaultSprite.Width * GameData.UIScale);
                    int scaledHeight = (int)(defaultSprite.Height * GameData.UIScale);
                    Vector2 offset = new Vector2((defaultSprite.Width - scaledWidth) / 2, (defaultSprite.Height - scaledHeight) / 2);
                    _spriteBatch.Draw(GameData.TextureAtlas, new Rectangle((int)location.X + (int)offset.X, (int)location.Y + (int)offset.Y, scaledWidth, scaledHeight), new Rectangle(defaultSprite.X, defaultSprite.Y, GameData.TileSize, GameData.TileSize), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);
                }
            }

            //Debug Texture
            _spriteBatch.DrawHollowRect(bounds, Color.Red);

        }
    }
}
