using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectDelta.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static ProjectDelta.Helpers.InputHelper;

namespace ProjectDelta
{
    public class Game1 : Game
    {
        enum Itemtypes { gears, bricks };
        Itemtypes currtype = Itemtypes.gears;

        private bool Active = true;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player _player;
        private Screen _screen;

        private bool isFirstClick = true;


        private List<Vector2[]> lines;

        //  Just a rectangle to represent a flat surface, or floor in our world
        private Rectangle _npcRect;
        private Rectangle _buttonRect;
        private Button OptionsButton;

        //  A 1x1 pixel that will be used to draw the screen and player texture.
        private Texture2D _view;
        private float rotation = 0.001f;
        private bool isTurned = false;
        private float rotation1 = 0.001f;

        // The camera
        Basic2DCamera _camera;
        InputHelper inputHelper;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _screen = new Screen(_graphics, GraphicsDevice, Window);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteBatchExtensions.Initialize(GraphicsDevice);
            lines = new List<Vector2[]>();

            GameData.LoadData(Content, GraphicsDevice);
            _player = new Player();
            _player.LoadData();

            //  Setting the player to a 32x32 sprite, but setting the position to be in the center of the screen rect
            //  which is why width and height are halved and then 16 (half the player size) subtracted

            _npcRect = new Rectangle((_screen.VirtualWidth / 2), (_screen.VirtualHeight / 2), 16, 16);


            // Create camera
            _camera = new(_screen.VirtualWidth, _screen.VirtualHeight);
            _camera.Zoom *= new Vector2(1f);

            // Create Input Helper
            inputHelper = new InputHelper();

            _buttonRect = new Rectangle(_screen.VirtualWidth - 24, 8, 16, 16);


            // LoadContent or When The Line Needs to be made
            //Vector2 startPoint = new Vector2(100, 100); // Replace with your desired coordinates
            //Vector2 endPoint = new Vector2(300, 200);   // Replace with your desired coordinates
            //int numberOfSegments = 100; // Adjust as needed
            //float sagAmount = 10f; // Adjust this value to control the sag
            //CalculateSaggingLine(startPoint, endPoint, numberOfSegments, sagAmount);

            CreateLine(new Vector2(100, 100), new Vector2(300, 100));
            CreateLine(new Vector2(300, 100), new Vector2(600, 100));
            CreateLine(new Vector2(100, 300), new Vector2(200, 300));

            OptionsButton = new Button();
            OptionsButton.CreateButton(GameData.TextureMap["Wrench"], GameData.TextureMap["Wrench"], false);
            OptionsButton.location = new Vector2(_screen.VirtualWidth - 24, 8);
            OptionsButton.buttonPress += SettingsPress;
        }

