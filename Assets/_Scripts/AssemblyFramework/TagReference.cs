using AssemblyFramework;
using UnityEngine;

[System.Serializable]
public class TagReference
{
    public TagRegistry registry;
    public int selectedIndex;

    public string Value => registry != null && registry.tags.Count > selectedIndex
        ? registry.tags[selectedIndex] : null;
}