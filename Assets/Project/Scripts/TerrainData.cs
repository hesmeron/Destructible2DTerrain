using System;
using UnityEngine;

public class TerrainData
{
    private bool [,] _points;
    int _width;
    int _height;

    public int Width => _width;
    public int Height => _height;

    public void DrawGizmos(float pixelsPerUnit)
    {
        if (_points != null)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Vector3 position = new Vector3(x, y, 0) / pixelsPerUnit;
                    Gizmos.color = _points[x, y] ? Color.red : Color.blue;
                    
                    Gizmos.DrawSphere(position, 0.5f / pixelsPerUnit);
                }
            }
        }
    }
    
    public TerrainData(Texture2D texture2D)
    {
        _width = texture2D.width;
        _height = texture2D.height;
        _points = new bool[_width, _height];
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _points[x, y] = texture2D.GetPixel(x, y).a >= Single.Epsilon;
            }
        }
    }
    
    public bool IsQuadUniform(Quad quad)
    {
        bool isOriginSolid = _points[quad.XAdress, quad.YAdress];
        for (int xDelta = 0; xDelta <= quad.Width-1; xDelta++)
        {
            for (int yDelta = 0; yDelta <= quad.Height-1; yDelta++)
            {
                bool isOtherSolid = _points[quad.XAdress + xDelta, quad.YAdress + yDelta];
                if (isOriginSolid != isOtherSolid)
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    public bool IsSolid(Quad quad)
    {
        return _points[quad.XAdress, quad.YAdress];
    }

    public void DestroyTerrain(int centerX, int centerY, int range)
    {
        int xOrigin = Mathf.Max(0, centerX - range);
        int xEnd =  Mathf.Min(_width-1,centerX + range);
        int yOrigin = Mathf.Max(0,centerY - range);
        int yEnd =  Mathf.Min(_height-1, centerY + range);

        for (int x = xOrigin; x < xEnd; x++)
        {
            for (int y = yOrigin; y < yEnd; y++)
            {
                int xDelta = x - centerX;
                int yDelta = y - centerY;
                float sqrDistance = (xDelta * xDelta) + (yDelta*yDelta);

                if (sqrDistance < range * range)
                {
                    _points[x, y] = false;
                }
            }
        }
    }
}
