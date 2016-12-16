using UnityEngine;
using System;
using UniRx;

public class Player : MonoBehaviour, RopeEventHandlar
{
    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float airMoveSpeed;

    [SerializeField, Tooltip("ジャンプの強さ")]
    private float jumpPower;

    [SerializeField, Range(0, 1), Header("上方向の減衰値")]
    private float upVecDampingPow;

    [SerializeField, Range(0, 1), Header("横方向の減衰値")]
    private float sideVecDampingPow;

    [SerializeField]
    private float ropeAcceleration;

    [SerializeField, Header("ロープの伸縮スピード")]
    private float ropeTakeSpeed;

    [SerializeField, Header("ロープの伸縮時に加える力")]
    private float ropeTakeForce;

    [SerializeField, Header("ロープの加える力")]
    private float ropeForcePower;

    [SerializeField, Header("移動用・捕獲用ロープの飛ぶ長さ")]
    private float normalRopeDistance = Mathf.Infinity;

    [SerializeField, Header("打ち込み用ロープの飛ぶ長さ")]
    private float lockRopeDistance = 50.0f;

    [SerializeField]
    private Transform footOrigin;

    bool isJump = false;
    float jumpTime = 0.0f;
    Vector3 gravity;
    Vector3 playerVelocity;

    RopeController ropeController;

    PlayerMove     playerMove;
    PlayerRopeMove playerRopeMove;

    PlayerCameraInfo cameraInfo;

    [NonSerialized]
    public Animator animator;
    [NonSerialized]
    public CharacterController controller;

    public bool IsJump
    {
        get { return isJump; }
    }

    public bool IsRopeExist
    {
        get { return ropeController.IsRopeExist; }
    }

    void Awake()
    {
        playerMove     = gameObject.AddComponent<PlayerMove>();
        playerRopeMove = gameObject.AddComponent<PlayerRopeMove>();

        playerMove.player     = this;
        playerRopeMove.player = this;

        animator       = GetComponent<Animator>();
        controller     = GetComponent<CharacterController>();
        ropeController = GetComponent<RopeController>();

        cameraInfo     = GetComponent<PlayerCameraInfo>();

        //物理演算と出来る限り同じ挙動にするため
        gravity = Physics.gravity;
    }

    void Start()
    {
        ropeController.normalRopeDistance = normalRopeDistance;
    }

    void Update()
    {
        bool keyDown = RopeInput.isLeftRopeButton || RopeInput.isRightRopeButton;
        playerMove.enabled     = !keyDown;
        playerRopeMove.enabled =  keyDown;
    }

    public bool IsGround()
    {
        //CharacterControllerのisGroundedだけだとうまく判定しない為
        Vector3 position = footOrigin.position;
        position.y += transform.localScale.y * 0.2f; //始点を上げないと
        Vector3 size = new Vector3()
        {
            x = controller.radius * transform.localScale.x,
            y = transform.localScale.y,
            z = controller.radius * transform.localScale.z
        };

        int  layerMask = -1 - (1 << gameObject.layer |
                               1 << LayerMask.NameToLayer("Rope/Normal") |
                               1 << LayerMask.NameToLayer("Rope/Lock"));

        bool isBoxHit  = Physics.CheckBox(position, size, Quaternion.identity, layerMask);

        bool isUpVelocity = playerVelocity.y > 0.25f;

        return ((controller.isGrounded || isBoxHit) && !isUpVelocity);
    }

    public void NormalMove()
    {
        if(isJump) isJump = false;
        Vector3 velocity   = cameraInfo.GetInputVelocity();
        float   speed      = velocity.magnitude;

        //velocityがゼロベクトルだとだと変な方向に向くため
        if(velocity != Vector3.zero) transform.forward = velocity;

        //回転の後に移動
        //アニメーションで移動させる
        animator.SetFloat("MoveSpeed", speed);
        animator.speed = moveSpeed;
    }

    public void AirMove()
    {
        if(IsGround()) return;
        transform.position += cameraInfo.GetInputVelocity() * airMoveSpeed * Time.deltaTime;
    }

    public void RopeControl()
    {
        Vector3 velocity = cameraInfo.GetInputVelocity();
        if(velocity != Vector3.zero && !IsGround())
        {
            ropeController.AddForce(velocity * ropeForcePower, ForceMode.Force);
        }

        RopeTake();
    }

    public void RopeTake()
    {
        ropeController.AddLength(ropeTakeSpeed * -Input.GetAxisRaw("RopeTake") * Time.deltaTime);
    }
    public void Jump()
    {
        if(!IsGround())                  return;
        if(!Input.GetButtonDown("Jump")) return;

        playerVelocity += Vector3.up * jumpPower;
        controller.Move(playerVelocity * Time.deltaTime);
        isJump = true;
        jumpTime = Time.time;
    }

    public void ApplyGravity()
    {
        DampingVerticalVelocity();
        DampingHorizontalVelocity();

        playerVelocity += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        CheckJumpState();
    }

    void CheckJumpState()
    {
        //重力加速度をみて初速度より速くなればjump状態解除
        if(jumpPower + gravity.y * (Time.time - jumpTime) < -jumpPower * Time.deltaTime)
        {
            isJump = false;
        }
    }

    void DampingVerticalVelocity()
    {
        if(playerVelocity.y > 0.1f)
        {
            playerVelocity.y *= upVecDampingPow;
        }
    }

    void DampingHorizontalVelocity()
    {
        Vector2 hrVel = new Vector2(playerVelocity.x, playerVelocity.z);
        if(hrVel.magnitude == 0) return;

        hrVel *= sideVecDampingPow;
        playerVelocity.x = hrVel.x;
        playerVelocity.z = hrVel.y;
    }

    public void ResetGravity()
    {
        //地面についているときなど
        playerVelocity = Vector3.zero;
    }

    public void ResetUpTrans()
    {
        transform.up = Vector3.up;
    }

    public void StartRopeSimulate()
    {
        ropeController.SimulateStart();
    }

    public void StopRopeSimulate()
    {
        ropeController.SimulateStop();
    }

    /// <summary>
    /// ロープの動きにプレイヤーを同期させる
    /// </summary>
    public void SyncPlayerToRope()
    {
        playerVelocity = Vector3.zero;
        transform.up = ropeController.Direction;

        ropeController.SimulateStart();
        ropeController.SyncTransformToRope(transform);
    }

    /// <summary>
    /// プレイヤーの動きにロープを同期させる
    /// </summary>
    public void SyncRopeToPlayer()
    {
        ropeController.SimulateStop();
        ropeController.SyncRopeToTransform();
    }

    public void OnNormalRopeCreate(NormalRopeSimulate rope)
    {
        rope.AddForce(playerVelocity * ropeAcceleration, ForceMode.VelocityChange);
        ResetGravity();
    }

    public void OnNormalRopeRelease(NormalRopeSimulate rope)
    {
        ResetGravity();
        if(IsGround()) return;
        playerVelocity = rope.velocity;
    }

    public void ShootLockRope()
    {
        if(!Input.GetButtonDown("LockRope")) return;
        
        Vector3 rayOffset = Vector3.up * 0.01f;
        if(ropeController.CreateLockRope(footOrigin.position + rayOffset, lockRopeDistance))
        {
            animator.SetTrigger("ShootLockRope");
        }
    }
}