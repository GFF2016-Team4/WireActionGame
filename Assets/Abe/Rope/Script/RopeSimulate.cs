using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ListLineDraw))]
public class RopeSimulate : MonoBehaviour
{
    [SerializeField]
    private Transform originRope;

    [SerializeField]
    private Transform tailRope;

    [SerializeField, Range(0, 180), Tooltip("判定する角度(小さすぎると戻らなくなる)")]
    private float maxAngle;

    [SerializeField, Range(0, 60), Tooltip("巻き取るのに必要な時間")]
    private float takeupTime = 0.5f;

    private SpringJoint     tailJoint;              //ロープ末尾のジョイント
    private ListLineDraw    listLineDraw;           //LineRenderer管理スクリプト
    private float           maxAngle_;              //Inspector変更しないように
    private const float     ignoreDistance = 0.1f;
    private bool            isEnd = false;          //ロープのシミュレーションはおわりか？

    void Awake()
    {
        tailJoint = tailRope.GetComponent<SpringJoint>();
        listLineDraw = GetComponent<ListLineDraw>();
        CalcMinDistance();

        //このままだと2倍の角度になるので
        maxAngle_ = maxAngle/2;
    }

    void Start()
    {
        listLineDraw.DrawStart();
    }

    void Update()
    {
        if(isEnd) return;

        Vector3 origin = originRope.position;
        Vector3 tail   = tailRope.position;
        Vector3 nowVec = origin - tail;

        RaycastHit hitInfo;
        Ray ray;
        float maxDistance;

        Joint nowOriginJoint = originRope.GetComponent<Joint>();
        
        if(!nowOriginJoint.IsRootJoint())
        {
            Transform previousOrigin = nowOriginJoint.connectedBody.transform;
            Vector3   previous    = previousOrigin.position;
            Vector3   previousVec = previous - tail;
            
            //上方向ベクトルの統一
            Vector3   upVector = Vector3.Cross(previousVec, nowVec); 

            Quaternion previousAngle = Quaternion.LookRotation(previousVec, upVector);
            Quaternion nowAngle      = Quaternion.LookRotation(nowVec,      upVector);

            float angle = Quaternion.Angle(previousAngle, nowAngle);

            ray =  new Ray(tail, previousVec);
            maxDistance = previousVec.magnitude - ignoreDistance;

            if(angle <= maxAngle_ && !Physics.Raycast(ray, maxDistance))
            {
                //最下層から１つ上のロープオブジェクトを削除
                listLineDraw.RemoveDrawList(originRope);
                Destroy(originRope.gameObject);
                originRope = null;

                Rigidbody previousRig   = previousOrigin.GetComponent<Rigidbody>();
                tailJoint.connectedBody = previousRig;

                originRope = previousRig.transform;

                CalcMinDistance();
                return;
            }
        }

        ray = new Ray(tail, nowVec);
        maxDistance = nowVec.magnitude - ignoreDistance;

        if(Physics.Raycast(ray, out hitInfo, maxDistance))
        {
            //生成
            Transform newRopeObj = Instantiate(tailRope);
            Rigidbody newRopeRig = newRopeObj.GetComponent<Rigidbody>();
            tailJoint.connectedBody = newRopeRig;

            //新しいオブジェクトの設定
            newRopeRig.isKinematic = true;
            newRopeObj.position = hitInfo.point;

            originRope = newRopeObj;
            CalcMinDistance();

            //最後尾の１つ上に挿入
            listLineDraw.Insert(listLineDraw.listCount-1, newRopeObj);
            newRopeObj.parent = transform;
        }
    }

    private void CalcMinDistance()
    {
        float distance = originRope.position.Distance(tailRope.position);
        tailJoint.minDistance = distance;
    }

    /// <summary>
    /// ロープの初期化
    /// </summary>
    /// <param name="origin">ロープの原点の座標(動かない部分)</param>
    /// <param name="tail">ロープの末尾の座標(振り子のように動く部分)</param>
    public void RopeInitialize(Vector3 origin, Vector3 tail)
    {
        originRope.transform.position = origin;
        tailRope.transform.position   = tail;
    }

    /// <summary>
    /// ロープの長さを変更します
    /// </summary>
    /// <param name="distance">ロープの長さ</param>
    public void ChangeRopeLength(float distance)
    {
        Debug.Assert(distance > 0, "マイナスを指定しないでください");
        tailJoint.minDistance = distance;
        
        Vector3 vec = originRope.position - tailRope.position;
        Vector3 dir = vec.normalized;

        tailRope.position = originRope.position + dir * distance;
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
        Vector3 vec = tailRope.position - originRope.position;
        Vector3 dir = vec.normalized;
        float   dis = vec.magnitude;

        dis = Mathf.Max(0, dis + distance);
        tailRope.position = originRope.position + dir * dis;

        CalcMinDistance();
    }

    /// <summary>
    /// ロープを巻き取ります
    /// </summary>
    public void RopeEnd()
    {
        isEnd = true;

        //親の取得と削除
        Joint originJoint = originRope.GetComponent<Joint>();
        while(!originJoint.IsRootJoint())
        {
            GameObject oldObject  = originJoint.gameObject;
            Rigidbody  prevOrigin = originJoint.connectedBody;
            originJoint = prevOrigin.GetComponent<Joint>();

            listLineDraw.RemoveDrawList(oldObject.transform);
            Destroy(oldObject);
        }
        
        //ロック解除
        Rigidbody originRig = originJoint.GetComponent<Rigidbody>();
        originRig.isKinematic = false;

        //Jointを削除して自然落下をさせる
        Destroy(originJoint);
        StartCoroutine(RopeEnd_(originRig.transform));
    }

    private IEnumerator RopeEnd_(Transform origin)
    {
        //巻き取りの開始
        Vector3 startPos  = origin.position;

        for(float time = takeupTime; time > 0.0f; time -= Time.deltaTime)
        {
            Vector3 tail = tailRope.position;
            float t = 1 - (time / takeupTime);
            origin.position = Vector3.Lerp(startPos, tail, t);

            yield return null;
        }

        Destroy(gameObject);
    }

    //デバッグ
    void OnDrawGizmosSelected()
    {
        Vector3 origin = originRope.position;
        Vector3 rope   = tailRope.position;
        Gizmos.DrawLine(origin, rope);
    }
}