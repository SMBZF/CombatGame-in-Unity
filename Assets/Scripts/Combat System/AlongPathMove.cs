using System.Collections.Generic;
using UnityEngine;


public enum MoveType
{
    once,
    loop,
    yoso,
}

public class AlongPathMove : MonoBehaviour
{
    public float v;//速度km/h

    public float currentV;

    private List<Vector3> path = new List<Vector3>();

    public MoveType moveType;

    private float totalLength;//路的总长度

    private float currentS;//当前已经走过的路程

    private float s;//下一秒需要走的路程

    private int index;

    private Vector3 dir;//方向
    private Vector3 pos;//下一个位置

    private bool isInit;

    public void Init( float v,Vector3 startPos,Vector3 midPos,Vector3 endPos)
    {
        this.v = v;
        path = BazerUtility.BazierIntepolate3List(startPos,midPos,endPos,30);
        Once();
        isInit = true;
        currentV = v;
    }

    private void FixedUpdate()
    {
        if (!isInit) return;


        s += (currentV * 10 / 36) * Time.fixedDeltaTime;

        if (currentS < totalLength)
        {
            for (int i = index; i < path.Count - 1; i++)
            {
                currentS += (path[i + 1] - path[i]).magnitude;//计算下一个点的路程

                if (currentS > s)
                {
                    index = i;
                    currentS -= (path[i + 1] - path[i]).magnitude;
                    dir = (path[i + 1] - path[i]).normalized;
                    pos = path[i] + dir * (s - currentS);
                    break;
                }
            }

            transform.position = pos;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir, transform.up), Time.deltaTime * 5);
        }
        else
        {
            Debug.Log("抵达终点！！");
            Destroy(gameObject);
        }
    }

    private void Once()
    {
        index = 0;
        s = 0;
        currentS = 0;
        totalLength = 0;

        for (int i = index + 1; i < path.Count - 1; i++)
        {
            //Debug.Log(startIndex);
            totalLength += (path[i] - path[i - 1]).magnitude;
        }
    }
}
