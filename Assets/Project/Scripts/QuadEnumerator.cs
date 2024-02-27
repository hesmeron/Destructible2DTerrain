using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadEnumerator : IEnumerator<Quad>
{
    Stack<Quad> _quadsToIterate = new Stack<Quad>();
    private Quad _root;
    private Quad _current;
    
    public QuadEnumerator(Quad root)
    {
        _root = root;
        _quadsToIterate.Push(_root);
    }


    public bool MoveNext()
    {
        if (_quadsToIterate.TryPop(out Quad quad))
        {
            _current = quad;
            if (quad.TryGetChildren(out Quad[] children))
            {
                foreach (Quad child in children)
                {
                    _quadsToIterate.Push(child);
                }
            }

            return true;
        }
        return false;
    }

    public void Reset()
    {
        _quadsToIterate.Clear();
        _current = _root;
        if (_current.TryGetChildren(out Quad[] children))
        {
            foreach (Quad child in children)
            {
                _quadsToIterate.Push(child);
            }
        }
    }

    public Quad Current => _current;

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        _quadsToIterate.Clear();
    }
}
