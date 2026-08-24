using UnityEngine;
using TMPro;   // Import TextMeshPro

public class HUDManager : MonoBehaviour
{
    public TMP_Text scoreText;          //  .---.   
    public Renderer[] quadRenderers;    // / o o \ [Go Away!]
    public Material defaultMat;         // |  ^  |/
    public Material yellowMat;          // '-----'

    private int score = 0;

    void Start()
    {
        UpdateDisplay();
    }

    public void IncrementScore()
    {
        score++;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        // Update the score text
        if (scoreText != null)
            scoreText.text = score.ToString();

        // Why do I have to do math again
        int litCount = ((score - 1) % 10) + 1;

        // sWITCH uP mATERIALS bETWEEN DEFAULT AND YELLOW
        for (int i = 0; i < quadRenderers.Length; i++)
        {
            if (i < litCount)
                quadRenderers[i].material = yellowMat;
            else
                quadRenderers[i].material = defaultMat;
        }
    }

    public int GetScore()
    {
        return score;
    }

    public void SetScore(int newScore)
    {
        score = newScore;
        UpdateDisplay();
    }
}

