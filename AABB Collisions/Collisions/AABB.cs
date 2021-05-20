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

    }
}
