using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static int combo;

    public static UnityEngine.Color HexToColor(string hex)
    {
        UnityEngine.Color color = new UnityEngine.Color();

        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        else 
        {
            Debug.LogError("Invalid hex color: " + hex);
            return UnityEngine.Color.white;
        }
    }
}
