using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaytestTest : MonoBehaviour
{
    private string songEditorSceneName = "SongEditorScene";
    private string gameSceneName = "GameScene88";

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
        yield return new WaitUntil(() => SceneManager.GetSceneByName(gameSceneName).isLoaded);

        // Find an object in the loaded scene and call the playtest method
        var gameMan = FindObjectOfType<GameManager>();
        gameMan.inEditor = true;
        var mInput = FindObjectOfType<MidiInput>();
        List<NoteEventInfo> noteEvents = new();
        foreach (var note in SongEditor.instance.editorNotes) {
            noteEvents.Add(note.noteEvent);
        }
        GameSettings.currentSongPath = $"{Application.persistentDataPath}/Songs/FurElise";
        GameSettings.bpm = 130;
        mInput.StartSong(noteEvents, $"{Application.persistentDataPath}/Songs/FurElise/FurElise.mp3");
  
    }

    // Unload the game scene and return to the song editor
    public void UnloadGameScene()
    {
        SceneManager.UnloadSceneAsync(gameSceneName);
    }
}
