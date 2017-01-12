using UnityEngine;
using System.Collections;

namespace Gaken
{

    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent agent;

        private float time;
        private float time2;

        public bool Attack;
        private float NextAttackTime = 2;
        public Transform Destination;
        public Transform player;

        public float m_MaxSpeed = 4.0f;         // 最高速度（メートル/秒）
        public float m_MinSpeed = -2.0f;        // 最低速度（最大バック速度）
        public float m_AccelPower = 2.0f;       // 加速度（メートル/秒/秒）
        public float m_BrakePower = 6.0f;       // ブレーキ速度（メートル/秒/秒）
        public float m_RotateSpeed = 45.0f;    // 回転速度（度/秒）
        public float m_Gravity = 18.0f;         // 重力加速度（メートル/秒/秒）
        public float m_JumpPower = 0.0f;        // ジャンプ力（初速(メートル/秒)）
        public float m_LazerCoolDown = 20.0f;

        float m_VelocityY = 0f;                 // y軸方向の移動量
        public float m_Speed = 0f;                     // 前進速度（前進はプラス、後退はマイナス）

        bool m_IsAttack = false;                         //攻撃パターンフラグ
        bool m_IsAttackReady = false;
        bool m_IsAttackFinish = false;

        public bool ikActive = false;
        public Transform leftShoulder;
        public Transform leftHand;      //左腕Transform

        public int attackCount = 0;

        /************************************************************************
                                      仮宣言 
        ************************************************************************/
        CharacterController m_Controller;    //キャラクタコントローラ
        Animator m_Animator;                 //アニメター

        public GameObject deadReplacement;   //死亡切替オブジェクト
        private bool deadReplace;            //死亡切替を行うか?
        private int ropeCounter;             //ロープとエネミーの接触点数


        private float x = 0; // 水平方向
        public float xLimit; // 水平方向の限度値
        private float initX = 30;
        private float y = 0; // 垂直方向
        public float yLimit = 30; //垂直方向の限度値
        private float initY;

        private bool setWaitshotFlag = false; //　構え終わった時に値をセットしたらOn

        void Start()
        {
            //初期化
            deadReplace = false;
            m_Controller = GetComponent<CharacterController>();
            agent = GetComponent<NavMeshAgent>();
            agent.speed = m_Speed;

            time2 += 1;

            ropeCounter = 0;

            //アニメターは子のアニメターを取得
            m_Animator = transform.Find("EnemyRobot").GetComponent<Animator>();             //こっちはfbx形式

            /************************************************************************
                                        仮初期化 
            ************************************************************************/
            //右腕の取得（使えるかどうか確定できていない）
            leftHand = transform.GetComponent<Transform>();
            //player = transform.Find("Player").GetComponent<Transform>();

        }

        void Update()
        {
            ////垂直軸の値取得
            //float axisVertical = Input.GetAxisRaw("Vertical");

            //// 減速する（入力が無い場合 or 進行方向と逆に入力がある場合）
            //if ((axisVertical == 0) || (m_Speed * axisVertical < 0))
            //{
            //    if (m_Speed > 0)
            //    {
            //        m_Speed = Mathf.Max(m_Speed - m_BrakePower * Time.deltaTime, 0);
            //    }
            //    else {
            //        m_Speed = Mathf.Min(m_Speed + m_BrakePower * Time.deltaTime, 0);
            //    }
            //}

            //// 上下キーで加速
            //m_Speed += m_AccelPower * axisVertical * Time.deltaTime;

            //// 速度制限
            //m_Speed = Mathf.Clamp(m_Speed, m_MinSpeed, m_MaxSpeed);

            //// 速度を、エネミーが向いている方向のベクトルに変換する
            //Vector3 velocity = transform.forward * m_Speed;

            //// 接地しているなら
            //if (m_Controller.isGrounded)
            //{
            //    // 落下を止める
            //    m_VelocityY = 0;
            //}

            //// スペースキーが押されたら、ジャンプする
            //if (m_Controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
            //{
            //    m_VelocityY = m_JumpPower;
            //}

            //// 重力加速度を加算
            //m_VelocityY -= m_Gravity * Time.deltaTime;

            //// y軸方向の移動量を加味する
            //velocity.y = m_VelocityY;

            // CharacterControllerに命令して移動する

            //アニメターに数値を知らせる
            //m_Animator.SetFloat("Speed", m_Speed);

            //エネミーを即死させる
            if (Input.GetKey(KeyCode.Z))
            {
                deadReplace = true;
            }
            //死亡交代
            if (deadReplace)
            {
                m_Animator.SetBool("IsDead", true);

                m_Controller.enabled = false;
                agent.enabled = false;
                transform.GetComponent<CapsuleCollider>().enabled = false;
            }

            agent.speed = m_Speed;
            agent.destination = Destination.position;
            AnimatorStateInfo Info = m_Animator.GetCurrentAnimatorStateInfo(0);
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction, Color.blue, 2f);

