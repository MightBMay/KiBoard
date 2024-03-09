using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CameraScrollManager : MonoBehaviour
{
    [SerializeField] float scrollSpeed; // rate at which the camera scrolls.
    [SerializeField] float minHeight;

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
        if (transform.position.y <= minHeight) { transform.position = new(transform.position.x, minHeight, transform.position.z); }
    }
}
