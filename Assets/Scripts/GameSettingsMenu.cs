using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsMenu : MonoBehaviour
{
    public void SetUsePiano(bool enabled)
    {
        GameSettings.usePiano = enabled;
        Debug.Log(GameSettings.usePiano);
    }
    public void SetUsePedal(bool enabled)
    {
        GameSettings.usePedal = enabled;
        Debug.Log(GameSettings.usePedal);
    }

    public void SetVolume(float volume)
    {
        SettingsManager.SetVolume(volume);
    }
}
