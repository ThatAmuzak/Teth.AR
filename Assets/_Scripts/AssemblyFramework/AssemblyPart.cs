using System;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using UnityEngine;

namespace AssemblyFramework
{
    public class AssemblyPart : MonoBehaviour
    {
        [Header("This Part")]
        public TagReference partTag;
        
        [Header("Relationships")]
        [SerializeField] private List<ChildPart> childParts;
        
        [Header("Snap Zones")]
        [SerializeField] private List<SnapZone> snapZones;
        
        [Header("Create New Snap Zones")]
        [SerializeField] private SnapZoneData zoneType;
        [Button(true, "zoneType")]
        public void AddSnapZone(SnapZoneData data)
        {
            snapZones.Clear();
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out SnapZone _snapZone))
                {
                    snapZones.Add(_snapZone);
                }
            }
            SnapZone snapZone = new GameObject("SnapZone").AddComponent<SnapZone>();
            snapZone.transform.SetParent(transform);
            snapZone.Init(data );
        }
        
    }
    
    [Serializable]
    public class ChildPart
    {
        public TagReference partTag;
        public Vector3 position;
        public Quaternion rotation;
    }

    [Serializable]
    public class SnapZoneData
    {
        public ZoneType zoneType;
        public TagReference partToAccept;
    }
    
    [Serializable]
    public enum ZoneType
    {
        Box = 0,
        Cylinder = 1,
        Sphere = 2
    }
    
}