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

        /// <summary>
        /// Creates a rigidbody and adds it to the object list, also setting it's name
        /// </summary>
        public static T Create<T>(T rigidbody, string name = "") where T: Rigidbody
        {
            objectList.Add(rigidbody, rigidbody.CreateTexture(rigidbody.color));
            rigidbody.name = name;
            return rigidbody;
        }

    }
}
