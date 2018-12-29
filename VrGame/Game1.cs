using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpVR;
using Valve.VR;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace VrGame
{
    public class Game1 : Game
    {
        public const int Width = 1280;
        public const int Height = 800;

        public const int NearPlane = 1;
        public const int FarPlane = 10000;

        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _graphics;

        private Camera _camera;
        private CameraInput _cameraInput;

        private RenderTarget2D _leftEye;
        private RenderTarget2D _rightEye;

        private VrContext _vrContext;
        private bool VrEnabled => _vrContext != null;

        public Game1() {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
            _graphics.PreferredBackBufferWidth = Width;
            _graphics.PreferredBackBufferHeight = Height;

            Content.RootDirectory = "Content";

            // unlock framerate
            IsFixedTimeStep = false;
            _graphics.SynchronizeWithVerticalRetrace = false;
        }

        protected override void Initialize()
        {
            Components.Add(new Input(this));
            Components.Add(new FrameRateCounter(this));

            _camera = new Camera(new Vector3(0, 0, 800), new Vector3(0, 0, 0));
            _camera.Up = Vector3.Up;
            var pp = GraphicsDevice.PresentationParameters;
            _cameraInput = new CameraInput(_camera, pp.BackBufferWidth, pp.BackBufferHeight);

            _vrContext = InitializeVr();

            base.Initialize();
        }

        private static VrContext InitializeVr()
        {
            if (!VrContext.CanCallNativeDll(out var error))
            {
                Console.WriteLine(error);
                return null;
            }

            var runtime = VrContext.RuntimeInstalled();
            var hmdConnected = VrContext.HmdConnected();
            Console.WriteLine("Runtime:   " + (runtime ? "yes" : "no"));
            Console.WriteLine("Hmd:       " + (hmdConnected ? "yes" : "no"));

            Console.WriteLine();

            if (!runtime)
            {
                Console.WriteLine("Runtime not installed, failed to create VR service...");
                return null;
            }

            if (!hmdConnected)
            {
                Console.WriteLine("No hmd connected, failed to create VR service...");
                return null;
            }

            var ctx = VrContext.Get();

            Console.WriteLine("Trying to initialize the runtime...");
            try
            {
                ctx.Initialize();
            }
            catch (SharpVRException e)
            {
                if (e.ErrorCode == 108)
                    Console.WriteLine("No HMD is connected and SteamVR failed to report it...");
                else
                    Console.WriteLine("Initializing the runtime failed with error:\n\t" + e.Message);
                return null;
            }

            return ctx;
        }

        protected override void LoadContent()
        {
            if (VrEnabled)
            {
                _vrContext.GetRenderTargetSize(out var width, out var height);

                _leftEye = new RenderTarget2D(GraphicsDevice, width, height);
                _rightEye = new RenderTarget2D(GraphicsDevice, width, height);

                Console.WriteLine($"Rendering to HMD with size ({width}, {height})");
            }
            else
            {
                _leftEye = new RenderTarget2D(GraphicsDevice, Width, Height);
            }

            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            if (VrEnabled)
                _vrContext.Shutdown();
        }

        protected override void Update(GameTime gameTime)
        {
            if (Input.KeyDown(Keys.Escape))
                Exit();

            _cameraInput.Update(gameTime);

            base.Update(gameTime);
        }

        private void RenderEye(Eye eye, Matrix hmd)
        {
            Matrix projection;
            var eyeMatrix = Matrix.Identity;
            if (VrEnabled)
            {
                _vrContext.GetProjectionMatrix(eye, NearPlane, FarPlane, out var hmdProjection);
                _vrContext.GetEyeMatrix(eye, out var hmdEye);
                projection = hmdProjection.ToMg();
                eyeMatrix = hmdEye.ToMg();
            }
            else
            {
                var aspect = GraphicsDevice.Viewport.AspectRatio;
                projection = Matrix.CreatePerspectiveFieldOfView(_camera.FieldOfView, aspect, NearPlane, FarPlane);
            }

            var view = Matrix.CreateLookAt(_camera.Position, _camera.Lookat, _camera.Up);
            var forward = Vector3.TransformNormal(_camera.Forward, Matrix.Invert(hmd * eyeMatrix));
            DrawToTarget(forward, projection, view * hmd * eyeMatrix);
        }

        private void DrawToTarget(Vector3 forward, Matrix proj, Matrix view)
        {
            // Render your stuff here
        }

        protected override void Draw(GameTime gameTime)
        {
            Matrix hmdMatrix = Matrix.Identity;
            if (VrEnabled)
            {
                _vrContext.WaitGetPoses();
                hmdMatrix = _vrContext.Hmd.GetNextPose().ToMg();
            }
            GraphicsDevice.SetRenderTarget(_leftEye);
            GraphicsDevice.Clear(Color.Black);
            RenderEye(Eye.Left, hmdMatrix);

            if (VrEnabled)
            {
                GraphicsDevice.SetRenderTarget(_rightEye);
                GraphicsDevice.Clear(Color.Black);
                RenderEye(Eye.Right, hmdMatrix);
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            // Draw the left eye to the screen
            _spriteBatch.Begin();
            _spriteBatch.Draw(_leftEye, Vector2.Zero, Color.White);
            _spriteBatch.End();

            if (VrEnabled)
            {
                // Submit the render targets for both eyes
                var fieldInfo = typeof(Texture2D).GetField("glTexture", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var leftPtr = new IntPtr((int) fieldInfo.GetValue(_leftEye));
                var rightPtr = new IntPtr((int) fieldInfo.GetValue(_rightEye));
                _vrContext.Submit(Eye.Left, leftPtr, ETextureType.OpenGL);
                _vrContext.Submit(Eye.Right, rightPtr, ETextureType.OpenGL);
            }

            base.Draw(gameTime);
        }
    }
}
