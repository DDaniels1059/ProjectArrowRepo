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
        private SpriteAnimation _anim;
        private Rectangle _collider;
        private Vector2 _position;
        private Vector2 _lastPosition;
        private Vector2 _origin;
        private int _speed = 100;
        private float _depth;
        private bool _isMoving = false;


        private SpriteAnimation[] _animations = new SpriteAnimation[4];
        public Vector2 Position { get { return _position; } private set { _position = value; } }
        public Rectangle PlayerCollisionBox { get { return _collider; } }

        public void LoadData()
        {
            _animations[0] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerDown", 4, 6);
            _animations[1] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerUp", 4, 6);
            _animations[2] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerLeft", 4, 6);
            _animations[3] = new SpriteAnimation(GameData.PlayerAtlas, GameData.PlayerMap, "PlayerRight", 4, 6);
            _anim = _animations[0];
            _position = new Vector2(300, 300);
        }

        //This Gets Called In Game1 Update
        public void Update(GameTime gameTime, float deltaTime, InputHelper inputHelper)
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

            if (_isMoving)
            {
                switch (direction)
                {
                    case Direction.Right:
                            _position.X += (int)(_speed * deltaTime);
                        break;
                    case Direction.Left:
                            _position.X -= (int)(_speed * deltaTime);
                        break;
                    case Direction.Up:
                            _position.Y -= (int)(_speed * deltaTime);
                        break;
                    case Direction.Down:
                            _position.Y += (int)(_speed * deltaTime);
                        break;
                }
            }

            _anim = _animations[(int)direction];
            _anim.Position.X = _position.X;
            _anim.Position.Y = _position.Y;

            if (_isMoving)
                _anim.Update(gameTime);
            else
                _anim.SetFrame(0);


            _collider.X = (int)_position.X + 4;
            _collider.Y = (int)_position.Y + 8;
            _collider.Width = 8;
            _collider.Height = 4;

            _origin.X = _position.X + (GameData.TileSize / 2);
            _origin.Y = _position.Y + (GameData.TileSize - 2);
            _depth = Helper.GetDepth(_origin);

            //Crude Check To Look For Collisions
            for (int i = 0; i < GameData.GameObjects.Count; i++)
            {
                GameObject gameObject = GameData.GameObjects[i];
                if (_collider.Intersects(gameObject.collider))
                {
                    _position = _lastPosition;
                    _anim.Position = new Vector2((int)_position.X, (int)_position.Y);
                    _collider = new Rectangle((int)_position.X + 4, (int)_position.Y + 8, 8, 4);
                    _origin = new Vector2(_position.X + (GameData.TileSize / 2), _position.Y + (GameData.TileSize - 2));
                    _depth = Helper.GetDepth(_origin);
                    break;
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            //Draw Player
            _anim.Draw(_spriteBatch, _depth);

            if (GameData.IsDebug)
            {
                //Collider Debug
                _spriteBatch.DrawHollowRect(_collider, Color.Red);
                //Origin / Depth Sorting Debug
                _spriteBatch.Draw(GameData.Pixel, _origin, null, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
            }
        }
    }
}
