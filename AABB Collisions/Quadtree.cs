using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
    public class Quadtree
    {

        private int MAX_OBJECTS = 1;
        private int MAX_LEVELS = 5;

        private int level;
        private List<Rigidbody> objects;
        public AABB bounds;
        public Quadtree[] nodes;

        /*
         * Constructor
         */
        public Quadtree(int pLevel, AABB pBounds)
        {
            level = pLevel;
            objects = new List<Rigidbody>();
            bounds = pBounds;
            nodes = new Quadtree[4];
        }

        public void Clear()
        {
            objects.Clear();

            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }
        }


        private void Split()
        {
            int subWidth = (int)(bounds.max.X / 2);
            int subHeight = (int)(bounds.max.Y / 2);
            int x = (int)bounds.min.X;
            int y = (int)bounds.min.Y;

            nodes[0] = new Quadtree(level + 1, new AABB(x + subWidth, y, subWidth, subHeight));
            nodes[1] = new Quadtree(level + 1, new AABB(x, y, subWidth, subHeight));
            nodes[2] = new Quadtree(level + 1, new AABB(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new Quadtree(level + 1, new AABB(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        private int GetIndex(Rigidbody pRect)
        {
            int index = -1;
            double verticalMidpoint = bounds.min.X + (bounds.max.X / 2);
            double horizontalMidpoint = bounds.min.Y + (bounds.max.Y / 2);

            // Object can completely fit within the top quadrants
            bool topQuadrant = (pRect.aabb.min.Y < horizontalMidpoint && pRect.aabb.min.Y + pRect.aabb.max.Y < horizontalMidpoint);
            // Object can completely fit within the bottom quadrants
            bool bottomQuadrant = (pRect.aabb.min.Y > horizontalMidpoint);

            // Object can completely fit within the left quadrants
            if (pRect.aabb.min.X < verticalMidpoint && pRect.aabb.min.X + pRect.aabb.max.X < verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 1;
                }
                else if (bottomQuadrant)
                {
                    index = 2;
                }
            }
            // Object can completely fit within the right quadrants
            else if (pRect.aabb.min.X > verticalMidpoint)
            {
                if (topQuadrant)
                {
                    index = 0;
                }
                else if (bottomQuadrant)
                {
                    index = 3;
                }
            }

            return index;
        }

        /*
 * Insert the object into the quadtree. If the node
 * exceeds the capacity, it will split and add all
 * objects to their corresponding nodes.
 */
        public void insert(Rigidbody pRect)
        {
            if (nodes[0] != null)
            {
                int index = GetIndex(pRect);

                if (index != -1)
                {
                    nodes[index].insert(pRect);

                    return;
                }
            }

            objects.Add(pRect);

            if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
            {
                if (nodes[0] == null)
                {
                    Split();
                }

                int i = 0;
                while (i < objects.Count)
                {
                    int index = GetIndex(objects[i]);
                    if (index != -1)
                    {
                        nodes[index].insert(objects[i]);
                        objects.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        public List<Rigidbody> Retrieve(List<Rigidbody> returnObjects, Rigidbody pRect)
        {
            int index = GetIndex(pRect);
            if (index != -1 && nodes[0] != null)
            {
                nodes[index].Retrieve(returnObjects, pRect);
            }

            returnObjects.AddRange(objects);

            return returnObjects;
        }


    }
}
