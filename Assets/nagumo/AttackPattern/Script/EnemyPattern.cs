using UnityEngine;
using System.Collections;

public class EnemyPattern : MonoBehaviour {
    public GameObject m_player;
    public float LaserAttackTime;

    //パンチの範囲
    public float Punch;

    //レーザーの範囲
    public float m_Laser;

    private float timer;

    [System.NonSerialized]
    public bool Laser;

    //[System.NonSerialized]
    public bool m_changeTag;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        int xDeistance = Mathf.RoundToInt(this.transform.position.x - m_player.transform.position.x);
        int zDeistance = Mathf.RoundToInt(this.transform.position.z - m_player.transform.position.z);
        //Debug.Log(zDeistance);
        //Debug.Log(xDeistance);

        //パンチの範囲内処理
        if (xDeistance <= Punch && xDeistance >= -Punch && zDeistance <= Punch && zDeistance >= -Punch)
        {
            //テスト用
            timer += Time.deltaTime;
            if (timer >= 10)
            {
                m_changeTag = true;
                Debug.Log("パンチ範囲");
            }
            if (timer >= 15)
            {
                timer = 0;
                m_changeTag = false;
            }
        }
        //レーザーの範囲内処理
        else if (xDeistance <= m_Laser && xDeistance >= -m_Laser && zDeistance <= m_Laser && zDeistance >= -m_Laser)
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
