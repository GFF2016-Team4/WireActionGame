using UnityEngine;
using System.Collections;

public class InertialCharacterController : MonoBehaviour
{
    public float m_MaxSpeed = 4.0f;         // 最高速度（メートル/秒）
    public float m_MinSpeed = -2.0f;        // 最低速度（最大バック速度）
    public float m_AccelPower = 2.0f;       // 加速度（メートル/秒/秒）
    public float m_BrakePower = 6.0f;       // ブレーキ速度（メートル/秒/秒）
    public float m_RotateSpeed = 180.0f;    // 回転速度（度/秒）
    public float m_Gravity = 18.0f;         // 重力加速度（メートル/秒/秒）
    public float m_JumpPower = 0.0f;        // ジャンプ力（初速(メートル/秒)）

    float m_VelocityY = 0f;                 // y軸方向の移動量
    float m_Speed = 0f;                     // 前進速度（前進はプラス、後退はマイナス）

    int m_Flag = 0;                         //攻撃パターンフラグ

    public Transform child;                 //Enemyの子供

    /************************************************************************
                                  仮宣言 
    ************************************************************************/
    public Transform rightHand;          //右腕Transform
    float x, y, speed;                   //ローテート引数

    CharacterController m_Controller;    //キャラクタコントローラ
    Animator m_Animator;                 //アニメター

    public GameObject deadReplacement;   //死亡切替オブジェクト
    private bool deadReplace;            //死亡切替を行うか?
    private int ropeCounter;             //ロープとエネミーの接触点数


    void Start()
    {
        //初期化
        deadReplace = false;
        m_Controller = GetComponent<CharacterController>();

        ropeCounter = 0;

        //アニメターは子のアニメターを取得
        //m_Animator = transform.Find("EnemyRobot").GetComponent<Animator>();             //こっちはfbx形式
        m_Animator = transform.Find("EnemyRobotPre_Ragdoll").GetComponent<Animator>();  //こっちはラグドール

        /************************************************************************
                                    仮初期化 
        ************************************************************************/
        //右腕の取得（使えるかどうか確定できていない）
        //rightHand = transform.Find("mixamorig:RightArm").GetComponent<Transform>();
        speed = 1;
    }

