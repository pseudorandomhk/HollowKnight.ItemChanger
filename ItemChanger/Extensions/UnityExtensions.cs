﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemChanger.Extensions
{
    public static class UnityExtensions
    {
        public static PlayMakerFSM LocateFSM(this GameObject g, string name)
        {
            return FSMUtility.LocateFSM(g, name);
        }

        public static GameObject FindChild(this GameObject g, string name)
        {
            return g.transform.Find(name).gameObject;
        }

        public static GameObject FindChild(this GameObject g, IEnumerable<string> steps)
        {
            var t = g.transform;
            foreach (string s in steps) t = t.Find(s);
            return t.gameObject;
        }

        static readonly List<GameObject> rootObjects = new List<GameObject>(500);
        /// <summary>
        /// Finds a GameObject in the given scene by its full path.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="path">The full path to the GameObject, with forward slash ('/') separators.</param>
        /// <returns></returns>
        public static GameObject FindGameObject(this Scene s, string path)
        {
            s.GetRootGameObjects(rootObjects); // clears list

            int index = path.IndexOf('/');
            GameObject result = null;
            if (index >= 0)
            {
                string rootName = path.Substring(0, index);
                GameObject root = rootObjects.FirstOrDefault(g => g.name == rootName);
                if (root != null) result = root.transform.Find(path.Substring(index + 1)).gameObject;
            }
            else
            {
                result = rootObjects.FirstOrDefault(g => g.name == path);
            }

            rootObjects.Clear();
            return result;
        }

        /// <summary>
        /// Breadth first search through the entire hierarchy. Returns the first GameObject with the given name, or null.
        /// </summary>
        public static GameObject FindGameObjectByName(this Scene s, string name)
        {
            s.GetRootGameObjects(rootObjects);
            GameObject result = null;

            foreach (GameObject g in rootObjects)
            {
                if (g.name == name)
                {
                    result = g;
                    break;
                }
            }

            if (result == null)
            {
                foreach (GameObject g in rootObjects)
                {
                    result = g.FindChildInHierarchy(name);
                    if (result != null) break;
                }
            }

            rootObjects.Clear();
            return result;
        }

        /// <summary>
        /// Breadth first search. Returns GameObject with given name, or null if not found. Parent object not included in search.
        /// </summary>
        public static GameObject FindChildInHierarchy(this GameObject g, string name)
        {
            Queue<Transform> q = new Queue<Transform>();
            q.Enqueue(g.transform);

            while (q.Any())
            {
                Transform t = q.Dequeue();
                foreach (Transform u in t)
                {
                    if (u.name == name) return u.gameObject;
                    else q.Enqueue(u);
                }
            }

            return null;
        }
    }
}