
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// SongEditor custom Button Ui script to allow for middle and right mouse clicks on buttons.
/// </summary>
public class TriMouseButton : Button
{
    public UnityEvent onMiddleClick;
    public UnityEvent onRightClick;
    /// <summary>
    /// what number mouse button this button's action is bound to currently.
    /// </summary>
    public sbyte mouseButton;
    
    protected override void Awake()
    {
        onMiddleClick.AddListener(MiddleClick);
        onRightClick.AddListener(RightClick);
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            onMiddleClick?.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            onRightClick?.Invoke();
        }
    }
    /// <summary>
    /// Assigns an EditorAction based on gameObject.name to the coresponding mouse button this script was activated with.
    /// </summary>
    /// <param name="action">Action to be assigned to</param>
    /// <param name="mouseButton">mouse button number to assign action to.</param>
    void SetAction(ref EditorAction action, sbyte mouseButton)
    {
       
        if (action != null) // if an action was already selected:
        {
           
            action.Reset();// clear any variables that specific action may use
            var tmb = SongEditor.instance.triMouseButtons.Where(button => button.mouseButton == mouseButton); // get all trimousebuttons with matching button number.
            foreach(var b in tmb) { b.ResetColour(); } // reset their colours.             
            
        }
        action = SongEditor.instance.InitializeAction(gameObject.name, mouseButton); // initialize and assign new editor action.
        colors = SetInactiveColour(SongEditor.instance.mouseButtonColours[mouseButton]); // on selection, change colour of the button to corespond with which mouse button it is now bound to.

    }
    public void LeftClick()
    {        
        SetAction(ref SongEditor.instance.leftAction, mouseButton = 0);  
        
    }
    public void MiddleClick()
    {
        SetAction(ref SongEditor.instance.middleAction, mouseButton =2);
    }
    public void RightClick()
    {
        SetAction(ref SongEditor.instance.rightAction, mouseButton = 1);
    }
    /// <summary>
    /// Reset colour block to White.
    /// </summary>
    public void ResetColour()
    {
        colors = SetInactiveColour(Color.white);
    }
    /// <summary>
    /// Creates and returns a color block with all values set to NewColour.
    /// </summary>
    /// <param name="newColour"> new colour to fill the colorblock.</param>
    /// <returns></returns>
    ColorBlock SetInactiveColour (Color newColour)
    {
        return new ColorBlock()
        {
            highlightedColor = newColour,
            normalColor = newColour,
            pressedColor = newColour,
            selectedColor = newColour,
            disabledColor = newColour,
            colorMultiplier = 1
        };

    }
}
