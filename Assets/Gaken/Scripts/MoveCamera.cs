using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class MoveCamera : MonoBehaviour
{

    public float m_MaxSpeed = 6.0f;     // 最高速度（メートル/秒）
    public float m_MinSpeed = -2.0f;        // 最低速度（最大バック速度）
    public float m_AccelPower = 2.0f;       // 加速度（メートル/秒/秒）
    public float m_BrakePower = 6.0f;       // ブレーキ速度（メートル/秒/秒）

    public float m_BlurSpeed1 = 3.0f;
    public float m_BlurSpeed2 = 5.0f;

    float m_Speed = 0f;         // 前進速度（前進はプラス、後退はマイナス）
    CharacterController m_Controller;

    // Use this for initialization
    void Start()
    {
        m_Controller = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        float axisVertical = Input.GetAxisRaw("Vertical");

        // 減速する（入力が無い場合 or 進行方向と逆に入力がある場合）
        if ((axisVertical == 0) || (m_Speed * axisVertical < 0))
        {
            if (m_Speed > 0)
            {
                m_Speed = Mathf.Max(m_Speed - m_BrakePower * Time.deltaTime, 0);
            }
            else {
                m_Speed = Mathf.Min(m_Speed + m_BrakePower * Time.deltaTime, 0);
            }
        }

        // 上下キーで加速
        m_Speed +=
            m_AccelPower
            * axisVertical
            * Time.deltaTime;

        // 速度制限
        m_Speed = Mathf.Clamp(m_Speed, m_MinSpeed, m_MaxSpeed);

        Blur();

        // 速度を、プレイヤーが向いている方向のベクトルに変換する
        Vector3 velocity = transform.forward * m_Speed;

        m_Controller.Move(velocity * Time.deltaTime);

    }

    void Blur()
    {
        if (m_Speed >= m_BlurSpeed1)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MotionBlur>().enabled = true;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MotionBlur>().blurAmount = 0.4f;
        }
        else if (m_Speed >= m_BlurSpeed2 && m_Speed < m_BlurSpeed1)
        {
            //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MotionBlur>().enabled = true;
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MotionBlur>().blurAmount = 0.8f;
        }
        else
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MotionBlur>().enabled = false;
    }
}
