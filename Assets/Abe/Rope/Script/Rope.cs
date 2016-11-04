using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Rope
{
    //末尾
    public Transform   tailRope;    //末尾のロープオブジェクト

    [NonSerialized]
    public SpringJoint tailJoint;   //末尾のロープのジョイント

    [NonSerialized]
    public Rigidbody   tailRig;     //末尾のロープのリジッドボディ

    //現在の振り子運動の基準
    public Transform   originRope;  //現在の振り子運動の基準となるオブジェクト

    [NonSerialized]
    public SpringJoint originJoint; //現在の振り子運動の基準となるジョイント

    [NonSerialized]
    public Rigidbody   originRig;   //現在の振り子運動の基準となるリジッドボディ

    public GameObject previousOrigin
    {
        get
        {
            Rigidbody previousRig = originJoint.connectedBody;
            return    previousRig.gameObject;
        }
    }

    public Vector3 tailPos
    {
        get { return tailRope.position;  }
        set { tailRope.position = value; }
    }
    public Vector3 originPos
    {
        get { return originRope.position;  }
        set { originRope.position = value; }
    }

    public void Initailize()
    {
        tailJoint   = tailRope.GetComponent<SpringJoint>();
        tailRig     = tailRope.GetComponent<Rigidbody>();

        originJoint = originRope.GetComponent<SpringJoint>();
        originRig   = originRope.GetComponent<Rigidbody>();
    }

    public void SetOrigin(GameObject newOrigin)
    {
        Rigidbody connectBody   = newOrigin.GetComponent<Rigidbody>();
        tailJoint.connectedBody = connectBody;

        ////１つ前のオブジェクトに戻れるように
        //SpringJoint newJoint   = newOrigin.GetComponent<SpringJoint>();
        //newJoint.connectedBody = originRope.GetComponent<Rigidbody>();

        originRope  = newOrigin.transform;
        originJoint = originRope.GetComponent<SpringJoint>();
    }
}