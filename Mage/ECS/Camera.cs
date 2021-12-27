using System;
using Microsoft.Xna.Framework;

namespace Mage
{
    public class Camera
    {
        private float _zoom = 1f;
        private const float MinZoom = 0.1f;
        public float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = value;
                if (_zoom < MinZoom) _zoom = MinZoom;
            }
        }

        private Vector2 _position;
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                _x = _position.X;
                _y = _position.Y;
            }
        }

        private float _x;
        public float X
        {
            get => _x;
            set
            {
                _x = value;
                _position.X = _x;
            }
        }

        private float _y;
        public float Y
        {
            get => _y;
            set
            {
                _y = value;
                _position.Y = _y;
            }
        }

        public int Width { get; }

        public int Height { get; }

        public Layer Layer { get; }

        public Camera(Layer layerGroup, int width, int height)
        {
            Layer = layerGroup;
            Width = width;
            Height = height;
        }

        #region Utilities

        internal Matrix GetTranslation(float depth)
        {
            return Matrix.CreateTranslation(-Position.X * depth,
             -Position.Y * depth, 0);
        }

        internal Matrix GetOffset()
        {
            return Matrix.CreateTranslation(Width / 2, Height / 2, 0);
        }

        internal Matrix GetZoom()
        {
            return Matrix.CreateScale(Zoom);
        }

        internal Matrix GetFullTransform(float depth)
        {
            return GetTranslation(depth) * GetZoom() * GetOffset();
        }

        internal Matrix GetFullMatrix(float depth)
        {
            return GetFullTransform(depth) *
                Matrix.CreateOrthographicOffCenter(0, Width,
                0, Height, 0, -1);
        }

        #endregion
    }
}