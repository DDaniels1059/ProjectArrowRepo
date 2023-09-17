using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDelta.Helpers;
using System;
using System.Collections.Generic;

namespace ProjectDelta.UI
{
    public class SettingsMenu
    {
        private Rectangle BackDrop;
        private Button ZoomSelectionLeft;
        private Button ZoomSelectionRight;
        private Button UISelectionLeft;
        private Button UISelectionRight;
        private Button WindowSelectionLeft;
        private Button WindowSelectionRight;
        private Button DebugToggle;

        private bool isOpen = false;
        private  Color BackDropColor = new(70, 65, 65, 230);
        //private static Color BackDropColor = new Color(220, 122, 100, 0);

        // Set Window Display Text
        private int currentWindow = 0; // 0 for windowed, 1 for borderless, 2 for fullscreen
        private static readonly string[] WindowString = new string[3] { " WINDOWED ", " BORDERLESS ", " FULLSCREEN " };
        private string currWindowString;

        // Set Scale Display Text
        private int currentUIScale = 0;
        private static readonly string[] ScaleString = new string[2] { " UI SCALE x1 ", " UI SCALE x2 "};
        private string currUIScaleString;

        // Text Rectangles We Base The Button Position On
        private Vector2 WindowSelectionTextSize;
        private Rectangle WindowTextRectangle;

        private Vector2 UISelectionTextSize;
        private Rectangle UITextRectangle;

        private Vector2 ZoomSelectionTextSize;
        private Rectangle ZoomTextRectangle;
        private string ZoomString = " CAMERA ZOOM ";

        // Make The Code Cleaner
        private static List<Rectangle> TextRectangles;
        private static List<Button> SettingsButtons;

        // Scroll Wheel
        private float scrollSpeed = 7f; // Adjust the scroll speed as needed

