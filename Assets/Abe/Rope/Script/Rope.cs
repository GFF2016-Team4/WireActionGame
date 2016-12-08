using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public struct Rope
{
    [Header("末尾")]
    public Transform   tail;           //末尾のロープオブジェクト

    [NonSerialized]
    public SpringJoint tailJoint;      //末尾のロープのジョイント

    [NonSerialized]
    public Rigidbody   tailRig;        //末尾のロープのリジッドボディ

    [Header("現在の振り子運動の基準")]
    public Transform   rigOrigin;      //現在の振り子運動の基準となるオブジェクト

    [NonSerialized]
    public SpringJoint rigOriginJoint; //現在の振り子運動の基準となるジョイント

    [NonSerialized]
    public Rigidbody   rigOriginRig;   //現在の振り子運動の基準となるリジッドボディ

    public Vector3 tailPosition
    {
        get { return tail.position;  }
        set { tail.position = value; }
    }
    public Vector3 rigOriginPosition
    {
        get { return rigOrigin.position;  }
        set { rigOrigin.position = value; }
    }

    public Vector3 direction
    {
        get { return rigOriginPosition - tailPosition; }
    }

    public float length
    {
        get { return direction.magnitude; }
    }

    public bool isKinematic
    {
        get { return tailRig.isKinematic;  }
        set { tailRig.isKinematic = value; }
    }

    public void Initailize()
    {
        tailJoint   = tail.GetComponent<SpringJoint>();
        tailRig     = tail.GetComponent<Rigidbody>();

        rigOriginJoint = rigOrigin.GetComponent<SpringJoint>();
        rigOriginRig   = rigOrigin.GetComponent<Rigidbody>();
    }

    public void SetRigOrigin(Transform newOrigin)
    {
        Rigidbody connectBody   = newOrigin.GetComponent<Rigidbody>();
        tailJoint.connectedBody = connectBody;

        rigOrigin      = newOrigin;
        rigOriginJoint = rigOrigin.GetComponent<SpringJoint>();
        rigOriginRig   = connectBody;
    }

    public void SetPosition(Vector3 origin, Vector3 tail)
    {
        rigOriginPosition = origin;
        tailPosition      = tail;
    }

    public Transform AddRigOrigin(Vector3 createPoint, bool isCalcMinDistance = true)
    {
        Transform newRigOrigin    = GameObject.Instantiate(tail);
        Rigidbody newRigOriginRig = newRigOrigin.GetComponent<Rigidbody>();

        //新しいオブジェクトの設定
        newRigOriginRig.isKinematic = true;
        newRigOrigin.position       = createPoint;

        SetRigOrigin(newRigOrigin);

        if(isCalcMinDistance)
        {
            //これをしないとおかしくなる
            CalcMinDistance();
        }
        return newRigOrigin;
    }

    //tailから１つ上のrigOrigin削除
    public void RemoveLastRigOrigin()
    {
        //１つ上のrigOrigin取得
        Transform prevRigOrigin = GetPrevRigOrigin<Transform>();
        GameObject.Destroy(rigOrigin.gameObject);

        SetRigOrigin(prevRigOrigin);
    }

    public void CalcMinDistance()
    {
        float distance = rigOriginPosition.Distance(tailPosition);
        tailJoint.minDistance = distance;
    }

    public void ChangeLength(float distance)
    {
        distance = Mathf.Max(0, distance);
        tailJoint.minDistance = distance;

        Vector3 vec = rigOriginPosition - tailPosition;
        Vector3 dir = vec.normalized;

        tailPosition = rigOriginPosition + dir * distance;
    }

    public void AddLength(float distance)
    {
        Vector3 vec = tailPosition - rigOriginPosition;
        Vector3 dir = vec.normalized;
        float   dis = vec.magnitude;

        dis = Mathf.Max(1, dis + distance);
        if(dis == 0 && !rigOriginJoint.IsRootJoint())
        {
            RemoveLastRigOrigin();
        }
        
        tailPosition = rigOriginPosition + dir * dis;

        CalcMinDistance();
    }

    public void SubLength(float distance)
    {
        AddLength(-distance);
    }

    //巡回用デリゲート(T : 引数が指定したコンポーネントで渡る)
    public delegate void EachFunc<T>(T rigOrigin) where T : Component;

    //巡回
    public void EachOrigin<T>(EachFunc<T> func)  where T : Component
    {
        SpringJoint joint = rigOriginJoint;
        EachOrigin<T>(func, rigOriginJoint);
        if(joint != null)
        {
            rigOriginJoint = joint;
        }
    }

    //巡回
    void EachOrigin<T>(EachFunc<T> func, SpringJoint rigOriginJoint) where T : Component
    {
        if(rigOriginJoint == null) return;

        //Destoryも考慮
        this.rigOriginJoint     = rigOriginJoint;
        SpringJoint parentJoint = GetPrevRigOrigin<SpringJoint>();

        func(rigOriginJoint.GetComponent<T>());

        //再帰処理 ルートのジョイントになるまで処理を続ける
        EachOrigin(func, parentJoint);
    }

    //前のrigOrigin取得
    public T GetPrevRigOrigin<T>() where T : Component
    {
        if(rigOriginJoint == null)       return default(T);
        if(rigOriginJoint.IsRootJoint()) return default(T);

        Joint joint = rigOriginJoint.GetParentJoint();
        return joint.GetComponent<T>();
    }

    //ルートのrigOrigin取得
    public T GetRootRigOrigin<T>() where T : Component
    {
        Joint rigOriginJoint = this.rigOriginJoint;
        while( rigOriginJoint != null &&
              !rigOriginJoint.IsRootJoint())
        {
            rigOriginJoint = rigOriginJoint.GetParentJoint();
        }

        return rigOriginJoint.GetComponent<T>();
    }
}