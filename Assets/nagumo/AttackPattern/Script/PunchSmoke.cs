using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class PunchSmoke : MonoBehaviour {

    public GameObject m_Somke;

    private GameObject exp;
    private GameObject En_p;
    private float timer;
    private bool Smo;
    private bool m_nul;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        En_p = transform.root.gameObject;
        //exp = Instantiate(m_Somke);
        if (Smo == true)
            timer += Time.deltaTime;
        if (timer >= 5.0f)
        {
            timer = 0;
            Destroy(exp);
            Smo = false;
        }

        if (En_p.GetComponent<EnemyPattern>() == null)
        {
            m_nul = false;
        }
        else
        {
            m_nul = true;
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Field")
        {
            exp = (GameObject)Instantiate(m_Somke.gameObject, transform.position - new Vector3(0, 5, 0),
                Quaternion.identity * Quaternion.AngleAxis(90, Vector3.left));
            Smo = true;
        }
       
        //攻撃中に地面に当たったら
        if (m_nul == true)
            SoundManager.Instance.PlaySE(AUDIO.SE_Enemy_Punch);
    }
}
