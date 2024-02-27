using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class QuadTree : IEnumerable<Quad>
{
    private TerrainData _terrainData;
    private Quad _root;
    
    public QuadTree(TerrainData terrainData)
    {
        Debug.Log("Texture width and height" + terrainData.Width + " "  +  terrainData.Height);
        _terrainData = terrainData;
        _root = new Quad(0,0, terrainData.Width ,  terrainData.Height);
        Stack<Quad> quadsToCheck = new Stack<Quad>();
        quadsToCheck.Push(_root);
        while (quadsToCheck.TryPop(out Quad quad))
        {
            if (!terrainData.IsQuadUniform(quad))
            {
                Quad[] children = quad.Subdivide();
                foreach (Quad child in children)
                {
                    if (child.IsDivisible())
                    {
                        quadsToCheck.Push(child);
                    }
                }
            }
        }
    }

    public IEnumerator<Quad> GetEnumerator()
    {
        return new QuadEnumerator(_root);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
