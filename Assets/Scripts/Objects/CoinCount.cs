using System;
using UnityEngine;
using UnityEngine.UI;

public class CoinCount : MonoBehaviour
{
    private int score = 0;
    private Text scoreText;
    private void Awake()
    {
        scoreText = GetComponent<Text>();
    }
    private void Update()
    {
        scoreText.text = Convert.ToString(score);
    }
    public void AddScore(int scoreToAdd) {
        score += scoreToAdd;
    }
}
