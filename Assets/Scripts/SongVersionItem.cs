using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

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
    public void SetValues(string replayPath)
    {
        if(string.IsNullOrEmpty(replayPath)) { return; }
        versionPath = replayPath;
        text.text = gameObject.name = Path.GetFileNameWithoutExtension(versionPath);
       
    }

    public void SelectVersion()
    {
        GameSettings.currentSongPath = versionPath;
        button.interactable = false;
        SongVersionMenu.instance.selectedButton.interactable = true;
        SongVersionMenu.instance.selectedButton = button;
    }

}
