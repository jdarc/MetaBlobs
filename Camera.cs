using Microsoft.Xna.Framework;

namespace MetaBlobs
{
    public sealed class Camera
    {
        public Camera(float fov, float aspectRatio, float near, float far)
        {
            FieldOfView = fov;
            AspectRatio = aspectRatio;
            NearPlane = near;
            FarPlane = far;
            Up = Vector3.Up;
        }

        public Vector3 Position { get; set; }

        public Vector3 Target { get; set; }

        public Vector3 Up { get; set; }

        public float FieldOfView { get; set; }

        public float AspectRatio { get; set; }

        public float NearPlane { get; set; }

        public float FarPlane { get; set; }

        public Matrix View => Matrix.CreateLookAt(Position, Target, Up);

        public Matrix Projection => Matrix.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearPlane, FarPlane);
    }
}