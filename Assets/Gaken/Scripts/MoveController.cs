using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour
{
    public float moveSpeed = 6.0f;
    public float jumpForce = 300f;
    public float gravity = 10.0f;

    private float velocityY = 0;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 playerRotation;
    private Vector3 cameraRotation;
    public Camera camera;

    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        //Vector3 velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 velocity = Vector3.zero;
        transform.Rotate(camera.transform.rotation.x, 0, 0);

        velocityY -= gravity * Time.deltaTime;

        //velocity = transform.forward * Input.GetAxis("Vertical") * moveSpeed;

        playerRotation = transform.rotation.eulerAngles;
        cameraRotation = camera.transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(playerRotation.x, cameraRotation.y, playerRotation.z);

        if (controller.isGrounded)
        {
            velocityY = 0;

            //moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //moveDirection = transform.TransformDirection(moveDirection);
            //moveDirection *= moveSpeed;
            velocity = transform.forward * Input.GetAxis("Vertical") * moveSpeed;

            if (Input.GetKeyDown(KeyCode.Space))
                velocityY = jumpForce;
            //transform.GetComponent<Rigidbody>().AddForce(transform.up * jumpForce);
        }

        velocity.y = velocityY;
        controller.Move(velocity * Time.deltaTime);
    }
}
