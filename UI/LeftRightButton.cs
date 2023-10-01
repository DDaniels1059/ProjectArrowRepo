using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectArrow.Helpers;
using System;
using System.Collections.Generic;

namespace ProjectArrow.UI
{
    internal class LeftRightButton
    {
        public Button LeftButton; 
        public Button RightButton;
        public string[] CenterText;
        public bool CanDraw = false;

        public int currentOption = 0;
        public string currOptionString = "";

        public Vector2 TextSize;
        public Rectangle TextRectangle = new();

        private Vector2 textLocation;
        
        public LeftRightButton(Rectangle defaultSprite, Rectangle pressedSprite) 
        {
            LeftButton =  new Button(defaultSprite, pressedSprite, false, false, false);
            RightButton = new Button(defaultSprite, pressedSprite, false, true, true);
            UpdateButtonPosition(0, 0);
        }

        public void AddToList(List<Button> ButtonList)
        {
            ButtonList.Add(LeftButton);
            ButtonList.Add(RightButton);
        }

        public void DrawString(SpriteBatch _spriteBatch)
        {
            _spriteBatch.DrawString(GameData.GameFont, currOptionString, textLocation, Color.Black, 0.0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, 1f);
        }

        public void UpdateButtonY(int Y)
        {
            TextRectangle.Y = Y;
            LeftButton.position = TextRectangle.Location.ToVector2();
            RightButton.position = new Vector2(TextRectangle.Right, TextRectangle.Y);
            textLocation.Y = TextRectangle.Y + (4 * GameData.UIScale);
        }   

        public void UpdateButtonPosition(int X, int Y)
        {
            TextSize = GameData.GameFont.MeasureString(currOptionString) * GameData.UIScale;
            TextRectangle.Width = (int)TextSize.X;
            TextRectangle.Height = (int)TextSize.Y * 2;


            TextRectangle.X = X - ((int)TextSize.X / 2);
            TextRectangle.Y = Y;

            LeftButton.position = TextRectangle.Location.ToVector2();
            RightButton.position = new Vector2(TextRectangle.Right, TextRectangle.Y);

            textLocation.X = TextRectangle.X + (GameData.UIScale);
            textLocation.Y = TextRectangle.Y + (4 * GameData.UIScale);
        }
    }
}
