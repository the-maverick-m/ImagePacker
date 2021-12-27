using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mage
{
    public static class Input
    {
        private static KeyboardState _kState, _kStateOld;
        private static MouseState _mState, _mStateOld;
        private static GamePadState _gState, _gStateOld;
        private static Engine _engine;

        private static int _lastXInput;
        private static int _lastYInput;

        internal static void Initialize(Engine engine)
        {
            _engine = engine;
            _kStateOld = _kState = Keyboard.GetState();
            _mState = _mStateOld = Mouse.GetState();
            _gState = _gStateOld = GamePad.GetState(PlayerIndex.One);
        }

        #region Polling

        public static void Poll()
        {
            _kStateOld = _kState;
            _mStateOld = _mState;
            _gStateOld = _gState;

            _kState = Keyboard.GetState();
            _mState = Mouse.GetState();
            _gState = GamePad.GetState(PlayerIndex.One);

            if (IsKeyPressed(Keys.Right))
                _lastXInput = 1;
            else if (IsKeyPressed(Keys.Left))
                _lastXInput = -1;
            else if (IsButtonPressed(Buttons.DPadRight))
                _lastXInput = 1;
            else if (IsButtonPressed(Buttons.DPadLeft))
                _lastXInput = -1;

            if (IsKeyPressed(Keys.Up))
                _lastYInput = 1;
            else if (IsKeyPressed(Keys.Down))
                _lastYInput = -1;
            else if (IsButtonPressed(Buttons.DPadUp))
                _lastYInput = 1;
            else if (IsButtonPressed(Buttons.DPadDown))
                _lastYInput = -1;
        }

        public static bool IsKeyDown(Keys key)
        {
            return _kState.IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return _kState.IsKeyUp(key);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return _kStateOld.IsKeyUp(key) && _kState.IsKeyDown(key);
        }

        public static bool IsKeyReleased(Keys key)
        {
            return _kStateOld.IsKeyDown(key) && _kState.IsKeyUp(key);
        }

        public static bool IsButtonDown(Buttons button)
        {
            return _gState.IsButtonDown(button);
        }

        public static bool IsButtonUp(Buttons button)
        {
            return _gState.IsButtonUp(button);
        }

        public static bool IsButtonPressed(Buttons button)
        {
            return _gStateOld.IsButtonUp(button) && _gState.IsButtonDown(button);
        }

        public static bool IsButtonReleased(Buttons button)
        {
            return _gStateOld.IsButtonDown(button) && _gState.IsButtonUp(button);
        }

        public static Vector2 GetPosition()
        {
            return _gState.ThumbSticks.Left;
        }

        public static void SetRumble(float amount)
        {
            GamePad.SetVibration(PlayerIndex.One, amount, amount, amount, amount);
        }

        #endregion

        #region Mouse Input

        public static Vector2 GetMousePosition(Entity entity)
        {
            return Vector2.Transform(MouseNormalized,
            Matrix.Invert(entity.Layer.Camera.GetFullMatrix(entity.Depth)));
        }

        public static Vector2 MouseNormalized
        {
            get
            {
                Vector2 mousePosInTarget = new Vector2((_mState.Position.X - _engine.Border.X),
                 (_mState.Position.Y - _engine.Border.Y));

                // Normalize X and Y
                mousePosInTarget.X -= _engine.TargetBounds.Width / 2;
                mousePosInTarget.X /= _engine.TargetBounds.Width / 2;

                mousePosInTarget.Y -= _engine.TargetBounds.Height / 2;
                mousePosInTarget.Y /= _engine.TargetBounds.Height / 2;

                return mousePosInTarget;
            }
        }

        public static float MouseScroll =>
            _mState.ScrollWheelValue - _mStateOld.ScrollWheelValue;

        public static bool IsLeftButtonDown => _mState.LeftButton == ButtonState.Pressed;
        public static bool IsRightButtonDown => _mState.RightButton == ButtonState.Pressed;

        #endregion

        #region Polling Properties

        private static bool PressingLeftAndRight()
        {
            return (_kState.IsKeyDown(Keys.Right) && _kState.IsKeyDown(Keys.Left));
        }

        private static bool PressingUpAndDown()
        {
            return (_kState.IsKeyDown(Keys.Up) && _kState.IsKeyDown(Keys.Down));
        }

        public static float X
        {
            get
            {
                if (PressingLeftAndRight())
                    return _lastXInput;
                else if (IsKeyDown(Keys.Right))
                    return 1;
                else if (IsKeyDown(Keys.Left))
                    return -1;
                else if (IsButtonDown(Buttons.DPadRight))
                    return 1;
                else if (IsButtonDown(Buttons.DPadLeft))
                    return -1;

                return Math.Sign(_gState.ThumbSticks.Left.X);
            }
        }
        public static float Y
        {
            get
            {
                if (PressingUpAndDown())
                    return _lastYInput;
                else if (IsKeyDown(Keys.Up))
                    return 1;
                else if (IsKeyDown(Keys.Down))
                    return -1;
                else if (IsButtonDown(Buttons.DPadUp))
                    return 1;
                else if (IsButtonDown(Buttons.DPadDown))
                    return -1;

                return Math.Sign(_gState.ThumbSticks.Left.Y);
            }
        }
        public static Vector2 Position => _gState.ThumbSticks.Left;

        #endregion
    }
}