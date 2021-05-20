using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
    public static class Extensions
    {

        public static void Vector2Normalise(Vector2 vec2, out Vector2 vec2Out)
        {
            float len = vec2.Length();

            if (len > float.Epsilon)
            {
                float invLen = 1.0f / len;
                vec2.X *= invLen;
                vec2.Y = vec2.Y * invLen;
            }
            vec2Out = vec2;
        }

        public static float Vector2Dot(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

    }
}
