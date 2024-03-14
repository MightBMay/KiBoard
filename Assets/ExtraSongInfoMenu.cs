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

    public void SetText()
    {
        childObjects.SetActive(true);
        SongScore score = GameManager.instance.selectedSongHighScore;
        if (score == null) { text.text = ""; childObjects.SetActive(false);  return; }

        string newText =
            $"Score: {score.score}\nPerfect: {score.perfect}\nGood: {score.good}\nOkay: {score.okay}\nExtra: {score.extra}\nMissed: {score.miss}\nBest Combo: {score.highestCombo}";
        text.text = newText ;
    }
}