using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongVersionItem : MonoBehaviour
{
    public string versionPath;
    [SerializeField]Button button;
    [SerializeField]TextMeshProUGUI text;
    public void SetValues(string midiPath, string jsonPath)
    {

        versionPath = !string.IsNullOrEmpty(jsonPath) ? jsonPath : midiPath;
        gameObject.name = Path.GetFileNameWithoutExtension(versionPath);
        text.text = SongSelection.GetPostUnderscoreSubstring(Path.GetFileNameWithoutExtension(versionPath));
    }

    public void SelectVersion()
    {
        SettingsManager.instance.gameSettings.currentSongName = Path.GetFileNameWithoutExtension(versionPath);
        button.interactable = false;
        SongVersionMenu.instance.selectedButton.interactable = true;
        SongVersionMenu.instance.selectedButton = button;
    }

}
