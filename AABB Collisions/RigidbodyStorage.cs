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

        public static T Create<T>(T rigidbody) where T: Rigidbody
        {
            objectList.Add(rigidbody, rigidbody.CreateTexture(rigidbody.color));
            return rigidbody;
        }

    }
}
