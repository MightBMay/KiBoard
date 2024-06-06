using NAudio.Midi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Represents a note in the editor with associated event information.
/// </summary>
public class EditorNote : MonoBehaviour
{
    public NoteEventInfo noteEvent;
    SpriteRenderer sprite;
    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Updates the note event information. start and end time are based off of height of mouse when adding a note.
    /// </summary>
    /// <param name="noteNumber">The note number.</param>
    /// <param name="startTime">The start time of the note event.</param>
    /// <param name="endTime">The end time of the note event.</param>
    public void UpdateNoteEvent(int noteNumber, float startTime, float endTime)
    {
        noteEvent.startTime = startTime/15;
        noteEvent.endTime = endTime/15;
        noteEvent.noteNumber = noteNumber;
    }

    /// <summary>
    /// Sets the highlight color of the note.
    /// </summary>
    /// <param name="newColour">The new highlight color.</param>
    public void SetColour(Color newColour)
    {
        sprite.color = newColour;
    }


    /// <summary>
    /// Sets the position of the note based on mouse height and key origin X.
    /// </summary>
    /// <param name="mouseHeight">The height of the mouse.</param>
    /// <param name="keyOriginX">The origin X position of the key.</param>
    public void SetNotePosition(float mouseHeight)
    {
        float height = mouseHeight >= Mathf.NegativeInfinity ? mouseHeight : 1;
        //Vector2 gridSnap = GridSnapNote();
        transform.position = new Vector2(noteEvent.noteNumber, height);
    }

    /// <summary>
    /// Snaps the note position to the grid based on the specified increment.
    /// </summary>
    /// <param name="input">The input position to snap.</param>
    /// <param name="increment">The grid snap increment.</param>
    /// <returns>The snapped position.</returns>
    public Vector2 GridSnapNote(Vector2 input, float increment = 0.204545f)
    {
        float x = Mathf.Round(input.x / increment) * increment;
        return new Vector2(x, input.y);
    }
}



public class oldEN :MonoBehaviour
{
    /// <summary>
    /// Information about the note event.
    /// </summary>
    public NoteEventInfo noteEvent;

    // Private variables
    SpriteRenderer spriteRenderer;
    public Color startColour;

    /// <summary>
    /// Initializes the note by retrieving the SpriteRenderer component and storing the initial color.
    /// </summary>
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColour = spriteRenderer.color;
    }

    /// <summary>
    /// Updates the note event information.
    /// </summary>
    /// <param name="noteNumber">The note number.</param>
    /// <param name="startTime">The start time of the note event.</param>
    /// <param name="endTime">The end time of the note event.</param>
    public void UpdateNoteEvent(int noteNumber, float startTime, float endTime)
    {
        noteEvent.startTime = startTime;
        noteEvent.endTime = endTime;
        noteEvent.noteNumber = noteNumber;
    }

    /// <summary>
    /// Sets the highlight color of the note.
    /// </summary>
    /// <param name="newColour">The new highlight color.</param>
    public void SetHighlightColour(Color newColour)
    {
        spriteRenderer.color = newColour;
    }

    /// <summary>
    /// Resets the highlight color of the note to its initial color.
    /// </summary>
    public void ResetHighlightColour()
    {
        spriteRenderer.color = startColour;
    }

    /// <summary>
    /// Sets the position of the note based on mouse height and key origin X.
    /// </summary>
    /// <param name="mouseHeight">The height of the mouse.</param>
    /// <param name="keyOriginX">The origin X position of the key.</param>
    public void SetNotePosition(float mouseHeight)
    {
        float height = mouseHeight >= Mathf.NegativeInfinity ? mouseHeight : 1;
        //Vector2 gridSnap = GridSnapNote();
        transform.position = new Vector2(noteEvent.noteNumber, height);
    }

    /// <summary>
    /// Snaps the note position to the grid based on the specified increment.
    /// </summary>
    /// <param name="input">The input position to snap.</param>
    /// <param name="increment">The grid snap increment.</param>
    /// <returns>The snapped position.</returns>
    public Vector2 GridSnapNote(Vector2 input, float increment = 0.204545f)
    {
        float x = Mathf.Round(input.x / increment) * increment;
        return new Vector2(x, input.y);
    }

    /// <summary>
    /// Sets the size of the shadow sprite.
    /// </summary>
    /// <param name="ySize">The size of the shadow along the Y-axis.</param>
    public void SetShadowSize(float ySize)
    {
        var spriterend = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriterend.size = new Vector2(1.35f, ySize + 0.075f);
    }
}