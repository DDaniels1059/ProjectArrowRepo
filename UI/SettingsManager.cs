using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDelta.Helpers;
using System;

namespace ProjectDelta.UI
{
    public class SettingsManager
    {
        private static Rectangle BackDrop;
        private static Button ZoomOut;
        private static Button ZoomIn;
        private static Button UIMinus;
        private static Button UIPlus;
        private static Button FullScreen;
        private static Button Windowed;
        private static Button FullScreen1;
        private static Button Windowed1;
        private static Button FullScreen2;
        private static Button Windowed2;
        private static bool isOpen = false;
        //private static Color BackDropColor = new Color(220, 122, 100, 0);
        private static Color BackDropColor = new Color(70, 65, 65, 230);


        private float menuPosition = 0;
        float scrollSpeed = 15f; // Adjust the scroll speed as needed

        public SettingsManager(Screen _screen)
        {
            int x = _screen.VirtualWidth / 2;
            int offset = x / 2;
            BackDrop = new Rectangle(x - offset, 0, _screen.VirtualWidth / 2, _screen.VirtualHeight + 150);
            ZoomOut = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, false, false, false, 1);
            ZoomOut.location = new Vector2(BackDrop.Width - 45, BackDrop.Y + 15);
            ZoomIn = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, true, true, false, 1);
            ZoomIn.location = new Vector2(BackDrop.Width + 30, BackDrop.Y + 15);
            UIMinus = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, false, false, true, 2);
            UIMinus.location = new Vector2(BackDrop.Width - 45, BackDrop.Y + 40);
            UIPlus = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, true, true, true, 2);
            UIPlus.location = new Vector2(BackDrop.Width + 30, BackDrop.Y + 40);
            FullScreen = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, false, false, true, 3);
            FullScreen.location = new Vector2(BackDrop.Width - 45, BackDrop.Y + 65);
            Windowed = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, true, true, true, 3);
            Windowed.location = new Vector2(BackDrop.Width + 30, BackDrop.Y + 65);
            FullScreen1 = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, false, false, true, 4);
            FullScreen1.location = new Vector2(BackDrop.Width - 45, BackDrop.Y + 90);
            Windowed1 = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, true, true, true, 4);
            Windowed1.location = new Vector2(BackDrop.Width + 30, BackDrop.Y + 90);
            FullScreen2 = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, false, false, true, 5);
            FullScreen2.location = new Vector2(BackDrop.Width - 45, BackDrop.Y + 115);
            Windowed2 = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, true, true, true, 5);
            Windowed2.location = new Vector2(BackDrop.Width + 30, BackDrop.Y + 115);
        }

        public void Update(InputHelper inputHelper, Screen _screen)
        {
            if (inputHelper.IsKeyPress(Keys.Escape))
            {
                isOpen = !isOpen;

                if(!isOpen)
                {
                    BackDrop.Y = 0;
                }
            }


            if (isOpen)
            {
                GameData.IsPaused = true;

                // Check if the scroll wheel is actively being scrolled up
                if (inputHelper.IsScrollingUp() && BackDrop.Y > -150)
                {
                    BackDrop.Y -= (int)scrollSpeed;
                }
                // Check if the scroll wheel is actively being scrolled down
                else if (inputHelper.IsScrollingDown() && BackDrop.Y < 0)
                {
                    BackDrop.Y += (int)scrollSpeed;
                }

                ZoomIn.canDraw = true;
                ZoomOut.canDraw = true;
                UIPlus.canDraw = true;
                UIMinus.canDraw = true;
                FullScreen.canDraw = true;
                Windowed.canDraw = true;
                FullScreen1.canDraw = true;
                Windowed1.canDraw = true;
                FullScreen2.canDraw = true;
                Windowed2.canDraw = true;
                ZoomIn.location.Y = BackDrop.Y + 15;
                ZoomOut.location.Y = BackDrop.Y + 15;
                UIMinus.location.Y = BackDrop.Y + 40;
                UIPlus.location.Y = BackDrop.Y + 40;
                FullScreen.location.Y = BackDrop.Y + 65;
                Windowed.location.Y = BackDrop.Y + 65;
                FullScreen1.location.Y = BackDrop.Y + 90;
                Windowed1.location.Y = BackDrop.Y + 90;
                FullScreen2.location.Y = BackDrop.Y + 115;
                Windowed2.location.Y = BackDrop.Y + 115;
            }
            else
            {
                GameData.IsPaused = false;
                ZoomIn.canDraw = false;
                ZoomOut.canDraw = false;
                UIPlus.canDraw = false;
                UIMinus.canDraw = false;
                FullScreen.canDraw = false;
                Windowed.canDraw = false;
                FullScreen1 .canDraw = false;
                Windowed1 .canDraw = false;
                FullScreen2.canDraw = false;
                Windowed2.canDraw = false;
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            if (isOpen)
            {
                _spriteBatch.DrawString(GameData.GameFont, "ZOOM", new Vector2(BackDrop.X - (GameData.UIScale * 15 - 1) + ((BackDrop.Width / 2)), (int)(ZoomOut.bounds.Bottom - GameData.UIScale * 10)), Color.Black, 0.0f, Vector2.Zero, (0.50f * GameData.UIScale), SpriteEffects.None, 1f);


                Vector2 textSize = GameData.GameFont.MeasureString("SCALE") * (0.5f * GameData.UIScale); // Measure the text size and scale it down by half


                if (GameData.UIScale == 1)
                {
                    _spriteBatch.DrawString(GameData.GameFont, "SCALE", new Vector2(UIMinus.bounds.X - (GameData.UIScale * 15 - 1) + textSize.X + 8 , (int)(UIMinus.bounds.Bottom - GameData.UIScale * 10)), Color.Black, 0.0f, Vector2.Zero, (0.50f * GameData.UIScale), SpriteEffects.None, 1f);
                }
                else
                {
                    _spriteBatch.DrawString(GameData.GameFont, "SCALE", new Vector2(UIMinus.bounds.X - (GameData.UIScale * 15 - 1) + textSize.X - 3 , (int)(UIMinus.bounds.Bottom - GameData.UIScale * 10)), Color.Black, 0.0f, Vector2.Zero, (0.50f * GameData.UIScale), SpriteEffects.None, 1f);
                }

                _spriteBatch.DrawFilledRect(BackDrop, BackDropColor);
            }
        }

        public void AssignButtonFunctions(Basic2DCamera _camera)
        {
            ZoomIn.buttonPress = () => ZoomInPress(_camera);
            ZoomOut.buttonPress = () => ZoomOutPress( _camera);
            UIPlus.buttonPress = UIPlusPress;
            UIMinus.buttonPress = UIMinusPress;
        }

        #region Zoom Buttons
        private void ZoomInPress(Basic2DCamera _camera)
        {
            if(_camera.Zoom.X < 4)
            _camera.Zoom *= new Vector2(2f);
        }

        private void ZoomOutPress(Basic2DCamera _camera)
        {
            if (_camera.Zoom.X > 1)

                _camera.Zoom /= new Vector2(2f);
        }
        #endregion

        #region UI Scale Buttons
        private void UIPlusPress()
        {
            if(GameData.UIScale < 5)
            GameData.UIScale += 1f;
        }

        private void UIMinusPress()
        {
            if (GameData.UIScale > 1)
                GameData.UIScale -= 1f;
        }
        #endregion

    }

}
