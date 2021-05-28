using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AABB_Collisions
{

    public struct AABB2
    {
       public float xMin;
       public float yMin;

       public float xMax;
       public float yMax;


        public Vector2 TopLeft => new Vector2(xMin, yMin);
        public Vector2 TopRight => new Vector2(xMax, yMin);
        public Vector2 BottomLeft => new Vector2(xMin, yMax);
        public Vector2 BottomRight => new Vector2(xMax, yMax);
    }
    //public static class SAP
    //{
    //    public static List<AABB2> sapList = new List<AABB2>();
    //    static AABB2Comparer comparer;

    //    public static AABB2 RandomAABB()
    //    {
    //        var rand = Game1.instance.rand;
    //        int sizeX = rand.Next(20, Screen.width / 16);
    //        int sizeY = rand.Next(20, Screen.height / 16);
    //        int posX = rand.Next(sizeX, Screen.width - sizeX);
    //        int posY = rand.Next(sizeY, Screen.height - sizeY);

    //        return new AABB2() { xMin = posX - sizeX, yMin = posY - sizeY, xMax = posX + sizeX, yMax = posY + sizeY };
    //    }
    //    public static void Generate()
    //    {
    //        for (int i = 0; i < 100; i++)
    //        {
    //            sapList.Add(RandomAABB());
    //        }
    //    }
    //    public static void Sort()
    //    {
    //        sapList.Sort(comparer);
    //        int j;
    //        AABB2 temp;
    //        for (int i = 1; i <= sapList.Count - 1; i++)
    //        {
    //            temp = sapList[i];
    //            j = i - 1;
    //            while (j >= 0 && sapList[j].xMin > temp.xMin)
    //            {
    //                sapList[j + 1] = sapList[j];
    //                j--;
    //            }
    //            sapList[j + 1] = temp;
    //        }
    //    }

    //    public static void Draw()
    //    {
    //        int index = 0;
    //        foreach (var item in sapList)
    //        {
    //            if (index > 1000)
    //            {
    //                return;
    //            }
    //            //DrawAABB2(item);
    //            Vector2 dsPos = new Vector2(item.xMin, 400);
    //            Screen.Draw.DrawString(Game1.instance.defaultFont, index.ToString(), dsPos, Color.Navy);
    //            Screen.Draw.DrawLine(dsPos, new Vector2(item.xMin, Screen.width), Color.Red);
    //            index++;

    //        }
    //    }

    //    public static void DrawAABB(AABB aabb)
    //    {
    //        Game1.instance._spriteBatch.DrawPoint(aabb.max, Color.Red, 10);
    //        Game1.instance._spriteBatch.DrawPoint(aabb.min, Color.Blue, 10);
    //        Game1.instance._spriteBatch.DrawLine(aabb.min, new Vector2(aabb.max.X, aabb.min.Y), Color.Red);
    //        Game1.instance._spriteBatch.DrawLine(new Vector2(aabb.max.X, aabb.min.Y), aabb.max, Color.Red);
    //        Game1.instance._spriteBatch.DrawLine(aabb.min, new Vector2(aabb.min.X, aabb.max.Y), Color.Red);
    //        Game1.instance._spriteBatch.DrawLine(new Vector2(aabb.min.X, aabb.max.Y), aabb.max, Color.Red);
    //    }

    //    public static void DrawAABB2(AABB2 aabb)
    //    {
    //        Game1.instance._spriteBatch.DrawLine(aabb.TopLeft, aabb.TopRight, Color.Red);
    //        Game1.instance._spriteBatch.DrawLine(aabb.TopRight, aabb.BottomRight, Color.Red);
    //        Game1.instance._spriteBatch.DrawLine(aabb.BottomRight,aabb.BottomLeft, Color.Red);
    //        Game1.instance._spriteBatch.DrawLine(aabb.BottomLeft, aabb.TopLeft, Color.Red);
    //    }

    //}

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
        static AABBComparer comparer;

        public static AABB RandomAABB()
        {
            var rand = Game1.instance.rand;
            int sizeX = rand.Next(20, Screen.width / 16);
            int sizeY = rand.Next(20, Screen.height / 16);
            int posX = rand.Next(sizeX, Screen.width - sizeX);
            int posY = rand.Next(sizeY, Screen.height - sizeY);

            return AABB.CreateRectAABB(new Vector2(posX, posY), sizeX, sizeY);
        }
        public static void Generate()
        {
            for (int i = 0; i < 100; i++)
            {
                sapList.Add(RandomAABB());
            }
        }
        public static void Sweep()
        {
            //sapList.Sort(comparer);
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

                //Console.WriteLine($"COLLISION {item.rb1.name} & {item.rb2.name}");
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
                DrawAABB(item);
                Vector2 dsPos = new Vector2(item.min.X, 400);
                Screen.Draw.DrawString(Game1.instance.defaultFont, index.ToString(), item.TopLeft - Vector2.UnitY * 20, Color.Navy);
                //Screen.Draw.DrawLine(dsPos, new Vector2(item.min.X, Screen.width), Color.Red);
                index++;
            }
        }

        public static void DrawAABB(AABB aabb)
        {
            //Game1.instance._spriteBatch.DrawPoint(aabb.max, Color.Red, 10);
            //Game1.instance._spriteBatch.DrawPoint(aabb.min, Color.Blue, 10);
            Game1.instance._spriteBatch.DrawLine(aabb.min, new Vector2(aabb.max.X, aabb.min.Y), Color.Red);
            Game1.instance._spriteBatch.DrawLine(new Vector2(aabb.max.X, aabb.min.Y), aabb.max, Color.Red);
            Game1.instance._spriteBatch.DrawLine(aabb.min, new Vector2(aabb.min.X, aabb.max.Y), Color.Red);
            Game1.instance._spriteBatch.DrawLine(new Vector2(aabb.min.X, aabb.max.Y), aabb.max, Color.Red);
        }

        public static void DrawAABB2(AABB2 aabb)
        {
            Game1.instance._spriteBatch.DrawLine(aabb.TopLeft, aabb.TopRight, Color.Red);
            Game1.instance._spriteBatch.DrawLine(aabb.TopRight, aabb.BottomRight, Color.Red);
            Game1.instance._spriteBatch.DrawLine(aabb.BottomRight, aabb.BottomLeft, Color.Red);
            Game1.instance._spriteBatch.DrawLine(aabb.BottomLeft, aabb.TopLeft, Color.Red);
        }

    }


    public class AABBComparer : IComparer<AABB>
    {
        public int Compare([AllowNull] AABB x, [AllowNull] AABB y)
        {
            return (int)(x.min.X - y.min.X);
        }
    }

    public struct AABB2Comparer : IComparer<AABB2>
    {
        public int Compare(AABB2 x,AABB2 y)
        {
            return (int)(x.xMin - y.xMin);
        }
    }
}
