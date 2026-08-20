using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ClipboardMinimize : MonoBehaviour
{
    [Header("References")]
    public Transform content;              // 'Contents' object
    public Button minimizeButton;          // the MinimizeButton button component
    public GameObject[] objectsToHide;     // ProgressButton, SettingsButton, Contents

    [Header("Scale Settings")]
    public Vector3 minimizedScale = new Vector3(0.2f, 0.2f, 0.2f);
    public float animationSpeed = 8f;

    [Header("Position Settings")]
    public Vector3 minimizedPosition = new Vector3(-0.15f, -0.05f, 0.5f);

    private bool isMinimized = false;
    private Vector3 originalContentScale;
    private Vector3 originalPosition;
    private Dictionary<GameObject, bool> objectStates = new Dictionary<GameObject, bool>();

    void Start()
    {
        if (content != null)
            originalContentScale = content.localScale;

        originalPosition = transform.localPosition;

        if (minimizeButton != null)
            minimizeButton.onClick.AddListener(ToggleMinimize);
    }

    public void ToggleMinimize()
    {
        isMinimized = !isMinimized;
        StopAllCoroutines();
        StartCoroutine(AnimateMinimize());
    }

    private IEnumerator AnimateMinimize()
    {
        Vector3 targetScale = isMinimized ? minimizedScale : originalContentScale;
        Vector3 targetPos   = isMinimized ? minimizedPosition : originalPosition;

        Vector3 startScale  = content.localScale;
        Vector3 startPos    = transform.localPosition;

        // Save active states before hiding
        if (isMinimized)
        {
            objectStates.Clear();
            foreach (var go in objectsToHide)
            {
                if (go != null)
                    objectStates[go] = go.activeSelf;
            }
        }

        // Hide or restore
        foreach (var go in objectsToHide)
        {
            if (go == null) continue;
            if (isMinimized)
                go.SetActive(false);
            else
            {
                if (objectStates.ContainsKey(go))
                    go.SetActive(objectStates[go]);
                else
                    go.SetActive(true);
            }
        }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * animationSpeed;
            content.localScale = Vector3.Lerp(startScale, targetScale, t);
            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        content.localScale = targetScale;
        transform.localPosition = targetPos;
    }
}