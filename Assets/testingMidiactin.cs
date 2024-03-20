using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testingMidiactin : MonoBehaviour
{
    public List<MidiInputActions> actions = new();
    public bool[] temp = new bool[12];
    public void Update()
    {

        foreach (var action in actions)
        {
            Debug.Log(action.action.isActive(temp));
        }
    }

    public bool[][] SubArray(bool[] originalArray, int substringLength)
    {
        int subArrayLength = 3; // Example subarray length
        int[] subArray = new int[subArrayLength];

        for (int i = 0; i < subArrayLength; i++)
        {
            subArray[i] = originalArray[i + startIndex]; // Adjust startIndex as needed
        }

        // Get notes for the first octave (elements 0 to 11)
        bool[] octaveActiveKeys = new bool[12];
        Array.Copy(enabledKeys, 0, octaveActiveKeys, 0, 12);
    }
}
