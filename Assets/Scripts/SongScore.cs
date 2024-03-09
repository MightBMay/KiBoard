using System;
using UnityEngine;

[System.Serializable]
public class SongScore
{
    public int score;
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
        int[] scores = new int[6];
        scores[0] = score;
        scores[1] = perfect;
        scores[2] = good;
        scores[3] = okay;
        scores[4] = extra;
        scores[5] = noteCount - (perfect + good + okay);
        return scores;
    }
    public void AddScore(string Score) {
        switch (Score)
        {
            case "Perfect":
                perfect++;
                score += 100;
                break;

            case "Good":
                good++;
                score += 75;
                break;

            case "Okay":
                okay++;
                score += 50;
                break;
            default:
                extra++;
                score -= 10;
                CameraShake.ShakeCamera(3f, 0.1f);
                break;
        }
        TimingHitText.instance.CreateTimingText(Score);
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