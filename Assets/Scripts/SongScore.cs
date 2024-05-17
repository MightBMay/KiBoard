using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SongScore
{
    public DateTime timeSet;
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
        int scoreChange = 0;
        switch (Score)
        {
            case "Perfect":
                perfect++;
                scoreChange = (250);
                break;

            case "Good":
                good++;
                scoreChange = (150);
                break;

            case "Okay":
                okay++;
                scoreChange = (75);
                break;
            default:
                extra++;
                scoreChange = -100;
                CameraShake.ShakeCamera(2.5f, 0.1f);
                break;


        }
        int finalScoreChange = (int)(scoreChange * multiplier);
        score += finalScoreChange;
        GameUI.instance.CreateTimingText(finalScoreChange, Score);
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
        timeSet = System.DateTime.Now;
        noteAccuracy = GetNotePercentage(GameManager.instance.totalNotes);
        if (score > savedScores.score) { writeScore = true; }
        if (savedScores.highestCombo > curHighestCombo) { highestCombo = savedScores.highestCombo; }
        else { highestCombo = curHighestCombo; writeScore = true; }
        if (writeScore) WriteScoreToJson(GameSettings.currentFileGroup.FolderPath);
        
        return writeScore;

    }

    public void WriteScoreToJson(string filename)
    {
        Debug.Log(filename);
        try
        {
            // Serialize the object to JSON format
            string json = JsonUtility.ToJson(this);
            string filePath = filename +"/"+ GameSettings.currentFileGroup.FileName + ".score";


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

