using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazerUtility : MonoBehaviour
{
    public static Vector3 BazierIntepolate3(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 p0p1 = Vector3.Lerp(p0, p1, t);
        Vector3 p1p2 = Vector3.Lerp(p1, p2, t);

        return Vector3.Lerp(p0p1, p1p2, t);
    }

    public static Vector3 BazierIntepolate4(Vector3 p0,Vector3 p1,Vector3 p2,Vector3 p3,float t)
    {
        Vector3 p0p1 = Vector3.Lerp(p0,p1,t);
        Vector3 p1p2 = Vector3.Lerp(p1,p2,t);
        Vector3 p2p3 = Vector3.Lerp(p2,p3,t);

        Vector3 px = Vector3.Lerp(p0p1, p1p2, t);
        Vector3 py = Vector3.Lerp(p1p2,p2p3, t);

        return Vector3.Lerp(px,py,t);

    }
    public static List<Vector3> BazierIntepolate3List(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        List<Vector3> pointList = new List<Vector3>();

        for (int i = 0; i < t; i++)
        {
            pointList.Add(BazierIntepolate3(p0, p1, p2, i / t));
        }
        return pointList;
    }
    public static List<Vector3> BazierIntepolate4List(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
       List<Vector3> pointList = new List<Vector3>();

        for (int i = 0; i < t; i++)
        {
            pointList.Add(BazierIntepolate4(p0, p1, p2, p3, i / t));
        }
        return pointList;
    }
}
