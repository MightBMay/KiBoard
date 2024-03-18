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
        PlayerSettings.musicVolume = volume;
    }

}

[System.Serializable]
public class GameSettings
{
    public static GameSettings instance;
    public static string currentSongName;
    public static bool usePedal;
    public static float timeInterval;
    public static bool usePiano;
    public static float bpm = 0;
    public static int noteCount;

    public GameSettings()
    {
        instance = this;
    }

    public static void ResetSettings()
    {
        currentSongName = "";
        bpm = 0;
        noteCount = 0;
    }
}

[System.Serializable]
public class PlayerSettings
{
    public static PlayerSettings instance;
    public static float musicVolume;

    public PlayerSettings()
    {
        instance = this;
    }
}
