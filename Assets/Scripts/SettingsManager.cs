using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;
    public GameSettings gameSettings = new();
    public PlayerSettings playerSettings = new();

    private void Awake()
    {
        if (instance == null)
        {
            SettingsManager.instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }
    }

    public void SetVolume(float volume)
    {
        playerSettings.musicVolume = volume;
    }

}

[System.Serializable]
public class GameSettings
{
    public string currentSongName;
    public bool usePedal;
    public float timeInterval;
    public bool usePiano;
    public float bpm;
    public int noteCount;

    public void ResetSettings()
    {
        currentSongName = "";
        bpm = 0;
        noteCount = 0;
    }
}

[System.Serializable]
public class PlayerSettings
{
    public float musicVolume;
}
