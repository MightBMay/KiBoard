using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class CameraScrollManager : MonoBehaviour
{
    public float scrollSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ScrollCamera(Input.mouseScrollDelta.y);
    }

    public void ScrollCamera(float scrollDelta)
    {
        
        if( scrollDelta == 0 || Input.GetKey(KeyCode.LeftShift)) { return; }

        transform.position += Mathf.Sign(scrollDelta) * scrollSpeed * Vector3.up;
        if (transform.position.y <= 3.5f) { transform.position = new(transform.position.x, 3.55f, transform.position.z); }
    }
}
