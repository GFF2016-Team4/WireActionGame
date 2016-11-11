using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class FadeTitle : MonoBehaviour
{

    public float fadeInSpeed = 0.01f;     //フェードインの長さ
    public float fadeOutSpeed = 0.01f;     //フェードアウトの長さ
    float alfaIn;
    float alfaOut;
    float red, green, blue;         //RGB変数
    private bool fadeIn;
    private bool fadeOut;

    // Use this for initialization
    void Start()
    {
        red = GetComponent<Image>().color.r;
        green = GetComponent<Image>().color.g;
        blue = GetComponent<Image>().color.b;

        fadeIn = false;
        fadeOut = true;

        alfaIn = 1.0f;
        alfaOut = 0.0f;

    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOut == true)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            GetComponent<Image>().color = new Color(red, green, blue, alfaIn);
            alfaIn -= fadeInSpeed;

        }

        if (Input.GetKeyDown(KeyCode.Space) && alfaIn <= 0.0f)
        {
            fadeIn = true;
        }
        if (fadeIn == true)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            GetComponent<Image>().color = new Color(red, green, blue, alfaOut);
            alfaOut += fadeOutSpeed;

            if (sceneName == "Title" && alfaOut >= 1.0f)
            {
                SceneManager.LoadScene("Main");
            }
        }

    }
}
