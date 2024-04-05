using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class SongItem : MonoBehaviour
{
    public TextMeshProUGUI songName, songContains;
    public FileGroup fileGroup;


    public void OpenSongVersions()
    {
        if (Input.GetKey(KeyCode.LeftShift)) { SongVersionMenu.instance.OpenReplayMenu(fileGroup); }
        else{ SongVersionMenu.instance.OpenMenu(fileGroup); }
    }
    public void LoadSongInfoToGameSettings()
    {

        OpenSongVersions();
        GameSettings.currentFileGroup = fileGroup;
        GameSettings.currentSongPath = fileGroup.JsonFiles.Count() > 0 ? fileGroup.JsonFiles[0] : fileGroup.MidiFiles[0];
        SongSelection.instance.startButton.interactable = true;
        SongList.instance.SelectItem(this);
        GameManager.instance.selectedSongHighScore = SongScore.ReadFieldsFromJsonFile(fileGroup.ScoreFile);
        ExtraSongInfoMenu.instance.SetText(fileGroup.FileName);
        MP3Handler.instance.StartSongDemo(fileGroup.Mp3File);
        UiHolder.instance.animator.SetBool("isSongSelected", true);
    }
}
