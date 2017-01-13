using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuCuror : MonoBehaviour {

    private RectTransform m_RectTransform;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {

	}

    void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null)
        {
            return;
        }
        m_RectTransform.anchoredPosition =
            selectedObject.GetComponent<RectTransform>().anchoredPosition;
    }

    //決定音
    public void PlaySE_touch()
    {
        //再生
        //audiosource.Play();
        SoundManager.Instance.PlaySE(AUDIO.SE_enter1);
    }
}
