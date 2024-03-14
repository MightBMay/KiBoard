using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Combo
{
    const float maxMultiplier = 3;
    const int comboKillThreshold = -10;
    public float multiplier = 1;
    public int count;
    public float dropCounter;
    public int highestCount = 0;

    public void ClearCombo()
    {
        multiplier = 1;
        count = 0;
        dropCounter = 0;
        highestCount = 0;

    }
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

    }


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