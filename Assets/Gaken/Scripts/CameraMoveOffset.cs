using UnityEngine;
using System.Collections;

public class CameraMoveOffset : MonoBehaviour
{

    public Transform target;

    public float distance;

    public Vector3 offset;

    [Header("引く速度")]
    public float pullSpeed = 0.1f;

    [Header("戻る速度")]
    public float reboundSpeed = 0.3f;

    [Header("引く距離上限")]
    public float pullDistance = 8.0f;

    // Use this for initialization
    void Start()
    {
        distance = transform.GetComponent<PlayerCamera>().distance;
    }

    // Update is called once per frame
    void Update()
    {
        if (target.GetComponent<Animator>().GetFloat("MoveSpeed") > 0.5f)
        {
            transform.GetComponent<PlayerCamera>().distance += pullSpeed;
            if (transform.GetComponent<PlayerCamera>().distance >= pullDistance) transform.GetComponent<PlayerCamera>().distance = pullDistance;
        }
        else
        {
            transform.GetComponent<PlayerCamera>().distance -= reboundSpeed;
            if (transform.GetComponent<PlayerCamera>().distance <= distance) transform.GetComponent<PlayerCamera>().distance = distance;
        }

        Vector3 position = target.position;       //初期化
        position -= transform.forward * distance; //ターゲットの後ろに下がって見やすいように
        position += offset;                       //オフセット値

        //座標の変更
        transform.position = position;

    }
}
