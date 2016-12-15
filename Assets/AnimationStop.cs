using UnityEngine;
using System.Collections;

public class AnimationStop : MonoBehaviour {

    Animator m_Animator;

    public Transform player;
    public Transform spine;

    private float timeCount;

	// Use this for initialization
	void Start () {
        m_Animator = GetComponent<Animator>();
        timeCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (m_Animator.GetBool("IsAttackReady"))
        {
            timeCount += Time.time;
            m_Animator.SetBool("IsScript", true);
            //if(timeCount > 200)
            //{
            //    m_Animator.enabled = false;
            //}
        }
        else if (m_Animator.GetBool("IsAttackReady") && m_Animator.GetBool("IsScript"))
        {
            Vector3 vec = player.position - spine.position;
            m_Animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(vec.x, vec.y, vec.z)), 30));

            m_Animator.SetBool("IsScript", false);
        }
        else if(m_Animator.GetBool("IsAttackReady") && !m_Animator.GetBool("IsScript"))
        {
            m_Animator.enabled = true;
            m_Animator.SetBool("IsAttack", true);
        }
        else if(m_Animator.GetBool("IsAttack"))
        {
            m_Animator.SetBool("IsScript", true);
            m_Animator.enabled = false;
        }
        else if(m_Animator.GetBool("IsAttack") && m_Animator.GetBool("IsScript"))
        {

        }
	}
}
