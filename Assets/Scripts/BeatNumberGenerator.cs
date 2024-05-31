using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Generates beat numbers as TextMeshProUGUI objects.
/// </summary>
public class BeatNumberGenerator : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int quantity;
    float heightMultiplier;


    private void Start()
    {
        heightMultiplier = SongEditor.instance.heightMultiplier;
        CreateBeatNumbers();
       
    }
    /// <summary>
    /// Instantiates beat number TextMeshProUGUI objects.
    /// </summary>
    public void CreateBeatNumbers()
    {
        for (int i = 0; i <= quantity; i++)
        {
            Transform trans = Instantiate(prefab, transform).transform;
            trans.position = new Vector2(trans.position.x, 2.5f+ i*heightMultiplier);
            trans.GetComponent<TextMeshProUGUI>().text = i.ToString();
        }
    }
}
