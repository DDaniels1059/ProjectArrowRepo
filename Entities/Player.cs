using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using ProjectDelta.Helpers;
using ProjectDelta.Objects;

namespace ProjectDelta
{
    class Player
    {
        private enum Direction { Down, Up, Left, Right }
        private Direction direction = Direction.Down;
        private SpriteAnimation anim;
        private Rectangle collider;
        private Vector2 position;
        private Vector2 lastPosition;
        private Vector2 origin;
        private int speed = 100;
        private float depth;
        private bool isMoving = false;


        private SpriteAnimation[] animations = new SpriteAnimation[4];
        public Vector2 Position { get { return position; } private set { position = value; } }
        public Rectangle PlayerCollisionBox { get { return collider; } }

        public void LoadData()
        {
            animations[0] = new SpriteAnimation(GameData.PlayerDown, 4, 6);
            animations[1] = new SpriteAnimation(GameData.PlayerUp, 4, 6);
            animations[2] = new SpriteAnimation(GameData.PlayerLeft, 4, 6);
            animations[3] = new SpriteAnimation(GameData.PlayerRight, 4, 6);
            anim = animations[0];
            position = new Vector2(300, 300);
        }

        //This Gets Called In Game1 Update
        public void Update(GameTime gameTime, float deltaTime, InputHelper inputHelper)
        {   
            isMoving = false;
            lastPosition = position;

            if (inputHelper.IsKeyDown(Keys.D))
            {
                direction = Direction.Right;
                isMoving = true;
            }
            if (inputHelper.IsKeyDown(Keys.A))
            {
                direction = Direction.Left;
                isMoving = true;
            }
            if (inputHelper.IsKeyDown(Keys.W))
            {
                direction = Direction.Up;
                isMoving = true;
            }
            if (inputHelper.IsKeyDown(Keys.S))
            {
                direction = Direction.Down;
                isMoving = true;
            }

            if (isMoving)
            {
                switch (direction)
                {
                    case Direction.Right:
                            position.X += (int)(speed * deltaTime);
                        break;
                    case Direction.Left:
                            position.X -= (int)(speed * deltaTime);
                        break;
                    case Direction.Up:
                            position.Y -= (int)(speed * deltaTime);
                        break;
                    case Direction.Down:
                            position.Y += (int)(speed * deltaTime);
                        break;
                }
            }

            anim = animations[(int)direction];
            anim.Position = new Vector2(position.X, position.Y);

            if (isMoving)
                anim.Update(gameTime);
            else
                anim.setFrame(0);


            //Offset Collider So It Is Where I Want At The Feet
            collider = new Rectangle((int)position.X + 4, (int)position.Y + 8, 8, 4);
            origin = new Vector2(position.X + (GameData.TileSize / 2), position.Y + (GameData.TileSize - 2));
            depth = Helper.GetDepth(origin);

            //Crude Check To Look For Collisions
            for (int i = 0; i < GameData.GameObjects.Count; i++)
            {
                GameObject gameObject = GameData.GameObjects[i];
                if (collider.Intersects(gameObject.collider))
                {
                    position = lastPosition;
                    anim.Position = new Vector2((int)position.X, (int)position.Y);
                    collider = new Rectangle((int)position.X + 4, (int)position.Y + 8, 8, 4);
                    origin = new Vector2(position.X + (GameData.TileSize / 2), position.Y + (GameData.TileSize - 2));
                    depth = Helper.GetDepth(origin);
                    break;
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {      
            //Draw Player
            anim.Draw(_spriteBatch, depth);

            if(GameData.IsDebug)
            {
                //Collider Debug
                _spriteBatch.DrawHollowRect(collider, Color.Red);
                //Origin / Depth Sorting Debug
                _spriteBatch.Draw(GameData.Pixel, origin, null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
        }
    }
}
