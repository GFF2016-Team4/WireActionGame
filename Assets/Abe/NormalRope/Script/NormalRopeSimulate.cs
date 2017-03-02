using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ListLineDraw))]
public class NormalRopeSimulate : MonoBehaviour
{
    [SerializeField, Tooltip("ロープの構造体")]
    NormalRope  rope;

    [SerializeField]
    float checkDistance;

    [SerializeField]
    float takeupTime;

    public  bool            isCalcDistance  = false;

    private ListLineDraw    listLineDraw;
    private const float     ignoreDistance  = 0.2f;
    private int             ignoreLayer;

    bool isEnd = false;

    public bool isContorll = true;

    public Vector3 originPosition
    {
        get { return rope.rigOriginPosition; }
    }

    public Transform nowOrigin
    {
        get { return rope.rigOrigin;}
    }

    public Vector3 tailPosition
    {
        get { return rope.tailPosition; }

        set
        {
            if(!rope.isKinematic)
            {
                Debug.Log("物理挙動している時は位置を変えられません");
                return;
            }
            rope.tailPosition = value;
        }
    }

    public Transform tail
    {
        get { return rope.tail; }
    }

    public float ropeLength
    {
        get { return rope.length; }
    }

    public Vector3 velocity
    {
        get { return rope.tailRig.velocity; }
    }

    public Vector3 direction
    {
        get { return rope.direction; }
    }

    public MeshRenderer hookRenderer
    {
        get { return rope.GetRootRigOrigin<MeshRenderer>(); }
    }

    void Awake()
    {
        listLineDraw = GetComponent<ListLineDraw>();
        rope.Initailize();
        rope.CalcMinDistance();

        ignoreLayer = LayerMask.NameToLayer("Player");
    }
    
    public void Initialize(Vector3 origin, Vector3 tail)
    {
        rope.rigOriginPosition = origin;
        rope.tailPosition      = tail;
        rope.CalcMinDistance();
        rope.rigOrigin.transform.rotation = Quaternion.LookRotation(-direction);
    }

    IEnumerator Start()
    {
        if(isEnd == true) yield break;

        //main loop
        while(true)
        {
            CheckObstacle();
            yield return null;
        }
    }

    void CheckObstacle()
    {
        if(!rope.rigOriginJoint.IsRootJoint())
        {
            //成功時のみリターン
            if(CheckRemoveOrigin()) return;
        }

        RaycastHit hitInfo;
        if(IsCollisionObstacle(rope.tailPosition, rope.rigOriginPosition, out hitInfo))
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

        if(!IsCheckRange(prevRigOrigin.position, rope.rigOriginPosition))  return false;

        //当たっている -> 引っかかっている
        if(IsCollisionObstacle(rope.tailPosition, prevRigOrigin.position)) return false;
        
        //引っかかりを取る
        listLineDraw.RemoveDrawList(rope.rigOrigin);
        rope.RemoveLastRigOrigin();

        return true;
    }

    bool IsCollisionObstacle(Vector3 linePos1, Vector3 linePos2)
    {
        RaycastHit hitInfo;
        return IsCollisionObstacle(linePos1, linePos2, out hitInfo);
    }

    bool IsCollisionObstacle(Vector3 linePos1, Vector3 linePos2, out RaycastHit hitInfo)
    {
        Ray ray = new Ray()
        {
            origin    =  linePos1,
            direction = -linePos1 + linePos2
        };

        float maxDistance = Vector3.Distance(linePos1, linePos2);
        maxDistance -= ignoreDistance;
        
        return Physics.Raycast(ray, out hitInfo, maxDistance, ignoreLayer);
    }

    bool IsCheckRange(Vector3 linePoint1, Vector3 linePoint2)
    {
        Vector3 v = rope.tailPosition.DistanceToLine(linePoint1, linePoint2);
        float distance = v.magnitude;

        return distance <= checkDistance;
    }

    public void SimulationStart() { rope.isKinematic = false; }
    public void SimulationStop()  { rope.isKinematic = true;  }

    public void SimulationEnd(Transform sync)
    {
        if(isEnd) return;
        isEnd = true;

        isContorll = false;

        StopAllCoroutines();
        SimulationStop();
        
        //親の取得と削除
        Joint originJoint = rope.rigOriginJoint;
        while(!originJoint.IsRootJoint())
        {
            GameObject oldObject = originJoint.gameObject;
            originJoint = originJoint.GetParentJoint();

            listLineDraw.RemoveDrawList(oldObject.transform);
            Destroy(oldObject);
        }
        rope.SetRigOrigin(originJoint.transform);

        //自然落下
        rope.rigOriginRig.isKinematic = false;
        Destroy(rope.rigOriginJoint);
        Destroy(rope.rigOriginJoint.GetComponent<SyncObject>());

        rope.rigOrigin.transform.rotation = Quaternion.LookRotation(-direction);

        StartCoroutine(TakeUp(sync));
    }

    private IEnumerator TakeUp(Transform sync)
    {
        //巻き取りの開始
        Vector3 startPos = rope.rigOriginPosition;

        SoundManager.Instance.PlaySE(AUDIO.SE_ropeRoll);
        for(float time = takeupTime; time > 0.0f; time -= Time.deltaTime)
        {
            rope.tailPosition = sync.position;

            Vector3 tail = rope.tailPosition;
            float   t    = 1 - (time / takeupTime);
            rope.rigOriginPosition = Vector3.Lerp(startPos, tail, t);

            yield return null;
        }
        SoundManager.Instance.StopSE();
        Destroy(gameObject);
    }

    public void AddForce(Vector3 force, ForceMode forceMode)
    {
        rope.tailRig.AddForce(force, forceMode);
    }

    public void AddLength(float length)
    {
        rope.AddLength(length);
    }
}