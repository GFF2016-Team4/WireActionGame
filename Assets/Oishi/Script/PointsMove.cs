using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointsMove : MonoBehaviour
{
    [Header("親オブジェクト")]
    public GameObject oyaObj;

    private GameObject[] childObj;
    private Vector3[] childPos;

    [Header("移動速度")]
    public float moveTime;

    [Header("ループ時[on]、停止時[off]")]
    public bool isLoop;

    int childcount;
    bool isMove = false;

    // Use this for initialization
    void Start()
    {
        childcount = oyaObj.transform.childCount;

        childObj = new GameObject[childcount];
        childPos = new Vector3[childcount];

    }
    void Update()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            isMove = true;

        }
        Move();
    }

    void Move()
    {
        if (isMove == true && isLoop == true)
        {

            iTween.MoveTo(gameObject, iTween.Hash("path", childPos,
                                                  "time", moveTime,
                                                  "onstart", "getchildPos"));
        }
    }
    public void getchildPos()
    {
        for (int i = 0; i < childcount; i++)
        {
            childObj[i] = oyaObj.transform.GetChild(i).gameObject;
            childPos[i] = childObj[i].transform.position;

        }

    }

}
