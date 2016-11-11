using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class FadeMain : MonoBehaviour
{

    public float fadeInSpeed = 0.01f;     //フェードインの長さ
    public float fadeOutSpeed = 0.01f;    //フェードアウトの長さ
    public GameObject judgeObj;           //フェードアウト開始判定用
    float alfaIn;
    float alfaOut;
    float alfaInTemp;
    float red, green, blue;         //RGB変数
    private bool fadeIn;
    private bool fadeOut;

    private bool testClear;


    // Use this for initialization
    void Start()
    {
        red = GetComponent<Image>().color.r;
        green = GetComponent<Image>().color.g;
        blue = GetComponent<Image>().color.b;

        fadeIn = true;
        fadeOut = false;

        testClear = false;

        alfaIn = 1.0f;
        alfaOut = 0.0f;
        alfaInTemp = alfaIn / 2.0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIn == true)
        {
            GetComponent<Image>().color = new Color(red, green, blue, alfaIn);
            alfaIn -= fadeInSpeed;
            Time.timeScale = 0;
        }
        if (alfaIn <= alfaInTemp)
        {
            Time.timeScale = 1;
        }
        if (judgeObj != null)
        {
            fadeOut = false;
        }
        else
        {
            fadeIn = false;
            fadeOut = true;
        }
        if (fadeOut == true)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            GetComponent<Image>().color = new Color(red, green, blue, alfaOut);
            alfaOut += fadeOutSpeed;
            Time.timeScale = 0;

            if (sceneName == "Main" && alfaOut >= 1.0f)
            {
                SceneManager.LoadScene("GameOver");
            }
        }

        //クリアテスト用
        if (Input.GetKeyDown(KeyCode.C) && alfaIn <= 0.0f)
        {
            testClear = true;
        }
        if (testClear == true)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            GetComponent<Image>().color = new Color(red, green, blue, alfaOut);
            alfaOut += fadeOutSpeed;
            Time.timeScale = 0;

            if (sceneName == "Main" && alfaOut >= 1.0f)
            {
                SceneManager.LoadScene("GameClear");
            }
        }
    }

}
