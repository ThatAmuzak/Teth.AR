using System;
using UnityEditor;
using UnityEngine;

namespace AssemblyFramework
{
    public class SnapZone : MonoBehaviour
    {
        
        public SnapZoneData Data => data;
        
        [SerializeField] private SnapZoneData data;
        public Material previewMat;
        GameObject duplicate;

        public void Init(SnapZoneData _data)
        {
            data = _data;
            switch (data.zoneType)
            {
                case ZoneType.Box:
                    gameObject.AddComponent<BoxCollider>();
                    break;
                case ZoneType.Sphere:
                    gameObject.AddComponent<SphereCollider>();
                    break;
                case ZoneType.Cylinder:
                    gameObject.AddComponent<CapsuleCollider>();
                    break;
                default:
                    gameObject.AddComponent<BoxCollider>();
                    break;
            }

            gameObject.GetComponent<Collider>().isTrigger = true;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = new(0.05f, 0.05f, 0.05f);
            transform.gameObject.name = $"SnapZone_{_data.zoneType.ToString()}_{_data.partToAccept.Value}";
            Debug.Log(data.partData.position);
            Debug.Log(data.partData.rotation);
        }

        
        private void OnTriggerEnter(Collider trigger)
        {
            if(trigger.TryGetComponent(out AssemblyPart part))
            {
                MeshRenderer meshRenderer = trigger.GetComponent<MeshRenderer>();
                MeshFilter meshFilter = trigger.GetComponent<MeshFilter>();

                duplicate = new();
                duplicate.name = trigger.gameObject.name + "_preview";
                MeshFilter newFilter = duplicate.AddComponent<MeshFilter>();
                MeshRenderer newRenderer = duplicate.AddComponent<MeshRenderer>();

                newFilter.sharedMesh = meshFilter.sharedMesh;
                newRenderer.sharedMaterial = previewMat;

                duplicate.transform.parent = transform;
                duplicate.transform.localPosition = data.partData.position;
                duplicate.transform.localRotation = data.partData.rotation;
               
                Vector3 targetLossy = trigger.transform.lossyScale;
                Vector3 parentLossy = duplicate.transform.parent.lossyScale;

                duplicate.transform.localScale = new Vector3(
                    targetLossy.x / parentLossy.x,
                    targetLossy.y / parentLossy.y,
                    targetLossy.z / parentLossy.z
                );
            }
        }

        private void OnTriggerStay(Collider trigger)
        {
            if (trigger.TryGetComponent(out AssemblyPart part))
            {
                if (part.IsGrabbed == false)
                {
                    part.transform.position = transform.TransformPoint(data.partData.position);
                    part.transform.rotation = transform.rotation * data.partData.rotation;
                    
                    Destroy(duplicate);
                }
            }
        }

        private void OnTriggerExit(Collider trigger)
        {
            Destroy(duplicate);
        }

    }
}