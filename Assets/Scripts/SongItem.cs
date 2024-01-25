using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongItem : MonoBehaviour
{
    public TextMeshProUGUI songName, songContains;
    public FileGroup fileGroup;
    public void LoadSongInfoToGameSettings()
    {
        SettingsManager.instance.gameSettings.currentSongName = fileGroup.FileName;
        SongSelection.instance.startButton.interactable = true;
    }
}
