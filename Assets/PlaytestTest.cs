using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaytestTest : MonoBehaviour
{
    [SerializeField] string songFolderPath;
    [SerializeField] FileGroup group;


    private void Start()
    {
        songFolderPath   = Application.persistentDataPath + "/Songs/FurElise";
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadGameScene();
        }
    }
    // Load the GameScene88 additively and call a method in it
    public void LoadGameScene()
    {
        group = SongSelection.AssembleFileGroup(songFolderPath);
        Debug.Log(group.ToString());
        GameManager.instance.LoadSongToGameSettingsAndStart(group, SongEditor.GetNoteEventInfos(),130f);
    }

}


