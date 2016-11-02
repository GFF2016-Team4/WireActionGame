using UnityEngine;
using System.Collections;

public class kinoko : MonoBehaviour
{
    public GameObject targetPos;
    public GameObject kinokoPos;
    public float speed;
    public float Power;     //ジャンプ力
    public float limit;     //ジャンプする時間
    private bool isJump;        //ジャンプしてるか
    private float Jumptemp;     //Jumplimit保存用

    // Use this for initialization
    void Start()
    {
        isJump = false;
    }

    // Update is called once per frame
    void Update()
    {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Rigidbody rd = GetComponent<Rigidbody>();
        rd.AddForce(x * speed, 0, z * speed);

        //ジャンプ処理
        if (isJump == true && limit >= 0)
        {
            //rd.AddForce(Vector3.up * Power);

            limit -= 1.0f;
            Debug.Log("ジャンプ中です");
        }
        if (limit <= 0)
        {
            isJump = false;
            limit = Jumptemp;
        }

        //if (Input.GetKey(KeyCode.W))
        //{
        //    transform.position += transform.forward * 0.1f;
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    transform.position -= transform.forward * 0.1f;
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    transform.Rotate(0, 10, 0);
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    transform.Rotate(0, -10, 0);
        //}
    }

    //Jumpタグオブジェクトに接触したら
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player" && isJump == false)
        {
            isJump = true;
            Jumptemp = limit;

            Debug.Log("ジャンプします");
        }
    }
}
