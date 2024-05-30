using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class KiboardDebug : MonoBehaviour
{

    #region Variables
    public static KiboardDebug instance;
    /// <summary>
    /// Is game currently overriding the volume.
    /// </summary>
    public static bool overideVolume = false;
    /// <summary>
    /// Volume value at which to override.
    /// </summary>
    public static float overideVolumeValue;

    /// <summary>
    /// Returns true if any midi device is detected
    /// </summary>
    public static bool isMidiConnected
    {
        get
        {
            return midiInGetNumDevs() > 0;
        }
    }
    #endregion

    #region References
    /// <summary>
    /// canvas containing Debug UI
    /// </summary>
    static Canvas debugCanvas;
    [SerializeField] Toggle midiConnectedToggle;
    [SerializeField] Text fpsText;
    [SerializeField] InputField volumeOverrideIF;
    [SerializeField] GameObject parent;
    #endregion

    #region Other
    // Define the Winmm.dll functions
    [DllImport("winmm.dll")]
    private static extern int midiInGetNumDevs();

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(parent);
            debugCanvas = GetComponent<Canvas>();
        }
        else { Destroy(this); }
        
    }

    private void Update()
    {
        CheckOpenMenu();
    }
    /// <summary>
    /// check if the open/close menu hotkey is set.
    /// </summary>
    void CheckOpenMenu()
    {
        if (Input.GetKey(KeyCode.LeftAlt)  && Input.GetKeyDown(KeyCode.F1))
        {
            if (debugCanvas.enabled) { HideDebugMenu(); }
            else { ShowDebugMenu(); }
        }
        
    }


    public void OnEnable()
    {
        StartCoroutine(CalculateFPS());
        StartCoroutine(MonitorMidiDevices());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// begin checking if there is a connected midi device.
    /// </summary>
    /// <returns></returns>
    IEnumerator MonitorMidiDevices()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            try { midiConnectedToggle.isOn = isMidiConnected; }
            catch { Debug.LogWarning("MidiConnectedToggle was null"); yield break; }
        }
    }
    /// <summary>
    /// begin calculating Fps.
    /// </summary>
    /// <returns></returns>
    IEnumerator CalculateFPS()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            
            try { fpsText.text = $"FPS:  {Mathf.Round(10 / Time.deltaTime )/10}"; } // calculate fps and assign to text.
            catch { Debug.LogWarning("Assigning Fps to Debug menu's fpsText encountered an error."); yield break; }

        }
    }

    /// <summary>
    /// Override the volume of all music played.
    /// </summary>
    /// <param name="enabled"> is the volume override enabled?</param>
    public void VolumeOverride(bool enabled)
    {
        if (enabled)
        {
            if (!int.TryParse(volumeOverrideIF.text, out int newValue))
            {
                Debug.LogWarning("Error Parsing Volume Override Value");
            }
            else
            {
                MP3Handler.instance.SetVolume( PlayerSettings.musicVolume = Math.Clamp(newValue, 0, 100) ); 
                overideVolume = true;
            }
        }
        else
        {
            overideVolume = false;
        }

    }
    /// <summary>
    /// show the debug canvas.
    /// </summary>
    public static void ShowDebugMenu()
    {
        debugCanvas.enabled = true;
    }
    /// <summary>
    /// hide the debug canvas.
    /// </summary>
    public static void HideDebugMenu()
    {
        debugCanvas.enabled = false;
    }
}
