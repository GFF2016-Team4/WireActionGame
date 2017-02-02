using UnityEngine;
using System.Collections;

public class Spark : MonoBehaviour {
    public GameObject m_Spark;
    public GameObject m_Spark2;

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
            Debug.Log(m_Sp.GetComponent<fukusei>().H_s);
            exp = (GameObject)Instantiate(m_Spark.gameObject, transform.position,
                Quaternion.identity);
            Spk = true;
            m_Spark2.SetActive(true);
            SoundManager.Instance.PlaySE(AUDIO.SE_Enemy_breakArm);
        }
        if (Spk == true)
            timer += Time.deltaTime;
        if (timer >= 10.0f)
        {
            timer = 0;
            Destroy(exp);
            Spk = false;
            m_Spark2.SetActive(false);
        }
    }
}
