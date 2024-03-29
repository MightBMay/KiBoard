using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoad : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float delay;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Color onColour;
    void Start()
    {
        StartCoroutine(FlickerText());
        StartCoroutine(LoadToSongSelect());
        
    }
    IEnumerator LoadToSongSelect()
    {
        yield return new WaitUntil(() => Input.anyKeyDown || MidiInput.instance.GetAnyNoteActive());
        SceneManager.LoadScene("SongSelect", LoadSceneMode.Single); // dont use transistion manager because i do not want a fade out.
    }
    IEnumerator FlickerText()
    {
        while (true)
        {
            text.color = onColour;
            yield return new WaitForSecondsRealtime(delay);
            text.color = Color.clear;
            yield return new WaitForSecondsRealtime(delay);
        }
    }
}
