using UnityEngine;
using System.Collections;

public class PunchSmoke : MonoBehaviour {

    public GameObject m_Somke;

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
            GameObject exp = (GameObject)Instantiate(m_Somke.gameObject, transform.position,
                Quaternion.identity);
        }
    }
}
