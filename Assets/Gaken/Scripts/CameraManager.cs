using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    Camera m_Camera;

    public GameObject m_Enemy;

    Animator m_Animator;

    bool IsChange = false;

	// Use this for initialization
	void Start () {
        m_Camera = transform.GetChild(0).GetComponent<Camera>();
        m_Enemy.GetComponent<GameObject>();
        m_Animator = m_Enemy.transform.Find("EnemyRobot").GetComponent<Animator>();

        IsChange = false;

        IsChange = m_Animator.GetBool("IsExplosion");
    }
	
	// Update is called once per frame
	void Update () {
        if (IsChange)
        {
            m_Camera.depth = 2;
        }
	}
}
