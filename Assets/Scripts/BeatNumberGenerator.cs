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

    /// <summary>
    /// Instantiates beat number TextMeshProUGUI objects.
    /// </summary>
    private void Start()
    {
        for (int i = -3; i <= quantity; i++)
        {
            Transform trans = Instantiate(prefab, transform).transform;
            trans.position = new Vector2(trans.position.x, i);
            trans.GetComponent<TextMeshProUGUI>().text = i.ToString();
        }
    }
}
