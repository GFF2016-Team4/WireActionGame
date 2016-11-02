using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

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

    [Header("編集不可")]
    [SerializeField, Tooltip("左射出弾インスタンス")]
    private GameObject leftBulletInst;

    [Header("編集不可")]
    [SerializeField, Tooltip("左ロープインスタンス")]
    private GameObject leftRopeInst;

    [Header("Right")]
    [SerializeField, Tooltip("右射出機")]
    public Transform rightGun;

    [Header("編集不可")]
    [SerializeField, Tooltip("右射出弾インスタンス")]
    private GameObject rightBulletInst;

    [Header("編集不可")]
    [SerializeField, Tooltip("右ロープインスタンス")]
    private GameObject rightRopeInst;

    private bool isLeftShoot  = false;
    private bool isRightShoot = false;

    public void OnValidate()
    {
        //変更不可
        if(leftBulletInst  != null) { leftBulletInst  = null; }
        if(leftRopeInst    != null) { leftRopeInst    = null; }
        
        if(rightBulletInst != null) { rightBulletInst = null; }
        if(rightRopeInst   != null) { rightRopeInst   = null; }
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            LeftRopeShoot();
        }

        if(Input.GetButtonDown("Fire2"))
        {
            RightRopeShoot();
        }
    }
    
    void LeftRopeShoot()
    {
        StartCoroutine(RopeShoot(leftGun,  "Fire1", (result)=>leftRopeInst  = result));
    }

    void RightRopeShoot()
    {
        StartCoroutine(RopeShoot(rightGun, "Fire2", (result)=>rightRopeInst = result));
    }

    //ref outが使えないのでUnityActionを使う
    IEnumerator RopeShoot(Transform gun, string buttonName, UnityAction<GameObject> callback)
    {
        //射出弾の生成
        GameObject bulletInst = Instantiate(bulletPrefab) as GameObject;
        bulletInst.transform.position = gun.position;

        //飛ばす
        Rigidbody bulletRig = bulletInst.GetComponent<Rigidbody>();
        bulletRig.AddForce(camera.transform.forward * bulletSpeed, ForceMode.VelocityChange);

        RopeBullet collisionCheck = bulletInst.GetComponent<RopeBullet>();

        //当たるまで待機
        while(!collisionCheck.IsCollision)
        {
            yield return null;
        }
        
        //ロープの生成

        //射出弾のあたった情報
        Collision hitInfo  = collisionCheck.CollisionInfo;
        Vector3   hitPoint = hitInfo.contacts[0].point;
        
        GameObject   rope     = Instantiate(ropePrefab) as GameObject;
        RopeSimulate simulate = rope.GetComponent<RopeSimulate>();
        simulate.RopeInitialize(hitPoint, gun.position);

        simulate.RopeLock();
        if(Input.GetButton(buttonName))
        {
            
            callback(rope);
        }
        else
        {
            simulate.RopeEnd();
        }
    }
}