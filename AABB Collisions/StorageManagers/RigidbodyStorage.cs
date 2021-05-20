using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
    public static class RigidbodyStorage
    {

        public static Dictionary<Rigidbody, Texture2D> objectList = new Dictionary<Rigidbody, Texture2D>();

        public static List<Rigidbody> bodies = new List<Rigidbody>();

        public static T Create<T>(T rigidbody, string name = "") where T: Rigidbody
        {
            objectList.Add(rigidbody, rigidbody.CreateTexture(rigidbody.color));
            bodies.Add(rigidbody);
            rigidbody.name = name;
            return rigidbody;
        }

    }
}
