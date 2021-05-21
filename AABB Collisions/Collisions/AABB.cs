using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AABB_Collisions
{
    public class AABB
    {

        public Vector2 min;
        public Vector2 max;

        public bool IsInside(Vector2 vec2)
        {
            return (vec2.X > min.X && vec2.X < max.X && vec2.Y > min.Y && vec2.Y < max.Y);
        }

        public static bool InsideCircle(Vector2 point, Vector2 centre, float radius)
        { 
            return ((point.X - centre.X) * (point.X - centre.X) + (point.Y - centre.Y) * (point.Y - centre.Y)) < radius * radius;
        }


        public static bool InsideSquare(Vector2 min, Vector2 max, Vector2 point)
        {
            return (point.X > min.X && point.X < max.X && point.Y > min.Y && point.Y < max.Y);
        }

    }
}
