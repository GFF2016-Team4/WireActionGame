using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class Fade : MonoBehaviour
{
    FadeManager m_fademanager;

    // Use this for initialization
    void Awake()
    {
        m_fademanager = GetComponent<FadeManager>();
    }

    public void ClickFade()
    {
        m_fademanager.SceneChange("Title");
    }

    public void ClickFade2()
    {
        m_fademanager.SceneChange("Main_Sub");
    }
    public void ClickFade3()
    {
        m_fademanager.GameQuit();
    }
}
