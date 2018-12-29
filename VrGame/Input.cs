using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace VrGame
{
    /// <summary>
    /// Simple helper class to track keyboard input.
    /// </summary>
    public class Input : GameComponent
    {
        public static KeyboardState KeyboardState { get; private set; }
        public static KeyboardState LastKeyboardState { get; private set; }

        public static MouseState MouseState { get; private set; }
        public static MouseState LastMouseState { get; private set; }

        public Input(Game game)
            : base(game)
        {
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
        }

        /// <summary>
        /// Check if the given key is released this frame.
        /// </summary>
        public static bool KeyReleased(Keys key)
        {
            return KeyboardState.IsKeyUp(key) &&
                   LastKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Check if the given key is pressed this frame. /// </summary>
        public static bool KeyPressed(Keys key)
        {
            return KeyboardState.IsKeyDown(key) &&
                   LastKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Check if the given key is down.
        /// </summary>
        public static bool KeyUp(Keys key)
        {
            return KeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Check if the given key is down.
        /// </summary>
        public static bool KeyDown(Keys key)
        {
            return KeyboardState.IsKeyDown(key);
        }

        public static bool LeftClicked()
        {
            return MouseState.LeftButton == ButtonState.Released &&
                   LastMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool RightClicked()
        {
            return MouseState.RightButton == ButtonState.Released &&
                   LastMouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Updates keyboard and gamepad states
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            LastKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();

            LastMouseState = MouseState;
            MouseState = Mouse.GetState();

            base.Update(gameTime);
        }
    }
}
