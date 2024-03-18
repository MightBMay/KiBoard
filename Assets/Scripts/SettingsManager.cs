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
    public static string currentSongName = "";
    public static bool usePedal = false;
    public static bool usePiano = true;
    public static float bpm = 0;
    public static int noteCount = 0;

    public GameSettings()
    {
        instance = this;
    }

    public static void ResetSettings(bool fullReset)
    {
        currentSongName = "";
        bpm = 0;
        noteCount = 0;

        if (fullReset)
        {
            usePedal = false;
            usePiano = true;
        }
    }
}

[System.Serializable]
public class PlayerSettings
{
    public static PlayerSettings instance;
    public static float musicVolume = 20;

    public PlayerSettings()
    {
        instance = this;
    }
}
