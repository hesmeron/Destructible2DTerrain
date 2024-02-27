using System;
using UnityEngine;

/// <summary>
/// This is my custom trigonometry class. It is usefull engough so i just brought it over to this test project
/// </summary>
public static class Trigonometry 
{
    public static void GetCastPoint(Vector3 from, Vector3 to, Vector3 pointToCast, out Vector3 result)
    {
        GetCastPoint(from, to, pointToCast, out result);
    }
    
    public static void GetCastPoint(Vector3 from, Vector3 to, Vector3 pointToCast, out Vector3 result, out float signedCompletion)
    {
        Vector3 direction = to - from;
        float magnitude = direction.sqrMagnitude;
        Vector3 distanceFromPoint = from - pointToCast;
        signedCompletion = -Vector3.Dot(direction, distanceFromPoint) / magnitude;
        float completion = Mathf.Clamp01(signedCompletion);
        result = from + (direction * completion);
    }
    
    public static void GetCastPoint(Vector2 from, Vector2 to, Vector2 pointToCast, out Vector2 result, out float signedCompletion)
    {
        Vector2 direction = to - from;
        float magnitude = direction.sqrMagnitude;
        Vector2 distanceFromPoint = from - pointToCast;
        signedCompletion = -Vector2.Dot(direction, distanceFromPoint) / magnitude;
        float completion = Mathf.Clamp01(signedCompletion);
        result = from + (direction * completion);
    }
    public static bool PointIntersectsAPlane(Vector3 from, Vector3 to, Vector3 planeOrigin, Vector3 normal, out Vector3 result)
    {
        Vector3 translation = to - from;
        float dot = Vector3.Dot(normal, translation);
        if (Mathf.Abs(dot) > Single.Epsilon)
        {
            Vector3 fromOrigin = from - planeOrigin;
            float fac = -Vector3.Dot(normal, fromOrigin) / dot;
            translation = translation * fac;
            result = from + translation;
            return true;
        }
        
        result = Vector3.zero;
        return false;
    }
}
