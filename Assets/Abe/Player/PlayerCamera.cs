using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Tooltip("注目するオブジェクト")]
    public Transform target;

    [Tooltip("ターゲットとの距離")]
    public float distance = 1.0f;

    [Tooltip("オフセット")]
    public Vector3 offset;

    [Tooltip("回転スピード")]
    public Vector2 rotationSpeed = new Vector2(120.0f, 120.0f);

    [Tooltip("カメラの上下回転の限界")]
    public float cameraLimitUp = 30f;

    [Tooltip("カメラの上下回転の限界")]
    public float cameraLimitDown = -30f;
    
    void LateUpdate()
    {
        ChangeCursorState();

        Vector2 rotate;
        rotate.x = Input.GetAxis("Horizontal2") * rotationSpeed.x * Time.deltaTime;
        rotate.y = Input.GetAxis("Vertical2") * rotationSpeed.y * Time.deltaTime;

        //回転
        transform.RotateAround(target.position, Vector3.up     , rotate.x);
        transform.RotateAround(target.position, transform.right, rotate.y);

        FixedAngle();

        Vector3 position = target.position;       //初期化
        position -= transform.forward * distance; //ターゲットの後ろに下がって見やすいように
        position += offset;                       //オフセット値

        //座標の変更
        transform.position = Vector3.Lerp(transform.position, position, 0.8f);
    }

    void ChangeCursorState()
    {
        //EscapeだけだとEscapeを押したときに表示しっぱなしになる
        if(Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UnLockCursor();
        }
    }

    void LockCursor()
    {
         Cursor.visible   = false;
         Cursor.lockState = CursorLockMode.Locked;
    }

    void UnLockCursor()
    {
        Cursor.visible   = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void FixedAngle()
    {
        Vector3 angle = transform.eulerAngles;

        if(Input.GetButtonDown("ResetCamera"))
        {
            angle.x = 0;
        }
        else
        {
            //angleは取得時に0～360の値になるため
            if(angle.x >= 180)
            {
                angle.x -= 360;
            }
            
            //上限値・下限値を設定してカメラが変な挙動をしないように
            angle.x = Mathf.Clamp(angle.x, -20, 80); 
        }

        angle.z = 0;
        transform.rotation = Quaternion.Euler(angle);
    }
}