using UnityEditor;
using UnityEngine;

namespace Onca
{
    [CustomEditor(typeof(GenerateAgentsInArea))]
    [CanEditMultipleObjects]
    public class GenerateAgentsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
        
            if (GUILayout.Button("Generate"))
            {
                (target as GenerateAgentsInArea)?.RegenerateObjects();
            }
            if (GUILayout.Button("Clear"))
            {
                (target as GenerateAgentsInArea)?.RemoveChildren();
            }
        }
    }
}
