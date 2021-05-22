using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AABB_Collisions
{
    public class QuadTree
    {

        public AABB boundary;
        public int capacity;
        public bool isDivided;
        public List<Rigidbody> rigidbodies;

        public QuadTree topLeft;
        public QuadTree topRight;
        public QuadTree bottomLeft;
        public QuadTree bottomRight;

        public QuadTree(AABB boundary, int capacity)
        {
            this.boundary = boundary;
            this.capacity = capacity;
            rigidbodies = new List<Rigidbody>();
        }

        public void Subdivide()
        {

            AABB tlAABB = new AABB(new Vector2(boundary.Pos.X - boundary.Size.X / 4, boundary.Pos.Y - boundary.Size.Y / 4), boundary.Size / 2);
            AABB trAABB = new AABB(new Vector2(boundary.Pos.X + boundary.Size.X / 4, boundary.Pos.Y - boundary.Size.Y / 4), boundary.Size / 2);
            AABB blAABB = new AABB(new Vector2(boundary.Pos.X - boundary.Size.X / 4, boundary.Pos.Y + boundary.Size.Y / 4), boundary.Size / 2);
            AABB brAABB = new AABB(new Vector2(boundary.Pos.X + boundary.Size.X / 4, boundary.Pos.Y + boundary.Size.Y / 4), boundary.Size / 2);

            topLeft = new QuadTree(tlAABB, capacity);
            topRight = new QuadTree(trAABB, capacity);
            bottomLeft = new QuadTree(blAABB, capacity);
            bottomRight = new QuadTree(brAABB, capacity);

        }

        public bool Insert(Rigidbody rb)
        {
            // If rb is not within this cells AABB
            if (!boundary.IsInside(rb.pos))
                // Don't bother
                return false;

            // If there is room
            if (rigidbodies.Count < capacity)
            {
                // Add the particle to this QuadTree
                rigidbodies.Add(rb);
                return false;
            }
            else
            {
                // Otherwise, if we *haven't* divided
                if (!isDivided)
                {
                    // Divide
                    Subdivide();
                    // Say we have divided
                    isDivided = true;
                }
                return topLeft.Insert(rb)
                || topRight.Insert(rb)
                || bottomLeft.Insert(rb)
                || bottomRight.Insert(rb);
            }
        }

        public void DrawTree()
        {
            Game1.instance._spriteBatch.DrawLine(boundary.TopLeft, new Vector2(boundary.BottomRight.X, boundary.TopLeft.Y), Color.Red);
            Game1.instance._spriteBatch.DrawLine(new Vector2(boundary.BottomRight.X, boundary.TopLeft.Y), boundary.BottomRight, Color.Red);
            Game1.instance._spriteBatch.DrawLine(boundary.TopLeft, new Vector2(boundary.TopLeft.X, boundary.BottomRight.Y), Color.Red);
            Game1.instance._spriteBatch.DrawLine(new Vector2(boundary.TopLeft.X, boundary.BottomRight.Y), boundary.BottomRight, Color.Red);
            topLeft?.DrawTree();
            topRight?.DrawTree();
            bottomLeft?.DrawTree();
            bottomRight?.DrawTree();
        }

        public void ClearTree()
        {
            rigidbodies.Clear();
            topLeft?.ClearTree();
            topRight?.ClearTree();
            bottomLeft?.ClearTree();
            bottomRight?.ClearTree();
            topLeft = null;
            topRight = null;
            bottomLeft = null;
            bottomRight = null;
            isDivided = false;
        }

    }
}
