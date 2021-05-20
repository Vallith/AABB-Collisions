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
        // AABBvsCircle =   0, 1
        // CirclevsAABB =   1, 0
        // AABBvsAABB =      1, 1

        static Func<Manifold, bool>[][] collisionMethods = new Func<Manifold, bool>[2][]
        {
                new Func<Manifold, bool>[]{
                    CollisionUtil.CircleVsCircle, CollisionUtil.AABBvsCircle
                },
                new Func<Manifold, bool>[]{
                    CollisionUtil.CirclevsAABB, CollisionUtil.AABBvsAABB
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
                Console.WriteLine($"Collision: {objectA.shape}:{objectA.color} and {objectB.shape}:{objectB.color}");
                CollisionUtil.ResolveCollision(this);
                PositionalCorrection();
            }
        }

        public void PositionalCorrection()
        {
            const float percentage = 0.2f; // Penetration percentage to correct
            const float slop = 0.01f;
            Extensions.Vector2Normalise(normal, out normal);
            Vector2 correction = new Vector2(0, Math.Max(penetration - slop, 0.0f) / (objectA.massData.inverseMass + objectB.massData.inverseMass)) * percentage * normal;
            objectA.pos += objectA.massData.inverseMass * correction;
            objectB.pos -= objectB.massData.inverseMass * correction;
        }

    }
}
