using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public TMP_Text scoreText; 
    public Renderer[] quadRenderers;
    public Material defaultMat;
    public Material yellowMat;

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
   
        scoreText.text = score.ToString();

        // Why do I have to do math again
        int NoLight = ((score - 1) % 10) + 1;

        // X = Light, X-Y = No Light, preetty self explainatory 
        for (int i = 0; i < quadRenderers.Length; i++)
        {
            if (i < NoLight)
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

