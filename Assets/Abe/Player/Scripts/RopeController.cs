﻿using UnityEngine;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using UnityEngine.EventSystems;

public class RopeController : MonoBehaviour
{
    [System.Serializable]
    public struct NormalRope
    {
        [SerializeField, Tooltip("同期するトランスフォーム")]
        public Transform sync;

        [SerializeField]
        public NormalRopeSimulate ropeInst;

        [SerializeField]
        public string[] shootButton;

        [SerializeField]
        public GameObject ropeBullet;

        public bool RopeExist
        {
            get { return ropeInst != null; }
        }

        public bool BulletExist
        {
            get { return ropeBullet != null; }
        }

        public bool IsCanControl
        {
            get
            {
                if(!RopeExist) return false;
                if(!InputExtension.GetButtonAll(shootButton)) return false;
                return true;
            }
        }
    }

    [SerializeField, Tooltip("ロープのプレハブ")]
    private GameObject normalRopePrefab;

    [SerializeField]
    private GameObject lockRopePrefab;

    [SerializeField]
    private GameObject catchRopePrefab;

    [SerializeField, Tooltip("射出弾インスタンス")]
    private GameObject bulletPrefab;

    [SerializeField, Tooltip("射出弾のスピード")]
    private float      bulletSpeed;

    [SerializeField, Tooltip("発射の間隔")]
    private float      intervalTime;

    [SerializeField]
    private RectTransform reticle;

    [SerializeField]
    NormalRope left;

    [SerializeField]
    NormalRope right;

    [SerializeField]
    NormalRope center;

    [SerializeField]
    NormalRope catchRope;

    [System.NonSerialized]
    public float normalRopeDistance;

    PlayerCameraInfo cameraInfo;

    public bool isControl = true;

    public bool IsRopeExist
    {
        get
        {
            return left     .RopeExist ||
                   right    .RopeExist ||
                   center   .RopeExist;
        }
    }

    public bool IsCenterRopeExist
    {
        get { return center.RopeExist; }
    }

    public Vector3 Origin
    {
        get
        {
            if(center.IsCanControl) return center.ropeInst.originPosition;
            if(left  .IsCanControl) return left  .ropeInst.originPosition;
            if(right .IsCanControl) return right .ropeInst.originPosition;

            throw null;
        }
    }

    public Vector3 LeftOrigin
    {
        get { return left. ropeInst.originPosition; }
    }

    public Vector3 RightOrigin
    {
        get { return right.ropeInst.originPosition; }
    }

    public Vector3 Direction
    {
        get
        {
            if(center.IsCanControl) return center.ropeInst.direction;
            if(left  .IsCanControl) return left  .ropeInst.direction;
            if(right .IsCanControl) return right .ropeInst.direction;

            throw null;
        }
    }

    public Vector3? LeftDirection
    {
        get
        {
            if(left.RopeExist) return null;

            return left.ropeInst.direction;
        }
    }

    public Vector3? RightDirection
    {
        get
        {
            if(right.RopeExist) return null;

            return right.ropeInst.direction;
        }
    }

    void Awake()
    {
        cameraInfo = GetComponent<PlayerCameraInfo>();

        left     .shootButton    = new string[1];
        right    .shootButton    = new string[1];
        center   .shootButton    = new string[2];
        catchRope.shootButton    = new string[1];

        left     .shootButton[0] = RopeInput.leftButton;
        right    .shootButton[0] = RopeInput.rightButton;
        center   .shootButton[0] = RopeInput.leftButton;
        center   .shootButton[1] = RopeInput.rightButton; 
        catchRope.shootButton[0] = RopeInput.ropeCatchButton;

        //クリックされたらロープが発射されるイベントを作成
        CreateInputEvent(left );
        CreateInputEvent(right);
    }

    void CreateInputEvent(NormalRope rope)
    {
        var clickStream = this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown(rope.shootButton[0]));

