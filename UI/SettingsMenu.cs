using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectArrow.Helpers;
using System;
using System.Collections.Generic;

namespace ProjectArrow.UI
{
    public class SettingsMenu
    {
        private Rectangle BackDrop;
        private bool isOpen = false;
        private Color BackDropColor = new(70, 65, 65, 230);

        // Set Window Display Text
        private readonly string[] WindowString = new string[3] { " WINDOWED ", " BORDERLESS ", " FULLSCREEN " };

        // Set Scale Display Text
        private readonly string[] UIScaleString = new string[1] { " UI SCALE x " + GameData.UIScale.ToString() + " "};

        // Set Zoom Display Text
        private readonly string[] ZoomString = new string[3] { " ZOOM x1 ", " ZOOM x2 ", " ZOOM x3 "};


        private List<Button> SettingsButtons;
        private int lastScreenWidthOffset;
        private int lastScreenHeightOffset;
        private int ScreenWidthOffset;
        private int ScreenHeightOffset;
        private int BackDropWidth = 150;
        private float scrollSpeed = 17f; // Adjust the scroll speed as needed


        private LeftRightButton WindowButton;
        private LeftRightButton UIButton;
        private LeftRightButton ZoomButton;
        private Button DebugToggle;


        public SettingsMenu()
        {
            ScreenWidthOffset = ((int)ScreenManager.VirtualWidth / 2) - (BackDropWidth / 2);
            ScreenHeightOffset = (int)ScreenManager.VirtualHeight + 170;
            lastScreenWidthOffset = ScreenWidthOffset;
            lastScreenHeightOffset = ScreenHeightOffset;

            BackDrop = new Rectangle(ScreenWidthOffset, 0, BackDropWidth, (int)ScreenManager.ScreenHeight + 170);
            SettingsButtons = new List<Button>();

            WindowButton = new(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"]);
            WindowButton.currOptionString = WindowString[0];
            WindowButton.UpdateButtonPosition((int)(ScreenManager.ScreenWidth / 2), 0);
            WindowButton.AddToList(SettingsButtons);

            UIButton = new(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"]);
            UIButton.currOptionString = UIScaleString[0];
            UIButton.UpdateButtonPosition((int)(ScreenManager.ScreenWidth / 2), 0);
            UIButton.AddToList(SettingsButtons);

            ZoomButton = new(GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"]);
            ZoomButton.currOptionString = ZoomString[0];
            ZoomButton.UpdateButtonPosition((int)(ScreenManager.ScreenWidth / 2), 0);
            ZoomButton.AddToList(SettingsButtons);

            DebugToggle = new Button(GameData.TextureMap["DebugButton"], GameData.TextureMap["DebugButtonPressed"], true, false, false);
            SettingsButtons.Add(DebugToggle);

            CalculateButtons();
        }

        public void Update(InputHelper inputHelper)
        {
            ScreenWidthOffset = ((int)ScreenManager.VirtualWidth / 2) - (BackDropWidth / 2);
            ScreenHeightOffset = (int)ScreenManager.VirtualHeight + 170;

            //AllowButtonUpdate(true);


            if (inputHelper.IsKeyPress(Keys.Escape))
            {
                AllowButtonUpdate(true);
                CalculateButtons();
                isOpen = !isOpen;

                if (!isOpen)
                {
                    AllowButtonUpdate(false);
                    BackDrop.Y = 0;
                }
            }

            if (lastScreenWidthOffset != ScreenWidthOffset)
            {
                BackDropWidth = 180 * (int)GameData.UIScale;
                BackDrop.Width = BackDropWidth;
                BackDrop.X = ScreenWidthOffset;
                CalculateButtons();
                lastScreenWidthOffset = ScreenWidthOffset;
            }

            if (lastScreenHeightOffset != ScreenHeightOffset)
            {
                ScreenHeightOffset = (int)ScreenManager.VirtualHeight + 170;
                BackDrop.Height = ScreenHeightOffset;
                lastScreenHeightOffset = ScreenHeightOffset;
                CalculateButtons();
            }

            if (isOpen)
            {
                // Check if the scroll wheel is actively being scrolled up
                if (inputHelper.IsScrollingUp() && BackDrop.Y > -150)
                {
                    BackDrop.Y -= (int)scrollSpeed;
                    UpdateButtonForScroll();
                }
                // Check if the scroll wheel is actively being scrolled down
                else if (inputHelper.IsScrollingDown() && BackDrop.Y < 0)
                {
                    BackDrop.Y += (int)scrollSpeed;
                    UpdateButtonForScroll();
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            if (isOpen)
            {
                for (int i = 0; i < SettingsButtons.Count; i++)
                {
                    Button button = GameData.ButtonList[i];
                    button.Draw(_spriteBatch);
                }

                WindowButton.DrawString(_spriteBatch);
                ZoomButton.DrawString(_spriteBatch);
                UIButton.DrawString(_spriteBatch);

                if (GameData.IsDebug)
                {
                    _spriteBatch.DrawHollowRect(UIButton.TextRectangle, Color.Red);
                    _spriteBatch.DrawHollowRect(WindowButton.TextRectangle, Color.Red);
                    _spriteBatch.DrawHollowRect(ZoomButton.TextRectangle, Color.Red);
                }

                _spriteBatch.DrawFilledRect(BackDrop, BackDropColor);
            }
        }

        #region Debug Toggle
        private void DebugPress()
        {
            if (!DebugToggle.GetToggleState())
            {
                GameData.IsDebug = false;
            }
            else
            {
                GameData.IsDebug = true;
            }
        }
        #endregion

        #region Zoom Buttons
        private void ZoomInPress(Basic2DCamera _camera)
        {
            if (_camera.Zoom.X < 4)
            {
                _camera.Zoom *= new Vector2(2f);

                if (_camera.Zoom.X == 1f)
                    ZoomButton.currOptionString = ZoomString[0];
                else if (_camera.Zoom.X == 2f)
                    ZoomButton.currOptionString = ZoomString[1];
                else if (_camera.Zoom.X == 4f)
                    ZoomButton.currOptionString = ZoomString[2];
                //CalculateButtons();
                //UpdateButtonForScroll();
            }
        }

        private void ZoomOutPress(Basic2DCamera _camera)
        {
            if (_camera.Zoom.X > 1)
                _camera.Zoom /= new Vector2(2f);

            if (_camera.Zoom.X == 1f)
                ZoomButton.currOptionString = ZoomString[0];
            else if (_camera.Zoom.X == 2f)
                ZoomButton.currOptionString = ZoomString[1];
            else if (_camera.Zoom.X == 4f)
                ZoomButton.currOptionString = ZoomString[2];


            //CalculateButtons();
            //UpdateButtonForScroll();
        }
        #endregion

        #region UI Scale Buttons
        private void UIPlusPress()
        {
            if (GameData.UIScale < 4.0f)
            {
                GameData.UIScale += 1f;
                UIScaleString[0] = " UI SCALE x" + GameData.UIScale.ToString() + " ";
                UIButton.currOptionString = UIScaleString[0];
                BackDrop.Y = 0;
                CalculateButtons();
                UpdateButtonForScroll();
            }
        }

        private void UIMinusPress()
        {
            if (GameData.UIScale > 1.0f)
            {
                GameData.UIScale -= 1f;
                UIScaleString[0] = " UI SCALE x" + GameData.UIScale.ToString() + " ";
                UIButton.currOptionString = UIScaleString[0];
                BackDrop.Y = 0;
                CalculateButtons();
                UpdateButtonForScroll();
            }
        }
        #endregion

        #region Window Buttons
        private void WindowSelectionRightPress()
        {
            WindowButton.currentOption++;
            if (WindowButton.currentOption > 2)
                WindowButton.currentOption = 2; // Wrap around to windowed
            ChangeWindow();
        }

        private void WindowSelectionLeftPress()
        {
            WindowButton.currentOption--;
            if (WindowButton.currentOption < 0)
                WindowButton.currentOption = 0; // Wrap around to fullscreen
            ChangeWindow();
        }

        private void ChangeWindow()
        {
            switch (WindowButton.currentOption)
            {
                case 0:
                    ScreenManager.SetWindowed(1280, 720);
                    WindowButton.currOptionString = WindowString[0];
                    break;
                case 1:
                    ScreenManager.SetBorderless();
                    WindowButton.currOptionString = WindowString[1];
                    break;
                case 2:
                    ScreenManager.SetFullscreen();
                    WindowButton.currOptionString = WindowString[2];
                    break;
                default:
                    break;
            }

            CalculateButtons();

        }
        #endregion

        private void CalculateButtons()
        {
            BackDropWidth = 180 * (int)GameData.UIScale; 
            BackDrop.Width = BackDropWidth;
            BackDrop.X = ScreenWidthOffset;

            int Num1 = ((int)ScreenManager.VirtualWidth / 2);
            WindowButton.UpdateButtonPosition(Num1, 10 * (int)GameData.UIScale);
            UIButton.UpdateButtonPosition(Num1, (int)WindowButton.TextRectangle.Bottom + (10 * (int)GameData.UIScale));
            ZoomButton.UpdateButtonPosition(Num1, (int)UIButton.TextRectangle.Bottom + (10 * (int)GameData.UIScale));

            DebugToggle.position.X = ZoomButton.LeftButton.position.X;
            DebugToggle.position.Y = ZoomButton.TextRectangle.Bottom + (20 * (int)GameData.UIScale);
        }

        private void UpdateButtonForScroll()
        {
            WindowButton.UpdateButtonY(BackDrop.Y + 10 * (int)GameData.UIScale);
            UIButton.UpdateButtonY(WindowButton.TextRectangle.Bottom + (10 * (int)GameData.UIScale));
            ZoomButton.UpdateButtonY(UIButton.TextRectangle.Bottom + (10 * (int)GameData.UIScale));
            DebugToggle.position.Y = ZoomButton.TextRectangle.Bottom + (20 * (int)GameData.UIScale);
        }

        private void AllowButtonUpdate(bool allow)
        {
            for (int i = 0; i < SettingsButtons.Count; i++)
            {
                SettingsButtons[i].allowUpdate = allow;
            }
        }

        public void AssignButtonFunctions(Basic2DCamera _camera)
        {
            ZoomButton.RightButton.buttonPress = () => ZoomInPress(_camera);
            ZoomButton.LeftButton.buttonPress = () => ZoomOutPress( _camera);
            UIButton.RightButton.buttonPress = UIPlusPress;
            UIButton.LeftButton.buttonPress = UIMinusPress;
            WindowButton.LeftButton.buttonPress =  WindowSelectionLeftPress;
            WindowButton.RightButton.buttonPress =  WindowSelectionRightPress;
            DebugToggle.buttonPress += DebugPress;
        }

    }

}
