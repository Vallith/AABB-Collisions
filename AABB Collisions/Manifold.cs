using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
    public class Manifold<T> where T: Rigidbody
    {

        public T objectA;
        public T objectB;
        public float penetration;
        public Vector2 normal;

        public Manifold(T objectA, T objectB)
        {
            this.objectA = objectA;
            this.objectB = objectB;
        }

    }
}
