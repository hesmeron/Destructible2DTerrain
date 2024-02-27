using System.Collections.Generic;
using UnityEngine;

public partial class QuadTree
{
#if UNITY_EDITOR
    public void DrawGizmos(float pixelsPerUnit)
    {
        Stack<Quad> quadsToDraw = new Stack<Quad>();
        quadsToDraw.Push(_root);
        foreach (Quad quad in this)
        {
            if (!quad.HasChildren)
            {
                bool isSolid = _terrainData.IsSolid(quad);
                Color color = isSolid ? Color.red : Color.blue;
                DrawQuadGizmos(quad, color);
            }
        }
    }

    private void DrawQuadGizmos(Quad quad, Color color)
    {
        float x =  quad.XAdress + (quad.Width/2f);
        float y =  quad.YAdress + (quad.Height/2f);;
        Vector3 center = new Vector3(x, y, 0) ;
        Vector3 size = new Vector3(quad.Width, quad.Height);
        Gizmos.color = color;
        Gizmos.DrawWireCube(center, size);
        color.a = 0.4f;
        Gizmos.color = color;
        Gizmos.DrawCube(center, size);
    }    
    private void DrawQuadWireGizmos(Quad quad, Color color)
    {
        float x =  quad.XAdress + (quad.Width/2f);
        float y =  quad.YAdress + (quad.Height/2f);;
        Vector3 center = new Vector3(x, y, 0) ;
        Vector3 size = new Vector3(quad.Width, quad.Height);
        Gizmos.color = color;
        Gizmos.DrawWireCube(center, size);
    }
#endif
}