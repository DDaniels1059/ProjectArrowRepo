using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDelta.Helpers;
using System;

namespace ProjectDelta.UI
{
    public class SettingsMenu
    {
        private Rectangle BackDrop;
        private Button ZoomOut;
        private Button ZoomIn;
        private Button UIMinus;
        private Button UIPlus;
        private Button WindowSelectionLeft;
        private Button WindowSelectionRight;
        private bool isOpen = false;
        private  Color BackDropColor = new(70, 65, 65, 230);
        //private static Color BackDropColor = new Color(220, 122, 100, 0);

        // Set Window
        private int currentWindow = 0; // 0 for windowed, 1 for borderless, 2 for fullscreen
        private static readonly string[] WindowString = new string[3] { "WINDOWED", "BORDERLESS", "FULLSCREEN" };
        private string currWindowString;

        // Button Text Locations
        private Vector2 WindowSelectionTextLoc;
        private Vector2 ZoomTextLoc;
        private Vector2 UITextLoc;

        // Scroll Wheel
        private float scrollSpeed = 8f; // Adjust the scroll speed as needed

        public SettingsMenu()
        {
            int X = (320 / 2) - (280 / 2);
            BackDrop = new Rectangle(X, 0, 280, 320 + 155);
            WindowSelectionLeft = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, false, false, true, 1);
            WindowSelectionLeft.location = new Vector2((int)BackDrop.Left + 80, BackDrop.Y + 10);
            WindowSelectionRight = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, true, true, true, 1);
            WindowSelectionRight.location = new Vector2((int)BackDrop.Right - 96, BackDrop.Y + 10);
            ZoomOut = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, false, false, true, 2);
            ZoomOut.location = new Vector2((int)((BackDrop.Left + 100)), BackDrop.Y + 40);
            ZoomIn = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, true, true, true, 2);
            ZoomIn.location = new Vector2((int)((BackDrop.Right - 116)), BackDrop.Y + 40);
            UIMinus = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, false, false, true, 3);
            UIMinus.location = new Vector2((int)BackDrop.Left + 100, BackDrop.Y + 70);
            UIPlus = new Button(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"], false, true, true, true, 3);
            UIPlus.location = new Vector2((int)BackDrop.Right - 116, BackDrop.Y + 70);
            currWindowString = WindowString[0];
        }

        public void Update(InputHelper inputHelper)
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

                Vector2 ZOOM = (GameData.GameFont.MeasureString("ZOOM") * GameData.UIScale) / 2; // Measure the text size and scale it down by half
                Vector2 SCALE = (GameData.GameFont.MeasureString("SCALE") * GameData.UIScale) / 2; // Measure the text size and scale it down by half
                Vector2 WINDOW = (GameData.GameFont.MeasureString(currWindowString) * GameData.UIScale) / 2; // Measure the text size and scale it down by half

                WindowSelectionTextLoc.X = (int)(BackDrop.X + (BackDrop.Width / 2)) - (WINDOW.X) + 2;
                WindowSelectionTextLoc.Y = (int)(WindowSelectionLeft.bounds.Bottom - GameData.UIScale * 10);

                ZoomTextLoc.X = (int)(BackDrop.X + ((BackDrop.Width / 2)) - (ZOOM.X) + 1);
                ZoomTextLoc.Y = (int)(ZoomOut.bounds.Bottom - GameData.UIScale * 10);

                UITextLoc.X = (int)(BackDrop.X + ((BackDrop.Width / 2)) - (SCALE.X) + 1);
                UITextLoc.Y = (int)(UIMinus.bounds.Bottom - GameData.UIScale * 10);


                ZoomIn.canDraw = true;
                ZoomOut.canDraw = true;
                UIPlus.canDraw = true;
                UIMinus.canDraw = true;
                WindowSelectionLeft.canDraw = true;
                WindowSelectionRight.canDraw = true;

                WindowSelectionLeft.location.Y = BackDrop.Y + 10;
                WindowSelectionRight.location.Y = BackDrop.Y + 10;
                ZoomIn.location.Y = BackDrop.Y + 40;
                ZoomOut.location.Y = BackDrop.Y + 40;
                UIMinus.location.Y = BackDrop.Y + 70;
                UIPlus.location.Y = BackDrop.Y + 70;
            }
            else
            {
                GameData.IsPaused = false;
                ZoomIn.canDraw = false;
                ZoomOut.canDraw = false;
                UIPlus.canDraw = false;
                UIMinus.canDraw = false;
                WindowSelectionLeft.canDraw = false;
                WindowSelectionRight.canDraw = false;
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            if (isOpen)
            {
                _spriteBatch.DrawString(GameData.GameFont, "ZOOM", ZoomTextLoc, Color.Black, 0.0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, 1f);
                _spriteBatch.DrawString(GameData.GameFont, "SCALE", UITextLoc, Color.Black, 0.0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, 1f);
                _spriteBatch.DrawString(GameData.GameFont, currWindowString, WindowSelectionTextLoc, Color.Black, 0.0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, 1f);
                _spriteBatch.DrawFilledRect(BackDrop, BackDropColor);
            }
        }

        public void AssignButtonFunctions(Basic2DCamera _camera, Screen _screen)
        {
            ZoomIn.buttonPress = () => ZoomInPress(_camera);
            ZoomOut.buttonPress = () => ZoomOutPress( _camera);
            UIPlus.buttonPress = UIPlusPress;
            UIMinus.buttonPress = UIMinusPress;
            WindowSelectionLeft.buttonPress = () => WindowSelectionLeftPress(_screen);
            WindowSelectionRight.buttonPress = () => WindowSelectionRightPress(_screen);
        }

        #region Zoom Buttons
        private void ZoomInPress(Basic2DCamera _camera)
        {
            if(_camera.Zoom.X < 4)
            _camera.Zoom *= new Vector2(2f);
        }

        private void ZoomOutPress(Basic2DCamera _camera)
        {
            if (_camera.Zoom.X > 0.75)

                _camera.Zoom /= new Vector2(2f);
        }
        #endregion

        #region UI Scale Buttons
        private void UIPlusPress()
        {
            if(GameData.UIScale < 2)
            GameData.UIScale += 1f;
        }

        private void UIMinusPress()
        {
            if (GameData.UIScale > 1)
                GameData.UIScale -= 1f;
        }
        #endregion

        #region Window Buttons
        private void WindowSelectionRightPress(Screen _screen)
        {
            currentWindow++;
            if (currentWindow > 2)
                currentWindow = 2; // Wrap around to windowed
            ChangeWindow(_screen);
        }

        private void WindowSelectionLeftPress(Screen _screen)
        {
            currentWindow--;
            if (currentWindow < 0)
                currentWindow = 0; // Wrap around to fullscreen
            ChangeWindow(_screen);
        }

        private void ChangeWindow(Screen _screen)
        {
            switch (currentWindow)
            {
                case 0:
                    _screen.SetWindowed(1280, 720);
                    currWindowString = WindowString[0];
                    break;
                case 1:
                    _screen.SetBorderless();
                    currWindowString = WindowString[1];
                    break;
                case 2:
                    _screen.SetFullscreen();
                    currWindowString = WindowString[2];
                    break;
                default:
                    break;
            }
        }
        #endregion

    }

}
