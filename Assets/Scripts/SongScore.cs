using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SongScore
{
    /// <summary>
    /// Time stamp of when score was performed.
    /// </summary>
    public DateTime timeSet;
    /// <summary>
    /// total score.
    /// </summary>
    public int score;
    /// <summary>
    ///  number of perfect notes
    /// </summary>
    public int perfect;
    /// <summary>
    ///  number of good notes
    /// </summary>
    public int good;
    /// <summary>
    ///  number of okay notes
    /// </summary>
    public int okay;
    /// <summary>
    ///  number of extra notes
    /// </summary>
    public int extra;
    /// <summary>
    ///  number of missd notes
    /// </summary>
    public int miss;
    /// <summary>
    /// percentage of notes hit with okay or better scoring.
    /// </summary>
    public float noteAccuracy;
    /// <summary>
    /// longest string of notes hit without dropping the combo.
    /// </summary>
    public int highestCombo;
    /// <summary>
    /// Calculates percentage of notes hit.
    /// </summary>
    /// <param name="noteCount">total number of notes.</param>
    /// <returns>percent accuracy.</returns>
    public float GetNotePercentage(int noteCount)
    {
        if (noteCount == 0) { return Mathf.NegativeInfinity; }
        return (perfect + good + okay) / noteCount * 100;
    }
    /// <summary>
    /// Combines score quantities of note scores hit into an int array.
    /// </summary>
    /// <param name="noteCount">total number of notes.</param>
    /// <returns> Array containing score, and quantities of note types.</returns>
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
    /// <summary>
    /// add a note to the song score.
    /// </summary>
    /// <param name="Score">String score for the note</param>
    /// <param name="multiplier">Current multiplier</param>
    public void AddScore(string Score)
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
        int finalScoreChange = (int)(scoreChange * GameManager.instance.combo.multiplier);
        score += finalScoreChange;
        GameUI.instance.CreateTimingText(finalScoreChange, Score);
    }
    /// <summary>
    /// resets score values to 0.
    /// </summary>
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
    /// <summary>
    /// Finishes processing score values and checks if any high scores are achieved. if so, writes them to a score file.
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// writes score file to Json with given name in the <see cref="FileGroup.FolderPath"/> of the currently selected song.
    /// </summary>
    /// <param name="filename"></param>
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
    /// <summary>
    /// Reads a .score file from a given path
    /// </summary>
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

