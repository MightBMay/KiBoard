using System;
using UnityEngine;

[System.Serializable]
public class SongScore
{

    public int perfect;
    public int good;
    public int okay;
    public int extra;
    public int miss;
    public float GetNotePercentage(int noteCount)
    {
        if (noteCount == 0) { return Mathf.NegativeInfinity; }
        return (perfect + good + okay) / noteCount * 100;
    }
    public int[] GetScoreArray(int noteCount)
    {
        int[] scores = new int[5];
        scores[0] = perfect;
        scores[1] = good;
        scores[2] = okay;
        scores[3] = extra;
        scores[4] = noteCount - (perfect + good + okay);
        return scores;
    }
    public void ClearScore()
    {
        perfect = 0;
        good = 0;
        okay = 0;
        extra = 0;
        miss = 0;
    }
}