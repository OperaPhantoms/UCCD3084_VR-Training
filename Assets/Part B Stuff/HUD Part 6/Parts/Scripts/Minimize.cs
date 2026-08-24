using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Credits to Stack Overflow and AI :D

public class ClipboardMinimize : MonoBehaviour
{
    public Transform content;
    public Button minimizeButton;
    public GameObject[] objectsToHide;
    public Vector3 minimizedScale = new Vector3(0.2f, 0.2f, 0.2f);
    public float animationSpeed = 8f;
    public Vector3 minimizedPosition = new Vector3(-0.15f, -0.05f, 0.5f);
    private bool isMinimized = false;
    private Vector3 originalContentScale;
    private Vector3 originalPosition;
    private Dictionary<GameObject, bool> objectStates = new Dictionary<GameObject, bool>();

    void Start()
    {
        originalContentScale = content.localScale;

        originalPosition = transform.localPosition;

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

        if (isMinimized)
        {
            objectStates.Clear();
            foreach (var go in objectsToHide)
            {
                if (go != null)
                    objectStates[go] = go.activeSelf;
            }
        }

        foreach (var go in objectsToHide)
        {
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

        // Assisted by AI, I really wanted that animation :D
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