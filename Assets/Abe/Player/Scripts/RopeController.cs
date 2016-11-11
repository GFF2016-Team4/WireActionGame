using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RopeController : MonoBehaviour
{
    [SerializeField, Tooltip("ロープのプレハブ")]
    private GameObject ropePrefab;

    [SerializeField, Tooltip("射出弾インスタンス")]
    private GameObject bulletPrefab;

    [SerializeField, Tooltip("射出弾のスピード")]
    private float bulletSpeed;

    [SerializeField, Tooltip("プレイヤーのカメラ")]
    private new Camera camera;

    [Header("Left")]
    [SerializeField, Tooltip("左射出機")]
    public Transform leftGun;

    [System.NonSerialized]
    public RopeSimulate leftRopeInst;

    [Header("Right")]
    [SerializeField, Tooltip("右射出機")]
    public Transform rightGun;

    [System.NonSerialized]
    public RopeSimulate rightRopeInst;

    public bool LeftRopeExist
    {
        get { return leftRopeInst != null;  }
    }

    public bool RightRopeExist
    {
        get { return rightRopeInst != null; }
    }

    void Update()
    {
        Shoot();
        Sync();
        TakeUp();
    }

    void Shoot()
    {
        if(RopeInput.isLeftRopeButtonDown)  { LeftRopeShoot();  }
        if(RopeInput.isRightRopeButtonDown) { RightRopeShoot(); }
    }

    void Sync()
    {
        if(LeftRopeExist)
        {
            leftRopeInst.SetLockTailPosition(leftGun.position);
        }

        if(RightRopeExist)
        {
            rightRopeInst.SetLockTailPosition(rightGun.position);
        }
    }

    void TakeUp()
    {
        if(RopeInput.isLeftRopeButtonUp  && LeftRopeExist)
        {
            leftRopeInst .RopeEnd();
        }

        if(RopeInput.isRightRopeButtonUp && RightRopeExist)
        {
            rightRopeInst.RopeEnd();
        }
    }
    
    void LeftRopeShoot()
    {
        StartCoroutine(RopeShoot(leftGun,  RopeInput.leftButton , (result)=>leftRopeInst  = result));
    }

    void RightRopeShoot()
    {
        StartCoroutine(RopeShoot(rightGun, RopeInput.rightButton, (result)=>rightRopeInst = result));
    }

    //ref outが使えないのでUnityActionを使う
    IEnumerator RopeShoot(Transform gun, string buttonName, UnityAction<RopeSimulate> callback)
    {
        //射出弾の生成
        GameObject bulletInst = Instantiate(bulletPrefab) as GameObject;
        bulletInst.transform.position = gun.position;

        //飛ばす
        Rigidbody bulletRig = bulletInst.GetComponent<Rigidbody>();
        bulletRig.AddForce(camera.transform.forward * bulletSpeed, ForceMode.VelocityChange);

        RopeBullet collisionCheck = bulletInst.GetComponent<RopeBullet>();
        collisionCheck.target = gun;

        //当たるまで待機
        while(!collisionCheck.IsCollision)
        {
            if(Input.GetButtonUp(buttonName))
            {
                GameObject   rope     = Instantiate(ropePrefab) as GameObject;
                RopeSimulate simulate = rope.GetComponent<RopeSimulate>();
                
                simulate.RopeInitialize(bulletInst.transform.position, gun.position);
                simulate.RopeLock();
                simulate.RopeEnd();
                Destroy(bulletInst);
                yield break;
            }
            yield return null;
        }
        {
            //ロープの生成

            //射出弾の当たった情報
            Collision hitInfo  = collisionCheck.CollisionInfo;
            Vector3   hitPoint = hitInfo.contacts[0].point;
        
            GameObject   rope     = Instantiate(ropePrefab) as GameObject;
            RopeSimulate simulate = rope.GetComponent<RopeSimulate>();
        
            simulate.RopeInitialize(hitPoint, gun.position);

            bool canRopeHook = hitInfo.transform.tag != "NoRopeHit";

            if(Input.GetButton(buttonName) && canRopeHook)
            {
                //引っかかった
                SendCreateRopeEvent(simulate);
                callback(simulate);
            }
            else
            {
                //引っかからなかった キャンセル
                simulate.RopeLock();
                simulate.RopeEnd();
            }
        }
        Destroy(bulletInst);
    }

    //イベントを送信
    void SendCreateRopeEvent(RopeSimulate rope)
    {
        ExecuteEvents.Execute<RopeEventHandlar>(
            gameObject,
            null,
            (obj, baseEvent) => { obj.OnRopeCreate(rope); }
        );
    }
}