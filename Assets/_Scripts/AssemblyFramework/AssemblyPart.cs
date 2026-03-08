using System;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace AssemblyFramework
{
    public class AssemblyPart : MonoBehaviour
    {
        public bool IsGrabbed => isGrabbed;
        
        [SerializeField] private Material previewMat;
        [SerializeField] HandGrabInteractable grabbable;
        [Header("This Part")]
        public TagReference partTag;
        
        [SerializeField] private bool isGrabbed = false;
        [Header("Relationships")]
        [SerializeField] private List<ChildPart> childParts;
        
        [Header("Snap Zones")]
        [SerializeField] private List<SnapZone> snapZones;
        
        [Header("Create New Snap Zones")]
        [SerializeField] private SnapZoneData zoneType;
        
        
        private void Start()
        {
            grabbable = transform.GetComponentInChildren<HandGrabInteractable>();
            grabbable.WhenStateChanged += GrabStateChanged;
        }

        private void GrabStateChanged(InteractableStateChangeArgs arg)
        {
            if (arg.NewState == InteractableState.Select)
            {
                isGrabbed = true;
            }
            else
            {
                isGrabbed = false;
            }
        }

        
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

        [Button]
        public void UpdateSnapZone()
        {
            snapZones.Clear();
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent(out SnapZone _snapZone))
                {
                    snapZones.Add(_snapZone);
                }
            }
            
            foreach (ChildPart childPart in childParts)
            {
                foreach (SnapZone zone in snapZones)
                {
                    if (zone.Data.partToAccept.Value == childPart.partTag.Value &&
                        zone.Data.partToAccept.registry == childPart.partTag.registry)
                    {
                        zone.Data.partData.position = childPart.position;
                        zone.Data.partData.rotation = childPart.rotation;

                        // // update transform preview if needed
                        // zone.transform.localPosition = childPart.position;
                        // zone.transform.localRotation = childPart.rotation;

                        break;
                    }
                }
            }
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
