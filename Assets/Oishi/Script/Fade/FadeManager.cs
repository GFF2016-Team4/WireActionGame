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

    [Header("リスポーンフェードインの速さ")]
    public float RespawnfadeInSpeed = 0.01f;

    [Header("リスポーンフェードアウトの長さ")]
    public float RespawnfadeOutSpeed = 0.01f;

    [Header("画面切り替え時、時間停止の長さ")]
    float alfaTemp = 0.0f;           //画面切り替え時、時間停止の長さ

    [System.NonSerialized]
    public float alfaIn = 1.0f;
    [System.NonSerialized]
    public float alfaOut = 0.0f;

    Object[] objs;  /**/

    float red, green, blue;         //RGB変数

    void Update()
    {
        Debug.Log("シーンの名前は" + SceneManager.GetActiveScene().name);


        if (SceneManager.GetSceneByName("LoadScene").isLoaded)
        {
            Debug.Log("ロードシーン中");
        }
    }

    public void FadeIn(float timeScale = 0)
    {
        //フェードインの色変更
        //c.a = alfaIn;
        //GetComponent<Image>().color = c;
        //SceneManager.UnloadScene("LoadScene");

        Time.timeScale = timeScale;

        GetComponent<Image>().color = new Color(red, green, blue, alfaIn);
        alfaIn -= fadeInSpeed;

        if (alfaIn <= 0) Time.timeScale = 1;
    }
    public void FadeOut(string sceneName, float timeScale = 0)
    {
        GetComponent<Image>().color = new Color(red, green, blue, alfaOut);
        if (alfaOut <= 1.0f)
        {
            alfaOut += fadeOutSpeed;
            if (alfaOut >= 1.0f)
            {
                SceneManager.LoadSceneAsync("LoadScene", LoadSceneMode.Additive);
                StartCoroutine("Load", sceneName);
            }
        }
        if (alfaOut <= alfaTemp) Time.timeScale = 0;
    }

    IEnumerator Load(string sceneName)
    {
        StartCoroutine("resourceLoad");

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        while (async.progress < 0.9f && async.isDone == false)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        async.allowSceneActivation = true;
    }
    public void RespawnFadeIn()
    {
        GetComponent<Image>().color = new Color(red, green, blue, alfaOut);
        if (alfaOut <= 1.0f)
        {
            alfaOut += RespawnfadeOutSpeed;
        }
        if (alfaOut <= alfaTemp) Time.timeScale = 0;
    }
    public void RespawnFadeOut()
    {
        if (alfaOut >= 1.0f)
        {
            GetComponent<Image>().color = new Color(red, green, blue, alfaIn);
            alfaIn -= RespawnfadeInSpeed;
        }
        if (alfaIn <= 0) Time.timeScale = 1;
    }

    IEnumerator resourceLoad()
    {
        yield return objs = Resources.LoadAll("Resources");
    }
}