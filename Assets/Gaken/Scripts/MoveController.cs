using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour
{
    //移動速度
    public float moveSpeed = 5f;
    //ジャンプ力
    public float jumpForce = 5f;
    //重力
    public float gravity = 10.0f;
    //Y軸移動量
    private float velocityY = 0;
    //移動方向
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 playerRotation;
    private Vector3 cameraRotation;
    public Camera camera;

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        //移動量
        Vector3 velocity = Vector3.zero;
        //常に重力を働かせる
        velocityY -= gravity * Time.deltaTime;

        playerRotation = transform.rotation.eulerAngles;
        cameraRotation = camera.transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(playerRotation.x, cameraRotation.y, playerRotation.z);

        //地面に着いているなら
        if (controller.isGrounded)
        {
            //Y軸を0
            velocityY = 0;

            //前後移動
            velocity = transform.forward * Input.GetAxis("Vertical") * moveSpeed;
            //左右移動
            velocity += transform.right * Input.GetAxis("Horizontal") * moveSpeed;

            //スペースでジャンプ
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocityY = jumpForce;
            }
        }
        else
        {
            //前後移動
            velocity = transform.forward * Input.GetAxis("Vertical") * moveSpeed * 0.2f;
            //左右移動
            velocity += transform.right * Input.GetAxis("Horizontal") * moveSpeed * 0.2f;
        }

        //重力の値を与える
        velocity.y = velocityY;
        //移動させる
        controller.Move(velocity * Time.deltaTime);
    }
}
