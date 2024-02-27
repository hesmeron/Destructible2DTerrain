using System;
using UnityEditor;
using UnityEngine;

public class TerrainCreatorWindow : EditorWindow
{
    private Texture2D _mapTexture;
    
    [MenuItem( ("Window/Terrain Creator"))]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(TerrainCreatorWindow));
    }

    public void OnGUI()
    {
        _mapTexture = EditorGUILayout.ObjectField("Map texture", _mapTexture, typeof(Texture2D), false) as Texture2D;
        if(GUILayout.Button("Generate terrain"))
        {
            TerrainCreator.CreateTerrainData(_mapTexture);
        }
    }
}
