using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using ProjectArrow.Helpers;
using ProjectArrow.Objects;
using ProjectArrow.Utility;
using System;

namespace ProjectArrow
{
    class Player
    {
        private enum Direction { Down, Up, Left, Right }
        private Direction direction = Direction.Down;
        private SpriteAnimation _playerAnim;
        private Rectangle _collider;
        private Vector2 _position;
        private Vector2 _lastPosition;
        private Vector2 _origin;
        private float _speed = 0.1f;
        private float _depth;
        private bool _isMoving = false;

        private Vector2 targetPos;

        private SpriteAnimation[] _animations = new SpriteAnimation[4];
        public Vector2 Position { get { return _position; } private set { _position = value; } }
        public Rectangle PlayerCollisionBox { get { return _collider; } }

        public Player()
        {
            _animations[0] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerDown", 4, 8);
            _animations[1] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerUp", 4, 8);
            _animations[2] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerLeft", 4, 8);
            _animations[3] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerRight", 4, 8);
            _playerAnim = _animations[0];
            _collider = new Rectangle(0,0,GameData.PlayerSize / 2, GameData.PlayerSize / 4);
            _position = new Vector2(660, 765);
        }

        public void Update(GameTime gameTime, float deltaTime, InputManager inputHelper)
        {   
            _isMoving = false;
            _lastPosition = _position;

            if (inputHelper.IsKeyDown(Keys.D))
            {
                direction = Direction.Right;
                _isMoving = true;
            }
            if (inputHelper.IsKeyDown(Keys.A))
            {
                direction = Direction.Left;
                _isMoving = true;
            }
            if (inputHelper.IsKeyDown(Keys.W))
            {
                direction = Direction.Up;
                _isMoving = true;
            }
            if (inputHelper.IsKeyDown(Keys.S))
            {
                direction = Direction.Down;
                _isMoving = true;
            }

            _playerAnim = _animations[(int)direction];


            if (_isMoving)
            {
                switch (direction)
                {
                    case Direction.Right:
                            _position.X += (_speed * deltaTime);
                        break;
                    case Direction.Left:
                            _position.X -= (_speed * deltaTime);
                        break;
                    case Direction.Up:
                            _position.Y -= (_speed * deltaTime);
                        break;
                    case Direction.Down:
                            _position.Y += (_speed * deltaTime);
                        break;
                }
            }

            _collider.X = (int)(_position.X + (GameData.PlayerSize / 4));
            _collider.Y = (int)(_position.Y + (GameData.PlayerSize / 2));

            _origin.X = _position.X + (GameData.PlayerSize / 2);
            _origin.Y = _position.Y + (GameData.PlayerSize - 2);
            _depth = Helper.GetDepth(_origin);


            if (_isMoving)
                _playerAnim.Update(gameTime);
            else
                _playerAnim.SetFrame(0);

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
                    _depth = Helper.GetDepth(_origin);
                    break;
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            //Vector2.Round(_position);

            _playerAnim.Draw(_spriteBatch, _position, _depth, Color.White);

            if (GameData.IsDebug)
            {
                _spriteBatch.DrawHollowRect(_collider, Color.Red);
                _spriteBatch.Draw(GameData.Pixel, _origin, null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
        }
    }
}
