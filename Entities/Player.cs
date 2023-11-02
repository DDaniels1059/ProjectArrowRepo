using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using ProjectArrow.Helpers;
using ProjectArrow.Objects;
using ProjectArrow.Utility;
using System;
using System.Linq.Expressions;

namespace ProjectArrow
{
    class Player
    {
        private enum Direction { Down, Up, Left, Right }
        public enum State { Walking, Fishing}

        private Direction direction = Direction.Down;
        public State state = State.Walking;
        public bool isWalking = false;


        private Rectangle _collider;
        private Rectangle _colliderBounds;
        private Vector2 _position;
        private Vector2 _lastPosition;
        private Vector2 _origin;

        private float speed = 60f;
        private float depth;
        private int animYoffset = 0;
        private int animXoffset = 0;

        public SpriteAnimation _playerAnim;
        public SpriteAnimation[] _walkAnimations = new SpriteAnimation[4];
        public SpriteAnimation[] _fishAnimations = new SpriteAnimation[4];

        public Vector2 Position { get { return _position; } private set { _position = value; } }
        public Rectangle PlayerCollisionBox { get { return _collider; } }
        public Rectangle PlayerCollisionBounds { get { return _colliderBounds; } }

        public Player()
        {
            _walkAnimations[0] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerDown", 4, 8);
            _walkAnimations[1] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerUp", 4, 8);
            _walkAnimations[2] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerLeft", 4, 8);
            _walkAnimations[3] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerRight", 4, 8);
            _playerAnim = _walkAnimations[0];


            _fishAnimations[0] = new SpriteAnimation(GameData.PlayerFishAtlas, GameData.PlayerFishMap, "FishDown", 3, 9, false);
            _fishAnimations[1] = new SpriteAnimation(GameData.PlayerFishAtlas, GameData.PlayerFishMap, "FishUp", 3, 9, false);
            _fishAnimations[2] = new SpriteAnimation(GameData.PlayerFishAtlas, GameData.PlayerFishMap, "FishLeft", 3, 9, false);
            _fishAnimations[3] = new SpriteAnimation(GameData.PlayerFishAtlas, GameData.PlayerFishMap, "FishRight", 3, 9, false);


            _collider = new Rectangle(0,0,GameData.PlayerSize / 2, GameData.PlayerSize / 4);
            _colliderBounds = new Rectangle(0, 0, GameData.PlayerSize  / 6, GameData.PlayerSize  / 6);
            _position = GameData.playerStart;
        }


        private void GetFishingOffset()
        {
            switch (direction)
            {
                case Direction.Up:
                    animXoffset = -8;
                    animYoffset = -6;
                    break;
                case Direction.Down:
                    animXoffset = -8;
                    animYoffset = -6;
                    break;
                case Direction.Left:
                    animXoffset = -8;
                    animYoffset = -6;
                    break;
                case Direction.Right:
                    animXoffset = -8;
                    animYoffset = -6;
                    break;
                default:
                    animXoffset = 0;
                    animYoffset = 0;
                    break;
            }
        }

        public void Update(GameTime gameTime, float deltaTime, InputManager inputHelper)
        {   
            _lastPosition = _position;
            isWalking = false;

            var dir = Vector2.Zero;

            if (inputHelper.IsKeyDown(Keys.W))
            {
                direction = Direction.Up;
                isWalking = true;
                dir.Y -= 1;
            }
            else if (inputHelper.IsKeyDown(Keys.A))
            {
                direction = Direction.Left;
                isWalking = true;
                dir.X -= 1;
            }
            else if (inputHelper.IsKeyDown(Keys.S))
            {
                direction = Direction.Down;
                isWalking = true;
                dir.Y += 1;
            }
            else if (inputHelper.IsKeyDown(Keys.D))
            {
                direction = Direction.Right;
                isWalking = true;
                dir.X += 1;
            }

            if(state == State.Fishing && isWalking)
            {
                _playerAnim.SetFrame(0);
                state = State.Walking;
            }

            switch (state)
            {
                case State.Walking:
                    _playerAnim = _walkAnimations[(int)direction];
                    animYoffset = 0;
                    animXoffset = 0;
                    break;
                case State.Fishing:
                    _playerAnim = _fishAnimations[(int)direction];
                    GetFishingOffset();
                        break;
                default:
                    _playerAnim = _walkAnimations[(int)direction];
                    break;
            }

            if (dir == Vector2.Zero)
            {
                if(state == State.Walking)
                {
                    _playerAnim.SetFrame(0);
                }
                else
                {
                    if(_playerAnim.FrameIndex != 3)
                    _playerAnim.Update(gameTime);
                }
                _origin.X = _position.X + (GameData.PlayerSize / 2);
                _origin.Y = _position.Y + (GameData.PlayerSize - 2);
                depth = Helper.GetDepth(_origin);
                return;
            }

            
            

            _position += dir *  speed * deltaTime;

            _colliderBounds.X = (int)(_position.X + (GameData.PlayerSize / 2.2));
            _colliderBounds.Y = (int)(_position.Y + (GameData.PlayerSize / 1.5));
            _collider.X = (int)(_position.X + (GameData.PlayerSize / 4));
            _collider.Y = (int)(_position.Y + (GameData.PlayerSize / 2));
            _origin.X = _position.X + (GameData.PlayerSize / 2);
            _origin.Y = _position.Y + (GameData.PlayerSize - 2);
            depth = Helper.GetDepth(_origin);


            //_playerAnim = _walkAnimations[(int)direction];
            _playerAnim.Update(gameTime);

            foreach (Rectangle rectangle in GameData.CollisionBorders)
            {
                if (_colliderBounds.Intersects(rectangle))
                {
                    _position = _lastPosition;
                    _colliderBounds.X = (int)(_position.X + (GameData.PlayerSize / 2.2));
                    _colliderBounds.Y = (int)(_position.Y + (GameData.PlayerSize / 1.5));
                    _collider.X = (int)(_position.X + (GameData.PlayerSize / 4));
                    _collider.Y = (int)(_position.Y + (GameData.PlayerSize / 2));
                    _origin = new Vector2(_position.X + (GameData.PlayerSize / 2), _position.Y + (GameData.PlayerSize - 2));
                    depth = Helper.GetDepth(_origin);
                    break;
                }
            }

            //Crude Check To Look For Collisions
            for (int i = 0; i < ObjectManager.GameObjects.Count; i++)
            {
                GameObject gameObject = ObjectManager.GameObjects[i];
                if (_collider.Intersects(gameObject.collider))
                {
                    _position = _lastPosition;
                    _collider.X = (int)(_position.X + (GameData.PlayerSize / 4));
                    _collider.Y = (int)(_position.Y + (GameData.PlayerSize / 2));
                    _origin = new Vector2(_position.X + (GameData.PlayerSize / 2), _position.Y + (GameData.PlayerSize - 2));
                    depth = Helper.GetDepth(_origin);
                    break;
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _playerAnim.Draw(_spriteBatch, new Vector2(_position.X + animXoffset, _position.Y + animYoffset), depth, Color.White);

            if (GameData.IsDebug)
            {
                _spriteBatch.DrawHollowRect(_colliderBounds, Color.Blue);
                _spriteBatch.DrawHollowRect(_collider, Color.Red);
                _spriteBatch.Draw(GameData.Pixel, _origin, null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
        }
    }
}
