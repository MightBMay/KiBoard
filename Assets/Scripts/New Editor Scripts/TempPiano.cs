using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TempPiano : MonoBehaviour
{
    // Start is called before the first frame update
    public Dictionary<int, GameObject> keyLanes = new();
    Camera cam;
    [SerializeField] GameObject prefab;
    [SerializeField] Color col1, col2;
    [SerializeField]Vector3 noteLaneScale = new Vector3(1, 20, 1);
    [SerializeField]Vector3 baseNoteScale = new Vector3(.8f, 5, 1); 
    void Start()
    {
        cam = Camera.main;

        InitializePianoRoll();
    }
    private void Update()
    {
        HandleMouseInput();
    }


    public void HandleMouseInput()
    {
        RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider == null) return; // change this to account for a isdragging variable when we get there.       

        Right();
        Left();
        Middle();

        void Left()
        {
            Down();
            Hold();
            Up();
            void Down()
            {

                if (!Input.GetMouseButtonDown(0)) { return; }
                Debug.Log(keyLanes[Mathf.RoundToInt(hit.point.x)]);
            }


            void Hold()
            {
                // if you are holding LMB but it isnt the first frame you pressed it:
                if(Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0)) { return; }

                //hold logic
            }


            void Up(){
                if(!Input.GetMouseButtonDown(0)) { return; }
            }
        }

        void Middle()
        {

        }

        void Right()
        {

        }

        // make one for scroll wheel.
    }




    public void InitializePianoRoll()
    {
        for (int i = 0; i < 88; i++)
        {
            Transform trans = Instantiate(prefab, transform).transform;
            trans.position = new(i, 0, 0);
            trans.localScale = baseNoteScale;
            trans.GetComponent<SpriteRenderer>().color = GetKeyColour(i);
            Transform lane = Instantiate(prefab, trans).transform;
            lane.GetComponent<SpriteRenderer>().color = i % 2 == 0 ? col1 : col2;
            lane.localScale = noteLaneScale;
            lane.position += new Vector3(0f, (noteLaneScale.y * baseNoteScale.y / 2) - 2.5f, 1f);
            lane.gameObject.tag = "KeyLane";
            SongEditor.instance.keyLanes[i] = trans.gameObject;

            keyLanes.Add(i, lane.gameObject);
        }
    }
    Color GetKeyColour(int i)
    {
        int value = i % 12;
        if (value == 0 || value == 2 || value == 3 || value == 5 || value == 7 || value == 9 || value == 11) return Color.white;
        else { return Color.black; }
    }

}
