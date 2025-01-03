using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI instance;
    [SerializeField] GameObject textPrefab;
    [SerializeField] float fadeoutTimer;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI comboText;
    [SerializeField] Slider comboBar;
    [SerializeField] Gradient comboGradient;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }
    private void Update()
    {
        SetComboBarValue(GameManager.instance.combo.multiplier);
    }
    /// <summary>
    /// spawns timing score text above the score.
    /// </summary>
    /// <param name="score">numeric change in score</param>
    /// <param name="color"> colour of the text</param>
    void SpawnTimingText(string score, Color color)
    {
        TextMeshProUGUI text = Instantiate(textPrefab, transform).GetComponent<TextMeshProUGUI>();
        text.text = score;
        text.color = color;
        text.transform.SetAsLastSibling();
        StartCoroutine(FadeoutDestroy(text));

    }
    /// <summary>
    /// Fades out the alpha of text object and destroys the text's gameObject after a delay.
    /// </summary>
    /// <param name="textMesh"></param>
    /// <returns></returns>
    IEnumerator FadeoutDestroy(TextMeshProUGUI textMesh)
    {
        Color startColor = textMesh.color; // Get the initial color
        Color targetColor = new(startColor.r, startColor.g, startColor.b, 0); // Target color with alpha set to 0

        float elapsedTime = 0f;

        while (elapsedTime < fadeoutTimer)
        {
            Color temp;
            // Assign the new color back to the TextMeshProUGUI
            textMesh.color = temp = Color.Lerp(startColor, targetColor, elapsedTime / fadeoutTimer);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure that the final color is set to the targetColor
        textMesh.color = targetColor;

        // Perform any other actions or destroy the TextMeshProUGUI object
        Destroy(textMesh.gameObject);
    }
    /// <summary>
    /// creates a different timing text based on the timing score.
    /// </summary>
    public void CreateTimingText(int score, string scoretext)
    {
        Color textColor;
        string scoreString = Mathf.Sign(score) >= 0 ? "+" : "";
        scoreString += score.ToString();
        switch (scoretext)
        {
            case "Perfect":
                textColor = new Color(1, 0, 0.75f, .5f);
                break;

            case "Good":
                textColor = new Color(0.15f, 1, .5f, .5f);
                break;

            case "Okay":
                textColor = new Color(0, 0.04f, 1, .5f);
                break;
            default:
                textColor= new Color(1, 0, 0, .5f);
                break;
        }

        SpawnTimingText(scoreString, textColor);
        scoreText.text = GameManager.instance.currentSongScore.score.ToString();
    }
    /// <summary>
    /// sets the value of the combo bar slider based off of the current combo's multiplier.
    /// </summary>
    /// <param name="value"></param>
    public void SetComboBarValue(float value)
    {
        // Assuming the input value ranges from 1 to 3
        float normalizedValue = (value - 1) / (3 - 1);
        float roundedNormalizedValue = Mathf.Round(value * 10) / 10;
        comboBar.value = normalizedValue;
        ColorBlock newColours = new ColorBlock();
        newColours.disabledColor = comboGradient.Evaluate(normalizedValue);
        newColours.colorMultiplier = 1;
        comboBar.colors = newColours;
        comboText.text = (roundedNormalizedValue).ToString() + "X";
    }

}
