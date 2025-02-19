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
    public float v;//�ٶ�km/h

    public float currentV;

    private List<Vector3> path = new List<Vector3>();

    public MoveType moveType;

    private float totalLength;//·���ܳ���

    private float currentS;//��ǰ�Ѿ��߹���·��

    private float s;//��һ����Ҫ�ߵ�·��

    private int index;

    private Vector3 dir;//����
    private Vector3 pos;//��һ��λ��

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
                currentS += (path[i + 1] - path[i]).magnitude;//������һ�����·��

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
            Debug.Log("�ִ��յ㣡��");
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
