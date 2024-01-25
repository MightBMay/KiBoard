using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NoteShadow : MonoBehaviour
{

    public void SetShadowSize(float ySize)
    {
        var spriterend = GetComponent<SpriteRenderer>();
        spriterend.size = new Vector2(1.35f, ySize);
    }
}
