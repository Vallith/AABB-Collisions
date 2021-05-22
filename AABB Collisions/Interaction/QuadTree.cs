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

        public QuadTree[] nodes;

        public int level;

        public QuadTree topLeft;
        public QuadTree topRight;
        public QuadTree bottomLeft;
        public QuadTree bottomRight;

        public QuadTree(AABB boundary, int capacity, int level)
        {
            this.boundary = boundary;
            this.capacity = capacity;
            this.level = level;
            rigidbodies = new List<Rigidbody>();
        }

        public void Subdivide()
        {
            isDivided = true;

            AABB tlAABB = new AABB(new Vector2(boundary.Pos.X - boundary.Size.X / 4, boundary.Pos.Y - boundary.Size.Y / 4), boundary.Size / 2);
            AABB trAABB = new AABB(new Vector2(boundary.Pos.X + boundary.Size.X / 4, boundary.Pos.Y - boundary.Size.Y / 4), boundary.Size / 2);
            AABB blAABB = new AABB(new Vector2(boundary.Pos.X - boundary.Size.X / 4, boundary.Pos.Y + boundary.Size.Y / 4), boundary.Size / 2);
            AABB brAABB = new AABB(new Vector2(boundary.Pos.X + boundary.Size.X / 4, boundary.Pos.Y + boundary.Size.Y / 4), boundary.Size / 2);

            topLeft = new QuadTree(tlAABB, capacity, level + 1);
            topRight = new QuadTree(trAABB, capacity, level + 1);
            bottomLeft = new QuadTree(blAABB, capacity, level + 1);
            bottomRight = new QuadTree(brAABB, capacity, level + 1);

            nodes = new QuadTree[4];

            nodes[0] = topLeft;
            nodes[1] = topRight;
            nodes[2] = bottomLeft;
            nodes[3] = bottomRight;
        }

        public void Insert(Rigidbody rb)
        {
            // Tree has divided
            if (isDivided)
            {
                int index = GetIndex(rb);

                if (index != -1)
                {
                    // Keep moving node down the tree until it reaches a
                    // node which hasn't divided. (Leaf node)
                    rb.collisionIndex = index;
                    nodes[index].Insert(rb);
                    return;
                }
            }

            rigidbodies.Add(rb);

            if (rigidbodies.Count > capacity)
            {

                if (nodes == null)
                {
                    Subdivide();
                }

                int i = 0;
                while (i < rigidbodies.Count)
                {
                    int index = GetIndex(rigidbodies[i]);
                    if (index != -1)
                    {
                        rigidbodies[i].collisionIndex = index;
                        nodes[index].Insert(rigidbodies[i]);
                        rigidbodies.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

            }
        }
        public int GetIndex(Rigidbody rb)
        {

            // TOP RIGHT
            if (rb.pos.X > boundary.Pos.X && rb.pos.Y < boundary.Pos.Y)
            {
                return 1;
            }

            // BOTTOM RIGHT
            if (rb.pos.X > boundary.Pos.X && rb.pos.Y > boundary.Pos.Y)
            {
                return 2;
            }

            // TOP LEFT
            if (rb.pos.X < boundary.Pos.X && rb.pos.Y < boundary.Pos.Y)
            {
                return 0;
            }

            // BOTTOM LEFT
            if (rb.pos.X < boundary.Pos.X && rb.pos.Y > boundary.Pos.Y)
            {
                return 3;
            }
            return -1;
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
            foreach (var item in rigidbodies)
            {
                Screen.Draw.DrawLine(item.pos, boundary.Pos, Color.White, 2);
                Screen.Draw.DrawString(Game1.instance.defaultFont, level.ToString(), new Vector2(item.pos.X, item.pos.Y + 20), Color.DarkGreen);
            }
            rigidbodies.Clear();
            isDivided = false;
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i] != null)
                    {
                        nodes[i].ClearTree();
                        nodes[i] = null;
                    }
                }
            }
            nodes = null;
            topLeft = null;
            topRight = null;
            bottomLeft = null;
            bottomRight = null;
        }

    }
}
