using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VrGame
{
    public class CameraInput
    {
        public Camera Camera { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public CameraInput(Camera camera, int screenWidth, int screenHeight)
        {
            Camera = camera;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardEvents(gameTime);
            MouseEvents();
        }

        private void MouseEvents()
        {
            var mouseAmount = 0.01f;

            var direction = Camera.Forward;
            direction.Normalize();

            var normal = Vector3.Cross(direction, Camera.Up);

            if (Input.MouseState.RightButton == ButtonState.Pressed)
            {
                float dx = Input.MouseState.X - Input.LastMouseState.X;
                float dy = Input.MouseState.Y - Input.LastMouseState.Y;

                dx *= ScreenWidth/1280.0f;
                dy *= ScreenHeight / 800.0f;

                Camera.Forward += dx * mouseAmount * normal;

                Camera.Forward -= dy * mouseAmount * Camera.Up;
                Camera.Forward.Normalize();
            }
        }

        private void KeyboardEvents(GameTime gameTime)
        {
            var delta = (float) gameTime.ElapsedGameTime.TotalMilliseconds * 60 / 1000;

            var direction = Camera.Forward;
            direction.Normalize();

            var normal = Vector3.Cross(direction, Camera.Up);

            var amount = 0.8f * delta;

            var amountNormal = 0.2f * delta;

            if (Input.KeyDown(Keys.W))
                Camera.Position += direction * amount;

            if (Input.KeyDown(Keys.S))
                Camera.Position -= direction * amount;

            if (Input.KeyDown(Keys.D))
                Camera.Position += normal * amountNormal;

            if (Input.KeyDown(Keys.A))
                Camera.Position -= normal * amountNormal;
        }
    }
}
