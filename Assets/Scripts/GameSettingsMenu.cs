using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TextMeshProUGUI bpmText;


    public void SetUsePiano(Toggle toggle)
    {
        GameSettings.usePiano = toggle.isOn;
    }
    public void SetUsePedal(Toggle toggle)
    {
        GameSettings.usePedal = toggle.isOn;
    }
}
