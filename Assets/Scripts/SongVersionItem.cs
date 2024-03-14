using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SongVersionItem : MonoBehaviour
{
    public string versionPath;
    Button button;
    private void Start()
    {
        button = GetComponent<Button>();
    }
    public void SetValues(string midiPath, string jsonPath)
    {

        versionPath = !string.IsNullOrEmpty(jsonPath) ? jsonPath : midiPath;
        gameObject.name = Path.GetFileNameWithoutExtension(versionPath);
    }

    public void SelectVersion()
    {
        SettingsManager.instance.gameSettings.currentSongName = Path.GetFileNameWithoutExtension(versionPath);
        button.interactable = false;
        SongVersionMenu.instance.selectedButton = button;
    }

}
