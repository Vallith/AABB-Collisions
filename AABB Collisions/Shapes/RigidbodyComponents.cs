using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
    /// <summary>
    /// Struct to store Mass Data
    /// </summary>
    public struct MassData
    {
        public float inverseMass;
        public float mass;

        public MassData(float mass)
        {
            this.mass = mass;
            inverseMass = mass == 0 ? 0 : 1 / mass;
        }
    }
    /// <summary>
    /// Struct to store information about the material of an object
    /// </summary>
    public struct Material
    {
        public float restitution;
        public float density;

        public Material(float density, float restitution)
        {
            this.density = density;
            this.restitution = restitution;
        }
    }

    public struct Pair
    {
        Rigidbody a;
        Rigidbody b;

        public Pair(Rigidbody a, Rigidbody b)
        {
            this.a = a;
            this.b = b;
        }
    }

}
