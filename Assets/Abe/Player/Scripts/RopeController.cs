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

        public bool RopeExist
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

    public bool LeftRopeExist
    {
        get { return left.RopeExist; }
    }

    public bool RightRopeExist
    {
        get { return right.RopeExist; }
    }

    public bool CenterRopeExist
    {
        get { return centerRopeInst != null; }
    }

    public bool RopeExist
    {
        get
        {
            return LeftRopeExist || RightRopeExist || CenterRopeExist;
        }
    }

    void Update()
    {
        Shoot();
        TakeUp();
    }



    public void Sync()
    {
        Sync(left);
        Sync(right);
    }

    void Shoot()
    {
        if(RopeInput.isLeftRopeButtonDown)  LeftRopeShoot();
        if(RopeInput.isRightRopeButtonDown) RightRopeShoot();
    }

    void Sync(RopeGun ropeGun)
    {
        if(!ropeGun.RopeExist) return;
        ropeGun.ropeInst.SetLockTailPosition(ropeGun.gun.position);
    }

    void TakeUp()
    {
        bool isLeftUp = RopeInput.isLeftRopeButtonUp;
        bool isRightUp = RopeInput.isRightRopeButtonUp;

        if(CenterRopeExist)
        {
            if(isLeftUp || isRightUp)
            {
                SendRopeReleaseEvent(centerRopeInst);
                centerRopeInst.RopeEnd();
                centerRopeInst = null;
            }
        }

        if(isLeftUp)
        {
            TakeUp(ref left);
        }
        if(isRightUp)
        {
            TakeUp(ref right);
        }
    }

    void TakeUp(ref RopeGun ropeGun)
    {
        if(ropeGun.RopeExist)
        {
            SendRopeReleaseEvent(ropeGun.ropeInst);
            ropeGun.ropeInst.RopeEnd();
            ropeGun.ropeInst = null;
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

                ropeSimulate.RopeInitialize(bulleftTrans.position, ropeGun.gun.position, null);
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

        simulate.RopeInitialize(hitPoint, ropeGun.gun.position, hitInfo.transform);

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
        Debug.Assert(left.RopeExist);
        Debug.Assert(right.RopeExist);

        //左のロープと右のロープの中間に作る
        Vector3 left2right = right.ropeInst.originPosition - left.ropeInst.originPosition;
        Vector3 centerPos = left.ropeInst.originPosition + left2right / 2;

        GameObject rope = Instantiate(ropePrefab) as GameObject;
        RopeSimulate simulate = rope.GetComponent<RopeSimulate>();
        rope.GetComponent<ListLineDraw>().DrawEnd();

        simulate.RopeInitialize(centerPos, transform.position + Vector3.up, null);

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