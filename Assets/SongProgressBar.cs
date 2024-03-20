using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SongProgressBar : MonoBehaviour
{
    [SerializeField]Slider bar;

    private void Start()
    {
        bar = GetComponent<Slider>();
        bar.minValue = MidiInput.instance.storedNoteEvents[0].startTime;
        bar.maxValue = MidiInput.instance.storedNoteEvents.Last().startTime;
    }
    void Update()
    {
        bar.value = GameManager.instance.songTime;

    }
}
