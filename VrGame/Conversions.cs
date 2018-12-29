using Microsoft.Xna.Framework;

namespace MusicVizz.RendererApp
{
    public static class Conversions
    {
        public static Vector3 ToMg(this System.Numerics.Vector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vector2 ToMg(this System.Numerics.Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }
    }
}
