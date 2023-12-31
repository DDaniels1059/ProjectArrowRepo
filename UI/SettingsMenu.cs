﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectArrow.Helpers;
using ProjectArrow.System;
using ProjectArrow.Utility;
using System;
using System.Collections.Generic;

namespace ProjectArrow.UI
{
    public class SettingsMenu
    {
        private Rectangle BackDrop;
        private bool isOpen = false;

        // Set Window Display Text
        private readonly string[] WindowString = new string[3] { " WINDOWED ", " BORDERLESS ", " FULLSCREEN " };

        // Set Scale Display Text
        private readonly string[] UIScaleString = new string[1] { " UI SCALE x" + GameData.UIScale.ToString() + " "};

        // Set Zoom Display Text
        private readonly string[] ZoomString = new string[1] { " ZOOM x" + GameData.CurrentZoom.ToString() + " " };


        private List<Button> SettingsButtons;
        private int lastScreenWidthOffset;
        private int lastScreenHeightOffset;
        private int ScreenWidthOffset;
        private int ScreenHeightOffset;
        private int BackDropWidth = 150;
        private float scrollSpeed = 5f;


        private LeftRightButton WindowButton;
        private LeftRightButton UIButton;
        private LeftRightButton ZoomButton;

        private TextButton DebugToggle;
        private TextButton VsyncToggle;
        private TextButton ExitButton;


        public SettingsMenu(Camera2d camera)
        {

            ScreenWidthOffset = ((int)ScreenManager.ScreenWidth / 2) - (BackDropWidth / 2);
            ScreenHeightOffset = (int)ScreenManager.ScreenHeight + (170 * GameData.UIScale);
            lastScreenWidthOffset = ScreenWidthOffset;
            lastScreenHeightOffset = ScreenHeightOffset;

            BackDrop = new Rectangle(ScreenWidthOffset, 0, BackDropWidth, (int)ScreenManager.ScreenHeight + (170 * GameData.UIScale));
            SettingsButtons = new List<Button>();

            WindowButton = new(GameData.UIMap["LeftArrow"], GameData.UIMap["LeftArrowPressed"]);
            WindowButton.currOptionString = WindowString[0];
            WindowButton.AddToList(SettingsButtons);

            UIButton = new(GameData.UIMap["LeftArrow"], GameData.UIMap["LeftArrowPressed"]);
            UIButton.currOptionString = UIScaleString[0];
            UIButton.AddToList(SettingsButtons);

            ZoomButton = new(GameData.UIMap["LeftArrow"], GameData.UIMap["LeftArrowPressed"]);
            ZoomButton.currOptionString = ZoomString[0];
            ZoomButton.AddToList(SettingsButtons); 

            DebugToggle = new TextButton(GameData.UIMap["DebugButton"], GameData.UIMap["DebugButtonPressed"], true, " DEBUG ");
            DebugToggle.AddToList(SettingsButtons);

            VsyncToggle = new TextButton(GameData.UIMap["NonPressedButton"], GameData.UIMap["PressedButton"], true, " VSYNC ");
            VsyncToggle.AddToList(SettingsButtons);
            VsyncToggle.Button.SetToggleState(GameData.AllowVysnc);

            ExitButton = new TextButton(GameData.UIMap["ExitButton"], GameData.UIMap["ExitButton"], false, " EXIT ");
            ExitButton.AddToList(SettingsButtons);

            CalculateButtons();
            AssignButtonFunctions(camera);
        }

