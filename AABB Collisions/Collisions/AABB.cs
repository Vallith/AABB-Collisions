using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AABB_Collisions
{
    public class AABB
    {
        // Min is top left
        public Vector2 min;

        // Max is bottom right
        public Vector2 max;

        public Vector2 TopLeft => new Vector2(min.X, min.Y);
        public Vector2 TopRight => new Vector2(max.X, min.Y);
        public Vector2 BottomLeft => new Vector2(min.X, max.Y);
        public Vector2 BottomRight => new Vector2(max.X, max.Y);

        public Rigidbody owner;

        public AABB(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public AABB()
        {

        }
        /// <summary>
        /// Returns whether or not the given Vector2 is within the instance AABB
        /// </summary>
        public bool IsInside(Vector2 vec2)
        {
            return (vec2.X > min.X && vec2.X < max.X && vec2.Y > min.Y && vec2.Y < max.Y);
        }

        public bool Intersects(AABB b)
        {
            return (min.X <= b.max.X && max.X >= b.min.X) && (min.Y <= b.max.Y && max.Y >= b.min.Y);
        }

        /// <summary>
        /// Returns whether or not the given Vector2 is within the AABB of a circle
        /// </summary>
        public static bool InsideCircle(Vector2 point, Vector2 centre, float radius)
        { 
            return ((point.X - centre.X) * (point.X - centre.X) + (point.Y - centre.Y) * (point.Y - centre.Y)) < radius * radius;
        }
        /// <summary>
        /// Returns whether or not the given Vector2 is within the supplied min and max Vectors
        /// </summary>
        public static bool InsideSquare(Vector2 min, Vector2 max, Vector2 point)
        {
            return (point.X > min.X && point.X < max.X && point.Y > min.Y && point.Y < max.Y);
        }

        /// <summary>
        /// Returns an AABB for a Circle
        /// </summary>
        public static AABB CreateCircleAABB(Vector2 pos, float radius)
        {
            return new AABB(new Vector2(pos.X - radius, pos.Y - radius), new Vector2(pos.X + radius, pos.Y + radius));
        }

        /// <summary>
        /// Returns an AABB for a RigidRect
        /// </summary>
        public static AABB CreateRectAABB(Vector2 pos, int width, int height)
        {
            int halfWidth = width / 2;
            int halfHeight = height / 2;

            Vector2 topLeft = new Vector2(pos.X - halfWidth, pos.Y - halfHeight);
            Vector2 bottomRight = new Vector2(pos.X + halfWidth, pos.Y + halfHeight);

            return new AABB(topLeft, bottomRight);
        }

    }
}
