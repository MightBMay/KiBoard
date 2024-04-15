using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    public static void SetVolume(float volume)
    {
        PlayerSettings.musicVolume = volume;
        MP3Handler.instance.SetVolume(volume);
    }

}

[System.Serializable]
public class GameSettings
{
    public static GameSettings instance;
    public static string currentSongPath = "";
    public static FileGroup currentFileGroup;
    public static GameType? gameType;
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
        currentSongPath = "";
        bpm = 0;
        noteCount = 0;
        gameType = null;

        if (fullReset)
        {
            usePedal = false;
            usePiano = true;
        }
    }

    public override string ToString()
    {
        return $"Bpm: {bpm}";
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


[System.Serializable]
public enum GameType
{
    Key12,
    Key88,

}