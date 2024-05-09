using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsMenu : MonoBehaviour
{
    [SerializeField] Toggle usePianoToggle;
    private void Start()
    {
        StartCoroutine(MonitorMidiConnections());
    }

    IEnumerator MonitorMidiConnections()
    {
        while (true)
        {
            
            CheckMidiDevices(KiboardDebug.isMidiConnected);
            yield return new WaitForSeconds(1);
        }
        void CheckMidiDevices(bool midiConnected)
        {
            if (usePianoToggle == null) return;
            usePianoToggle.interactable = midiConnected;
            if (!midiConnected) { usePianoToggle.isOn = false; }
        }


    }


    public void SetUsePiano(bool enabled)
    {
        GameSettings.usePiano = enabled;
    }
    public void SetUsePedal(bool enabled)
    {
        GameSettings.usePedal = enabled;
    }

    public void SetVolume(float volume)
    {
        SettingsManager.SetVolume(volume);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
