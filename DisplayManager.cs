using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MetaBlobs
{
    public sealed class DisplayManager : IDisposable
    {
        private const int MaxBufferSize = 65536 * 3;
        private readonly BasicEffect _basicEffect;

        private readonly GraphicsDeviceManager _graphics;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly VertexBuffer _vertexBuffer;
        private readonly VertexPositionNormalTexture[] _vertexData;

        public DisplayManager(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
            _graphicsDevice = graphics.GraphicsDevice;
            _vertexBuffer = new VertexBuffer(_graphicsDevice, typeof(VertexPositionNormalTexture), MaxBufferSize, BufferUsage.WriteOnly);
            _vertexData = new VertexPositionNormalTexture[MaxBufferSize];

            _graphics.GraphicsDevice.SetVertexBuffer(_vertexBuffer);

            _basicEffect = new BasicEffect(_graphicsDevice);
            _basicEffect.EnableDefaultLighting();
            _basicEffect.PreferPerPixelLighting = true;
            _basicEffect.SpecularPower = 16;
        }

        public Matrix WorldMatrix
        {
            get => _basicEffect.World;
            set => _basicEffect.World = value;
        }

        public Matrix ViewMatrix
        {
            get => _basicEffect.View;
            set => _basicEffect.View = value;
        }

        public Matrix ProjectionMatrix
        {
            get => _basicEffect.Projection;
            set => _basicEffect.Projection = value;
        }

        public bool Lighting
        {
            get => _basicEffect.LightingEnabled;
            set => _basicEffect.LightingEnabled = value;
        }

        public Color AmbientLightColor
        {
            get => new Color(_basicEffect.AmbientLightColor);
            set => _basicEffect.AmbientLightColor = value.ToVector3();
        }

        public Color DiffuseColor
        {
            get => new Color(_basicEffect.DiffuseColor);
            set => _basicEffect.DiffuseColor = value.ToVector3();
        }

        public Color SpecularColor
        {
            get => new Color(_basicEffect.SpecularColor);
            set => _basicEffect.SpecularColor = value.ToVector3();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Clear(Color color, float depth, int stencil)
        {
            _graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, color, depth, stencil);
        }

        public void DrawGrid(Vector3 spacing, float majorDelta, Color majorColor, Color minorColor)
        {
            var min = new Vector3(-500, -500, -500);
            var max = new Vector3(+500, +500, +500);

            var gridVertices = new Vector3[2 * (int) Math.Ceiling((max.X - min.X) / spacing.X + (max.Y - min.Y) / spacing.Y)];
            for (var t = 0; t < 2; t++)
            {
                var count = 0;

                var x = 0f;
                var dx = t == 0 ? spacing.X : spacing.X * majorDelta * t;
                while (x < max.X)
                {
                    gridVertices[count++] = new Vector3(x, 0, min.Y);
                    gridVertices[count++] = new Vector3(x, 0, max.Y);
                    gridVertices[count++] = new Vector3(-x, 0, min.Y);
                    gridVertices[count++] = new Vector3(-x, 0, max.Y);
                    x += dx;
                }

                var y = 0f;
                var dy = t == 0 ? spacing.Y : spacing.Y * majorDelta * t;
                while (y < max.Y)
                {
                    gridVertices[count++] = new Vector3(min.X, 0, y);
                    gridVertices[count++] = new Vector3(max.X, 0, y);
                    gridVertices[count++] = new Vector3(min.X, 0, -y);
                    gridVertices[count++] = new Vector3(max.X, 0, -y);
                    y += dy;
                }

                DrawLines(gridVertices, t == 0 ? minorColor : majorColor, count);
            }
        }

        public void DrawLines(Vector3[] vertices, Color color, int count)
        {
            if (vertices == null || count <= 1) return;

            _basicEffect.DiffuseColor = color.ToVector3();
            _basicEffect.Alpha = 1;
            _basicEffect.VertexColorEnabled = false;
            _basicEffect.TextureEnabled = false;
            _basicEffect.LightingEnabled = false;

            var total = 2 * (count >> 1);
            foreach (var effectPass in _basicEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                var ngv = 0;
                for (var i = 0; i < total;)
                {
                    _vertexData[ngv++] = new VertexPositionNormalTexture(vertices[i++], Vector3.Zero, Vector2.Zero);
                    _vertexData[ngv++] = new VertexPositionNormalTexture(vertices[i++], Vector3.Zero, Vector2.Zero);
                    if (ngv >= MaxBufferSize - 2 || i >= total - 1)
                    {
                        _vertexBuffer.SetData(_vertexData, 0, ngv);
                        _graphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, ngv >> 1);
                        ngv = 0;
                    }
                }
            }
        }

        public void FillTriangleList(Vector3[] vertices, Vector3[] normals, Color color, int count)
        {
            if (vertices == null || normals == null || count <= 0) return;

            _basicEffect.DiffuseColor = color.ToVector3();
            _basicEffect.Alpha = 1;
            _basicEffect.VertexColorEnabled = false;
            _basicEffect.TextureEnabled = false;
            _basicEffect.LightingEnabled = true;

            var total = count * 3;
            foreach (var effectPass in _basicEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                var ngv = 0;
                for (var i = 0; i < total;)
                {
                    _vertexData[ngv++] = new VertexPositionNormalTexture(vertices[i], normals[i++], Vector2.Zero);
                    _vertexData[ngv++] = new VertexPositionNormalTexture(vertices[i], normals[i++], Vector2.Zero);
                    _vertexData[ngv++] = new VertexPositionNormalTexture(vertices[i], normals[i++], Vector2.Zero);
                    if (ngv >= MaxBufferSize - 3 || i >= total)
                    {
                        _vertexBuffer.SetData(_vertexData, 0, ngv);
                        _graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, ngv / 3);
                        ngv = 0;
                    }
                }
            }
        }

        ~DisplayManager()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            _vertexBuffer?.Dispose();
            _basicEffect?.Dispose();
        }
    }
}