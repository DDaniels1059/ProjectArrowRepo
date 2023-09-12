using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using ProjectDelta.Helpers;

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
        private int speed = 150;
        private float depth;
        private bool isMoving = false;


        private SpriteAnimation[] animations = new SpriteAnimation[4];
        public Vector2 Position { get { return position; } private set { position = value; } }
        public Rectangle PlayerCollisionBox { get { return collider; } }

        public void LoadData()
        {
            animations[0] = new SpriteAnimation(GameData.TextureAtlas, 4, 0, "Player_Up");
            animations[1] = new SpriteAnimation(GameData.TextureAtlas, 4, 0, "Player_Down");
            animations[2] = new SpriteAnimation(GameData.TextureAtlas, 4, 0, "Player_Left");
            animations[3] = new SpriteAnimation(GameData.TextureAtlas, 4, 0, "Player_Right");
            anim = animations[0];

            position = new Vector2(100, 100);
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
                anim.setFrame(1);

            //Offset Collider So It Is Where I Want At The Feet
            collider = new Rectangle((int)position.X + 4, (int)position.Y + 10, 8, 8);
            origin = new Vector2(position.X + (GameData.TileSize / 2), position.Y + (GameData.TileSize - 2));
            depth = Helper.GetDepth(origin);


            //Crude Check To Look For Collisions
            for (int i = 0; i < GameData.gameObjects.Count; i++)
            {
                GameObject gameObject = GameData.gameObjects[i];
                if (collider.Intersects(gameObject.collider))
                {
                    position = lastPosition;
                    break;
                }
            }
        }

        //This Gets Called In Game1 Draw
        public void Draw(SpriteBatch _spriteBatch)
        {      
            //Draw Player
            anim.DrawFromAtlas(_spriteBatch, depth);

            //Draw Collider For Debug
            //_spriteBatch.Draw(GameData.TextureAtlas, collider, GameData.TextureMap["Debug"], Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1f);

            _spriteBatch.DrawHollowRect(collider, Color.Red);



            //Draw Origin For Depth Sorting Debug
            _spriteBatch.Draw(GameData._pixel, origin, null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }
    }
}
