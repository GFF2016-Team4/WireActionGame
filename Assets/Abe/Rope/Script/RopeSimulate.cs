using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ListLineDraw))]
public class RopeSimulate : MonoBehaviour
{
    [SerializeField, Tooltip("ロープの構造体")]
    Rope  rope;

    [SerializeField]
    float checkDistance;

    [SerializeField]
    float takeupTime;

    public  bool            isCalcDistance  = false;

    private ListLineDraw    listLineDraw;
    private const float     ignoreDistance  = 0.1f;
    private bool            isEnd           = true; //ロープのシミュレーションはおわりか？
    private int             ignorelayer;

    public Vector3 TailPosition
    {
        set
        {
            if(!rope.isKinematic) return;
            rope.tailPosition = value;
        }
    }

    void Awake()
    {
        listLineDraw = GetComponent<ListLineDraw>();
        rope.Initailize();
        rope.CalcMinDistance();

        ignorelayer = LayerMask.NameToLayer("Player");
    }
    
    IEnumerator Start()
    {
        listLineDraw.DrawStart();
        yield return null;

        //main loop
        while(true)
        {
            CheckObstacle();
            yield return null;
        }
    }
    
    void CheckObstacle()
    {
        Ray ray = new Ray(rope.tailPosition, rope.direction);
        float maxDistance = rope.length - ignoreDistance;
        RaycastHit hitInfo;

        if(!rope.rigOriginJoint.IsRootJoint())
        {
            if(CheckRemoveOrigin()) return;
        }

        if(Physics.Raycast(ray, out hitInfo, maxDistance, ignorelayer))
        {
            CreateRigOrigin(hitInfo.point);
        }
    }

    void CreateRigOrigin(Vector3 createPoint)
    {
        Transform newOrigin = rope.AddRigOrigin(createPoint, isCalcDistance);
        newOrigin.parent = transform;
        listLineDraw.Insert(listLineDraw.count-1, newOrigin);

        //名前の変更
        newOrigin.name = "origin" + (transform.childCount-1);
    }

    bool CheckRemoveOrigin()
    {
        Transform prevRigOrigin = rope.GetPrevRigOrigin<Transform>();
        
        Vector3 v = rope.tailPosition.DistanceToLine(prevRigOrigin.position, rope.rigOriginPosition);
        float distance = v.magnitude;

        //判定の範囲内か
        if(distance > checkDistance) return false;

        Ray ray = new Ray()
        {
            origin    =  rope.tailPosition,
            direction = -rope.tailPosition + prevRigOrigin.position
        };
        float maxDistance = rope.tailPosition.Distance(prevRigOrigin.position);
        maxDistance -= ignoreDistance;

        //当たっている -> 引っかかっている
        if(Physics.Raycast(ray, maxDistance, ignorelayer)) return false;

        listLineDraw.RemoveDrawList(rope.rigOrigin);

        //引っかかりを取る
        rope.RemoveLastRigOrigin();

        return true;
    }

    public void SimulationStart()
    {
        rope.isKinematic = true; 
    }

    public void SimulationStop()
    {
        rope.isKinematic = false;
    }

    public void SimulationEnd()
    {
        if(!rope.rigOriginRig.isKinematic) return;
        StopCoroutine(Start());

        //ルートrigOriginとtail以外を消す
        rope.EachOrigin<Joint>((origin) => 
        {
            if(origin.IsRootJoint())
            {
                rope.SetRigOrigin(origin.transform);
            }
            else
            {
                Destroy(origin.gameObject);
            }
        });

        //自然落下
        rope.rigOriginRig.isKinematic = false;
        Destroy(rope.rigOriginJoint);
    }
}