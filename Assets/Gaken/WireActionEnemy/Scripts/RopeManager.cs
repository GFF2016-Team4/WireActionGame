using UnityEngine;
using System.Collections;

public class RopeManager : MonoBehaviour {
    public GameObject Rope;
    public GameObject enemy;
    private GameObject m_Rope;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(1))
        {
            m_Rope = Instantiate(Rope, enemy.transform.position, new Quaternion(90, 90, 0, 0)) as GameObject;
            Debug.Log("ロープ");
        }
    }
}
