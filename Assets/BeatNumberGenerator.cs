using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeatNumberGenerator : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int quantity;
    private void Start()
    {
        for (int i = 0; i <= quantity; i++)
        {
            Transform trans = Instantiate(prefab, transform).transform;
            trans.position = new Vector2(trans.position.x, i);
            trans.GetComponent<TextMeshProUGUI>().text = i.ToString();
        }
    }
}
