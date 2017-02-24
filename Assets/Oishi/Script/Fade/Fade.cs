using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class Fade : MonoBehaviour
{
    public string sceneName;


    private bool fadeOut = false;
    private bool fadeQuit = false;

    FadeManager m_fademanager;

    // Use this for initialization
    void Start()
    {
        m_fademanager = GetComponent<FadeManager>();

        fadeOut = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    fadeOut = true;
        //}
        if (m_fademanager.alfaIn >= 0)
        {
            m_fademanager.FadeIn();
            //m_fademanager.FadeMainIn();
        }
        if (fadeOut == true)
        {
            m_fademanager.FadeOut(sceneName);
            //m_fademanager.FadeMainOut(sceneName);
        }
        if (fadeQuit == true)
        {
            m_fademanager.QuitFadeOut();
        }
    }



    public void ClickFade()
    {
        if (fadeOut == false)
        {
            fadeOut = true;
            sceneName = "Title";
        }
    }
    public void ClickFade2()
    {
        if (fadeOut == false)
        {
            fadeOut = true;
            sceneName = "Main_Sub";
        }
    }
    public void ClickFade3()
    {
        fadeQuit = true;
    }
}
