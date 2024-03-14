using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderSetText : MonoBehaviour
{

    [SerializeField]TextMeshProUGUI textmesh;

    private void Start()
    {
        float volume = SettingsManager.instance.playerSettings.musicVolume;
        SetText(volume);
        GetComponent<Slider>().value = volume;
    }

    public void SetText(int value)
    {
        textmesh.text = value .ToString();
    }
    public void SetText(float value)
    {
        textmesh.text = Mathf.RoundToInt(value*100).ToString();
    }

    public void SetText(string value)
    {
        textmesh.text = value;
    }
}
