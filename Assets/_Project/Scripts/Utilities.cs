using UnityEngine;

public static class Utilities
{
    public static bool IsPropertyNull<T>(T property)
    {
        if (property == null)
        {
            Debug.Log("Property Null!");
            return true;
        }

        return false;
    }
}