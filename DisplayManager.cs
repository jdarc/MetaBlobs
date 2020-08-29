using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MetaBlobs
{
    public class DisplayManager : IDisposable
    {
        public const int MaxVertexBufferSize = 512;

        private readonly GraphicsDeviceManager _graphics;
        private readonly VertexBuffer _vertexBuffer2D;
        private readonly VertexBuffer _vertexBuffer3D;
        private readonly BasicEffect _basicEffect;

        public DisplayManager(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
            var type2D = typeof(VertexPositionColor);
            var type3D = typeof(VertexPositionNormalTexture);
            _vertexBuffer3D = new VertexBuffer(graphics.GraphicsDevice, type3D, MaxVertexBufferSize, BufferUsage.WriteOnly);
            _vertexBuffer2D = new VertexBuffer(graphics.GraphicsDevice, type2D, MaxVertexBufferSize, BufferUsage.WriteOnly);

            // Create a BasicEffect, which will be used to render the primitive.
            _basicEffect = new BasicEffect(graphics.GraphicsDevice) {LightingEnabled = true};

            // Set up light 0 for per pixel lighting, 1 directoinal light
            _basicEffect.DirectionalLight0.Enabled = true;
            _basicEffect.DirectionalLight0.Direction = new Vector3(0.25f, -1.0f, -1.0f);
            _basicEffect.DirectionalLight0.Direction.Normalize();

            _basicEffect.SpecularColor = Vector3.One;
            _basicEffect.DirectionalLight0.SpecularColor = Vector3.One;
            _basicEffect.SpecularPower = 32;
            _basicEffect.PreferPerPixelLighting = false;
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

        public static Color IntToColor(int rgb) => new Color(rgb >> 16 & 0xFF, rgb >> 8 & 0xFF, rgb & 0xFF);

        public void Clear(Color color, float depth, int stencil)
        {
            _graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, color, depth, stencil);
        }

        public void DrawLines3D(Vector3[] vertices, int color, int count)
        {
            if (vertices != null && count > 1)
            {
                _basicEffect.DiffuseColor = IntToColor(color).ToVector3();
                _basicEffect.Alpha = 1;
                _basicEffect.VertexColorEnabled = false;
                _basicEffect.TextureEnabled = false;
                _basicEffect.LightingEnabled = false;

                _graphics.GraphicsDevice.SetVertexBuffer(_vertexBuffer3D);

                var verts = new VertexPositionNormalTexture[MaxVertexBufferSize];

                foreach (EffectPass effectPass in _basicEffect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();

                    var ngv = 0;
                    var pointCount = 2 * (count >> 1); // List list must be in pairs!
                    for (var i = 0; i < pointCount;)
                    {
                        verts[ngv++] = new VertexPositionNormalTexture(vertices[i++], Vector3.Zero, Vector2.Zero);
                        verts[ngv++] = new VertexPositionNormalTexture(vertices[i++], Vector3.Zero, Vector2.Zero);
                        if (ngv >= MaxVertexBufferSize - 2 || i >= pointCount - 1)
                        {
                            _vertexBuffer3D.SetData(verts);
                            _graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, vertices.Length >> 1);
                            ngv = 0;
                        }
                    }
                }
            }
        }

        public void DrawGrid3D(Vector3 spacing, float majorDelta, int majorColor, int minorColor)
        {
            var bmin = new Vector3(-500, -500, -500);
            var bmax = new Vector3(500, 500, 500);
            
            var estimate = (int) Math.Ceiling((bmax.X - bmin.X) / spacing.X + (bmax.Y - bmin.Y) / spacing.Y) * 2;
            var gridVertices = new Vector3[estimate];
            for (var t = 0; t < 2; t++)
            {
                var count = 0;

                var x = 0f;
                var dx = spacing.X + spacing.X * majorDelta * t;
                while (x < bmax.X)
                {
                    gridVertices[count++] = new Vector3(x, 0, bmin.Y);
                    gridVertices[count++] = new Vector3(x, 0, bmax.Y);
                    gridVertices[count++] = new Vector3(-x, 0, bmin.Y);
                    gridVertices[count++] = new Vector3(-x, 0, bmax.Y);
                    x += dx;
                }

                var y = 0f;
                var dy = spacing.Y + spacing.Y * majorDelta * t;
                while (y < bmax.Y)
                {
                    gridVertices[count++] = new Vector3(bmin.X, 0, y);
                    gridVertices[count++] = new Vector3(bmax.X, 0, y);
                    gridVertices[count++] = new Vector3(bmin.X, 0, -y);
                    gridVertices[count++] = new Vector3(bmax.X, 0, -y);
                    y += dy;
                }

                DrawLines3D(gridVertices, t == 0 ? minorColor : majorColor, count);
            }
        }

        ~DisplayManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            _vertexBuffer3D?.Dispose();
            _vertexBuffer2D?.Dispose();
        }
    }
}