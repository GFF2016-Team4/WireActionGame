using UnityEngine;
using System.Collections;

public class EnemyPattern : MonoBehaviour
{
    public GameObject m_player;
    public float LaserAttackTime;

    //パンチの範囲
    public float Punch;

    //レーザーの範囲
    public float m_Laser;

    public float Lasertimer;

    [System.NonSerialized]
    public bool Laser;

    //[System.NonSerialized]
    public bool m_changeTag;

    Animator m_Animator;
    private float m_LazerCoolDown = 10f;

    [System.NonSerialized]
    public bool counter;

    // Use this for initialization
    void Start()
    {
        m_Animator = transform.Find("Enemy3").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        m_LazerCoolDown -= 1f * Time.deltaTime;

        int xDeistance = Mathf.RoundToInt(this.transform.position.x - m_player.transform.position.x);
        int zDeistance = Mathf.RoundToInt(this.transform.position.z - m_player.transform.position.z);
        //Debug.Log(zDeistance);
        //Debug.Log(xDeistance);

        AnimatorStateInfo anim = this.m_Animator.GetCurrentAnimatorStateInfo(0);

        //パンチの範囲内処理
        if (xDeistance <= Punch && xDeistance >= -Punch && zDeistance <= Punch && zDeistance >= -Punch)
        {
            Debug.Log("パンチ範囲");

            if (!m_Animator.GetBool("IsAttack") && !m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion"))
            {
                m_Animator.SetBool("IsAttack", true);
                m_Animator.SetBool("IsLazer", false);
            }

            if (anim.fullPathHash == Animator.StringToHash("Base Layer.Attack"))
            {
                m_changeTag = true;
            }
            else
            {
                m_changeTag = false;
            }

            //Lasertimer = 0;

            //テスト用
            //timer += Time.deltaTime;
            //if (timer >= 10)
            //{
            //    m_changeTag = true;
            //    Debug.Log("パンチ範囲");
            //}
            //if (timer >= 15)
            //{
            //    timer = 0;
            //    m_changeTag = false;
            //}
        }
        //レーザーの範囲内処理
        else if (xDeistance <= m_Laser && xDeistance >= -m_Laser && zDeistance <= m_Laser && zDeistance >= -m_Laser)
        {

            m_Animator.SetBool("IsAttack", false);
            if (!m_Animator.GetBool("IsLazer") && m_LazerCoolDown <= 0)
            {
                m_Animator.SetBool("IsLazer", true);
                m_LazerCoolDown = 10f;
            }
           
            if (anim.fullPathHash == Animator.StringToHash("Base Layer.Lazer"))
            {
                counter = true;  
            }
            else
            {
                Lasertimer = 0;
            }
            

            Debug.Log("レーザー範囲");
        }
        else
        {
            Debug.Log("移動");
            m_Animator.SetBool("IsAttack", false);
            m_Animator.SetBool("IsLazer", false);
        }

        if (counter == true)
            LaserAttack();
    }
    void LaserAttack()
    {
        if (Laser == false)
            Lasertimer += Time.deltaTime;

        if (Lasertimer >= LaserAttackTime)
        {
            Lasertimer = 0;
            Laser = true;
        }
    }
}
