using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CameraScrollManager : MonoBehaviour
{
    public float scrollSpeed; // rate at which the camera scrolls.

    void Update()
    {
        ScrollCamera(); // scroll the camera.
    }

    /// <summary>
    /// Scrolls the camera up and down while in the Song Editor.
    /// </summary>
    public void ScrollCamera()
    {

        float scrollDelta = Input.mouseScrollDelta.y;
        if ( scrollDelta == 0 || Input.GetKey(KeyCode.LeftShift)) { return; }
        transform.position += Mathf.Sign(scrollDelta) * scrollSpeed * Vector3.up;
        if (transform.position.y <= 3.5f) { transform.position = new(transform.position.x, 3.55f, transform.position.z); }
    }
}
