using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectArrow.Helpers;
using System;
using System.Collections.Generic;

namespace ProjectArrow.UI
{
    internal class TextButton
    {
        public Button Button; 

        public string Text;
        public Color TextColor;
        public Vector2 TextSize;
        public Rectangle TextRectangle = new();
        private Vector2 textLocation;


        public TextButton(Rectangle defaultSprite, Rectangle pressedSprite, bool isToggle, string Text) 
        {
            this.Text = Text;
            Button =  new Button(defaultSprite, pressedSprite, isToggle, false, false);
            UpdateButtonPosition(0, 0);
        }

        public void AddToList(List<Button> ButtonList)
        {
            ButtonList.Add(Button);
        }

        public void DrawString(SpriteBatch _spriteBatch, Color color)
        {
            _spriteBatch.DrawString(GameData.GameFont, Text, textLocation, color, 0.0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, 1f);
        }

        public void DrawString(SpriteBatch _spriteBatch)
        {
            _spriteBatch.DrawString(GameData.GameFont, Text, textLocation, Color.Black, 0.0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, 1f);
        }

        public void UpdateButtonY(int Y)
        {
            TextRectangle.Y = Y;
            Button.position.Y = TextRectangle.Location.Y;
            textLocation.Y = TextRectangle.Y + (4 * GameData.UIScale);
        }   

        public void UpdateButtonPosition(int X, int Y, int OffCenterX = 0)
        {
            TextSize = GameData.GameFont.MeasureString(Text) * GameData.UIScale;
            TextRectangle.Width = (int)TextSize.X;
            TextRectangle.Height = (int)TextSize.Y * 2;


            TextRectangle.X = X - ((int)TextSize.X / 2);
            TextRectangle.Y = Y;

            Button.position.X = X + OffCenterX;
            Button.position.Y = TextRectangle.Location.Y;

            textLocation.X = TextRectangle.X + (GameData.UIScale);
            textLocation.Y = TextRectangle.Y + (4 * GameData.UIScale);
        }
    }
}
