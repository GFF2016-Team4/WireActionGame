using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{

    public float speed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Rigidbody rd = GetComponent<Rigidbody>();
        rd.AddForce(x * speed, 0, z * speed);

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
}
