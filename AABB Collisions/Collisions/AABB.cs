using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AABB_Collisions
{
    public class AABB
    {
        private Vector2 pos;

        private Vector2 size;
        public Vector2 halfSize;

        public Vector2 Size
        {
            get { return size; }
            set { size = value; halfSize = size / 2; }
        }

        public Vector2 Pos
        {
            get { return pos; }
            set { pos = value; }
        }


        public Vector2 TopLeft { get => new Vector2(pos.X - halfSize.X, pos.Y - halfSize.Y); }
        public Vector2 TopRight { get => new Vector2(pos.X + halfSize.X, pos.Y - halfSize.Y); }
        public Vector2 BottomLeft { get => new Vector2(pos.X - halfSize.X, pos.Y + halfSize.Y); }
        public Vector2 BottomRight { get => new Vector2(pos.X + halfSize.X, pos.Y + halfSize.Y); }

        public AABB(Vector2 pos, Vector2 size)
        {
            Pos = pos;
            Size = size;
        }

        public AABB()
        {

        }
        /// <summary>
        /// Returns whether or not the given Vector2 is within the instance AABB
        /// </summary>
        public bool IsInside(Vector2 vec2)
        {
            return (vec2.X > TopLeft.X && vec2.Y > TopLeft.Y && vec2.X < BottomRight.X && vec2.Y < BottomRight.Y);
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
            float diameter = radius * 2;
            return new AABB(pos, new Vector2(diameter, diameter));
        }

        /// <summary>
        /// Returns an AABB for a RigidRect
        /// </summary>
        public static AABB CreateRectAABB(Vector2 pos, int width, int height)
        {
            return new AABB(pos, new Vector2(width, height));
        }

    }
}
