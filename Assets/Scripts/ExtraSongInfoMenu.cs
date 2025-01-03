using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ExtraSongInfoMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField]GameObject childObjects;
    public static ExtraSongInfoMenu instance;
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(this); }
    }
    /// <summary>
    /// sets the text displaying the extra information about the song.
    /// </summary>
    /// <param name="songName"></param>
    public void SetText(string songName)
    {
        childObjects.SetActive(true);
        SongScore score = GameManager.instance.selectedSongHighScore;
        if (score == null) { text.text = ""; childObjects.SetActive(false);  return; }

        string newText =
            $"{songName}\nScore: {score.score}\nPerfect: {score.perfect}\nGood: {score.good}\nOkay: {score.okay}\nExtra: {score.extra}\nMissed: {score.miss}\nBest Combo: {score.highestCombo}/{MidiReadFile.CountNotes()}";
        text.text = newText ;
    }
}