        clickStream
            .ThrottleFirst(System.TimeSpan.FromSeconds(intervalTime))
            .Where(_ => isControl)
            .Where(_ => !rope .RopeExist && !rope .BulletExist)
            .Subscribe(_ =>
            {
                NormalRope r = rope;

                Debug.Log(rope.shootButton[0].ToString());
                StartCoroutine(ShootNormalRope(rope));
            });
    }

    void Update()
    {
        ShootRopes();
        if(catchRope.RopeExist)
        {
            catchRope.ropeInst.tailPosition = catchRope.sync.position;
        }
        SyncRopeToTransform_(catchRope);
        CheckSimulationEnd();
    }

    void ShootRopes()
    {
        if(Input.GetButtonDown(catchRope.shootButton[0]))
        {
            if(catchRope.RopeExist)
            {
                catchRope.ropeInst.SimulationEnd(catchRope.sync);
            }
            else
            {
                StartCoroutine(ShootCatchRope());
            }
        }
    }

    void CheckSimulationEnd()
    {
        CheckSimulationEnd(ref center);
        CheckSimulationEnd(ref left  );
        CheckSimulationEnd(ref right );
    }

    void CheckSimulationEnd(ref NormalRope rope)
    {
        if(!rope.RopeExist) return;
        if(!InputExtension.GetButtonAnyUp(rope.shootButton)) return;
        //ロープのボタンを離した

        SendNormalRopeReleaseEvent(rope.ropeInst);

        TakeOverVelocity(left,  rope);
        TakeOverVelocity(right, rope);

        rope.ropeInst.SimulationEnd(rope.sync);
        rope.ropeInst = null;
    }

    void TakeOverVelocity(NormalRope takeOverRope, NormalRope takenOverRope)
    {
        if(!takeOverRope.IsCanControl) return;
        takeOverRope.ropeInst.SimulationStart();
        Vector3 takenOverVelocity = takenOverRope.ropeInst.velocity;
        takeOverRope.ropeInst.AddForce(takenOverVelocity, ForceMode.VelocityChange);
    }

    IEnumerator ShootNormalRope(NormalRope rope)
    {
        //アップデートが終わるまで待機
        GameObject bulletInst = null;
        var wait = StartCoroutine(WaitForBullet(rope.sync.position, rope.sync, rope.shootButton[0],
            (inst) => 
            {
                bulletInst = inst;
            }));
        yield return wait;

        RopeBullet ropeBullet = bulletInst.GetComponent<RopeBullet>();
        
        //ボタンを離した場合
        if(!ropeBullet.IsHit)
        {
            CreateRopeFailed(normalRopePrefab, rope, bulletInst);
            Destroy(bulletInst);
            yield break;
        }
        
        CreateNormalRope(rope, ropeBullet.HitInfo);
        Destroy(bulletInst);
    }

    IEnumerator ShootCatchRope()
    {
        //アップデートが終わるまで待機
        GameObject bulletInst = null;
        var wait = StartCoroutine(WaitForBullet(catchRope.sync.position, catchRope.sync, catchRope.shootButton[0],
            (inst) => 
            {
                bulletInst = inst;
            }, 
            false));
        yield return wait;
        
        RopeBullet ropeBullet = bulletInst.GetComponent<RopeBullet>();

        if(!ropeBullet.IsHit)
        {
            CreateRopeFailed(catchRopePrefab, catchRope, bulletInst);
            Destroy(bulletInst);
            yield break;
        }

        CreateCatchRope(ropeBullet.HitInfo, catchRope);
        Destroy(bulletInst);
    }

    void Shoot(GameObject bulletInst)
    {
        Vector3 dir = GetShootDirection(bulletInst.transform.position);

        RopeBullet ropeBullet = bulletInst.GetComponent<RopeBullet>();
        ropeBullet.direction = dir.normalized;
        ropeBullet.speed     = bulletSpeed;
    }

    Vector3 GetShootDirection(Vector3 shootPosition)
    {
        //nullを入れたいので
        Vector3? position = IsPlayerBeforePoint(shootPosition, 50.0f);
        if(position.HasValue)
        {
            return position.Value;
        }

        Ray ray = Camera.main.ScreenPointToRay(reticle.position);
        Vector3 point = cameraInfo.position + (ray.direction * 50);
        return  point - shootPosition;
    }

    bool IsPlayerBeforePoint_(Vector3 point)
    {
        Vector3 trans2HitPoint = point - transform.position;
        Vector3 trans2Camera   = transform.position - cameraInfo.position;
        float dot = Vector3.Dot(trans2Camera, trans2HitPoint);
        
        //ドット積が正 -> 前方向にある
        return dot > 0.0f;
    }

    Vector3? IsPlayerBeforePoint(Vector3 shootPosition, float maxDistance)
    {
        //当たった場所を検証
        Ray ray = Camera.main.ScreenPointToRay(reticle.position);
        
        //Playerは判定しない
        int ignoreLayer =  PlayersLayerMask.IgnorePlayerAndRopes;

        RaycastHit[] raycasthit = Physics.RaycastAll(ray, maxDistance, ignoreLayer);

        if(raycasthit.Length == 0) return null;

        foreach(RaycastHit hit in raycasthit)
        {
            //プレイヤーとカメラの間にオブジェクトがあると意図しない方向に飛ぶ為
            if(!IsPlayerBeforePoint_(hit.point)) continue;
            return hit.point - shootPosition;
        }

        return null;
    }

    IEnumerator WaitForBullet(Vector3 shootPosition, Transform target,string shootButton, UnityAction<GameObject> callback, bool isButton = true)
    {
        //射出弾の生成
        GameObject bulletInst = Instantiate(bulletPrefab);
        bulletInst.transform.position = shootPosition;

        //弾丸を飛ばす
        Shoot(bulletInst);

        RopeBullet ropeBullet = bulletInst.GetComponent<RopeBullet>();
        ropeBullet.target = target;

        callback(bulletInst);
        //何かに当たるか、ボタンを離したら終了
        while(true)
        {
            if(Input.GetButtonUp(shootButton) && isButton) yield break;
            if(ropeBullet.IsHit)                           yield break;
            if(ropeBullet.Distance >= normalRopeDistance)  yield break;

            yield return null;
        }
    }

    bool IsBothRopeButtonDown()
    {
        return InputExtension.GetButtonAll(center.shootButton);
    }

    void CreateRopeFailed(GameObject prefab, NormalRope rope, GameObject bullet)
    {
        GameObject         ropeInst     = Instantiate(prefab) as GameObject;
        NormalRopeSimulate ropeSimulate = ropeInst.GetComponent<NormalRopeSimulate>();

        ListLineDraw lineDraw = ropeInst.GetComponent<ListLineDraw>();
        lineDraw.DrawStart();

        ropeSimulate.Initialize(bullet.transform.position, rope.sync.position);
        ropeSimulate.SimulationEnd(rope.sync);
    }

    void CreateCenterNormalRope()
    {
        Vector3 centerPos = Vector3.Lerp(right.ropeInst.originPosition,
                                         left .ropeInst.originPosition, 0.5f);

        GameObject         rope     = Instantiate(normalRopePrefab) as GameObject;
        NormalRopeSimulate simulate = rope.GetComponent<NormalRopeSimulate>();

        simulate.Initialize(centerPos, center.sync.position);
        simulate.isCalcDistance = true;

        center.ropeInst = simulate;

        Vector3 leftVelocity   = left .ropeInst.velocity;
        Vector3 rightVelocity  = right.ropeInst.velocity;

        Vector3 centerVelocity = leftVelocity + rightVelocity;

        simulate.AddForce(centerVelocity, ForceMode.VelocityChange);

        left .ropeInst.SimulationStop();
        right.ropeInst.SimulationStop();

        ListLineDraw lineDraw = center.ropeInst.GetComponent<ListLineDraw>();
        lineDraw.DrawEnd();

        MeshRenderer renderer = center.ropeInst.hookRenderer;
        Destroy(renderer);
    }

    public void CreateCatchRope(Collision hitInfo, NormalRope rope)
    {
        GameObject         ropeInst = Instantiate(catchRopePrefab) as GameObject;
        NormalRopeSimulate simulate = ropeInst.GetComponent<NormalRopeSimulate>();

        simulate.Initialize(hitInfo.contacts[0].point, catchRope.sync.position);
        simulate.SimulationStop();

        ListLineDraw lineDraw = ropeInst.GetComponent<ListLineDraw>();
        lineDraw.DrawStart();

        CatchRope inst = ropeInst.GetComponent<CatchRope>();
        inst.sync = rope.sync;

        bool canRopeHook = hitInfo.transform.tag != "NoRopeHit";

        if(canRopeHook)
        {
            //引っかかった
            catchRope.ropeInst = simulate;
            SendCreateNormalRopeEvent(simulate);
        }
        else
        {
            //引っかからなかった キャンセル
            simulate.SimulationEnd(rope.sync);
        }
    }

    public void CreateNormalRope(NormalRope rope, Collision hitInfo)
    {
        GameObject         ropeInst = Instantiate(normalRopePrefab) as GameObject;
        NormalRopeSimulate simulate = ropeInst.GetComponent<NormalRopeSimulate>();

        simulate.Initialize(hitInfo.contacts[0].point, rope.sync.position);

        ListLineDraw lineDraw = ropeInst.GetComponent<ListLineDraw>();
        lineDraw.DrawStart();

        bool canRopeHook = hitInfo.transform.tag != "NoRopeHit";

        if(Input.GetButton(rope.shootButton[0]) && canRopeHook)
        {
            switch(rope.shootButton[0])
            {
                case RopeInput.leftButton:  left.ropeInst  = simulate; break;
                case RopeInput.rightButton: right.ropeInst = simulate; break;
            }

            SendCreateNormalRopeEvent(simulate);

            if(IsCenterRopeCreate())
            {
                CreateCenterNormalRope();
            }
        }
        else
        {
            //引っかからなかった キャンセル
            simulate.SimulationEnd(rope.sync);
        }
    }

    bool IsCenterRopeCreate()
    {
        //どちらも存在している場合はtrue
        return left .IsCanControl && right.IsCanControl;
    }

    public bool CreateLockRope(Vector3 shootPosition, float ropeDistance)
    {
        Vector3? result = IsPlayerBeforePoint(Vector3.zero, ropeDistance);
        if(!result.HasValue) return false;

        GameObject lockRopeInst = Instantiate(lockRopePrefab);
        LockRope   lockRope     = lockRopeInst.GetComponent<LockRope>();
        lockRope.Initialize(shootPosition, result.Value);
        return true;
    }

    public bool CreateCatchRope(Vector3 shootPosition, float ropeDistance)
    {
        Vector3? result = IsPlayerBeforePoint(Vector3.zero, ropeDistance);
        if(!result.HasValue) return false;

        GameObject catchRopeInst = Instantiate(catchRopePrefab);
        return true;
    }

    delegate bool RopeEvent(NormalRope rope);
    delegate void Action(NormalRope rope);
    void RopeSendEvent(Action action)
    {
        RopeEvent e = (rope)=> {
            if(!rope.IsCanControl) return false;
            action(rope);
            return true;
        };

        if(e(center)) return;
        if(e(left  )) return;
        if(e(right )) return;
    }

    public void AddForce(Vector3 force, ForceMode forceMode)
    {
        RopeSendEvent((rope) => { rope.ropeInst.AddForce(force, forceMode); });
    }

    public void AddLength(float add)
    {
        RopeSendEvent((rope) => { rope.ropeInst.AddLength(add); });
    }

    public void SubLength(float sub)
    {
        AddLength(-sub);
    }

    public void SimulateStart()
    {
        RopeSendEvent((rope) => { rope.ropeInst.SimulationStart(); });
    }

    public void SimulateStop()
    {
        RopeSendEvent((rope) => { rope.ropeInst.SimulationStop(); });
        if(catchRope.RopeExist)
        {
            catchRope.ropeInst.SimulationStop();
        }
    }

    /// <summary>
    /// ロープの動きに指定したトランスフォームを同期
    /// </summary>
    /// <param name="syncTransform">同期させるトランスフォーム</param>
    public void SyncTransformToRope(Transform syncTransform)
    {
        if(center.RopeExist)
        {
            SyncTransformToRope_(center, syncTransform);
            SyncRopeToTransform_(left);
            SyncRopeToTransform_(right);
            return;
        }

        if(left .RopeExist) SyncTransformToRope_(left , syncTransform);
        if(right.RopeExist) SyncTransformToRope_(right, syncTransform);
    }

    private void SyncTransformToRope_(NormalRope rope, Transform syncTransform)
    {
        //差分を計算
        Vector3 offset = syncTransform.position - rope.sync.position;
        Vector3 transformPosition = offset + rope.ropeInst.tailPosition;
        syncTransform.position = transformPosition;
    }

    /// <summary>
    /// トランスフォームの動きにロープを同期
    /// </summary>
    public void SyncRopeToTransform()
    {
        SyncRopeToTransform_(center);
        SyncRopeToTransform_(left);
        SyncRopeToTransform_(right);
    }

    private void SyncRopeToTransform_(NormalRope rope)
    {
        if(!rope.IsCanControl) return;
        rope.ropeInst.tailPosition = rope.sync.position;
    }

    //イベントを送信
    //SendMessage("OnNormalRopeCreate");と同じ
    void SendCreateNormalRopeEvent(NormalRopeSimulate rope)
    {
        ExecuteEvents.Execute<RopeEventHandlar>(
            gameObject,
            null,
            (obj, baseEvent) =>
            {
                obj.OnNormalRopeCreate(rope);
            }
        );
    }

    //イベントを送信
    //SendMessage("OnRopeRelease");と同じ
    void SendNormalRopeReleaseEvent(NormalRopeSimulate rope)
    {
        ExecuteEvents.Execute<RopeEventHandlar>(
            gameObject,
            null,
            (obj, baseEvent) =>
            {
                obj.OnNormalRopeRelease(rope);
            }
        );
    }
}