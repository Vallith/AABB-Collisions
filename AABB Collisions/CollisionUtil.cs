using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AABB_Collisions
{
    public static class CollisionUtil
    {
        static Vector2 normal = new Vector2(1, 0);

        public static bool CirclevsCircleUnoptimized(Circle a, Circle b)
        {
            float r = a.radius + b.radius;
            float d = Vector2.Distance(a.pos, b.pos);
            return r > d;
        }

        // This is called when the program loops over every object and decides to test a collision, and only THEN is the Manifold generated between
        // 2 potentially colliding objects.
        public static bool CircleVsCircle(Manifold<Circle> m)
        {
            Circle a = m.objectA;
            Circle b = m.objectB;

            // Normal vector for 2 circles colliding is just the vector from A to B.
            Vector2 normal = b.pos - a.pos;

            float radiusSum = a.radius + b.radius;
            radiusSum *= radiusSum;

            // If the normal is longer than the radiusSum
            if (normal.LengthSquared() > radiusSum)
                // Then we aren't colliding
                return false;

            // Circles have collided, now compute manifold
            float dist = normal.Length();

            // If distance between circles is not zero (If they're not in the same position)
            if (dist != 0)
            {
                // Penetration distance is difference between radius and distance
                m.penetration = radiusSum - dist;

                // Points from A to B, and is a unit vector
                // NOT SURE ON THIS PART, TYPO IN GUIDE?
                m.normal = normal / dist;
                return true;
            }
            // Circles are in the same position
            else
            {
                // Choose random (but consistent) values
                m.penetration = a.radius;
                m.normal = new Vector2(1, 0);
                return true;
            }

        }

        public static void ResolveCollision(Circle a, Circle b)
        {
            // Calculate relativeVelocity
            Vector2 relativeVelocity = b.vel - a.vel;

            Vector2.Normalize(ref relativeVelocity, out normal);

            // Calculate relativeVelocity in terms of the normal direction
            float velAlongNormal = Vector2.Dot(relativeVelocity, normal);

            // If velocities are separating
            if (velAlongNormal < 0) 
                return;

            // Take min restitution out of 2 circles
            float e = Math.Min(a.restitution, b.restitution);

            // Calculate impulse scalar
            float j = -(1 + e) * velAlongNormal;
            j = j / (a.inverseMass + b.inverseMass);

            // Apply impulse
            Vector2 impulse = j * normal;

            a.vel -= a.inverseMass * impulse;
            b.vel += b.inverseMass * impulse;

        }

    }
}
