using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class Fade : MonoBehaviour
{
    public string sceneName;


    private bool fadeIn = false;
    private bool fadeOut = false;

    FadeManager m_fademanager;

    // Use this for initialization
    void Start()
    {
        m_fademanager = GetComponent<FadeManager>();

        fadeIn = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    fadeOut = true;
        //}
        if (fadeIn == true)
        {
            m_fademanager.FadeIn();
            //m_fademanager.FadeMainIn();
        }
        if (fadeOut == true)
        {
            m_fademanager.FadeOut(sceneName);
            //m_fademanager.FadeMainOut(sceneName);
        }
    }



    public void ClickFade()
    {
        fadeOut = true;
        sceneName = "Title";
    }
    public void ClickFade2()
    {
        fadeOut = true;
        sceneName = "Main_Sub";
    }
}
