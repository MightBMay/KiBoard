using System;
using System.IO;
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
    public float noteAccuracy;
    public int highestCombo;
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
    public void AddScore(string Score, float multiplier)
    {
        int ans = 0;
        switch (Score)
        {
            case "Perfect":
                perfect++;
                ans = (int)(100 * multiplier);
                score += ans;
                break;

            case "Good":
                good++;
                ans = (int)(60 * multiplier);
                score += ans;
                break;

            case "Okay":
                okay++;
                ans = (int)(30 * multiplier);
                score += ans;
                break;
            default:
                extra++;
                ans = -10;
                score += ans;
                CameraShake.ShakeCamera(2.5f, 0.1f);
                break;
        }
        GameUI.instance.CreateTimingText(ans, Score);
    }
    public void ClearScore()
    {
        score = 0;
        perfect = 0;
        good = 0;
        okay = 0;
        extra = 0;
        miss = 0;
        highestCombo = 0;
    }

    public bool FinalizeScore()
    {
        bool writeScore = false;
        SongScore savedScores = GameManager.instance?.selectedSongHighScore ?? new SongScore();
        int curHighestCombo = GameManager.instance.combo.highestCount;
        noteAccuracy = GetNotePercentage(GameManager.instance.totalNotes);
        if (score > savedScores.score) { writeScore = true; }
        if (savedScores.highestCombo > curHighestCombo) { highestCombo = savedScores.highestCombo; }
        else { highestCombo = curHighestCombo; writeScore = true; }
        if (writeScore) WriteScoreToJson(GameSettings.currentFileGroup.FolderPath);
        return writeScore;

    }

    public void WriteScoreToJson(string filename)
    {
        try
        {
            // Serialize the object to JSON format
            string json = JsonUtility.ToJson(this);
            string filePath =  filename + ".score";


            // Write the JSON string to the file
            File.WriteAllText(filePath, json);

        }
        catch (Exception ex)
        {
            Debug.LogError($"Error writing fields to JSON file: {ex.Message}");
        }
    }

    public static SongScore ReadFieldsFromJsonFile(string filePath)
    {
        
        try
        {
            // Check if the file exists
            if (File.Exists(filePath) && !string.IsNullOrEmpty(filePath))
            {
                // Read the JSON string from the file
                string json = File.ReadAllText(filePath);

                // Deserialize the JSON string into a SongScore object
                return JsonUtility.FromJson<SongScore>(json);
            }
            else
            {
                Debug.Log($"Score File '{filePath}' does not exist.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading fields from JSON file: {ex.Message}");
            return null;
        }
    }
}

