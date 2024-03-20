using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testingMidiactin : MonoBehaviour
{
    public static testingMidiactin instance;
    public bool enableActions;
    public List<MidiInputActions> midiActions = new();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance );
        }
    }
    public void test(bool[] activeKeys)
    {
        if (!enableActions) { return; }
        foreach (var action in midiActions)
        {
            Debug.Log(action.action.isActive(activeKeys));

        }
    }
}
