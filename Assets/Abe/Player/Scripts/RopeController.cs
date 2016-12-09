using UnityEngine;
using UnityEngine.Events;
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

        public bool RopeExist
        {
            get { return ropeInst != null; }
        }

        public bool IsCanConrol
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

    [SerializeField, Tooltip("射出弾インスタンス")]
    private GameObject bulletPrefab;

    [SerializeField, Tooltip("射出弾のスピード")]
    private float bulletSpeed;

    [SerializeField, Tooltip("プレイヤーのカメラ")]
    private new Transform camera;

    [SerializeField]
    NormalRope left;

    [SerializeField]
    NormalRope right;

    [SerializeField]
    NormalRope center;

    public bool IsRopeExist
    {
        get
        {
            return left  .RopeExist ||
                   right .RopeExist ||
                   center.RopeExist;
        }
    }

    public Vector3 Direction
    {
        get
        {
            if(center.IsCanConrol) return center.ropeInst.direction;
            if(left  .IsCanConrol) return left  .ropeInst.direction;
            if(right .IsCanConrol) return right .ropeInst.direction;
            Debug.Assert(false, "ロープは生成されていません");
            throw null;
        }
    }

    void Start()
    {
        left  .shootButton = new string[1];
        right .shootButton = new string[1];
        center.shootButton = new string[2];

        left .shootButton[0]  = "Fire1";
        right.shootButton[0]  = "Fire2";
        center.shootButton[0] = "Fire1";
        center.shootButton[1] = "Fire2"; 
    }

    void Update()
    {
        ShootRopes();
        CheckSimulationEnd();
    }

    void ShootRopes()
    {
        if(Input.GetButtonDown(left.shootButton[0]))
        {
            StartCoroutine(ShootNormalRope(left,  (result) =>
                left.ropeInst  = result
            ));
        }

        if(Input.GetButtonDown(right.shootButton[0]))
        {
            StartCoroutine(ShootNormalRope(right, (result) =>
                right.ropeInst = result
            ));
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
        SendNormalRopeReleaseEvent(rope.ropeInst);
        TakeOverVelocity(left,  rope);
        TakeOverVelocity(right, rope);
        rope.ropeInst.SimulationEnd();
        rope.ropeInst = null;
    }

    void TakeOverVelocity(NormalRope takeOverRope, NormalRope takenOverRope)
    {
        if(!takeOverRope.IsCanConrol) return;

        takeOverRope.ropeInst.SimulationStart();
        Vector3 takenOverVelocity = takenOverRope.ropeInst.velocity;
        takeOverRope.ropeInst.AddForce(takenOverVelocity, ForceMode.VelocityChange);
    }

    IEnumerator ShootNormalRope(NormalRope rope, UnityAction<NormalRopeSimulate> callback)
    {
        //射出弾の生成
        GameObject bulletInst = Instantiate(bulletPrefab);
        bulletInst.transform.position = rope.sync.position;

        //弾丸を飛ばす
        Shoot(bulletInst);

        RopeBullet ropeBullet = bulletInst.GetComponent<RopeBullet>();
        ropeBullet.target = rope.sync;

        //アップデートが終わるまで待機
        var wait = StartCoroutine(WaitForBulletUpdate(rope, ropeBullet));
        yield return wait;

        //ボタンを離した場合
        if(!ropeBullet.IsHit)
        {
            CreateNormalRopeFailed(rope, bulletInst);
            Destroy(bulletInst);
            yield break;
        }
        
        CreateNormalRope(rope, ropeBullet.HitInfo, callback);
        Destroy(bulletInst);
    }

    void Shoot(GameObject bulletInst)
    {
        Vector3 dir = GetShootDirection(bulletInst.transform.position);

        Rigidbody bulletRig = bulletInst.GetComponent<Rigidbody>();
        bulletRig.AddForce(dir.normalized * bulletSpeed, ForceMode.VelocityChange);
    }

    Vector3 GetShootDirection(Vector3 shootPosition)
    {
        Vector3? position = IsPlayerBeforePoint(shootPosition);
        if(position.HasValue)
        {
            return position.Value;
        }

        Vector3 point = camera.position + (camera.forward * 50);
        return  point - shootPosition;
    }

    bool IsPlayerBeforePoint_(Vector3 point)
    {
        Vector3 player2HitPoint = point - transform.position;
        float dot = Vector3.Dot(camera.forward, player2HitPoint);
        
        //ドット積が正 -> 前方向にある
        return dot > 0;
    }

    Vector3? IsPlayerBeforePoint(Vector3 shootPosition)
    {
        //当たった場所を検証
        Ray ray = new Ray(camera.position, camera.forward);

        //Playerは判定しない
        int ignoreLayer =  -1 - (1 << gameObject.layer | 1 << LayerMask.NameToLayer("Rope"));
        RaycastHit[] raycasthit = Physics.RaycastAll(ray, 50.0f, ignoreLayer);

        if(raycasthit.Length == 0) return null;

        foreach(RaycastHit hit in raycasthit)
        {
            //プレイヤーとカメラの間にオブジェクトがあると意図しない方向に飛ぶ為
            if(!IsPlayerBeforePoint_(hit.point)) continue;
            return hit.point - shootPosition;
        }

        return null;
    }

    IEnumerator WaitForBulletUpdate(NormalRope rope, RopeBullet ropeBullet)
    {
        //何かに当たるか、ボタンを離したら終了
        while(true)
        {
            if(Input.GetButtonUp(rope.shootButton[0])) yield break;
            if(ropeBullet.IsHit)                       yield break;

            yield return null;
        }
    }

    bool IsBothRopeButtonDown()
    {
        return InputExtension.GetButtonAll(center.shootButton);
    }

    void CreateNormalRopeFailed(NormalRope rope, GameObject bullet)
    {
        GameObject   ropeInst     = Instantiate(normalRopePrefab) as GameObject;
        NormalRopeSimulate ropeSimulate = ropeInst.GetComponent<NormalRopeSimulate>();

        ropeSimulate.Initialize(bullet.transform.position, rope.sync.position);
        ropeSimulate.SimulationEnd();
    }

    void CreateCenterNormalRope()
    {
        Vector3 centerPos = Vector3.Lerp(right.ropeInst.originPosition,
                                         left .ropeInst.originPosition, 0.5f);

        GameObject   rope     = Instantiate(normalRopePrefab) as GameObject;
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
    }

    public void CreateNormalRope(NormalRope rope, Collision hitInfo, UnityAction<NormalRopeSimulate> callback)
    {
        GameObject   ropeInst = Instantiate(normalRopePrefab) as GameObject;
        NormalRopeSimulate simulate = ropeInst.GetComponent<NormalRopeSimulate>();

        simulate.Initialize(hitInfo.contacts[0].point, rope.sync.position);

        bool canRopeHook = hitInfo.transform.tag != "NoRopeHit";

        if(Input.GetButton(rope.shootButton[0]) && canRopeHook)
        {
            //引っかかった
            callback(simulate);
            SendCreateNormalRopeEvent(simulate);

            if(left.IsCanConrol && right.IsCanConrol)
            {
                CreateCenterNormalRope();
            }
        }
        else
        {
            //引っかからなかった キャンセル
            simulate.SimulationEnd();
        }
    }

    public bool CreateLockRope(Vector3 shootPosition)
    {
        Vector3? result = IsPlayerBeforePoint(Vector3.zero);
        if(!result.HasValue) return false;

        GameObject lockRopeInst = Instantiate(lockRopePrefab);
        LockRope   lockRope     = lockRopeInst.GetComponent<LockRope>();
        lockRope.Initialize(shootPosition, result.Value);
        return true;
    }

    delegate bool RopeEvent(NormalRope rope);
    delegate void Action(NormalRope rope);
    void RopeSendEvent(Action action)
    {
        RopeEvent e = (rope)=> {
            if(!rope.IsCanConrol) return false;
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
        if(!rope.IsCanConrol) return;
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