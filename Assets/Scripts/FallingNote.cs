using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NAudio.Midi;

/// <summary>
/// Represents a falling note object in the game.
/// </summary>
public class FallingNote : MonoBehaviour
{
    /// <summary>
    /// Velocity of the falling note.
    /// </summary>
    public float velocity;

    /// <summary>
    /// Maximum Y bound for the falling note.
    /// </summary>
    public float maxYBound;

    /// <summary>
    /// Indicates if this is the last falling note.
    /// </summary>
    public bool isLast = false;


    private void Update()
    {
        // Move the falling note downwards
        transform.Translate(Vector2.down * velocity * Time.deltaTime, Space.World);

        // Destroy the falling note if it goes below a certain Y position
        if ((transform.position + (Vector3.up * maxYBound)).y < -10) { Destroy(gameObject); }
    }

    private void OnDestroy()
    {
        // Start the song end coroutine if this is the last falling note being destroyed
        if (isLast)
        {
            GameManager.instance.StartCoroutine(GameManager.instance.OnSongEnd());
            GameManager.instance.modifiedNoteScale = 0;
        }
    }
}