        public SettingsMenu(Screen _screen)
        {
            int X = (_screen.VirtualWidth / 2) - (280 / 2);
            BackDrop = new Rectangle(X, 0, 280, _screen.VirtualHeight + 150);
            currWindowString = WindowString[0];
            currUIScaleString = ScaleString[0];
            MeasureStrings();

            TextRectangles = new List<Rectangle>();
            SettingsButtons = new List<Button>();

            CreateRectangle(WindowTextRectangle);
            CreateRectangle(UITextRectangle);
            CreateRectangle(ZoomTextRectangle);

            CreateButton(ref WindowSelectionLeft, false, false, false, GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"]);
            CreateButton(ref WindowSelectionRight, false, true, true, GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"]);
            CreateButton(ref ZoomSelectionLeft, false, false, false, GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"]);
            CreateButton(ref ZoomSelectionRight, false, true, true, GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"]);
            CreateButton(ref UISelectionLeft, false, false, false, GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"]);
            CreateButton(ref UISelectionRight, false, true, true, GameData.TextureMap["LeftArrow"], GameData.TextureMap["LeftArrowPressed"]);

            CreateButton(ref DebugToggle, true, false, false, GameData.TextureMap["DebugButton"], GameData.TextureMap["DebugButtonPressed"]);
        }

        private void CreateButton(ref Button button, bool isToggle, bool isFlipped, bool isAnchoredRight, Rectangle defaultSprite,Rectangle pressedSprite)
        {
            button = new Button(defaultSprite, pressedSprite, isToggle, isFlipped, isAnchoredRight);
            SettingsButtons.Add(button);
        }

        private void CreateRectangle(Rectangle rectangle)
        {
            rectangle = new Rectangle(0, 0, rectangle.Width, rectangle.Height);
            TextRectangles.Add(rectangle);
        }

        public void Update(InputHelper inputHelper)
        {
            if (inputHelper.IsKeyPress(Keys.Escape))
            {
                isOpen = !isOpen;
                CalculateButtons();
                GameData.IsPaused = true;

                foreach (Button button in SettingsButtons)
                button.canDraw = true;

                if (!isOpen)
                {
                    BackDrop.Y = 0;
                    foreach (Button button in SettingsButtons)
                    button.canDraw = false;
                    GameData.IsPaused = false;
                }
            }

            if (isOpen)
            {
                // Check if the scroll wheel is actively being scrolled up
                if (inputHelper.IsScrollingUp() && BackDrop.Y > -150)
                {
                    BackDrop.Y -= (int)scrollSpeed;
                    //Update Button Y Positions
                    UpdateButtonForScroll();
                }
                // Check if the scroll wheel is actively being scrolled down
                else if (inputHelper.IsScrollingDown() && BackDrop.Y < 0)
                {
                    BackDrop.Y += (int)scrollSpeed;
                    //Update Button Y Positions
                    UpdateButtonForScroll();
                }
            }
        }
       
        private void CalculateButtons()
        {
            MeasureStrings();

            //I Base The Buttons Location On Their Respective TextRectangle (If They Have One)
            //Otherwise The Location Is Based On Whatever Rect It's Relative To For My Needs
            //The Magic Number 320 Is Just The Virtual Width. If You See This Abomination,
            //I Gave Up On Making A Simpler Solution. Sorry
            WindowTextRectangle.X = (320 / 2) - ((int)WindowSelectionTextSize.X / 2);
            WindowTextRectangle.Y = 10;
            WindowTextRectangle.Width = (int)WindowSelectionTextSize.X;
            WindowTextRectangle.Height = (int)WindowSelectionTextSize.Y * 2;

            UITextRectangle.X = (320 / 2) - ((int)UISelectionTextSize.X / 2);
            UITextRectangle.Y = WindowTextRectangle.Bottom + (10 * (int)GameData.UIScale);
            UITextRectangle.Width = (int)UISelectionTextSize.X;
            UITextRectangle.Height = (int)UISelectionTextSize.Y * 2;

            ZoomTextRectangle.X = (320 / 2) - ((int)ZoomSelectionTextSize.X / 2);
            ZoomTextRectangle.Y = UITextRectangle.Bottom + (10 * (int)GameData.UIScale);
            ZoomTextRectangle.Width = (int)ZoomSelectionTextSize.X;
            ZoomTextRectangle.Height = (int)ZoomSelectionTextSize.Y * 2;


            //Update The Buttons Position Based On Its Relative Text Rectangle
            //If The Button Is Not Anchored To The Right We Subtract The TileSize So It Is 

            WindowSelectionLeft.position = WindowTextRectangle.Location.ToVector2();
            WindowSelectionRight.position = new Vector2(WindowTextRectangle.Right, WindowTextRectangle.Y);

            UISelectionLeft.position = UITextRectangle.Location.ToVector2();
            UISelectionRight.position = new Vector2(UITextRectangle.Right, UITextRectangle.Y);

            ZoomSelectionLeft.position = ZoomTextRectangle.Location.ToVector2();
            ZoomSelectionRight.position = new Vector2(ZoomTextRectangle.Right, ZoomTextRectangle.Y);

            DebugToggle.position.X = ZoomSelectionLeft.position.X;
            DebugToggle.position.Y = ZoomTextRectangle.Bottom + (10 * (int)GameData.UIScale);
        }

        private void MeasureStrings()
        {
            WindowSelectionTextSize = (GameData.GameFont.MeasureString(currWindowString) * GameData.UIScale); // Measure the text size
            UISelectionTextSize = (GameData.GameFont.MeasureString(currUIScaleString) * GameData.UIScale); // Measure the text size
            ZoomSelectionTextSize = (GameData.GameFont.MeasureString(ZoomString) * GameData.UIScale); // Measure the text size
        }

        private void UpdateButtonForScroll()
        {
            WindowTextRectangle.Y = BackDrop.Y + 10;
            UITextRectangle.Y = WindowTextRectangle.Bottom + (10 * (int)GameData.UIScale);
            ZoomTextRectangle.Y = UITextRectangle.Bottom + (10 * (int)GameData.UIScale);

            WindowSelectionLeft.position.Y = WindowTextRectangle.Y;
            WindowSelectionRight.position.Y = WindowTextRectangle.Y;

            UISelectionLeft.position.Y = UITextRectangle.Y;
            UISelectionRight.position.Y = UITextRectangle.Y;            

            ZoomSelectionLeft.position.Y = ZoomTextRectangle.Y;
            ZoomSelectionRight.position.Y = ZoomTextRectangle.Y;

            DebugToggle.position.X = ZoomSelectionLeft.position.X;
            DebugToggle.position.Y = ZoomTextRectangle.Bottom + (10 * (int)GameData.UIScale);
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            if (isOpen)
            {
                _spriteBatch.DrawString(GameData.GameFont, currWindowString, new Vector2(WindowTextRectangle.X + 1, WindowTextRectangle.Y + (WindowSelectionTextSize.Y / 2)), Color.Black, 0.0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, 1f);
                _spriteBatch.DrawString(GameData.GameFont, currUIScaleString, new Vector2(UITextRectangle.X + 1, UITextRectangle.Y + (UISelectionTextSize.Y / 2)), Color.Black, 0.0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, 1f);
                _spriteBatch.DrawString(GameData.GameFont, ZoomString, new Vector2(ZoomTextRectangle.X + 1, ZoomTextRectangle.Y + (ZoomSelectionTextSize.Y / 2)), Color.Black, 0.0f, Vector2.Zero, GameData.UIScale, SpriteEffects.None, 1f);

                if (GameData.IsDebug)
                {
                    _spriteBatch.DrawHollowRect(UITextRectangle, Color.Red);
                    _spriteBatch.DrawHollowRect(WindowTextRectangle, Color.Red);
                    _spriteBatch.DrawHollowRect(ZoomTextRectangle, Color.Red);
                }

                _spriteBatch.DrawFilledRect(BackDrop, BackDropColor);
            }
        }

        public void AssignButtonFunctions(Basic2DCamera _camera, Screen _screen)
        {
            ZoomSelectionRight.buttonPress = () => ZoomInPress(_camera);
            ZoomSelectionLeft.buttonPress = () => ZoomOutPress( _camera);
            UISelectionRight.buttonPress = UIPlusPress;
            UISelectionLeft.buttonPress = UIMinusPress;
            WindowSelectionLeft.buttonPress = () => WindowSelectionLeftPress(_screen);
            WindowSelectionRight.buttonPress = () => WindowSelectionRightPress(_screen);
            DebugToggle.buttonPress += DebugPress;
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
            if(GameData.UIScale < 3)
                GameData.UIScale += 0.5f;
            currentUIScale = (int)GameData.UIScale;
            switch (currentUIScale)
            {
                case 1:
                    currUIScaleString = ScaleString[0];
                    break;
                case 2:
                    currUIScaleString = ScaleString[1];
                    break;
                default:
                    break;
            }
            MeasureStrings();
            CalculateButtons();
        }

        private void UIMinusPress()
        {
            if (GameData.UIScale > 1)
                GameData.UIScale -= 0.5f;
            currentUIScale = (int)GameData.UIScale;
            switch (currentUIScale)
            {
                case 1:
                    currUIScaleString = ScaleString[0];
                    break;
                case 2:
                    currUIScaleString = ScaleString[1];
                    break;
                default:
                    break;
            }
            CalculateButtons();
        }
        #endregion

        #region Window Buttons
        private void WindowSelectionRightPress(Screen _screen)
        {
            currentWindow++;
            if (currentWindow > 2)
                currentWindow = 2; // Wrap around to windowed
            ChangeWindow(_screen);
            MeasureStrings();
            CalculateButtons();
        }

        private void WindowSelectionLeftPress(Screen _screen)
        {
            currentWindow--;
            if (currentWindow < 0)
                currentWindow = 0; // Wrap around to fullscreen
            ChangeWindow(_screen);
            CalculateButtons();
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
