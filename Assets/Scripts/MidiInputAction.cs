using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class MidiInputActions
{
    public delegate void CustomInputAction(int note1, int note2);
    public event CustomInputAction OnCustomInputAction;
    public string actionName;
    public OctaveAction action;


}


public class MidiAction
{

}

[System.Serializable]
public class OctaveAction
{
    public bool a, ab, b, bb, c, cb, d, db, e, f, fb, g, gb;
    private bool wasActive = false; // Tracks previously active state

    public bool isActive(bool[] activeKeys)
    {
        if (activeKeys.Length != 12)
        {
            throw new ArgumentException("activeKeys must have 12 elements");
        }

        bool currentActive = true;
        for (int i = 0; i < 12; i++)
        {
            if (this[i] != activeKeys[i])
            {
                currentActive = false;
                break;
            }
        }

        // Check for state change (active now, but wasn't previously)
        bool isActiveDown = currentActive && !wasActive;
        wasActive = currentActive; // Update previous state

        return isActiveDown;
    }

    // Indexer to access OctaveAction bools by position (a, ab, ...)
    public bool this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return a;
                case 1: return ab;
                case 2: return b;
                case 3: return bb;
                case 4: return c;
                case 5: return cb;
                case 6: return d;
                case 7: return db;
                case 8: return e;
                case 9: return f;
                case 10: return fb;
                case 11: return g;
                case 12: return gb;
                default: throw new IndexOutOfRangeException();
            }
        }
    }



}

