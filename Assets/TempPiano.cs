using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TempPiano : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject prefab;
    [SerializeField] Color col1, col2;
    Vector3 noteLaneScale = new Vector3(1, 20, 1);
    Vector3 baseNoteScale = new Vector3(.8f, 5, 1); 
    void Start()
    {
        for(int i = 0; i < 88; i++)
        {
            Transform trans = Instantiate(prefab, transform).transform;
            trans.position = new(i, 0, 0);
            trans.localScale = new Vector3(1, 5, 1);
            trans.GetComponent<SpriteRenderer>().color = GetKeyColour(i);
            Transform  lane  = Instantiate(prefab, trans).transform;
            lane.GetComponent<SpriteRenderer>().color = i % 2 == 0 ? col1 : col2;
            lane.localScale = noteLaneScale;
            lane.position += new Vector3(0f, (noteLaneScale.y *baseNoteScale.y/ 2) - 2.5f, 1f);
        }
    }

    Color GetKeyColour(int i)
    {
        int value = i % 12;
        if (value == 0 || value == 2 || value == 3 || value == 5 || value == 7 || value == 9 || value == 11) return Color.white;
        else { return Color.black; }
    }

}
