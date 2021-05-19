using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
    public class Manifold
    {
        // CircleVsCircle = 0, 0
        // AABBvsAABB =     0, 1
        // AABBvsCircle =   1, 0

        static Func<Manifold, bool>[][] collisionMethods = new Func<Manifold, bool>[2][]
        {
                new Func<Manifold, bool>[]{
                    CollisionUtil.CircleVsCircle, CollisionUtil.AABBvsCircle
                },
                new Func<Manifold, bool>[]{
                    CollisionUtil.AABBvsAABB, CollisionUtil.AABBvsAABB
                },
        };

        public Rigidbody objectA;
        public Rigidbody objectB;
        public float penetration;
        public Vector2 normal;

        public Manifold(Rigidbody objectA, Rigidbody objectB)
        {
            this.objectA = objectA;
            this.objectB = objectB;
        }

        public void Solve()
        {
            bool result = collisionMethods[(int)objectA.shape][(int)objectB.shape](this);
            if (result)
            {

                CollisionUtil.ResolveCollision(objectA, objectB);
            }
            Console.WriteLine(result);
        }

    }
}
