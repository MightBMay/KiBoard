
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TriMouseButton : Button
{
    public UnityEvent onMiddleClick;
    public UnityEvent onRightClick;
    sbyte mouseButton;
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
    public void LeftClick()
    {
        mouseButton = 0;
        colors = SetInactiveColour(SongEditor.instance.mouseButtonColours[mouseButton]);
        SongEditor.instance.middleAction = SongEditor.instance.InitializeAction(gameObject.name, mouseButton);
        
    }
    public void MiddleClick()
    {
        mouseButton = 1;
        colors = SetInactiveColour(SongEditor.instance.mouseButtonColours[mouseButton]);
        SongEditor.instance.middleAction = SongEditor.instance.InitializeAction(gameObject.name, mouseButton);
        Debug.Log("m");
    }
    public void RightClick()
    {
        mouseButton = 2;
        colors = SetInactiveColour(SongEditor.instance.mouseButtonColours[mouseButton]);
        SongEditor.instance.middleAction = SongEditor.instance.InitializeAction(gameObject.name, mouseButton);
        Debug.Log("R");
    }
    public void ResetColour()
    {
        colors = SetInactiveColour(Color.white);
    }
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
