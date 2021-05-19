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

        public static bool AABBvsAABB(AABB a, AABB b)
        {

            if (a.max.X < b.min.X || a.min.X > b.max.X) return false;
            if (a.max.Y < b.min.Y || a.min.Y > b.max.Y) return false;
            return true;
        }

    }
}
