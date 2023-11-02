using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectArrow.Helpers;

namespace ProjectArrow.Utility
{
    public class Camera2d
    {
        protected float _zoom;
        public Matrix _transform;
        public Vector2 _pos;
        protected float _rotation;

        /*Vector2.Lerp(_pos, targetPosition, 0.21f) - new Vector2(0.5f, 0.5f)*/


        public Camera2d()
        {
            _zoom = GameData.CurrentZoom;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        }

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 1f) _zoom = 1f; } 
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public void Move(Vector2 amount)
        {
            _pos += amount;
        }

        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public void Follow(Vector2 followPos)
        {
            Vector2 targetPosition = followPos + new Vector2(16 / 2, 16 / 2);
            _pos = targetPosition;
        }

        public Vector2 ScreenToCamera(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(_transform));
        }

        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transform = Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                         Matrix.CreateRotationZ(Rotation) *
                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2, 0));
            return _transform;
        }
    }
}
