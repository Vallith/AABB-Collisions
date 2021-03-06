using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
    public static class MaterialStorage
    {
        /// <summary>
        /// Defines the properties of a material type
        /// </summary>
        public static Dictionary<string, Material> materialTypes = new Dictionary<string, Material>()
        {
            {"Rock", new Material(0.6f, 0.1f)},
            {"Wood", new Material(0.3f, 0.2f)},
            {"Metal", new Material(1.2f, 0.05f)},
            {"BouncyBall", new Material(0.3f, 0.8f)},
            {"SuperBall", new Material(0.3f, 0.95f)},
            {"Pillow", new Material(0.1f, 0.2f)},
            {"Static", new Material(0.0f, 0.4f)},
            {"'Aerogel'", new Material(0.02f, 0.5f) }
        };
    }
}
