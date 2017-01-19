using UnityEngine;
using System.Collections;

public class Spark : MonoBehaviour {
    public GameObject m_Spark;

    private GameObject m_Sp;
    private GameObject exp;
    private float timer;
    private bool Spk;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        m_Sp = transform.root.gameObject;

        //切れた腕のバチバチ
        if (m_Sp.GetComponent<fukusei>().H_s == true)
        {
            exp = (GameObject)Instantiate(m_Spark.gameObject, transform.position,
                Quaternion.identity);
            Spk = true;
        }
        if (Spk == true)
            timer += Time.deltaTime;
        if (timer >= 5.0f)
        {
            timer = 0;
            Destroy(exp);
            Spk = false;
        }
    }
}
