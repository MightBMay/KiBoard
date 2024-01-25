using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EditorNote : MonoBehaviour
{
    public NoteEventInfo noteEvent;
    SpriteRenderer spriteRenderer;
    public Color startColour;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColour = spriteRenderer.color;
    }
    public void UpdateNoteEvent(int noteNumber, float startTime, float endTime)
    {
        noteEvent.startTime = startTime;
        noteEvent.endTime = endTime;
        noteEvent.noteNumber = noteNumber;
    }

    public void SetHighlightColour(Color newColour)
    {
        spriteRenderer.color = newColour;
    }
    public void ResetHighlightColour()
    {
        spriteRenderer.color = startColour;
    }
    public void SetNotePosition(float mouseHeight, float keytOriginX)
    {

        float height = mouseHeight >= Mathf.NegativeInfinity ? mouseHeight : 1;
        Vector2 gridSnap = SongNoteEditor.GridSnapNote(
        new(keytOriginX + (0.20505f * (noteEvent.noteNumber - 1)), height));

        transform.position = new(gridSnap.x - 7, gridSnap.y, 0);

    }

    public Vector2 GridSnapNote(Vector2 input, float increment = 0.204545f)
    {
        float x = Mathf.Round(input.x / increment) * increment;

        return new Vector2(x, input.y);
    }

    public void SetShadowSize(float ySize)
    {
        var spriterend = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriterend.size = new Vector2(1.35f, ySize + 0.075f);
    }
}
