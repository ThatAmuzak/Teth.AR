using System;
using UnityEditor;
using UnityEngine;

namespace AssemblyFramework
{
    public class SnapZone : MonoBehaviour 
    {
        private SnapZoneData data;
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
            
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = new(0.05f, 0.05f, 0.05f);
            transform.gameObject.name = $"SnapZone_{_data.zoneType.ToString()}_{_data.partToAccept.Value}";
            
        }

//         private void OnDrawGizmos()
//         {
//             switch (snapType)
//             {
//                 case SnapZoneType.Box:
//                     Gizmos.DrawWireCube(
//                         transform.position,
//                         new Vector3(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z)
//                     );
//                     break;
//
//                 case SnapZoneType.Sphere:
//                     Gizmos.DrawWireSphere(transform.position, transform.lossyScale.z);
//                     break;
//
//                 case SnapZoneType.Cylinder:
//                     Gizmos.DrawWireMesh(
//                         Resources.GetBuiltinResource<Mesh>("Capsule.fbx"),
//                         transform.position,
//                         transform.rotation,
//                         transform.lossyScale
//                     );
//                     break;
//
//                 default:
//                     break;
// }
//             
//         }
    }
}
