using Microsoft.Xna.Framework;

namespace MetaBlobs
{
    public sealed class Ball
    {
        public Ball(Vector3 position, float radius)
        {
            Acceleration = new Vector3(0, 0, 0);
            Velocity = new Vector3(0, 0, 0);
            Position = position;
            Radius = radius;
        }

        public Vector3 Position { get; set; }

        public Vector3 Acceleration { get; set; }

        public Vector3 Velocity { get; set; }

        public float Radius { get; set; }

        public void Integrate(float dt)
        {
            Velocity += Acceleration * dt;
            Position += Velocity * dt;
        }
    }
}