        protected override void Update(GameTime gameTime)
        {
            if(this.IsActive)
            {
                Active = true;
                GameData.UIScale = 1F;
                inputHelper.Update(_screen, _camera);
                float deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;


                #region Testing


                if (inputHelper.IsKeyDown(Keys.Escape))
                {
                    Exit();
                }
                if (inputHelper.IsKeyPress(Keys.D1))
                {
                    if (!_screen._isFullscreen)
                    {
                        _screen.SetFullscreen();
                    }
                    else
                    {
                        _screen.SetWindowed();
                    }
                }
                if (inputHelper.IsKeyPress(Keys.D2))
                {
                    _screen.SetWindowed();
                }

                ////Adjust View Padding
                //if (inputHelper.IsKeyDown(Keys.Right) && (_screen.ViewWidth > _screen.VirtualWidth))
                //    _screen.ViewPadding += 5;

                //if (inputHelper.IsKeyDown(Keys.Left) && (_screen.ViewWidth < GraphicsDevice.PresentationParameters.BackBufferWidth))
                //    _screen.ViewPadding -= 5;

                //if (_buttonRect.Contains(VirtualMousePosition))
                //{
                //    if (rotation > .5)
                //    {
                //        isTurned = true;
                //    }
                //    else if (rotation <= -.5)
                //    {
                //        isTurned = false;
                //    }

                //    if (!isTurned)
                //    {
                //        rotation += 4f * deltatime;
                //    }
                //    else if (isTurned)
                //    {
                //        rotation -= 4f * deltatime;
                //    }

                //    if (inputHelper.IsMouseButtonPress(MouseButtons.RightButton))
                //    {
                //        _camera.Zoom /= new Vector2(2f);
                //    }

                //    if (inputHelper.IsMouseButtonPress(MouseButtons.LeftButton))
                //    {
                //        _camera.Zoom *= new Vector2(2f);
                //    }
                //}
                //else
                //{
                //    rotation = 0;
                //}
                ////Adjust View Padding
                if (inputHelper.IsKeyDown(Keys.Right))
                    currtype = Itemtypes.gears;
                if (inputHelper.IsKeyDown(Keys.Left))
                    currtype = Itemtypes.bricks;

                //if (inputHelper.IsMouseButtonPress(MouseButtons.LeftButton))
                //{
                //    if (isFirstClick)
                //    {
                //        firstpoint = new Vector2(WorldMousePosition.X, WorldMousePosition.Y);
                //        isFirstClick = false;
                //    }
                //    else
                //    {
                //        lastpoint = new Vector2(WorldMousePosition.X, WorldMousePosition.Y);
                //        CreateLine(firstpoint, lastpoint);
                //        isFirstClick = true;
                //    }
                //}


                if (inputHelper.IsMouseButtonPress(MouseButtons.LeftButton) && !_buttonRect.Contains(inputHelper.VirtualMousePosition))
                {
                    GameObject newObject = new GameObject();

                    switch (currtype)
                    {
                        case Itemtypes.gears:
                            {
                                newObject = new Tower(inputHelper);                                
                                break;
                            }
                        case Itemtypes.bricks:
                            {
                                newObject = new Bricks();
                                newObject.texture = GameData.TextureMap["Gear"];
                                break;
                            }
                    }

                    //newObject.position = new Vector2(inputHelper.WorldMousePosition.X - (newObject.texture.Width / 2), inputHelper.WorldMousePosition.Y - (newObject.texture.Height / 2));
                    //Vector2 origin = new Vector2(newObject.position.X + (GameData.TileSize / 2), newObject.position.Y + (GameData.TileSize - 2));
                    //newObject.depth = Helper.GetDepth(origin);
                    GameData.gameObjects.Add(newObject);
                }
                if (inputHelper.IsMouseButtonPress(MouseButtons.RightButton) && !_buttonRect.Contains(inputHelper.VirtualMousePosition))
                {
                    GameData.gameObjects.Clear();
                }
                #endregion

                //Ensure camera is centered on player
                _player.Update(gameTime, deltatime, inputHelper);
                _camera.Position = new Vector2(_player.Position.X + 8, _player.Position.Y + 8);
                _camera.CenterOrigin();

                for (int i = 0; i < GameData.ButtonList.Count; i++)
                {
                    Button button = GameData.ButtonList[i];
                    button.Update(inputHelper.VirtualMousePosition, inputHelper, deltatime, button.location);
                }

                rotation1 += 0.05f;
                base.Update(gameTime);
            }
            else
            {
                Active = false;
            }
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Viewport = _screen.Viewport;
            GraphicsDevice.Clear(Color.Black);

            #region World Draw

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix: _camera.TransformationMatrix * _screen.ScreenScaleMatrix);      

            //Background
            _spriteBatch.Draw(GameData._pixel, new Rectangle(0, 0, 800, 800), null, Color.Orange, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            //GameObjects
            for (int i = 0; i < GameData.gameObjects.Count; i++)
            {
                GameObject gameObject = GameData.gameObjects[i];
                _spriteBatch.Draw(GameData.TextureAtlas, new Vector2((int)gameObject.position.X, (int)gameObject.position.Y), gameObject.texture, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, gameObject.depth);
                //Draw Origin
                _spriteBatch.Draw(GameData._pixel, gameObject.origin, null, Color.Red,0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);


                // Check if the GameObject is of type Tower
                if (gameObject is Tower)
                {
                    Tower tower = (Tower)gameObject;
                    _spriteBatch.DrawHollowRect(tower.collider, Color.Red);
                }
            }

            //Power Lines
            for (int i = 0; i < lines.Count; i++)
            {
                Vector2[] line = lines[i];

                for (int j = 0; j < line.Length - 1; j++)
                {
                    _spriteBatch.Draw(GameData._pixel, line[j], null, Color.Black, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 1f);
                }
            }

            //Player
            _player.Draw(_spriteBatch);


            //NPC
            _spriteBatch.Draw(GameData.TextureAtlas, _npcRect, GameData.TextureMap["Gear"], Color.Green, 0f, Vector2.Zero, SpriteEffects.None, Helper.GetDepth(new Vector2(_npcRect.X + 8, _npcRect.Y + 14)));


            _spriteBatch.End();

            #endregion

            #region UI / HUD
            // Draw UI elements (buttons and text)
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, transformMatrix: _screen.ScreenScaleMatrix);

            //_spriteBatch.Draw(GameData.TextureAtlas, new Rectangle(_buttonRect.X + 8, _buttonRect.Y + 8, GameData.TileSize, GameData.TileSize), GameData.TextureMap["Wrench"], Color.White, rotation, _buttonRect.Size.ToVector2() / 2f, SpriteEffects.None, 1f);
            //_spriteBatch.Draw(_pixel, _buttonRect, null, Color.Red, 0f, Vector2.Zero, SpriteEffects.None, .9f);
            //This allows us to see the padding view change, much easier...
            //Show this only when the user wants to change padding?
            //_spriteBatch.Draw(_view, Vector2.Zero, null, Color.White);

