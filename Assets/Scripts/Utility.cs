using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;


public class Utility : MonoBehaviour
{
    public static float NormalizeNumberToRange(float input, float min, float max)
    {
        return min + (input * (max - min));
    }

}

