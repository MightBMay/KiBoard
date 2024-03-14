using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameSettings gs;
    [SerializeField] TextMeshProUGUI bpmText;

    void Start()
    {
        gs = SettingsManager.instance.gameSettings;
    }

    public void SetUsePiano(Toggle toggle)
    {
        gs.usePiano = toggle.isOn;
    }
    public void SetUsePedal(Toggle toggle)
    {
        gs.usePedal = toggle.isOn;
    }
}
