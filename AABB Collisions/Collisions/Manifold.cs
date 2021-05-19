using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
    public class Manifold<T1, T2> where T1 : Rigidbody where T2 : Rigidbody
    {

        public T1 objectA;
        public T2 objectB;
        public float penetration;
        public Vector2 normal;

        public Manifold(T1 objectA, T2 objectB)
        {
            this.objectA = objectA;
            this.objectB = objectB;
        }

    }
}
