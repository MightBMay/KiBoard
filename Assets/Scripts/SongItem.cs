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
        SettingsManager.instance.gameSettings.currentSongName = fileGroup.FileName;
        SongSelection.instance.startButton.interactable = true;
        SongList.instance.SelectItem(this);
        GameManager.instance.selectedSongHighScore = SongScore.ReadFieldsFromJsonFile(SettingsManager.instance.gameSettings.currentSongName);
        ExtraSongInfoMenu.instance.SetText();
    }
}
