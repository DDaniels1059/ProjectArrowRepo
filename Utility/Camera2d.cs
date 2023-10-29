using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectArrow.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectArrow.Utility
{
    public class Camera2d
    {
        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation

        public Camera2d()
        {
            _zoom = 1.0f;
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

        //Testing A Simpler way to update cam
        public void Follow(Vector2 followPos)
        {
            var position = new Vector2(-followPos.X - (GameData.PlayerSize / 2), -followPos.Y - (GameData.PlayerSize / 2));

            _pos = position;
        }

        public Vector2 ScreenToCamera(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(_transform));
        }

        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            _transform =  Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));
            return _transform;
        }
    }
}
