using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class FadeManager : MonoBehaviour
{
    [Header("フェードインの速さ")]
    public float fadeInSpeed = 0.01f;

    [Header("フェードアウトの長さ")]
    public float fadeOutSpeed = 0.01f;

    [Header("画面切り替え時、時間停止の長さ")]
    public float alfaTemp = 0.0f;           //画面切り替え時、時間停止の長さ

    float alfaIn = 1.0f;
    float alfaOut = 0.0f;

    //float alfaMainIn = 1.0f;
    //float alfaMainOut = 0.0f;

    float red, green, blue;         //RGB変数


    // Update is called once per frame
    public void FadeIn()
    {
        GetComponent<Image>().color = new Color(red, green, blue, alfaIn);
        alfaIn -= fadeInSpeed;
        Time.timeScale = 0;

        if (alfaIn <= alfaTemp) Time.timeScale = 1;

    }
    public void FadeOut(string sceneName)
    {
        GetComponent<Image>().color = new Color(red, green, blue, alfaOut);

        if (alfaOut <= 1.0f) alfaOut += fadeOutSpeed;
        else SceneManager.LoadScene(sceneName);

        if (alfaOut <= alfaTemp) Time.timeScale = 0;
    }
    //public void FadeMainIn()
    //{
    //    GetComponent<Image>().color = new Color(red, green, blue, alfaMainIn);
    //    alfaMainIn -= fadeInSpeed;
    //    Time.timeScale = 0;

    //    if (alfaMainIn <= alfaTemp) Time.timeScale = 1;

    //}
    //public void FadeMainOut(string sceneName)
    //{
    //    GetComponent<Image>().color = new Color(red, green, blue, alfaMainOut);
    //    Time.timeScale = 0;

    //    if (alfaMainOut <= 1.0f) alfaMainOut += fadeOutSpeed;
    //    else SceneManager.LoadScene(sceneName);
    //}

}
