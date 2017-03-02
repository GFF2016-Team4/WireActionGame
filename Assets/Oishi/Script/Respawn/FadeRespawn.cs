using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class FadeRespawn : MonoBehaviour
{

    //public Image panel;
    //public GameObject respawnJudge;
    //public GameObject respawnPosition;

    //public float animationTimer;
    //private float animationTimerTemp;

    //private bool isFade = false;
    //CharacterController m_characterController;
    //FadeManager m_fadeManager;

    //void Start()
    //{
    //    animationTimerTemp = animationTimer;
    //    m_characterController = GetComponent<CharacterController>();
    //    m_fadeManager = panel.GetComponent<FadeManager>();
    //}

    //public void Respawn()
    //{
    //    StartCoroutine(respawn());
    //}

    //IEnumerator respawn()
    //{
    //    isFade = true;
    //    m_fadeManager.alfaOut = 0.0f;
    //    m_fadeManager.alfaIn = 1.0f;

    //    while (m_fadeManager.alfaOut <= 1.0f)
    //    {
    //        m_fadeManager.RespawnFadeIn();
    //        yield return new WaitForEndOfFrame();
    //    }

    //    transform.position = respawnPosition.transform.position;


    //    while (m_fadeManager.alfaOut >= 1.0f && m_fadeManager.alfaIn >= 0)
    //    {
    //        m_fadeManager.RespawnFadeOut();
    //        yield return new WaitForEndOfFrame();
    //    }
    //    isFade = false;
    //    yield return null;
    //}

    ////リスポーンのフェードをしていたらtrue
    //public bool isFadeRespawn()
    //{
    //    return isFade;
    //}

    public Transform   respawnPosition;
    public FadeManager fadeManager;

    public float fadeSpeed = 0.05f;

    bool isRespawn = false;

    public void Respawn()
    {
        StartCoroutine(Respawn_());
    }

    IEnumerator Respawn_()
    {
        isRespawn = true;

        yield return fadeManager.FadeOut(fadeSpeed);
        transform.position = respawnPosition.transform.position;
        yield return fadeManager.FadeIn (fadeSpeed);

        isRespawn = false;
    }
}
