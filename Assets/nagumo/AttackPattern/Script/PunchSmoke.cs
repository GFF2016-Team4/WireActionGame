using UnityEngine;
using System.Collections;

public class PunchSmoke : MonoBehaviour {

    public GameObject m_Somke;

    private GameObject exp;
    private float timer;
    private bool Smo;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //exp = Instantiate(m_Somke);
        if (Smo == true)
            timer += Time.deltaTime;
        if (timer >= 3.0f)
        {
            timer = 0;
            Destroy(exp);
            Smo = false;
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Field")
        {
            exp = (GameObject)Instantiate(m_Somke.gameObject, transform.position,
                Quaternion.identity);
            Smo = true;
        }
    }
}
