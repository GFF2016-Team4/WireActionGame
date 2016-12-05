using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

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
        public string shootButton;

        public bool ropeExist
        {
            get { return ropeInst != null; }
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
    public Rope left;

    public Rope right;

    public Rope center;

    void Start()
    {
        left.shootButton  = "Fire1";
        right.shootButton = "Fire2";
    }

    void Update()
    {
        Shoot(left);
        Shoot(right);
    }

    void Shoot(Rope rope)
    {
        if(Input.GetButtonDown(rope.shootButton))
        {
            StartCoroutine(ShootRope(rope, (result) => 
                rope.ropeInst = result
            ));
        }
    }

    IEnumerator ShootRope(Rope rope, UnityAction<RopeSimulate> callback)
    {
        //射出弾の生成
        GameObject bulletInst = Instantiate(bulletPrefab);
        bulletInst.transform.position = rope.sync.position;

        //弾丸を飛ばす
        Shoot(bulletInst, rope);

        RopeBullet ropeBullet = bulletInst.GetComponent<RopeBullet>();

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
            if(Input.GetKeyUp(rope.shootButton)) yield break;
            if(ropeBullet.IsHit)                 yield break;

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

    void CreateRope(Rope rope, Collision hitInfo, UnityAction<RopeSimulate> callback)
    {
        GameObject ropeInst = Instantiate(ropePrefab) as GameObject;
        RopeSimulate simulate = ropeInst.GetComponent<RopeSimulate>();

        simulate.Initialize(hitInfo.transform.position, rope.sync.position);

        bool canRopeHook = hitInfo.transform.tag != "NoRopeHit";

        if(Input.GetButton(rope.shootButton) && canRopeHook)
        {
            //引っかかった
            callback(simulate);
            //SendCreateRopeEvent(simulate);

            if(IsBothRopeButtonDown())
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

    bool IsBothRopeButtonDown()
    {
        bool isLeftButton  = Input.GetButton(left.shootButton);
        bool isRightButton = Input.GetButton(right.shootButton);
        return isLeftButton && isRightButton;
    }

    void CreateCenterRope()
    {
        Vector3 centerPos = Vector3.Lerp(right.ropeInst.originPosition, 
                                         left .ropeInst.originPosition, 0.5f);

        GameObject   rope     = Instantiate(ropePrefab) as GameObject;
        RopeSimulate simulate = rope.GetComponent<RopeSimulate>();
        rope.GetComponent<ListLineDraw>().DrawEnd();

        simulate.Initialize(centerPos, center.sync.position);

        center.ropeInst = simulate;

        Vector3 leftVelocity  = left .ropeInst.velocity;
        Vector3 rightVelocity = right.ropeInst.velocity;

        Vector3 centerVelocity = leftVelocity + rightVelocity;

        simulate.AddForce(centerVelocity, ForceMode.VelocityChange);
    }
}