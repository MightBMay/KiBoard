using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NAudio.Midi;

public class FallingNote : MonoBehaviour
{
    public float velocity;
    public string noteName;
    public float maxYBound;
    public bool isLast = false;
    TextMeshProUGUI text;
    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = noteName;

    }
    private void Update()
    {
        transform.Translate(Vector2.down * velocity * Time.deltaTime, Space.World);
        if ((transform.position + (Vector3.up * maxYBound)).y < -10) { Destroy(gameObject); }
    }
    private void OnDestroy()
    {
        if (isLast){
            GameManager.instance.StartCoroutine(GameManager.instance.OnSongEnd());
        }
    }
}
