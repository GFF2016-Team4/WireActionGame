using UnityEngine;
using System.Collections;

public class WalkSE : MonoBehaviour {
    public GameObject m_Somke;

    private GameObject exp;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
       
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Field")
        {
            SoundManager.Instance.PlaySE(AUDIO.SE_Enemy_Walk);
            exp = (GameObject)Instantiate(m_Somke.gameObject, transform.position,
                Quaternion.identity);
        }
    }
}
