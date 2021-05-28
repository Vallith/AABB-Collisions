using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AABB_Collisions
{

    public struct CollisionPair
    {
        public bool resolved;
        public Rigidbody rb1;
        public Rigidbody rb2;
    }


    public static class SAP
    {
        public static List<AABB> sapList = new List<AABB>();
        public static List<CollisionPair> collisions = new List<CollisionPair>();

        public static void Sweep()
        {
            int j;
            AABB temp;
            for (int i = 1; i <= sapList.Count - 1; i++)
            {
                temp = sapList[i];
                j = i - 1;
                while (j >= 0 && sapList[j].min.X > temp.min.X)
                {
                    sapList[j + 1] = sapList[j];
                    j--;
                }
                sapList[j + 1] = temp;
            }
        }

        static Queue<CollisionPair> collisionList = new Queue<CollisionPair>();
        public static void Prune()
        {
            List<AABB> activeList = new List<AABB>();
            List<AABB> queue = new List<AABB>();
            
            for (int i = 0; i < sapList.Count; i++)
            {
                for (int j = 0; j < activeList.Count; j++)
                {
                    if (sapList[i].min.X <= activeList[j].max.X)
                    {
                        collisionList.Enqueue(new CollisionPair() { rb1 = sapList[i].owner, rb2 = activeList[j].owner });
                    }
                    else
                    {
                        activeList.RemoveAt(j);
                        j--;
                    }
                }

                activeList.Add(sapList[i]);
            }
        }

        public static void CollisionPass()
        {
            while (collisionList.Count > 0)
            {
                var pair = collisionList.Dequeue();
                if (pair.rb1.massData.inverseMass == 0 && pair.rb2.massData.inverseMass == 0)
                    continue;

                if (pair.rb1.aabb.Intersects(pair.rb2.aabb))
                {
                    Manifold m = new Manifold(pair.rb1, pair.rb2);
                    m.Solve();
                    Game1.instance.collisionCount++;
                }
            }
        }

        public static void Draw()
        {
            int index = 0;
            foreach (var item in sapList)
            {
                if (index > 1000)
                {
                    return;
                }
                Vector2 dsPos = new Vector2(item.min.X, 400);
                Screen.Draw.DrawString(Game1.instance.defaultFont, index.ToString(), item.TopLeft - Vector2.UnitY * 20, Color.Navy);
                index++;
            }
        }
    }
}
