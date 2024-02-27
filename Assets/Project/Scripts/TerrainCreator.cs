using System;
using UnityEngine;

public class TerrainCreator
{
    public static bool[,] CreateTerrainData(Texture2D texture2D)
    {

        bool[,] result = new bool[texture2D.width,texture2D.height];
        return result;
    }
}
