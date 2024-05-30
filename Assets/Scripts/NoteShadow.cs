using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NoteShadow : MonoBehaviour
{
    /// <summary>
    /// sets scale of drop shadow of note
    /// </summary>
    /// <param name="ySize"></param>
    public void SetShadowSize(float ySize)
    {
        var spriterend = GetComponent<SpriteRenderer>();
        spriterend.size =  new Vector2(GameSettings.usePiano ? 1.35f: 4.8f, ySize);
    }
}
