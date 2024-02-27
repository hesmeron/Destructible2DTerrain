using UnityEngine;

public class Quad 
{
    private readonly int _xAdress;
    private readonly int _yAdress;
    private readonly int _width;
    private readonly int _height;
    
    private bool _hasChildren = false;
    private Quad[] _children;

    public int XAdress => _xAdress;

    public int YAdress => _yAdress;

    public int Width => _width;

    public int Height => _height;

    public bool HasChildren => _hasChildren;

    public Quad(int xAdress, int yAdress, int width, int height)
    {
        _xAdress = xAdress;
        _yAdress = yAdress;
        _width = width;
        _height = height;
    }
    
    public Quad[] Subdivide()
    {
        if (_width >= 2 * _height)
        {
            DivideHorizontally();
        }
        else if(_height >= 2 * _width)
        {
            DivideVertically();     
        }
        else
        {
            DivideEqually();
        }
        _hasChildren = true;
        return _children;
    }

    private void DivideVertically()
    {
        if (_height == 2)
        {
            Debug.Log("Partial height subdivide");
            _children = new[]
            {
                new Quad(_xAdress, _yAdress, _width, 1),
                new Quad(_xAdress, _yAdress +1, _width, 1),
            };
        }
        else
        {
            int heightOddFix = _height % 4;
            int quarterHeight= _height / 4;
            _children = new[]
            {
                new Quad(_xAdress, _yAdress, _width, quarterHeight),
                new Quad(_xAdress, _yAdress+ quarterHeight, _width, quarterHeight),
                new Quad(_xAdress, _yAdress + (quarterHeight*2), _width, quarterHeight),
                new Quad(_xAdress, _yAdress  +(quarterHeight*3), _width, quarterHeight + heightOddFix),
            };
        }
    }

    private void DivideHorizontally()
    {
        if (_width == 2)
        {
            Debug.Log("Partial width subdivide");
            _children = new[]
            {
                new Quad(_xAdress, _yAdress, 1, _height),
                new Quad(_xAdress + 1, _yAdress, 1, _height),
            };
        }
        else
        {
            int widthOddFix = _width % 4;
            int quarterWidth = _width / 4;
            _children = new[]
            {
                new Quad(_xAdress, _yAdress, quarterWidth, _height),
                new Quad(_xAdress + quarterWidth, _yAdress, quarterWidth, _height),
                new Quad(_xAdress + (quarterWidth * 2), _yAdress, quarterWidth, _height),
                new Quad(_xAdress + (quarterWidth * 3) + widthOddFix, _yAdress, quarterWidth, _height),
            };
        }
    }

    private void DivideEqually()
    {
        int widthOddFix = _width % 2;
        int heightOddFix = _height % 2;
        int halfWidth = _width / 2;
        int halfHeight = _height / 2;
        int maxHeight = halfHeight + heightOddFix;
        int maxWidth = halfWidth + widthOddFix;
        _children = new[]
        {
            new Quad(_xAdress, _yAdress, halfWidth, halfHeight),
            new Quad(_xAdress + halfWidth, _yAdress, maxWidth, halfHeight),
            new Quad(_xAdress, _yAdress + halfHeight, halfWidth, maxHeight),
            new Quad(_xAdress + halfWidth, _yAdress + halfHeight, maxWidth, maxHeight )
        };
    }
    
    public bool IsDivisible()
    {
        return _width * _height >= 2;
    }
    
    public bool TryGetChildren(out Quad[] children)
    {
        if (_hasChildren)
        {
            children = _children;
            return true;
        }

        children = null;
        return false;
    }
}
