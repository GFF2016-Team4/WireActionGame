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
        public Transform PlayerTarget;

        public float m_MaxSpeed = 4.0f;         // 最高速度（メートル/秒）
        public float m_MinSpeed = -2.0f;        // 最低速度（最大バック速度）
        public float m_AccelPower = 2.0f;       // 加速度（メートル/秒/秒）
        public float m_BrakePower = 6.0f;       // ブレーキ速度（メートル/秒/秒）
        public float m_RotateSpeed = 45.0f;    // 回転速度（度/秒）
        public float m_Gravity = 18.0f;         // 重力加速度（メートル/秒/秒）
        public float m_JumpPower = 0.0f;        // ジャンプ力（初速(メートル/秒)）

        float m_VelocityY = 0f;                 // y軸方向の移動量
        public float m_Speed = 0f;                     // 前進速度（前進はプラス、後退はマイナス）

        bool m_IsAttack = false;                         //攻撃パターンフラグ
        bool m_IsAttackReady = false;
        bool m_IsAttackFinish = false;

        public bool ikActive = false;
        public Transform child;                 //Enemyの子供
        public Transform spine;

        public int attackCount = 0;

        /************************************************************************
                                      仮宣言 
        ************************************************************************/
        //public Transform rightHand;          //右腕Transform

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

        public GameObject rightArm;
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
            //m_Animator = transform.Find("EnemyRobotPre_Ragdoll02").GetComponent<Animator>();  //こっちはラグドール

            /************************************************************************
                                        仮初期化 
            ************************************************************************/
            //右腕の取得（使えるかどうか確定できていない）
            child = transform.GetComponent<Transform>();
            PlayerTarget = transform.GetComponent<Transform>();


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

            agent.speed = m_Speed;
            agent.destination = Destination.position;
            AnimatorStateInfo Info = m_Animator.GetCurrentAnimatorStateInfo(0);
            //Ray ray = new Ray(transform.position, transform.forward);
            //RaycastHit hit;

            //Debug.DrawRay(ray.origin, ray.direction, Color.blue, 2f);

            //if (Physics.Raycast(ray, out hit, 1.5f))
            //{
            //    if (hit.collider.gameObject.name == "kabe" && Info.IsName("BaseLayer.idle"))
            //    {
            //        agent.speed = 0;
            //        Debug.Log(hit.collider.gameObject.name + "発見");
            //        Attack = true;
            //        time += Time.deltaTime;
            //    }
            //}

            if (time2 >= NextAttackTime)
            {
                time2 = 0;
                Attack = true;

                //PlayerTargetに指定したObjectを見続ける
                Vector3 eye = PlayerTarget.position;
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

                Debug.Log("攻撃now");
            }

            m_Animator.SetFloat("Speed", m_Speed);


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
            //m_Animator.GetBoneTransform(HumanBodyBones.Hips).Rotate(x * Time.deltaTime, 0, 0);

            if (Input.GetKey(KeyCode.Space))
            {
                rightArm.GetComponent<SkinnedMeshRenderer>().enabled = false;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                m_Animator.SetLayerWeight(1, 1);
                m_Animator.SetBool("IsAttackReady", true);

            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                //m_Animator.enabled = true;

                m_Animator.SetBool("IsAttack", true);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                m_Animator.SetBool("IsAttackFinish", true);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                m_Animator.SetBool("IsAttackFinish", false);
                m_Animator.SetBool("IsAttackReady", false);
                m_Animator.SetBool("IsAttack", false);

                m_Animator.CrossFade("Base Layer.Finish", 10);

                if (m_Animator.GetAnimatorTransitionInfo(0).anyState)
                    m_Animator.SetLayerWeight(1, 0);
            }

            //攻撃です！
            if (Input.GetButtonDown("Fire1"))
            {
                //m_Animator.SetLayerWeight(1, 1);
                //m_Animator.SetBool("IsAttack", true);

                //float x = target.transform.position.x - this.transform.position.x;
                //float y = target.transform.position.y - this.transform.position.y;
                //float z = target.transform.position.z - this.transform.position.z;
                //this.transform.forward = target.transform.position;
                //this.transform.TransformDirection(new Vector3(x, -z, y) * 7);
                // ターゲット方向のベクトルを求める
                //Vector3 vec = PlayerTarget.position - child.position;

                // ターゲットの方向を向く
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(vec.x, 0, vec.z)), m_RotateSpeed);
                //child.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(0, vec.y, 0)), m_RotateSpeed);

                //m_Animator.GetBoneTransform(HumanBodyBones.Hips).Rotate(vec.x, vec.y, vec.z);

                //Quaternion direction = Quaternion.FromToRotation(Vector3.up, target.position - child.position);
                //transform.rotation = Quaternion.Slerp(transform.rotation, direction, m_Speed);
                //child.transform.Translate(Vector2.up * m_Speed * Time.deltaTime);
            }
            //攻撃回収です！（多分）
            //if (Input.GetButtonUp("Fire1"))
            //{
            //    m_Animator.SetLayerWeight(1, 0);
            //    m_Animator.SetBool("IsAttack", false);
            //}

            //アバターマスクテスト
            //if (Input.GetKey(KeyCode.Alpha1))
            //{
            //    m_Speed -= Time.deltaTime * 0.01f;
            //    if (m_Speed <= 0)
            //    {
            //        m_Speed = 0;
            //    }
            //    m_Animator.SetLayerWeight(1, 1f);
            //}
            //else
            //{
            //    m_Animator.SetLayerWeight(1, 0);
            //}

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
            //if (other.gameObject.tag == "NormalRope")
            //    ropeCounter += 1;

            //Debug.Log("入ったぜ!");

        }

        //トリガーに出ると同時に
        public void OnTriggerExit(Collider other)
        {
            Attack = false;



            //ロープカウンタ数を-1
            //if (other.gameObject.tag == "NormalRope")
            //ropeCounter -= 1;

            //Attack = false;

            //Debug.Log("出たぜ!");

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

            //AnimatorStateInfo Info = m_Animator.GetCurrentAnimatorStateInfo(0);
            //if (other.gameObject.tag == "Player" && Info.IsName("BaseLayer.idle"))
            //{
            //    //agent.speed = 0;
            //    //Attack = true;
            //    time += Time.deltaTime;
            //    time2 += Time.deltaTime;
            //}
        }

        public void OnAnimatorIK(int layerIndex)
        {

            if (m_Animator.enabled == false)
            {
                Vector3 vec = PlayerTarget.position - spine.position;
                m_Animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(vec.x, vec.y, vec.z)), 30));
            }


            float x = 0, y = 0;

            if (Input.GetKey(KeyCode.K))
            {
                x += 1 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.I))
            {
                x -= 1 * Time.deltaTime;
            }
            //m_Animator.GetBoneTransform(HumanBodyBones.Hips).Rotate(x, 0, 0);
            //m_Animator.SetBoneLocalRotation(HumanBodyBones.Hips, Quaternion.Euler(x, y, 0));
            m_Animator.rootRotation = Quaternion.Euler(x, y, 0);

            if (!m_Animator.enabled)
            {
                initX = m_Animator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles.x;
                initY = m_Animator.GetBoneTransform(HumanBodyBones.Spine).localEulerAngles.y;
                setWaitshotFlag = true;
            }
            float lastX;
            float lastY;

            if (child)
            {
                if (Input.GetKey(KeyCode.L))
                {
                    y += m_RotateSpeed;
                }
                if (Input.GetKey(KeyCode.J))
                {
                    y -= m_RotateSpeed;
                }
                if (Input.GetKey(KeyCode.I))
                {
                    x -= m_RotateSpeed;
                }
                if (Input.GetKey(KeyCode.K))
                {
                    x += m_RotateSpeed;
                }
                if (x > xLimit)
                {
                    x = xLimit;
                }
                if (x < -xLimit)
                {
                    x = -xLimit;
                }
                if (y > yLimit)
                {
                    y = yLimit;
                }
                if (y < -yLimit)
                {
                    y = -yLimit;
                }
                lastX = (x + initX) % 360;
                lastY = (y + initY) % 360;
                m_Animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(lastX, lastY, 0));

            }
            if (!Input.GetButton("Fire2"))
            {
                x = 0;
                y = 0;
                setWaitshotFlag = false;
            }
        }

        void FiringBeam(GameObject[] obj)
        {
            var lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.SetPosition(0, obj[0].transform.position);
            lineRenderer.SetPosition(1, obj[1].transform.position);
        }
    }
}

