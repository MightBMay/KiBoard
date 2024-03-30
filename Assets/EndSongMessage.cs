using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EndSongMessage : MonoBehaviour
{
    public static EndSongMessage instance;
    public GameObject messageObject;
    public TextMeshProUGUI scoreText, highScoreText, songName;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }
    private void Start()
    {  /*   int[] score = GameManager.instance.selectedSongHighScore.GetScoreArray(GameManager.instance.totalNotes);
            ShowScore($"Total Score: {score[0]}\nPerfect: {score[1]}\nGood: {score[2]}\nOkay: {score[3]}\nExtra: {score[4]}\nMissed: {score[5]}\nLongest Combo: {GameManager.instance.combo.highestCount}");
        */
    }
    public void ShowScore(string score, bool isHighScore)
    {
        messageObject.SetActive(true);
        songName.text = GameSettings.currentFileGroup.FileName;
        scoreText.text = score;
        if (isHighScore) { highScoreText.gameObject.SetActive(true); StartCoroutine(FlickerText()); }
    }
    IEnumerator FlickerText()
    {
        Color startColour = highScoreText.color;
        while (true)
        {
            highScoreText.color = new(startColour.r, startColour.b, startColour.g, 0);
            yield return new WaitForSecondsRealtime(0.4f);
            highScoreText.color = new(startColour.r, startColour.b, startColour.g, 1);
            yield return new WaitForSecondsRealtime(0.4f);
        }
    }

    public void ReturnToSongSelection()
    {
        GameManager.instance.ReturnToSongSelection();
    }

    public void RetrySong()
    {
        MidiInput.instance.LoadSongFromCurrentSettings(false);
    }
}
