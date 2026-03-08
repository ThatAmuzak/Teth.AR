using System;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using UnityEngine;

namespace AssemblyFramework
{
    public class AssemblyPart : MonoBehaviour
    {
        [SerializeField] private Material previewMat;
        
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
            data.partData = childParts.Find(x =>
                x.partTag.Value == data.partToAccept.Value &&
                x.partTag.registry == data.partToAccept.registry
            );
            data.previewMat = previewMat;
            
            if (data.partData == null)
            {
                Debug.LogError("Populate the intended part's data");
                return;
            }
            
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
            snapZone.Init(data);
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
        [HideInInspector] public ChildPart partData = null;
        [HideInInspector] public Material previewMat;
    }
    
    [Serializable]
    public enum ZoneType
    {
        Box = 0,
        Cylinder = 1,
        Sphere = 2
    }
    
}