using UnityEngine;

public class TouchButton : MonoBehaviour
{
    [Tooltip("Drag the child 'Button' object here (the blue part that you touch)")]
    public Transform buttonVisual;
    
    [Tooltip("How far the button sinks down when touched")]
    public float sinkDistance = 0.03f;
    
    private Vector3 originalLocalPos;
    private bool isPressed = false;

    void Start()
    {
        if (buttonVisual != null)
        {
            originalLocalPos = buttonVisual.localPosition;
        }
    }

    // Call this from XR Simple Interactable -> Hover Entered
    public void PressDown()
    {
        if (buttonVisual != null && !isPressed)
        {
            isPressed = true;
            buttonVisual.localPosition = originalLocalPos - new Vector3(0, sinkDistance, 0);
        }
    }

    // Call this from XR Simple Interactable -> Hover Exited
    public void ReleaseUp()
    {
        if (buttonVisual != null && isPressed)
        {
            isPressed = false;
            buttonVisual.localPosition = originalLocalPos;
        }
    }
}
