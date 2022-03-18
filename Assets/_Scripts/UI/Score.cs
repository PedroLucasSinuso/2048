using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;

    private int _score;    
    private int _highScore;

    private void Start() {
        _highScore = PlayerPrefs.GetInt("score");
        _highScoreText.text = _highScore.ToString();
    }
    public void ScoreUp(int points){
        _score+=points;
        _scoreText.text = _score.ToString();
        if(_score > _highScore){
            _highScore = _score;
            _highScoreText.text = _highScore.ToString();
        }
        
    }

    private void OnDestroy() {
        PlayerPrefs.SetInt("score",_highScore);
    }



}

    