    void Update()
    {
        //垂直軸の値取得
        float axisVertical = Input.GetAxisRaw("Vertical");

        // 減速する（入力が無い場合 or 進行方向と逆に入力がある場合）
        if ((axisVertical == 0) || (m_Speed * axisVertical < 0))
        {
            if (m_Speed > 0)
            {
                m_Speed = Mathf.Max(m_Speed - m_BrakePower * Time.deltaTime, 0);
            }
            else {
                m_Speed = Mathf.Min(m_Speed + m_BrakePower * Time.deltaTime, 0);
            }
        }

        // 上下キーで加速
        m_Speed += m_AccelPower * axisVertical * Time.deltaTime;

        // 速度制限
        m_Speed = Mathf.Clamp(m_Speed, m_MinSpeed, m_MaxSpeed);

        // 速度を、エネミーが向いている方向のベクトルに変換する
        Vector3 velocity = transform.forward * m_Speed;

        // 接地しているなら
        if (m_Controller.isGrounded)
        {
            // 落下を止める
            m_VelocityY = 0;
        }

        // スペースキーが押されたら、ジャンプする
        if (m_Controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            m_VelocityY = m_JumpPower;
        }

        // 重力加速度を加算
        m_VelocityY -= m_Gravity * Time.deltaTime;

        // y軸方向の移動量を加味する
        velocity.y = m_VelocityY;

        // CharacterControllerに命令して移動する

        //アニメターに数値を知らせる
        m_Animator.SetFloat("Speed", m_Speed);
        m_Animator.SetInteger("Flag", m_Flag);

        //エネミーを即死させる
        if (Input.GetKey(KeyCode.Z))
        {
            deadReplace = true;
        }
        //死亡交代
        if (deadReplace)
        {
            //アニメターを閉じる(ラグドール用)
            m_Animator.enabled = false;
            m_Controller.enabled = false;
            transform.GetComponent<CapsuleCollider>().enabled = false;

            //新しいラグドールを生成してモデル交代（fbx用）
            //Destroy(gameObject);

            //Transform dead = Instantiate(deadReplacement, transform.position, transform.rotation) as Transform;
            //dead.transform.position = gameObject.transform.position;
            //dead.transform.rotation = gameObject.transform.rotation;

            //CopyTransformsRecurse(transform, dead);
        }
        else
        {
            // 左右キーで回転
            transform.Rotate(0, Input.GetAxis("Horizontal") * m_RotateSpeed * Time.deltaTime, 0);

            //移動させる
            m_Controller.Move(velocity * Time.deltaTime);
        }

        /****************************************************************
                                    仮更新
        ****************************************************************/
        //右腕のカウント数(使えるかどうか)
        //Debug.Log("rightHand:" + rightHand.transform.childCount);

        //敵部分的な回転
        //if (Input.GetMouseButton(0))
        //{
        //    rightHand.Rotate(0, 0, 90);
        //}

        //使い方わかんないっす
        //m_Animator.GetBoneTransform(rightHand).transform.Rotate(x, y, 0);

        //攻撃です！
        if (Input.GetKeyDown(KeyCode.L))
        {
            m_Flag = 1;
        }
        //もう一種の攻撃です！
        else if (Input.GetKeyDown(KeyCode.K))
        {
            m_Flag = 2;
        }
        //攻撃回収です！（多分）
        else if (Input.GetKeyUp(KeyCode.L))
        {
            m_Flag = 0;
        }

        //アバターマスクテスト
        if (Input.GetKey(KeyCode.Alpha1))
        {
            m_Speed -= Time.deltaTime * 0.01f;
            if (m_Speed <= 0)
            {
                m_Speed = 0;
            }
            m_Animator.SetLayerWeight(1, 1f);
            //m_Animator.SetLayerWeight(2, 1f);
            //m_Animator.SetLayerWeight(4, 1f);
            //m_Animator.SetLayerWeight(5, 1f);
            //m_Controller.Move(Vector3.zero);
            //transform.Rotate(Vector3.zero);
        }
        else
        {
            m_Animator.SetLayerWeight(1, 0);
            m_Animator.SetLayerWeight(3, 0);
            m_Animator.SetLayerWeight(4, 0);
        }

        if (Input.GetKey(KeyCode.Alpha2)) m_Animator.SetLayerWeight(2, 1);
        else m_Animator.SetLayerWeight(2, 0);

        if (Input.GetKey(KeyCode.Alpha3)) m_Animator.SetLayerWeight(3, 1);
        else m_Animator.SetLayerWeight(3, 0);

        if (Input.GetKey(KeyCode.Alpha4)) m_Animator.SetLayerWeight(4, 1);
        else m_Animator.SetLayerWeight(4, 0);

        if (Input.GetKey(KeyCode.Alpha5)) m_Animator.SetLayerWeight(5, 1);
        else m_Animator.SetLayerWeight(5, 0);

    }

    //Transformの循環コピー
    static void CopyTransformsRecurse(Transform src, Transform dst)
    {
        dst.position = src.position;
        dst.rotation = src.rotation;

        foreach (Transform child in dst)
        {
            // 同じ名前でTransformをマッチする = Match the transform with the same name
            Transform curSrc = src.Find(child.name);
            if (curSrc)
                CopyTransformsRecurse(curSrc, child);
        }
    }

    //死んでいますか?
    public bool IsDead()
    {
        return deadReplace;
    }

    //トリガーに入ると同時に
    public void OnTriggerEnter(Collider other)
    {
        //ロープカウンタ数を+1
        if (other.gameObject.tag == "Rope")
            ropeCounter += 1;
        
        Debug.Log("入ったぜ!");

    }

    //トリガーに出ると同時に
    public void OnTriggerExit(Collider other)
    {
        //ロープカウンタ数を-1
        if (other.gameObject.tag == "Rope")
            ropeCounter -= 1;

        Debug.Log("出たぜ!");

    }

    //トリガに入っているときに
    public void OnTriggerStay(Collider other)
    {
        //同時に4点がロープとぶつかっているなら死ぬ
        if (ropeCounter >= 4)
            deadReplace = true;

        Debug.Log(ropeCounter);
    }
}
