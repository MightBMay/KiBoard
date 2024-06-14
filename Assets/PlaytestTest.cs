using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaytestTest : MonoBehaviour
{
    private string songEditorSceneName = "SongEditorScene";
    private string gameSceneName = "GameScene88";

    GameObject editorScene;

    private void Start()
    {

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadGameScene();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            UnloadGameScene();
        }
    }
    // Load the GameScene88 additively and call a method in it
    public void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Additive);
        StartCoroutine(CallPlaytestInGameScene());
    }

    // Coroutine to wait for the scene to load, then call the method
    private IEnumerator CallPlaytestInGameScene()
    {

        yield return null;
        //make this just load gamemanager as a scene and close editor scene. make a class that stores all the neccessary information to reload the editor tho.


    }




    // Unload the game scene and return to the song editor
    public void UnloadGameScene()
    {
        SceneManager.UnloadSceneAsync(gameSceneName);
    }
}


