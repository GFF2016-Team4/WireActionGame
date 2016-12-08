using UnityEngine;

public class InputExtension
{
    public static bool GetButtonAll(params string[] inputs)
    {
        foreach(string input in inputs)
        {
            if(!Input.GetButton(input)) return false;
        }

        return true;
    }

    public static bool GetButtonAny(params string[] inputs)
    {
        foreach(string input in inputs)
        {
            if(Input.GetButton(input)) return true;
        }

        return false;
    }

    public static bool GetButtonAnyUp(params string[] inputs)
    {
        foreach(string input in inputs)
        {
            if(Input.GetButtonUp(input)) return true;
        }

        return false;
    }

    public static bool GetButtonAnyDown(params string[] inputs)
    {
        foreach(string input in inputs)
        {
            if(Input.GetButtonDown(input)) return true;
        }

        return false;
    }
}