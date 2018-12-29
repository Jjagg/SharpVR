using System;
using Microsoft.Xna.Framework;

namespace VrGame
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Up { get; set; } = Vector3.UnitZ;
        public Vector3 Forward { get; set; } = Vector3.Up;
        public float FieldOfView { get; set; } = (float) Math.PI/4;

        public Camera(Vector3 position, Vector3 lookat)
        {
            Position = position;
            Forward = lookat - position;
            Forward.Normalize();
        }

        public Vector3 Lookat
        {
            get => Position + Forward;
            set => Forward = Vector3.Normalize(value - Position);
        }
    }
}
