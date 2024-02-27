using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DestructableTerrain))]
public class DestructibleTerrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DestructableTerrain terrain  = target as DestructableTerrain;
        if (GUILayout.Button("Generate terrain")) 
        {
            terrain.GenerateTerrain();
            EditorUtility.SetDirty(terrain);
        }
    }
}
