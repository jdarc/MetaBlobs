using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MetaBlobs
{
    public class MainGame : Game
    {
        private const float InitialZoom = 500.0f;
        private const float IsoValue = 0.1f;

        private static readonly Random Rnd = new Random(Environment.TickCount);

        private readonly GraphicsDeviceManager _graphics;
        private Camera _camera;
        private DisplayManager _displayManager;
        private int _faceCount;
        private MarchingCubes _marchingCubes;
        private bool _moving;
        private float _oldPitch;
        private int _oldX;
        private int _oldY;
        private float _oldYaw;
        private float _pitch;
        private float _yaw;
        private float _zoom;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _displayManager = new DisplayManager(_graphics)
            {
                SpecularColor = Color.GreenYellow,
                Lighting = true
            };

            _camera = new Camera(MathHelper.PiOver4, 1, 1, 2000);
            _yaw = 0.3f;
            _pitch = -0.5f;

            _marchingCubes = new MarchingCubes(new Vector3(-100, -100, -100), new Vector3(100, 100, 100), 64);
            for (var i = 0; i < 32; i++)
            {
                var rx = (float) (170 * Rnd.NextDouble() - 85);
                var ry = (float) (170 * Rnd.NextDouble() - 85);
                var rz = (float) (170 * Rnd.NextDouble() - 85);
                _marchingCubes.AddBall(new Sphere(new Vector3(rx, ry, rz), 12));
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            HandleInput();
            Animate(gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
            ComputeGrid();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Prepare();
            DrawGrid();
            DrawIsoSurface();
            base.Draw(gameTime);
        }

        private void Prepare()
        {
            _displayManager.Clear(new Color(80, 80, 80), 1, 0);
            _camera.Position = Vector3.TransformNormal(new Vector3(0, 0, _zoom), Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0));
            _camera.Target = new Vector3(0, 75, 0);
            _camera.Up = Vector3.Up;
            _camera.AspectRatio = _graphics.GraphicsDevice.Viewport.AspectRatio;
            _displayManager.ViewMatrix = _camera.View;
            _displayManager.ProjectionMatrix = _camera.Projection;
        }

        private void DrawGrid()
        {
            _displayManager.WorldMatrix = Matrix.Identity;
            _displayManager.DrawGrid(new Vector3(20, 20, 20), 5, new Color(140, 140, 140), new Color(100, 100, 100));
        }

        private void DrawIsoSurface()
        {
            _displayManager.WorldMatrix = Matrix.CreateTranslation(0, 100, 0);
            _displayManager.FillTriangleList(_marchingCubes.Vertices, _marchingCubes.Normals, Color.BlueViolet, _faceCount);
        }

        private void Animate(float dt)
        {
            _marchingCubes.Spheres.ForEach(ball =>
            {
                ball.Acceleration = 100.0f * (new Vector3(0, 0, 0) - ball.Position) / ball.Position.Length();
                ball.Integrate(dt);
            });
        }

        private void ComputeGrid()
        {
            _faceCount = _marchingCubes.Polygonize(IsoValue);
        }

        private void HandleInput()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            var state = Mouse.GetState();
            _zoom = Math.Max(200, InitialZoom - state.ScrollWheelValue * 0.25f);
            if (state.LeftButton == ButtonState.Pressed)
            {
                if (!_moving)
                {
                    _moving = true;
                    _oldX = state.X;
                    _oldY = state.Y;
                    _oldYaw = _yaw;
                    _oldPitch = _pitch;
                }

                _yaw = _oldYaw - (state.X - _oldX) / 100.0f;
                _pitch = _oldPitch - (state.Y - _oldY) / 100.0f;
            }
            else if (state.LeftButton == ButtonState.Released)
            {
                _moving = false;
            }
        }
    }
}