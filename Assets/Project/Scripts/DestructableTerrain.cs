using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class DestructableTerrain : MonoBehaviour
{
    [SerializeField] 
    private MeshFilter _meshFilter;
    [SerializeField] 
    private Texture2D _texture2D;
    [SerializeField]
    private float _pixelsPerUnit = 10f;
    [SerializeField] 
    private float _destructionRadius =3f;

    private TerrainData _terrainData;
    private QuadTree _quadTree;
    private Camera _camera;
    private Vector3 _mousePosition = Vector3.zero;

    private void OnDrawGizmosSelected()
    {
        if (_quadTree != null)
        {
            _quadTree.DrawGizmos(_pixelsPerUnit);
        }
        Gizmos.DrawSphere(_mousePosition, _destructionRadius);
    }

    private void Awake()
    {
        _camera = Camera.main;
        GenerateTerrain();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition + new Vector3(0,0,5f));
            if (Trigonometry.PointIntersectsAPlane(_camera.transform.position,
                    mousePosition,
                    transform.position,
                    Vector3.forward,
                    out Vector3 result))
            {
                _mousePosition = result;
                DestroyArea(result);
            }
        }
    }

    public void GenerateTerrain()
    {
        _terrainData = new TerrainData(_texture2D);
        _quadTree = new QuadTree(_terrainData);
        ConstructMeshes();
    }

    private void DestroyArea(Vector3 position)
    {
        int x = (int) (position.x * _pixelsPerUnit) + (_texture2D.width/ 2);
        int y = (int) (position.y * _pixelsPerUnit) + (_texture2D.height/2);
        
        _terrainData.DestroyTerrain(x,y, (int) (_destructionRadius* _pixelsPerUnit));
        _quadTree = new QuadTree(_terrainData);
        ConstructMeshes();
    }

    private void ConstructMeshes()
    {
        MeshContructionHelper meshContructionHelper = new MeshContructionHelper();
        foreach (Quad quad in _quadTree)
        {
            if (!quad.HasChildren)
            {
                bool isACornerSolid = _terrainData.IsSolid(quad);

                if (isACornerSolid)
                {
                    ConstructQuad(quad, meshContructionHelper);
                }
            }
        }
        _meshFilter.mesh = meshContructionHelper.ConstructMesh();
    }
    
    private void ConstructQuad(Quad quad, MeshContructionHelper meshConstructionHelper)
    {
            VertexData[] vertices = GetQuadCorners(quad);
            meshConstructionHelper.AddMeshSection(vertices[0],  vertices[2], vertices[1]);
            meshConstructionHelper.AddMeshSection(vertices[0], vertices[3], vertices[2]);
    }
    
    private VertexData[] GetQuadCorners(Quad quad)
    {
        VertexData[] vertices = new[]
        {
            ImageCoordinatesToVertex(quad.XAdress, quad.YAdress),
            ImageCoordinatesToVertex(quad.XAdress + quad.Width, quad.YAdress),
            ImageCoordinatesToVertex(quad.XAdress + quad.Width, quad.YAdress + quad.Height),
            ImageCoordinatesToVertex(quad.XAdress, quad.YAdress + quad.Height),
        };
        return vertices;
    }
    
    private VertexData ImageCoordinatesToVertex(int x, int y)
    {
        float uvX = (x / (float) _texture2D.width);
        float uvY = (y / (float) _texture2D.height);
        Vector3 position = new Vector3(x - ( _texture2D.width/2f), y - ( _texture2D.height/2f), 0) / (_pixelsPerUnit);
        return new VertexData()
        {
            Postion = position,
            Uv = new Vector2(uvX, uvY),
            Normal = transform.forward
        };
    }
}
