using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.EventSystems;

public class RopeController : MonoBehaviour
{
    [System.Serializable]
    public struct Rope
    {
        [SerializeField, Tooltip("同期するトランスフォーム")]
        public Transform sync;

        [SerializeField]
        public RopeSimulate ropeInst;

        [SerializeField]
        public string[] shootButton;

        public bool ropeExist
        {
            get { return ropeInst != null; }
        }

        public bool IsCanConrol
        {
            get
            {
                if(!ropeExist) return false;
                if(!InputExtension.GetButtonAll(shootButton)) return false;
                return true;
            }
        }
    }

    [SerializeField, Tooltip("ロープのプレハブ")]
    private GameObject ropePrefab;

    [SerializeField, Tooltip("射出弾インスタンス")]
    private GameObject bulletPrefab;

    [SerializeField, Tooltip("射出弾のスピード")]
    private float bulletSpeed;

    [SerializeField, Tooltip("プレイヤーのカメラ")]
    private new Transform camera;

    [SerializeField]
    Rope left;

    [SerializeField]
    Rope right;

    [SerializeField]
    Rope center;

    public bool IsRopeExist
    {
        get
        {
            return left  .ropeExist ||
                   right .ropeExist ||
                   center.ropeExist;
        }
    }

    public Vector3 direction
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
        Shoot();
        CheckSimulationEnd();
    }

    void Shoot()
    {
        if(Input.GetButtonDown(left.shootButton[0]))
        {
            StartCoroutine(ShootRope(left,  (result) =>
                left.ropeInst  = result
            ));
        }

        if(Input.GetButtonDown(right.shootButton[0]))
        {
            StartCoroutine(ShootRope(right, (result) =>
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

    void CheckSimulationEnd(ref Rope rope)
    {
        if(!rope.ropeExist) return;
        if(!InputExtension.GetButtonAnyUp(rope.shootButton)) return;
        SendRopeReleaseEvent(rope.ropeInst);
        TakeOverVelocity(left,  rope);
        TakeOverVelocity(right, rope);
        rope.ropeInst.SimulationEnd();
        rope.ropeInst = null;
    }

    void TakeOverVelocity(Rope takeOverRope, Rope takenOverRope)
    {
        if(!takeOverRope.IsCanConrol) return;

        takeOverRope.ropeInst.SimulationStart();
        Vector3 takenOverVelocity = takenOverRope.ropeInst.velocity;
        takeOverRope.ropeInst.AddForce(takenOverVelocity, ForceMode.VelocityChange);
    }

    IEnumerator ShootRope(Rope rope, UnityAction<RopeSimulate> callback)
    {
        //射出弾の生成
        GameObject bulletInst = Instantiate(bulletPrefab);
        bulletInst.transform.position = rope.sync.position;

        //弾丸を飛ばす
        Shoot(bulletInst, rope);

        RopeBullet ropeBullet = bulletInst.GetComponent<RopeBullet>();
        ropeBullet.target = rope.sync;

        //アップデートが終わるまで待機
        var wait = StartCoroutine(WaitForBulletUpdate(rope, ropeBullet));
        yield return wait;

        //ボタンを離した場合
        if(!ropeBullet.IsHit)
        {
            CreateFailed(rope, bulletInst);
            Destroy(bulletInst);
            yield break;
        }
        
        CreateRope(rope, ropeBullet.HitInfo, callback);
        Destroy(bulletInst);
    }

    void Shoot(GameObject bulletInst, Rope rope)
    {
        Vector3 dir = GetShootDirection(rope);

        Rigidbody bulletRig = bulletInst.GetComponent<Rigidbody>();
        bulletRig.AddForce(dir.normalized * bulletSpeed, ForceMode.VelocityChange);
    }

    Vector3 GetShootDirection(Rope rope)
    {
        //当たった場所を検証
        Ray ray = new Ray(camera.position, camera.forward);

        //Playerは判定しない
        int ignoreLayer =  -1 - 1 << gameObject.layer;
        RaycastHit[] raycasthit = Physics.RaycastAll(ray, 50.0f, ignoreLayer);
        if(raycasthit.Length != 0)
        {
            foreach(RaycastHit hit in raycasthit)
            {
                //プレイヤーとカメラの間にオブジェクトがあると意図しない方向に飛ぶ為
                if(!IsPlayerBeforePoint(hit.point)) continue;
                return hit.point - rope.sync.position;
            }
        }

        Vector3 point = camera.position + (camera.forward * 50);
        return  point - rope.sync.position;
    }

    bool IsPlayerBeforePoint(Vector3 point)
    {
        Vector3 player2HitPoint = point - transform.position;
        float dot = Vector3.Dot(camera.forward, player2HitPoint);
        
        //ドット積が正 -> 前方向にある
        return dot > 0;
    }

    IEnumerator WaitForBulletUpdate(Rope rope, RopeBullet ropeBullet)
    {
        //何かに当たるか、ボタンを離したら終了
        while(true)
        {
            if(Input.GetButtonUp(rope.shootButton[0])) yield break;
            if(ropeBullet.IsHit)                       yield break;

            yield return null;
        }
    }

    void CreateFailed(Rope rope, GameObject bullet)
    {
        GameObject   ropeInst     = Instantiate(ropePrefab) as GameObject;
        RopeSimulate ropeSimulate = ropeInst.GetComponent<RopeSimulate>();

        ropeSimulate.Initialize(bullet.transform.position, rope.sync.position);
        ropeSimulate.SimulationEnd();
    }

    bool IsBothRopeButtonDown()
    {
        return InputExtension.GetButtonAll(center.shootButton);
    }

    void CreateCenterRope()
    {
        Vector3 centerPos = Vector3.Lerp(right.ropeInst.originPosition,
                                         left .ropeInst.originPosition, 0.5f);

        GameObject   rope     = Instantiate(ropePrefab) as GameObject;
        RopeSimulate simulate = rope.GetComponent<RopeSimulate>();

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

    public void CreateRope(Rope rope, Collision hitInfo, UnityAction<RopeSimulate> callback)
    {
        GameObject   ropeInst = Instantiate(ropePrefab) as GameObject;
        RopeSimulate simulate = ropeInst.GetComponent<RopeSimulate>();

        simulate.Initialize(hitInfo.contacts[0].point, rope.sync.position);

        bool canRopeHook = hitInfo.transform.tag != "NoRopeHit";

        if(Input.GetButton(rope.shootButton[0]) && canRopeHook)
        {
            //引っかかった
            callback(simulate);
            SendCreateRopeEvent(simulate);

            if(left.IsCanConrol && right.IsCanConrol)
            {
                CreateCenterRope();
            }
        }
        else
        {
            //引っかからなかった キャンセル
            simulate.SimulationEnd();
        }
    }

    delegate bool RopeEvent(Rope rope);
    delegate void Action(Rope rope);
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
        if(center.ropeExist)
        {
            SyncTransformToRope_(center, syncTransform);
            SyncRopeToTransform_(left);
            SyncRopeToTransform_(right);
        }

        if(left .ropeExist) SyncTransformToRope_(left , syncTransform);
        if(right.ropeExist) SyncTransformToRope_(right, syncTransform);
    }

    private void SyncTransformToRope_(Rope rope, Transform syncTransform)
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

    private void SyncRopeToTransform_(Rope rope)
    {
        if(!rope.IsCanConrol) return;
        rope.ropeInst.tailPosition = rope.sync.position;
    }

    //イベントを送信
    //SendMessage("OnRopeCreate");と同じ
    void SendCreateRopeEvent(RopeSimulate rope)
    {
        ExecuteEvents.Execute<RopeEventHandlar>(
            gameObject,
            null,
            (obj, baseEvent) =>
            {
                obj.OnRopeCreate(rope);
            }
        );
    }

    //イベントを送信
    //SendMessage("OnRopeRelease");と同じ
    void SendRopeReleaseEvent(RopeSimulate rope)
    {
        ExecuteEvents.Execute<RopeEventHandlar>(
            gameObject,
            null,
            (obj, baseEvent) =>
            {
                obj.OnRopeRelease(rope);
            }
        );
    }
}