        public void Update(InputManager inputHelper, float deltaTime)
        {
            ScreenWidthOffset = ((int)ScreenManager.ScreenWidth / 2) - (BackDropWidth / 2);
            ScreenHeightOffset = (int)ScreenManager.ScreenHeight + (170 * GameData.UIScale);

            if (inputHelper.IsKeyPress(Keys.Escape))
            {
                CalculateButtons();
                isOpen = !isOpen;
                GameData.IsPaused = true;
                FileManager.SaveSettings();


                if (!isOpen)
                {
                    BackDrop.Y = 0;
                    GameData.IsPaused = false;
                }
            }

            if (lastScreenWidthOffset != ScreenWidthOffset)
            {
                BackDropWidth = 180 * GameData.UIScale;
                BackDrop.Width = BackDropWidth;
                BackDrop.X = ScreenWidthOffset;
                CalculateButtons();
                UpdateButtonForScroll();
                lastScreenWidthOffset = ScreenWidthOffset;
            }

            if (lastScreenHeightOffset != ScreenHeightOffset)
            {
                ScreenHeightOffset = (int)ScreenManager.ScreenHeight + (170 * GameData.UIScale);
                BackDrop.Height = ScreenHeightOffset;
                CalculateButtons();
                UpdateButtonForScroll();
                lastScreenHeightOffset = ScreenHeightOffset;
            }

            if (isOpen)
            {
                // Check if the scroll wheel is actively being scrolled up
                if (inputHelper.IsScrollingUp() && BackDrop.Y > (-150 * GameData.UIScale))
                {
                    BackDrop.Y -= (int)scrollSpeed * GameData.UIScale;
                    UpdateButtonForScroll();
                }
                // Check if the scroll wheel is actively being scrolled down
                else if (inputHelper.IsScrollingDown() && BackDrop.Y < 0)
                {
                    BackDrop.Y += (int)scrollSpeed * GameData.UIScale;
                    UpdateButtonForScroll();
                }

                for (int i = 0; i < SettingsButtons.Count; i++)
                {
                    Button button = SettingsButtons[i];
                    button.Update(inputHelper.ScreenMousePosition, inputHelper, deltaTime);
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            if (isOpen)
            {
                for (int i = 0; i < SettingsButtons.Count; i++)
                {
                    Button button = SettingsButtons[i];
                    button.Draw(_spriteBatch);
                }

                WindowButton.DrawString(_spriteBatch);
                ZoomButton.DrawString(_spriteBatch);
                UIButton.DrawString(_spriteBatch);
                VsyncToggle.DrawString(_spriteBatch);
                DebugToggle.DrawString(_spriteBatch);
                ExitButton.DrawString(_spriteBatch);



                if (GameData.IsDebug)
                {
                    _spriteBatch.DrawHollowRect(UIButton.TextRectangle, Color.Red);
                    _spriteBatch.DrawHollowRect(WindowButton.TextRectangle, Color.Red);
                    _spriteBatch.DrawHollowRect(ZoomButton.TextRectangle, Color.Red);
                    _spriteBatch.DrawHollowRect(VsyncToggle.TextRectangle, Color.Red);
                    _spriteBatch.DrawHollowRect(DebugToggle.TextRectangle, Color.Red);
                    _spriteBatch.DrawHollowRect(ExitButton.TextRectangle, Color.Red);
                }

                _spriteBatch.DrawFilledRect(BackDrop, Color.DarkSlateGray);
            }
        }

        #region Debug Toggle
        private void DebugPress()
        {
            if (!DebugToggle.Button.GetToggleState())
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
        private void ZoomInPress(Camera2d _camera)
        {
            if (_camera.Zoom < 4)
            {
                _camera.Zoom += 1;
                GameData.CurrentZoom = (int)_camera.Zoom;
                ZoomString[0] = " ZOOM x" + GameData.CurrentZoom.ToString() + " ";
                ZoomButton.currOptionString = ZoomString[0];
            }
        }

        private void ZoomOutPress(Camera2d _camera)
        {
            if (_camera.Zoom > 1)
            {
                _camera.Zoom -= 1;
                GameData.CurrentZoom = (int)_camera.Zoom;
                ZoomString[0] = " ZOOM x" + GameData.CurrentZoom.ToString() + " ";
                ZoomButton.currOptionString = ZoomString[0];
            }
        }
        #endregion

        #region UI Scale Buttons
        private void UIPlusPress()
        {
            if (GameData.UIScale < 8.0f)
            {
                GameData.UIScale += 1;
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
                GameData.UIScale -= 1;
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

        #region Vysnc Toggle
        private void ToggleVsync()
        {
            bool allow = GameData.AllowVysnc;

            if (allow)
            {
                ScreenManager.Graphics.SynchronizeWithVerticalRetrace = false;
                GameData.AllowVysnc = false;
                VsyncToggle.Button.SetToggleState(false);
                //VsyncToggle.TextColor = Color.DarkRed;
                ScreenManager.Graphics.ApplyChanges();
            }
            else
            {
                ScreenManager.Graphics.SynchronizeWithVerticalRetrace = true;
                GameData.AllowVysnc = true;
                VsyncToggle.Button.SetToggleState(true);
                //VsyncToggle.TextColor = Color.MediumSeaGreen;
                ScreenManager.Graphics.ApplyChanges();
            }
        }
        #endregion

        #region Exit Button
        private void ExitButtonPress()
        {
            FileManager.SaveSettings();
            GameData.ExitGame = true;
        }
        #endregion
        private void CalculateButtons()
        {
            BackDropWidth = 180 * (int)GameData.UIScale; 

            int Num1 = ((int)ScreenManager.ScreenWidth / 2);
            WindowButton.UpdateButtonPosition(Num1, 10 * (int)GameData.UIScale);
            UIButton.UpdateButtonPosition(Num1, (int)WindowButton.TextRectangle.Bottom + (10 * (int)GameData.UIScale));
            ZoomButton.UpdateButtonPosition(Num1, (int)UIButton.TextRectangle.Bottom + (10 * (int)GameData.UIScale));
            VsyncToggle.UpdateButtonPosition(Num1, (int)ZoomButton.TextRectangle.Bottom + (20 * (int)GameData.UIScale), - 32 * GameData.UIScale);
            DebugToggle.UpdateButtonPosition(Num1, (int)VsyncToggle.TextRectangle.Bottom + (20 * (int)GameData.UIScale), - 32 * GameData.UIScale);
            ExitButton.UpdateButtonPosition(Num1, (int)DebugToggle.TextRectangle.Bottom + (20 * (int)GameData.UIScale), -32 * GameData.UIScale);
        }

        private void UpdateButtonForScroll()
        {
            WindowButton.UpdateButtonY(BackDrop.Y + 10 * (int)GameData.UIScale);
            UIButton.UpdateButtonY(WindowButton.TextRectangle.Bottom + (10 * (int)GameData.UIScale));
            ZoomButton.UpdateButtonY(UIButton.TextRectangle.Bottom + (10 * (int)GameData.UIScale));
            VsyncToggle.UpdateButtonY(ZoomButton.TextRectangle.Bottom + (20 * (int)GameData.UIScale));
            DebugToggle.UpdateButtonY(VsyncToggle.TextRectangle.Bottom + (20 * (int)GameData.UIScale));
            ExitButton.UpdateButtonY(DebugToggle.TextRectangle.Bottom + (20 * (int)GameData.UIScale));
        }

        private void AssignButtonFunctions(Camera2d _camera)
        {
            ZoomButton.RightButton.buttonPress = () => ZoomInPress(_camera);
            ZoomButton.LeftButton.buttonPress = () => ZoomOutPress( _camera);
            UIButton.RightButton.buttonPress = UIPlusPress;
            UIButton.LeftButton.buttonPress = UIMinusPress;
            WindowButton.LeftButton.buttonPress =  WindowSelectionLeftPress;
            WindowButton.RightButton.buttonPress =  WindowSelectionRightPress;
            DebugToggle.Button.buttonPress += DebugPress;
            VsyncToggle.Button.buttonPress += ToggleVsync;
            ExitButton.Button.buttonPress = ExitButtonPress;
        }
    }
}
