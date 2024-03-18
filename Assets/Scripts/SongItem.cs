using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SongItem : MonoBehaviour
{
    public TextMeshProUGUI songName, songContains;
    public FileGroup fileGroup;


    public void OpenSongVersions()
    {
        SongVersionMenu.instance.OpenMenu(fileGroup);
    }
    public void LoadSongInfoToGameSettings()
    {
        
        OpenSongVersions();
        GameSettings.currentSongName = fileGroup.FileName;
        string songName = GameSettings.currentSongName;
        SongSelection.instance.startButton.interactable = true;
        SongList.instance.SelectItem(this);
        GameManager.instance.selectedSongHighScore = SongScore.ReadFieldsFromJsonFile(songName);
        ExtraSongInfoMenu.instance.SetText(songName);
        MP3Handler.instance.StartSongDemo(songName);
    }
}
