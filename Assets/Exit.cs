using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Exit : MonoBehaviour
{
    bool exitOnClick = false;
    Coroutine exitButtonCoroutine;
    public void ExitToDesktopButton(TextMeshProUGUI text)
    {
        if (exitOnClick) { Application.Quit(); }
        else
        {
            if (exitButtonCoroutine == null)
            {
                exitButtonCoroutine = StartCoroutine(ResetExitButton(text));
            }
        }
    }

    public void ExitToDesktopImmediate()
    {
        Application.Quit();
    }
    IEnumerator ResetExitButton(TextMeshProUGUI text)
    {
        exitOnClick = true;
        yield return new WaitForSecondsRealtime(5f);
        text.text = "Exit";
        exitOnClick = false;
    }

}
