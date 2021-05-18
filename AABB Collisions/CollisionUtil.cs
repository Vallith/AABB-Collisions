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

        public static bool CircleVCircle(Circle a, Circle b)
        {
            float r = a.radius + b.radius;
            r *= r;
            float aPosX = a.pos.X;
            float aPosY = a.pos.Y;
            float bPosX = b.pos.X;
            float bPosY = b.pos.Y;

            return r < (aPosX + bPosX) * (aPosX + bPosX) + (aPosY + bPosY) * (aPosY + bPosY);
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
