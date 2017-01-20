using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class FadeRespawn : MonoBehaviour
{

    public Image panel;
    public GameObject respawnJudge;
    public GameObject respawnPosition;

    public float animationTimer;
    private float animationTimerTemp;

    public string sceneName;
    private bool fadeIn = false;
    private bool fadeOut = true;

    CharacterController m_characterController;
    FadeManager m_fadeManager;

    void Start()
    {
        animationTimerTemp = animationTimer;
        m_characterController = GetComponent<CharacterController>();
        m_fadeManager = panel.GetComponent<FadeManager>();
    }

    void Update()
    {
        //if (fadeIn == false && fadeOut == false)
        //    m_characterController.Move(new Vector3(0, -0.1f, 0));

        //if (fadeOut == true)
        //{
        //    m_fadeManager.FadeOut(sceneName, 1);
        //    if (m_fadeManager.alfaOut >= 1.0f)
        //    {
        //        gameObject.transform.position = respawnPosition.transform.position;
        //        fadeOut = false;
        //        fadeIn = true;
        //        m_fadeManager.alfaOut = 0.0f;
        //    }
        //}
        //if (fadeIn == true)
        //{
        //    m_fadeManager.FadeIn(1);
        //    if (m_fadeManager.alfaIn <= 0)
        //    {
        //        fadeIn = false;
        //        respawnJudge.SetActive(true);
        //        m_fadeManager.alfaIn = 1.0f;
        //        animationTimer = animationTimerTemp;
        //    }
        //}
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //animationTimer -= Time.deltaTime;
        //if (respawnJudge != null)
        //{
        //    if (animationTimer <= 0)
        //        fadeOut = true;
        //}
        //respawnJudge.SetActive(false);
    }

    public void Respawn()
    {
        //if (fadeIn == false && fadeOut == false)
        //m_characterController.Move(new Vector3(0, -0.1f, 0));
        StartCoroutine("respawn");
    }

    IEnumerator respawn()
    {
        while (fadeOut == true)
        {
            m_fadeManager.FadeOut(sceneName, 1);
            yield return new WaitForEndOfFrame();
            if (m_fadeManager.alfaOut >= 1.0f)
            {
                gameObject.transform.position = respawnPosition.transform.position;
                fadeOut = false;
                fadeIn = true;
                m_fadeManager.alfaOut = 0.0f;
            }
        }
        while (fadeIn == true)
        {
            m_fadeManager.FadeIn(1);
            yield return new WaitForEndOfFrame();
            if (m_fadeManager.alfaIn <= 0)
            {
                fadeIn = false;
                respawnJudge.SetActive(true);
                m_fadeManager.alfaIn = 1.0f;
                animationTimer = animationTimerTemp;
            }
        }
        yield return null;
    }
}
