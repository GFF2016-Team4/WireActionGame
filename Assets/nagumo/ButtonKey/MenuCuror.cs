using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuCuror : MonoBehaviour {

    private RectTransform m_RectTransform;
    private Vector2 m_Curor;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        //カーソル移動音
        //if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ||
        //    Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        //    SoundManager.Instance.PlaySE(AUDIO.SE_enter2);

	}

    //カーソル移動
    void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    //カーソル移動
    void LateUpdate()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
        m_Curor = m_RectTransform.anchoredPosition;

        if (selectedObject == null)
        {
            return;
        }

        m_RectTransform.anchoredPosition =
            selectedObject.GetComponent<RectTransform>().anchoredPosition;

        if (m_Curor != selectedObject.GetComponent<RectTransform>().anchoredPosition)
            PlaySE_move();
    }

    //決定音
    public void PlaySE_touch()
    {
        //再生
        //audiosource.Play();
        SoundManager.Instance.PlaySE(AUDIO.SE_enter1);
    }

    void PlaySE_move()
    {
        SoundManager.Instance.PlaySE(AUDIO.SE_enter2);
    }
}
