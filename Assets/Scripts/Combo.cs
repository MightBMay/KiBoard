using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Combo
{
    /// <summary>
    /// Maximum multiplier that can be achieved.
    /// </summary>
    const float maxMultiplier = 3;
    /// <summary>
    /// how many missed notes before multiplier is set to 1.
    /// </summary>
    const int comboKillThreshold = -10;
    /// <summary>
    /// Current score multiplier.
    /// </summary>
    public float multiplier = 1;
    /// <summary>
    /// Current number of consecutive notes without dropping combo.
    /// </summary>
    public int count;
    /// <summary>
    /// current amount of notes dropped. multiplier is reset to 1 when this reaches -10.
    /// </summary>
    public float dropCounter;
    /// <summary>
    ///  tracks the highest count reached this song.
    /// </summary>
    public int highestCount = 0;

    /// <summary>
    /// Resets combo variables to default values.
    /// </summary>
    public void ClearCombo()
    {
        multiplier = 1;
        count = 0;
        dropCounter = 0;
        highestCount = 0;

    }
    /// <summary>
    /// Updates the multiplier based on the timing score of the note pressed.
    /// </summary>
    /// <param name="score">String containing "Perfect, Good, Okay" used to determine how much to add to the multiplier.</param>
    public void ChangeMultiplier(string score)
    {
        float increase = GetIncrease(score);
        multiplier += increase;
        if (Mathf.Sign(increase) >= 0)
        {
            count++;
            if (highestCount < count) { highestCount = count; }
            dropCounter += 0.1f;
        }
        else
        {
            if (multiplier > 1) { dropCounter -= 1; }
            else { dropCounter = 0; }
        }

        if (dropCounter <= comboKillThreshold)
        {
            multiplier = 0;
            count = 0;
            dropCounter = 0;
        }

        multiplier = Mathf.Clamp(multiplier, 1, maxMultiplier);
        dropCounter = Mathf.Clamp(dropCounter, comboKillThreshold, 0);
        try { GameUI.instance.SetComboBarValue(multiplier); } catch { Debug.Log("Error Setting ComboBarValue"); }

    }

    /// <summary>
    /// Get the Multiplier increase based on note timing score.
    /// </summary>
    /// <param name="score">string score for note score timing.</param>
    /// <returns>float Change in multiplier based off of note score timing</returns>
    public float GetIncrease(string score)
    {
        switch (score)
        {
            case "Perfect": return 0.05f;
            case "Good": return 0.025f;
            case "Okay": return 0.01f;
            default: return -0.05f;
        }
    }
}