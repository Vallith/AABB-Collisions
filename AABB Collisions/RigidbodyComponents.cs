using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
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

}
