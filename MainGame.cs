using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MetaBlobs
{
    public class MainGame : Game
    {
        private const int InitialZoom = 400;

        private GraphicsDeviceManager _graphics;
        private Camera _cam;
        private float _yaw;
        private float _zoom;
        private Vector3[] _vertices;
        private float _pitch;
        private Vector3[] _normals;
        private MetaBallGrid _grid;
        private int _faceCount;
        private float _isoValue = 0.33333F;
        private DisplayManager _displayManager;
        private bool _moving;
        private int _oldX;
        private int _oldY;
        private float _oldYaw;
        private float _oldPitch;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _displayManager = new DisplayManager(_graphics);

            _cam = new Camera(Vector3.UnitZ, Vector3.Zero, Vector3.Up, MathHelper.PiOver4, 1, 1, 10000);
            _yaw = 0.55f;
            _pitch = -0.5f;
            _zoom = 400;

            _vertices = new Vector3[65536];
            _normals = new Vector3[65536];

            _grid = new MetaBallGrid(new Vector3(-200, -200, -200), new Vector3(200, 200, 200), 10);
            _grid.AddBall(new MetaBall(new Vector3(0, 0, 0), 25));


            var rnd = new Random(238943894);
            for (var i = 0; i < 20; i++)
            {
                var rx = (float) (-80 + 160 * rnd.NextDouble());
                var ry = (float) (-80 + 160 * rnd.NextDouble());
                var rz = (float) (-80 + 160 * rnd.NextDouble());
                _grid.AddBall(new MetaBall(new Vector3(rx, ry, rz), 15));
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            HandleInput();
            // _faceCount = _grid.Generate(_vertices, _normals, _isoValue);

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            var state = Mouse.GetState();
            _zoom = InitialZoom - state.ScrollWheelValue * 0.25f;
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

        protected override void Draw(GameTime gameTime)
        {
            _displayManager.Clear(new Color(80, 80, 80), 1, 0);

            _cam.Position = Vector3.TransformNormal(new Vector3(0, 0, _zoom), Matrix.CreateFromYawPitchRoll(_yaw, _pitch, 0));
            _cam.AspectRatio = _graphics.GraphicsDevice.Viewport.AspectRatio;

            _displayManager.ViewMatrix = _cam.View;
            _displayManager.ProjectionMatrix = _cam.Projection;
            _displayManager.WorldMatrix = Matrix.Identity;
            var gray = System.Drawing.Color.FromArgb(140, 140, 140).ToArgb();
            var dark = System.Drawing.Color.FromArgb(100, 100, 100).ToArgb();
            _displayManager.DrawGrid3D(new Vector3(20, 20, 20), 10, gray, dark);

            base.Draw(gameTime);
        }
    }
}