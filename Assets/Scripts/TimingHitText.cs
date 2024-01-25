using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics.Tracing;

public class TimingHitText : MonoBehaviour
{
    public static TimingHitText instance;
    [SerializeField] GameObject textPrefab;
    [SerializeField] float fadeoutTimer;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }
    void SpawnTimingText(string score, Color color)
    {
        TextMeshProUGUI text = Instantiate(textPrefab, transform).GetComponent<TextMeshProUGUI>();
        text.text = score;
        text.color = color;
        text.transform.SetAsLastSibling();
        StartCoroutine(FadeoutDestroy(text));

    }
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

    public void CreateTimingText(string score)
    {
        switch (score)
        {
            case "Perfect":
                SpawnTimingText(score, new Color(1, 0, 0.75f, .5f));
                break;

            case "Good":
                SpawnTimingText(score, new Color(0.15f, 1, .5f, .5f));
                break;

            case "Okay":
                SpawnTimingText(score, new Color(0, 0.04f, 1, .5f));
                break;
            default:
                SpawnTimingText(score, new Color(1, 0, 0, .5f));
                break;
        }
    }

}
