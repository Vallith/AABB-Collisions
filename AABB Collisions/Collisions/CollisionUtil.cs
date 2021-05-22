using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AABB_Collisions
{
    public static class CollisionUtil
    {

        // This is called when the program loops over every object and decides to test a collision, and only THEN is the Manifold generated between
        // 2 potentially colliding objects.
        /// <summary>
        /// Narrow phase collision between a Circle and a Circle
        /// </summary>
        public static bool CircleVsCircle(Manifold m)
        {
            Circle a = (Circle)m.objectA;
            Circle b = (Circle)m.objectB;

            // Normal vector for 2 circles colliding is just the vector from A to B.
            Vector2 normal = b.pos - a.pos;

            float radiusSum = a.radius + b.radius;
            float radiusSumSquared = radiusSum * radiusSum;

            // If the normal is longer than the radiusSum
            if (normal.LengthSquared() > radiusSumSquared)
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
                m.normal = -normal / dist;
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

        // RigidRect will eventually become Polygon
        /// <summary>
        /// Narrow phase collision between a RigidRect and a RigidRect
        /// </summary>
        public static bool AABBvsAABB(Manifold m)
        {
            //Console.WriteLine("AABBvsAABB");
            RigidRect a = (RigidRect)m.objectA;
            RigidRect b = (RigidRect)m.objectB;

            // Vector from A to B
            Vector2 n = b.pos - a.pos;

            AABB abox = a.aabb;
            AABB bbox = b.aabb;

            // Calculate half extents along x axis for each object
            float aExtentX = (abox.BottomRight.X - abox.TopLeft.X) / 2;
            float bExtentX = (bbox.BottomRight.X - bbox.TopLeft.X) / 2;

            // Calculate overlap on x axis
            float xOverlap = aExtentX + bExtentX - Math.Abs(n.X);

            // SAT test on x axis
            if (xOverlap > 0)
            {
                // Calculate half extents along y axis for each object
                float aExtentY = (abox.BottomRight.Y - abox.TopLeft.Y) / 2;
                float bExtentY = (bbox.BottomRight.Y - bbox.TopLeft.Y) / 2;

                // Calculate overlap on y axis
                float yOverlap = aExtentY + bExtentY - Math.Abs(n.Y);

                // SAT test on y axis
                if (yOverlap > 0)
                {
                    // Find out which axis is axis of least penetration
                    if (xOverlap < yOverlap)
                    {
                        // Point towards B knowing that n points from A to B
                        if (n.X < 0)
                            m.normal = new Vector2(1, 0);
                        else
                            m.normal = new Vector2(-1, 0);

                        m.penetration = xOverlap;
                        return true;
                    }
                    else
                    {
                        // Point towards B knowing that N points from B to A
                        if (n.Y > 0)
                            m.normal = new Vector2(0, -1);
                        else
                            m.normal = new Vector2(0, 1);

                        m.penetration = yOverlap;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Narrow phase collision between a Circle and a RigidRect
        /// </summary>
        public static bool CirclevsAABB(Manifold m)
        {
            Rigidbody temp = m.objectA;
            m.objectA = m.objectB;
            m.objectB = temp;
            bool result = AABBvsCircle(m);

            temp = m.objectA;
            m.objectA = m.objectB;
            m.objectB = temp;

            if (result == true)
            {
                m.normal = -m.normal;
            }
            return result;
        }

        /// <summary>
        /// Reverses manifold objects for narrow phase collision between a Circle and a RigidRect
        /// </summary>
        public static bool AABBvsCircle(Manifold m)
        {
            //Console.WriteLine("AABBvsCircle");
            RigidRect a = (RigidRect)m.objectB;
            Circle b = (Circle)m.objectA;

            // Vector from A to B
            Vector2 n = b.pos - a.pos;

            // Closest point on A to center of B
            Vector2 closest = n;

            // Calculate half extents along each axis
            float xExtentA = (a.aabb.BottomRight.X - a.aabb.TopLeft.X) / 2;
            float yExtentA = (a.aabb.BottomRight.Y - a.aabb.TopLeft.Y) / 2;

            // Clamp point to edges of the AABB
            closest.X = Math.Clamp(closest.X, -xExtentA, xExtentA);
            closest.Y = Math.Clamp(closest.Y, -yExtentA, yExtentA);

            bool inside = false;

            // Circle is inside the AABB, so we need to clamp
            // the Circle's center to the closest edge
            if (n == closest)
            {
                inside = true;

                // Find closest axis
                if(Math.Abs(n.X) > Math.Abs(n.Y))
                {
                    // Clamp to closest extent
                    if (closest.X > 0)
                        closest.X = xExtentA;
                    else
                        closest.X = -xExtentA;
                }
                // y axis is shorter
                else
                {
                    // Clamp to closest extent
                    if (closest.Y > 0)
                        closest.Y = yExtentA;
                    else
                        closest.Y = -yExtentA;
                }
            }

            Vector2 normal = n - closest;

            float dist = normal.LengthSquared();
            float r = b.radius;

            // If distance out of the radius is shorter than distance to closest
            // point, and Circle not inside the AABB
            if (dist > r * r && !inside)
                return false;

            dist = (float)Math.Sqrt(dist);
            if (a == Game1.instance.selectedShape || b == Game1.instance.selectedShape)
            {

            }
            // Collision normal needs to be flipped to point outside
            // if the circle was inside the AABB
            if (inside)
            {
                m.normal = -normal;
                m.penetration = r - dist;
            }
            else
            {
                m.normal = normal;
                m.penetration = r - dist;
            }
            Extensions.Vector2Normalise(m.normal, out m.normal);
            return true;
        }

        /// <summary>
        /// Calculates impulse to apply to 2 objects which are colliding
        /// </summary>
        public static void ResolveCollision(Manifold m)
        {
            Rigidbody a = m.objectA;
            Rigidbody b = m.objectB;
            // Calculate relativeVelocity
            Vector2 relativeVelocity = b.vel - a.vel;

            // Calculate relativeVelocity in terms of the normal direction
            float velAlongNormal = Extensions.Vector2Dot(relativeVelocity, m.normal);

            // If velocities are separating
            if (velAlongNormal < 0) 
                return;

            // Take min restitution out of 2 circles
            float e = Math.Min(a.material.restitution, b.material.restitution);

            // Calculate impulse scalar
            float j = -(1 + e) * velAlongNormal;
            j = j / (a.massData.inverseMass + b.massData.inverseMass);

            // Apply impulse
            Vector2 impulse = j * m.normal;

            a.vel -= a.massData.inverseMass * impulse;
            b.vel += b.massData.inverseMass * impulse;
        }

    }
}
