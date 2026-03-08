using UnityEditor;
using UnityEngine;

namespace AssemblyFramework
{
    [CustomPropertyDrawer(typeof(TagReference))]
    public class TagReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var registryProp = property.FindPropertyRelative("registry");
            var indexProp = property.FindPropertyRelative("selectedIndex");

            float half = position.width / 2f;
            Rect regRect = new Rect(position.x, position.y, half, position.height);
            Rect dropRect = new Rect(position.x + half, position.y, half, position.height);

            EditorGUI.PropertyField(regRect, registryProp, GUIContent.none);

            var registry = registryProp.objectReferenceValue as TagRegistry;
            if (registry != null && registry.tags.Count > 0)
            {
                string[] options = registry.tags.ToArray();
                indexProp.intValue = EditorGUI.Popup(dropRect, indexProp.intValue, options);
            }
            else
            {
                EditorGUI.LabelField(dropRect, "No registry / empty");
            }
        }
    }
}
