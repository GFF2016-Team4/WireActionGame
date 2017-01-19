using UnityEngine;
using System.Collections;

public class ArmInstantiate : MonoBehaviour
{

    [System.NonSerialized]
    public float H = 3;

    private GameObject H_s2;
    private bool Inst;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        H_s2 = transform.root.gameObject;

        if (H <= 0)
        {
            H = 3;
            H_s2.GetComponent<fukusei>().H_s = true;

            Inst = false;
        }
        else H_s2.GetComponent<fukusei>().H_s = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Rope/Lock" && Inst == true)
        {
            H -= 1;
            Debug.Log(H);
        }

        //地面と当たったら
        if (col.gameObject.tag == "Field")
        {
            Inst = true;
            Debug.Log(Inst);
        }

        //攻撃中に地面に当たったら
        if (H_s2.GetComponent<EnemyPattern>().m_changeTag == true)
            SoundManager.Instance.PlaySE(AUDIO.SE_Enemy_Punch);
    }

        void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Rope/Lock")
        {
            H += 1;
            if (H >= 4) H = 3;
            Debug.Log(H);
        }

        if (col.gameObject.tag == "Field")
        {
            Inst = false;
            Debug.Log(Inst);
        }
    }
}
