using UnityEngine;
using System.Collections;

public class ChangeTag : MonoBehaviour {
    private GameObject m_Tag;

    //タグを変えたいオブジェクトを指定
    public GameObject m_change;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        m_Tag = transform.root.gameObject;

        if(m_Tag.GetComponent<EnemyPattern>() == null)
        {
            
        }
        else
        {
            if (m_Tag.GetComponent<EnemyPattern>().m_changeTag == true)
                m_change.tag = "Enemy";
            if (m_Tag.GetComponent<EnemyPattern>().m_changeTag == false)
                m_change.tag = "Untagged";
        }

        //テスト用
        //if(Input.GetKeyDown(KeyCode.Space)) m_change.tag = "Enemy";
        //if (Input.GetKeyDown(KeyCode.LeftShift)) m_change.tag = "Untagged";
    }
}