            if (Physics.Raycast(ray, out hit, 1.5f))
            {
                if (hit.collider.gameObject.name == "kabe" && Info.IsName("BaseLayer.idle"))
                {
                    agent.speed = 0;
                    Debug.Log(hit.collider.gameObject.name + "発見");
                    Attack = true;
                    time += Time.deltaTime;
                }
            }

            if (time2 >= NextAttackTime)
            {
                time2 = 0;
                Attack = true;

                //PlayerTargetに指定したObjectを見続ける
                Vector3 eye = player.position;
                eye.y = transform.position.y;
                transform.LookAt(eye);
            }

            if (Attack == true)
            {
                if (time >= 0.3f)
                {
                    m_Animator.SetTrigger("Attack");
                    Attack = false;
                    time = 0;
                }
            }
            //モーションのAttack中の処理
            if (Info.IsName("UpperBody.Attack"))
            {
                agent.speed = 0;

                m_Animator.SetBool("IsAttack", true);

                Debug.Log("攻撃now");
            }

            m_Animator.SetFloat("Speed", agent.speed);


            /****************************************************************
                                        仮更新
            ****************************************************************/
            //アニメーション一時停止
            if (Input.GetKey(KeyCode.Alpha3))
            {
                m_Animator.speed = 0;
            }
            else
            {
                m_Animator.speed = 1;
            }

            //右腕のちぎれ
            if (Input.GetKey(KeyCode.Space))
            {
                leftShoulder.GetComponent<SkinnedMeshRenderer>().enabled = false;
                m_Animator.speed = 0;
            }

            m_LazerCoolDown -= 1f * Time.deltaTime;

            //攻撃
            if (Input.GetKey(KeyCode.Alpha1))
            {
                EnemyAttack();
            }
            else if (Input.GetKey(KeyCode.Alpha2) && m_LazerCoolDown <= 0)
            {
                EnemyLazer();
                m_LazerCoolDown = 20.0f;
            }
            else
            {
                EnemyNormal();
            }

            //自爆
            if (m_Animator.GetBool("IsExplosion"))
            {
                EnemyExplosion();
            }

            //Mathf.Atan2();

            //Debug.Log(this.transform.position);
            //Debug.Log(player.transform.position);

            //Debug.Log((Vector3.Angle(this.transform.position, player.transform.position) / Mathf.Deg2Rad));
            Debug.Log((Vector2.Angle(new Vector2(this.transform.position.x, this.transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)) / Mathf.Deg2Rad));

        }

        //Transformの循環コピー
        //static void CopyTransformsRecurse(Transform src, Transform dst)
        //{
        //    dst.position = src.position;
        //    dst.rotation = src.rotation;

        //    foreach (Transform child in dst)
        //    {
        //        // 同じ名前でTransformをマッチする = Match the transform with the same name
        //        Transform curSrc = src.Find(child.name);
        //        if (curSrc)
        //            CopyTransformsRecurse(curSrc, child);
        //    }
        //}

        //死んでいますか?
        public bool IsDead()
        {
            return deadReplace;
        }

        //トリガーに入ると同時に
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Destination")
            {
                m_Animator.SetBool("IsExplosion", true);
            }
        }

        //トリガーに出ると同時に
        public void OnTriggerExit(Collider other)
        {
            Attack = false;
        }

        //トリガに入っているときに
        public void OnTriggerStay(Collider other)
        {
            AnimatorStateInfo Info = m_Animator.GetCurrentAnimatorStateInfo(0);
            if (other.gameObject.tag == "Player" && Info.IsName("UpperBody.MoveTree"))
            {
                //agent.speed = 0;
                //Attack = true;
                time += Time.deltaTime;
                time2 += Time.deltaTime;
            }

            //同時に4点がロープとぶつかっているなら死ぬ
            //if (ropeCounter >= 4)
            //deadReplace = true;

            //Debug.Log(ropeCounter);
        }

        void FiringBeam(GameObject[] obj)
        {
            var lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, obj[0].transform.position);
            lineRenderer.SetPosition(1, obj[1].transform.position);
        }

        void EnemyAttack()
        {

            agent.enabled = false;
            m_Animator.SetBool("IsAttack", true);

            GameObject target = GameObject.FindGameObjectWithTag("Player");
            int rotateSpeed = 1;
            Transform myTransform = this.transform;

            Debug.DrawLine(target.transform.position, this.transform.position, Color.yellow);

            myTransform.rotation = Quaternion.Slerp(
                myTransform.rotation,
                Quaternion.LookRotation(target.transform.position - myTransform.transform.position),
                rotateSpeed * Time.deltaTime);

        }

        void EnemyLazer()
        {

        }

        void EnemyNormal()
        {
            agent.enabled = true;
            m_Animator.SetBool("IsAttack", false);
            agent.speed = m_Speed;

        }

        void EnemyExplosion()
        {
            transform.Find("DYNAMITE").transform.gameObject.SetActive(true);
        }
    }
}

