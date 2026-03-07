using System.Collections.Generic;
using UnityEngine;

namespace AssemblyFramework
{
    [CreateAssetMenu]
    public class TagRegistry : ScriptableObject
    {
        public List<string> tags;
    }
}