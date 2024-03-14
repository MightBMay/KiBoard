using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SongSelectionUI : MonoBehaviour
{

    [SerializeField] GameObject songSelectorUI;
    [SerializeField] GameObject gameSettingsUI;
    [SerializeField] GameObject songVersionUI;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OpenGameSettings(GameObject disable)
    {
        if (disable != null) { disable.SetActive(false); }
        gameSettingsUI.SetActive(true);
    }


    public void OpenSongSelector(GameObject disable)
    {
        if (disable != null) { disable.SetActive(false); }
        songSelectorUI.SetActive(true);
    }

    public void OpenSongVersion(GameObject disable)
    {
        if (disable != null) { disable.SetActive(false); }
        songVersionUI.SetActive(true);
    }



    public static void OpenFileExplorer(string path)
    {
        UnityEngine.Debug.Log(Application.persistentDataPath);
        if (Directory.Exists(path))
        {
            //  Use Process.StartInfo for additional control
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = path,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }
        else
        {
            
        }
    }


}
