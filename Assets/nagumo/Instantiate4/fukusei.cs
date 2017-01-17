using UnityEngine;
using System.Collections;

public class fukusei : MonoBehaviour {

	//複製元のオブジェクト
	public GameObject orgObj;
    public GameObject orgObj2;

    private GameObject m_H;

    //public Transform parent;

    //複製オブジェクト
    private GameObject fksObj;

	//複製元オブジェクトのRenderer
	Renderer org_renderer;
	//複製オブジェクトのRenderer
	Renderer fks_renderer;

	colorManager m_colorManager;

	bool isDestroy;

    public float d_timer;
    float destroyTimer;

    [System.NonSerialized]
    public bool H_s;

    //テスト用
    //Vector3 testposition;


	void Start ()
	{
		m_colorManager = GetComponent<colorManager> ();
		org_renderer = orgObj.GetComponent<Renderer> ();
		destroyTimer = 0;

		//テスト用
		//testposition = new Vector3 (5f, 0f, 0f);
	}
	
	void Update ()
	{
        if (H_s == true) 
		{
            Debug.Log("aaa");
			fksObj = Instantiate (orgObj, orgObj.transform.position /*+ testposition*/, 
                orgObj.transform.rotation)as GameObject;
            fksObj.transform.localScale = new Vector3(20, 20, 20);
            //Instantiate(orgObj, parent, false);

            //複製オブジェクトのRendererを取得
            fks_renderer = fksObj.GetComponent<Renderer> ();
            //アルファ初期値を0に設定
            //m_colorManager.minusResetAlpha(fks_renderer);
            m_colorManager.plusResetAlpha(fks_renderer);

            orgObj.GetComponent<BoxCollider> ().enabled = false;
            orgObj2.GetComponent<BoxCollider>().enabled = false;
		}

		if (fksObj != null) 
		{
            //orgObj.GetComponent<SkinnedMeshRenderer>().enabled = false;
            
            destroyTimer += Time.deltaTime;

			if (destroyTimer >= d_timer) 
			{
				isDestroy = true;
                //orgObj.GetComponent<SkinnedMeshRenderer>().enabled = true;
            }

			if (isDestroy == false && org_renderer.material.color.a > 0) 
			{
                //複製オブジェクトのアルファを徐々に増やす
                //m_colorManager.plusAlpha(fks_renderer);
                //複製元オブジェクトのアルファを徐々に減らす
                //m_colorManager.minusAlpha(org_renderer);
                m_colorManager.minusResetAlpha(org_renderer);
            }
			if (isDestroy == true)
			{
                orgObj.GetComponent<BoxCollider>().enabled = true;
                orgObj2.GetComponent<BoxCollider>().enabled = true;
                fksObj.GetComponent<BoxCollider> ().enabled = false;

                //複製オブジェクトのアルファを徐々に減らす
                m_colorManager.minusAlpha (fks_renderer);
				//複製元オブジェクトのアルファを徐々に増やす
                if(org_renderer.material.color.a <= 1)
				m_colorManager.plusAlpha (org_renderer);

				//複製オブジェクトのアルファ値が0になったら削除
				if (fks_renderer.material.color.a <= 0)
				{
					Destroy (fksObj);

					destroyTimer = 0;

					isDestroy = false;
				}
			}
		}
	}
}
