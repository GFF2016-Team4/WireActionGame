using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RopeController : MonoBehaviour
{
    [System.Serializable]
    public struct RopeGun
    {
        [SerializeField, Tooltip("射出機")]
        public Transform gun;

        [SerializeField]
        public RopeSimulate ropeInst;

        public bool ropeExist
        {
            get
            {
                return ropeInst != null;
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
    public RopeGun left;

    [SerializeField]
    public RopeGun right;


    [Header("Center")]
    public RopeSimulate centerRopeInst;

    public bool leftRopeExist
    {
        get
        {
            return left.ropeExist;
        }
    }

    public bool rightRopeExist
    {
        get
        {
            return right.ropeExist;
        }
    }

    public bool centerRopeExist
    {
        get
        {
            return centerRopeInst != null;
        }
    }

    public bool ropeExist
    {
        get
        {
            return leftRopeExist || rightRopeExist || centerRopeExist;
        }
    }

    public void FixedUpdate()
    {
        Sync(left);
        Sync(right);
    }

    void Update()
    {
        Shoot();
        TakeUp();
    }

    public void LateUpdate()
    {
        Sync(left);
        Sync(right);
    }

    void Shoot()
    {
        if(RopeInput.isLeftRopeButtonDown)
        {
            LeftRopeShoot();
        }
        if(RopeInput.isRightRopeButtonDown)
        {
            RightRopeShoot();
        }
    }

    void Sync(RopeGun ropeGun)
    {
        if(!ropeGun.ropeExist)
            return;
        ropeGun.ropeInst.SetLockTailPosition(ropeGun.gun.position);
    }

    void TakeUp()
    {
        bool isLeftUp = RopeInput.isLeftRopeButtonUp;
        bool isRightUp = RopeInput.isRightRopeButtonUp;

        if(centerRopeExist)
        {
            if(isLeftUp || isRightUp)
            {
                SendRopeReleaseEvent(centerRopeInst);
                centerRopeInst.RopeEnd();
            }
        }

        if(isLeftUp)
        {
            TakeUp(left);
        }
        if(isRightUp)
        {
            TakeUp(right);
        }
    }

    void TakeUp(RopeGun ropeGun)
    {
        if(ropeGun.ropeExist)
        {
            SendRopeReleaseEvent(ropeGun.ropeInst);
            ropeGun.ropeInst.RopeEnd();
        }
    }

    void LeftRopeShoot()
    {
        StartCoroutine(RopeShoot(left, RopeInput.leftButton, (result) => left.ropeInst = result));
    }

    void RightRopeShoot()
    {
        StartCoroutine(RopeShoot(right, RopeInput.rightButton, (result) => right.ropeInst = result));
    }

    IEnumerator RopeShoot(RopeGun ropeGun, string buttonName, UnityAction<RopeSimulate> callback)
    {
        //射出弾の生成
        GameObject bulletInst = Instantiate(bulletPrefab) as GameObject;
        Transform bulleftTrans = bulletInst.transform;
        bulleftTrans.position = ropeGun.gun.position;

        Ray ray = new Ray(camera.position, camera.forward);
        //飛ばす
        Rigidbody bulletRig = bulletInst.GetComponent<Rigidbody>();

        Vector3 dir = Vector3.zero;
        RaycastHit[] raycasthit = Physics.RaycastAll(ray, 50.0f);
        if(raycasthit.Length != 0) //当たった場合
        {
            foreach(RaycastHit hit in raycasthit)
            {
                Vector3 player2HitPoint = hit.point - transform.position;
                float dot = Vector3.Dot(camera.forward, player2HitPoint);

                //プレイヤーより前にオブジェクトがある
                if(dot > 0)
                {
                    dir = hit.point - ropeGun.gun.position;
                    break;
                }
            }
        }
        else
        {
            Vector3 point = camera.position + (camera.forward * 50);
            dir = point - ropeGun.gun.position;
        }
        bulletRig.AddForce(dir.normalized * bulletSpeed, ForceMode.VelocityChange);


        RopeBullet collisionCheck = bulletInst.GetComponent<RopeBullet>();
        collisionCheck.target = ropeGun.gun;

        //当たるまで待機
        while(!collisionCheck.IsCollision)
        {
            if(Input.GetButtonUp(buttonName))
            {
                GameObject ropeInst = Instantiate(ropePrefab) as GameObject;
                RopeSimulate ropeSimulate = ropeInst.GetComponent<RopeSimulate>();

                ropeSimulate.RopeInitialize(bulleftTrans.position, ropeGun.gun.position);
                ropeSimulate.RopeLock();
                ropeSimulate.RopeEnd();
                Destroy(bulletInst);
                yield break;
            }
            yield return null;
        }
        //ロープの生成

        //射出弾の当たった情報
        Collision hitInfo = collisionCheck.CollisionInfo;
        Vector3 hitPoint = hitInfo.contacts[0].point;

        GameObject rope = Instantiate(ropePrefab) as GameObject;
        RopeSimulate simulate = rope.GetComponent<RopeSimulate>();

        simulate.RopeInitialize(hitPoint, ropeGun.gun.position);

        bool canRopeHook = hitInfo.transform.tag != "NoRopeHit";

        if(Input.GetButton(buttonName) && canRopeHook)
        {
            //引っかかった
            callback(simulate);
            SendCreateRopeEvent(simulate);
        }
        else
        {
            //引っかからなかった キャンセル
            simulate.RopeLock();
            simulate.RopeEnd();
        }
        Destroy(bulletInst);
    }

    public void CreateCenterRope()
    {
        Debug.Assert(left.ropeExist);
        Debug.Assert(right.ropeExist);

        //左のロープと右のロープの中間に作る
        Vector3 left2right = right.ropeInst.originPosision - left.ropeInst.originPosision;
        Vector3 centerPos = left.ropeInst.originPosision + left2right / 2;

        GameObject rope = Instantiate(ropePrefab) as GameObject;
        RopeSimulate simulate = rope.GetComponent<RopeSimulate>();
        rope.GetComponent<ListLineDraw>().DrawEnd();

        simulate.RopeInitialize(centerPos, transform.position + Vector3.up);

        //現在の加速度を第三のロープに反映
        Vector3 leftVelocity = left.ropeInst.tailRig.velocity;
        Vector3 rightVelocity = right.ropeInst.tailRig.velocity;

        Vector3 centerVelocity = leftVelocity + rightVelocity;

        simulate.tailRig.AddForce(centerVelocity, ForceMode.VelocityChange);

        centerRopeInst = simulate;
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