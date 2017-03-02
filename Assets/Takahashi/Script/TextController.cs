using UnityEngine;
using System.Collections;

public class TextController : MonoBehaviour {
    public GameObject messageWindow; 
    public GameObject[] messageTexts;
    private int num = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            NextText();
        }
        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            TextOff();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            TextOn();
        }
	}

    void NextText()
    {
        messageTexts[num].SetActive(false);
        num += 1;
        messageTexts[num].SetActive(true);
    }

    void TextOff()
    {
        messageWindow.SetActive(false);
        messageTexts[num].SetActive(true);
    }

    void TextOn()
    {
        messageWindow.SetActive(true);
        messageTexts[num].SetActive(false);
    }
}
