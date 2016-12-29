using UnityEngine;

public class RopeInput
{
    public const string leftButton          = "Fire1";
    public const string rightButton         = "Fire2";
    public const string ropeTakeUpButton    = "RopeTakeUp";
    public const string ropeTakeDownButton  = "RopeTakeDown";
    public const string ropeCatchButton     = "CatchRope";

    //ロープの上下
    public static bool isTakeUpButton
    {
        get { return Input.GetButton(ropeTakeUpButton);   }
    }

    public static bool isTakeDownButton
    {
        get { return Input.GetButton(ropeTakeDownButton); }
    }

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

    public static bool isCatchRopeButton
    {
        get { return Input.GetButton(ropeCatchButton); }
    }
}