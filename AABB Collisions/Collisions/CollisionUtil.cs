using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AABB_Collisions
{
    public static class CollisionUtil
    {
        /// <summary>
        /// Narrow phase collision between a Circle and a Circle
        /// </summary>
        public static bool CirclevsCircle(Manifold m)
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
        public static bool RectvsRect(Manifold m)
        {
            //Console.WriteLine("AABBvsAABB");
            RigidRect a = (RigidRect)m.objectA;
            RigidRect b = (RigidRect)m.objectB;

            // Vector from A to B
            Vector2 n = b.pos - a.pos;

            AABB abox = a.aabb;
            AABB bbox = b.aabb;

            // Calculate half extents along x axis for each object
            float aExtentX = (abox.max.X - abox.min.X) / 2;
            float bExtentX = (bbox.max.X - bbox.min.X) / 2;

            // Calculate overlap on x axis
            float xOverlap = aExtentX + bExtentX - Math.Abs(n.X);

            // SAT test on x axis
            if (xOverlap > 0)
            {
                // Calculate half extents along y axis for each object
                float aExtentY = (abox.max.Y - abox.min.Y) / 2;
                float bExtentY = (bbox.max.Y - bbox.min.Y) / 2;

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
        /// Narrow phase collision between Circle and RigidRect
        /// </summary>
        public static bool CirclevsRect(Manifold m)
        {
            Rigidbody temp = m.objectA;
            m.objectA = m.objectB;
            m.objectB = temp;
            bool result = RectvsCircle(m);

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
        /// Narrow phase collision between a RigidRect and a Circle
        /// </summary>
        public static bool RectvsCircle(Manifold m)
        {
            RigidRect a = (RigidRect)m.objectB;
            Circle b = (Circle)m.objectA;

            // Vector from A to B
            Vector2 n = b.pos - a.pos;
            // Closest point on A to center of B
            Vector2 closest = n;

            // Calculate half extents along each axis
            float xExtentA = (a.aabb.max.X - a.aabb.min.X) / 2;
            float yExtentA = (a.aabb.max.Y - a.aabb.min.Y) / 2;

            // Clamp point to edges of the AABB
            closest.X = Math.Clamp(closest.X, -xExtentA, xExtentA);
            closest.Y = Math.Clamp(closest.Y, -yExtentA, yExtentA);

            //DrawQueue.Add(DrawLine.Create(b.pos, a.pos,Color.White));
            //DrawQueue.Add(DrawPoint.Create(closest + a.pos, Color.Sienna));
            bool inside = false;

            // Circle is inside the AABB, so we need to clamp
            // the Circle's center to the closest edge
            if (n == closest)
            {
                inside = true;

                //Draw circle around our circle if it is inside AABB
                //DrawQueue.Add(DrawCircle.Create(b.pos,b.radius, Color.LawnGreen));

                // Find closest axis
                if (Math.Abs(n.X) > Math.Abs(n.Y))
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

            Vector2 diff = a.pos + closest - b.pos;

            // Collision normal needs to be flipped to point outside
            // if the circle was inside the AABB
            if (inside)
            {
                m.normal = -normal;
            }
            else
            {
                m.normal = normal;
            }

            m.penetration = b.radius - diff.Length();

            Extensions.Vector2Normalise(m.normal, out m.normal);

            //Draw debug normals and penetration vectors ( ͡° ͜ʖ ͡°) 
            //DrawQueue.Add(DrawLine.Create(closest + a.pos, closest + a.pos + m.normal * 30, Color.BlueViolet));
            //DrawQueue.Add(DrawLine.Create(closest + a.pos, closest + a.pos + -m.normal * m.penetration, Color.Red));

            return true;
        }

        /// <summary>
        /// Calculates impulse to apply to the 2 objects which are colliding
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
