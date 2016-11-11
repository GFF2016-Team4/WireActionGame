using UnityEngine;

public class RopeInput
{
    public const string leftButton  = "Fire1";
    public const string rightButton = "Fire2";

    //左
    public static bool isLeftRopeButton
    {
        get { return Input.GetButton(leftButton);      }
    }

    public static bool isLeftRopeButtonUp
    {
        get { return Input.GetButtonUp(leftButton);    }
    }

    public static bool isLeftRopeButtonDown
    {
        get { return Input.GetButtonDown(leftButton);  }
    }


    //右
    public static bool isRightRopeButton
    {
        get { return Input.GetButton(rightButton);     }
    }

    public static bool isRightRopeButtonUp
    {
        get { return Input.GetButtonUp(rightButton);   }
    }

    public static bool isRightRopeButtonDown
    {
        get { return Input.GetButtonDown(rightButton); }
    }
}