            _spriteBatch.DrawString(GameData.GameFont, "WorldMousePos: " + ((int)inputHelper.WorldMousePosition.X).ToString() + " " + ((int)inputHelper.WorldMousePosition.Y).ToString(), new Vector2((int)5, (int)5), Color.White, 0f, Vector2.Zero, .35f, SpriteEffects.None, 1f);
            _spriteBatch.DrawString(GameData.GameFont, "VirtualMousePos: " + ((int)inputHelper.VirtualMousePosition.X).ToString() + " " + ((int)inputHelper.VirtualMousePosition.Y).ToString(), new Vector2((int)5, (int)15), Color.White, 0f, Vector2.Zero, .35f, SpriteEffects.None, 1f);
            //_spriteBatch.DrawString(GameData.GameFont, "LEFT And RIGHT Arrows To Change View Padding", new Vector2((int)5, (int)15), Color.White, 0f, Vector2.Zero, .25f, SpriteEffects.None, 1f);

            string textToCenter = "Paused";
            float textSize = GameData.GameFont.MeasureString(textToCenter).X * 1f;

            for (int i = 0; i < GameData.ButtonList.Count; i++)
            {
                Button button = GameData.ButtonList[i];
                button.Draw(_spriteBatch);
            }

            // Pause Text
            if (!Active)
                _spriteBatch.DrawString(GameData.GameFont, textToCenter, new Vector2((int)(324 - textSize) / 2, (int)(180 / 3)), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);


            _spriteBatch.End();

            #endregion

            base.Draw(gameTime);
        }
        private void CalculateSaggingLine(Vector2 startPoint, Vector2 endPoint, Vector2[] newlineVertices, int numberOfSegments, float sagAmount)
        {
            for (int i = 0; i <= numberOfSegments; i++)
            {
                float t = (float)i / numberOfSegments;
                float y = MathHelper.Lerp(startPoint.Y, endPoint.Y, t);
                float x = MathHelper.Lerp(startPoint.X, endPoint.X, t);

                // Add a slight sine wave effect to create the sag in the middle
                if (i > 0 && i < numberOfSegments)
                {
                    float sagOffset = sagAmount * (float)Math.Sin(t * MathHelper.Pi);
                    y += sagOffset;
                }

                newlineVertices[i] = new Vector2(x, y);
            }
            lines.Add(newlineVertices);
        }
        private void CreateLine(Vector2 StartPoint, Vector2 EndPoint)
        {

            if(EndPoint.X < (StartPoint.X * 2) && EndPoint.Y < (StartPoint.Y * 2))
            {
                int numberOfSegments = 150; // Adjust as needed
                float sagAmount = 15f; // Adjust this value to control the sag
                Vector2[] newlineVertices = new Vector2[numberOfSegments + 1];
                CalculateSaggingLine(StartPoint, EndPoint, newlineVertices, numberOfSegments, sagAmount);
            }
        }   
        public void SettingsPress()
        {
            if (inputHelper.IsMouseButtonPress(MouseButtons.RightButton))
            {
                _camera.Zoom /= new Vector2(2f);
            }

            if (inputHelper.IsMouseButtonPress(MouseButtons.LeftButton))
            {
                _camera.Zoom *= new Vector2(2f);
            }
        }

        // For Creating A Line That Follows The Player Not Needed ATM
        // Initialize         private Vector2[] lineVertices;
        // Create In Update   CreateCharLine(new Vector2(100, 200),  new Vector2(_player.Position.X + 7, _player.Position.Y + 8));

        //private void CreateCharLine(Vector2 StartPoint, Vector2 EndPoint)
        //{
        //    int numberOfSegments = 150; // Adjust as needed
        //    float sagAmount = 15f; // Adjust this value to control the sag
        //    lineVertices = new Vector2[numberOfSegments + 1];

        //    for (int i = 0; i <= numberOfSegments; i++)
        //    {
        //        float t = (float)i / numberOfSegments;
        //        float y = MathHelper.Lerp(StartPoint.Y, EndPoint.Y, t);
        //        float x = MathHelper.Lerp(StartPoint.X, EndPoint.X, t);

        //        // Add a slight sine wave effect to create the sag in the middle
        //        if (i > 0 && i < numberOfSegments)
        //        {
        //            float sagOffset = sagAmount * (float)Math.Sin(t * MathHelper.Pi);
        //            y += sagOffset;
        //        }

        //        lineVertices[i] = new Vector2(x, y);
        //    }
        //}
    }
}