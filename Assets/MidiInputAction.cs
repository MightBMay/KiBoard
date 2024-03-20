using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class MidiInputActions
{
    public string actionName;
    public OctaveAction action;



}

[System.Serializable]
public class OctaveAction
{
    public List<int> noteNumbers;
    private bool wasActive = false; // Tracks previously active state

    public bool isActive(bool[] activeKeys)
    {
        var indicies = GetTrueIndices(activeKeys);
        foreach (var key in indicies) { Debug.Log(key); }

        bool currentActive = noteNumbers.All(num => indicies.Contains(num));



        // Check for state change (active now, but wasn't previously)
        bool isActiveDown = currentActive && !wasActive;
        wasActive = currentActive; // Update previous state

        return isActiveDown;
    }



    public static int[] GetTrueIndices(bool[] boolArray)
    {
        List<int> trueIndices = new List<int>();

        for (int i = 0; i < boolArray.Length; i++)
        {
            if (boolArray[i])
            {
                trueIndices.Add(i);
            }
        }

        return trueIndices.ToArray();
    }


}

