using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (!string.IsNullOrEmpty(name))
                rigidbody.name = name;
            else
                rigidbody.name = RandomString(5);
            rigidbody.aabb.owner = rigidbody;
            SAP.sapList.Add(rigidbody.aabb);
            return rigidbody;
        }

        public static void Delete(Rigidbody rb)
        {
            objectList.Remove(rb);
            SAP.sapList.Remove(rb.aabb);
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
