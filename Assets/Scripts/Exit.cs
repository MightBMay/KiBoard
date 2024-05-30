using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Exit : MonoBehaviour
{
    /// <summary>
    /// Should the next click of the exit button close the game?
    /// </summary>
    bool exitOnClick = false;
    Coroutine exitButtonCoroutine;
    /// <summary>
    /// Exits the game when button is clicked twice.
    /// </summary>
    /// <param name="text"> Text object to update the value of.</param>
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
    /// <summary>
    /// Immediately exits the game.
    /// </summary>
    public void ExitToDesktopImmediate()
    {
        Application.Quit();
    }
    /// <summary>
    /// Resets the text of the exit button after 5 seconds of not clicking it. PROBABLY UPDATE WITH AN ANIMATION INSTEAD.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    IEnumerator ResetExitButton(TextMeshProUGUI text)
    {
        exitOnClick = true;
        yield return new WaitForSecondsRealtime(5f);
        text.text = "Exit";
        exitOnClick = false;
    }

}
