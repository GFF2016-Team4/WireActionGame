using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ListLineDraw))]
public class RopeSimulate : MonoBehaviour
{
    [SerializeField]
    private Rope rope;

    [SerializeField, Range(0, 10), Tooltip("判定する角度(小さすぎると戻らなくなる)")]
    private float maxDis;

    [SerializeField, Range(0.0001f, 10.0f), Tooltip("巻き取るスピード")]
    private float takeupTime = 0.5f;

    private ListLineDraw    listLineDraw;           //LineRenderer管理スクリプト
    private const float     ignoreDistance = 0.1f;
    private bool            isEnd = false;          //ロープのシミュレーションはおわりか？
    private int             ignorelayer;


    /// <summary>
    /// 末尾から引っかかっている部分の角度
    /// </summary>
    public Vector3 ropeDirection
    {
        get
        {
            Vector3 dir = rope.originPos - rope.tailPos;
            return dir;
        }
    }

    public Vector3 tailPosition
    {
        get { return rope.tailPos; }
    }

    public Rigidbody tailRig
    {
        get { return rope.tailRig; }
    }

    public Vector3 originPosision
    {
        get { return rope.originPos; }
    }

    void Awake()
    {
        listLineDraw  = GetComponent<ListLineDraw>();
        rope.Initailize();
        CalcMinDistance();

        ignorelayer = LayerMask.NameToLayer("Player");
    }

    void Start()
    {
        listLineDraw.DrawStart();
    }

    void Update()
    {
        if(isEnd) return;

        Vector3 origin = rope.originPos;
        Vector3 tail   = rope.tailPos;
        Vector3 nowVec = origin - tail;

        Joint nowOriginJoint = rope.originJoint;
        
        if(!nowOriginJoint.IsRootJoint())
        {
            if(CheckObstacle()) return;
        }

        Ray   ray         = new Ray(tail, nowVec);
        float maxDistance = nowVec.magnitude - ignoreDistance;
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo, maxDistance, ignorelayer))
        {
            //生成
            Transform newRopeObj = Instantiate(rope.tailRope);
            Rigidbody newRopeRig = newRopeObj.GetComponent<Rigidbody>();

            //新しいオブジェクトの設定
            newRopeRig.isKinematic = true;
            newRopeObj.position = hitInfo.point;

            rope.SetOrigin(newRopeObj.gameObject);
            CalcMinDistance();

            //最後尾の１つ上に挿入
            listLineDraw.Insert(listLineDraw.listCount-1, newRopeObj);
            newRopeObj.parent = transform;

#warning ここでイベントを送信して第三のロープの位置を変える処理を

        }
    }

    private bool CheckObstacle()
    {
        Transform  previousOrigin = rope.previousOrigin.transform;
        Vector3    previous       = previousOrigin.position;
        Vector3    prev2tail      = rope.tailPos   - previous;
        Vector3    prev2now       = rope.originPos - previous;
        
        Vector3 normal = prev2now.normalized;
        float dot = Vector3.Dot(prev2tail, normal);
        Vector3 height = -prev2tail + normal * dot;

        float distance = height.magnitude;

        Ray ray =  new Ray(rope.tailPos, -prev2tail);
        float maxDistance = prev2tail.magnitude - ignoreDistance;

        if(distance <= maxDis && !Physics.Raycast(ray, maxDistance, ignorelayer))
        {
            RemoveOrigin();
            return true;
        }
        return false;
    }
    private void CalcMinDistance()
    {
        float distance = rope.originPos.Distance(rope.tailPos);
        rope.tailJoint.minDistance = distance;
    }

    private void RemoveOrigin()
    {
        Rigidbody previousRig = rope.originJoint.connectedBody;
        
        //最下層から１つ上のロープオブジェクトを削除
        listLineDraw.RemoveDrawList(rope.originRope);
        Destroy(rope.originRope.gameObject);

        rope.SetOrigin(previousRig.gameObject);
        CalcMinDistance();
    }

    /// <summary>
    /// ロープの初期化
    /// </summary>
    /// <param name="origin">ロープの原点の座標(動かない部分)</param>
    /// <param name="tail">ロープの末尾の座標(振り子のように動く部分)</param>
    public void RopeInitialize(Vector3 origin, Vector3 tail)
    {
        rope.originRope.transform.position = origin;
        rope.tailRope.transform.position   = tail;
        CalcMinDistance();
    }

    /// <summary>
    /// ロープの長さを変更します
    /// </summary>
    /// <param name="distance">ロープの長さ</param>
    public void ChangeRopeLength(float distance)
    {
        Debug.Assert(distance > 0, "マイナスを指定しないでください");
        rope.tailJoint.minDistance = distance;
        
        Vector3 vec = rope.originPos - rope.tailPos;
        Vector3 dir = vec.normalized;

        rope.tailPos = rope.originPos + dir * distance;
    }

    /// <summary>
    /// ロープの長さを指定した分短くします
    /// </summary>
    /// <param name="distance">短くするロープの長さ</param>
    public void SubRopeLength(float distance)
    {
        AddRopeLength(-distance);
    }

    /// <summary>
    /// ロープの長さを指定した分長くします
    /// </summary>
    /// <param name="distance">長くするロープの長さ</param>
    public void AddRopeLength(float distance)
    {
        Vector3 vec = rope.tailPos - rope.originPos;
        Vector3 dir = vec.normalized;
        float   dis = vec.magnitude;

        dis = Mathf.Max(0, dis + distance);
        if(dis == 0 && !rope.originJoint.IsRootJoint())
        {
            RemoveOrigin();
        }

        rope.tailPos = rope.originPos + dir * dis;

        CalcMinDistance();
    }

    /// <summary>
    /// ロック中の末尾のオブジェクトの位置を変更します
    /// </summary>
    public void SetLockTailPosition(Vector3 position)
    {
       if(!rope.tailRig.isKinematic) return;

        rope.tailPos = position;
    }

    public void RopeLock()
    {
        rope.tailRig.isKinematic = true;
    }

    public void RopeUnLock()
    {
        rope.tailRig.isKinematic = false;
    }

    /// <summary>
    /// ロープを巻き取ります
    /// </summary>
    public void RopeEnd()
    {
        if(isEnd) return;
        isEnd = true;

        //親の取得と削除
        Joint originJoint = rope.originJoint;
        while(!originJoint.IsRootJoint())
        {
            GameObject oldObject  = originJoint.gameObject;
            Rigidbody  prevOrigin = originJoint.connectedBody;
            originJoint = prevOrigin.GetComponent<Joint>();

            listLineDraw.RemoveDrawList(oldObject.transform);
            Destroy(oldObject);
        } 
        
        //ロック解除
        Rigidbody originRig = rope.originRig;
        originRig.isKinematic = false;

        rope.originRope = originJoint.transform;

        //Jointを削除して自然落下をさせる
        Destroy(originJoint);

        StartCoroutine(RopeEnd_(originRig.transform));
    }

    private IEnumerator RopeEnd_(Transform origin)
    {
        //巻き取りの開始
        Vector3 startPos = origin.position;

        for(float time = takeupTime; time > 0.0f; time -= Time.deltaTime)
        {
            Vector3 tail = rope.tailPos;
            float t = 1 - (time / takeupTime);
            origin.position = Vector3.Lerp(startPos, tail, t);

            yield return null;
        }

        Destroy(gameObject);
    }
}
