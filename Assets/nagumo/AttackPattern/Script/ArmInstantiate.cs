using UnityEngine;
using System.Collections;

public class ArmInstantiate : MonoBehaviour
{

    [System.NonSerialized]
    public float H = 3;

    private GameObject H_s2;
    private bool Inst;
    private bool fuku;
    // Use this for initialization
    void Start()
    {
        fuku = true;
    }

    // Update is called once per frame
    void Update()
    {
        H_s2 = transform.root.gameObject;

        if(H_s2.GetComponent<fukusei>() == null)
        {
            fuku = false;
        }
        else
        {
            fuku = true;

            if (H <= 0)
            {
                H = 3;
                H_s2.GetComponent<fukusei>().H_s = true;

                Inst = false;
            }
            else H_s2.GetComponent<fukusei>().H_s = false;
        }  
    }

    void OnTriggerEnter(Collider col)
    {
        if (fuku == true)
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
            }
        }
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
        }
    }
}
