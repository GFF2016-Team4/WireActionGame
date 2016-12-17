using UnityEngine;
using System.Collections;

public class EnemyPattern : MonoBehaviour {
    public GameObject m_player;
    public float LaserAttackTime;
    private float timer;

    [System.NonSerialized]
    public bool Laser;

    private GameObject LaserSwith;
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        int xDeistance = Mathf.RoundToInt(this.transform.position.x - m_player.transform.position.x);
        int zDeistance = Mathf.RoundToInt(this.transform.position.z - m_player.transform.position.z);
        //Debug.Log(zDeistance);
        //Debug.Log(xDeistance);

        LaserSwith = transform.FindChild("Sphere").gameObject;

        if (xDeistance <= 40 && xDeistance >= -40 && zDeistance <= 40 && zDeistance >= -40)
        {
            Debug.Log("パンチ範囲");
        }
        else if (xDeistance <= 100 && xDeistance >= -100 && zDeistance <= 100 && zDeistance >= -100)
        {
            LaserAttack();
            Debug.Log("レーザー範囲");
        }
        else
        {
            Debug.Log("移動");
        }
    }
    void LaserAttack()
    {
        if (Laser == false)
            timer += Time.deltaTime;

        if(timer >= LaserAttackTime)
        {
            timer = 0;
            Laser = true;
        }
    }
}
