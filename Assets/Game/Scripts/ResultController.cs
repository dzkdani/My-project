using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultController : MonoBehaviour
{
    [Header("Result Component")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreText;
    public Button nextButton;

    private void OnEnable() {
        nextButton.onClick.AddListener(delegate {MatchController.Instance.MatchStart();});
    }

    public void SetMatchWinner(string _winner, int _red, int _blue) {
        resultPanel.SetActive(true);
        resultText.text = _winner+"\nWON";
        scoreText.text = "Score\n RED "+_red+" : "+" "+_blue+" BLUE";
    }

    public void SetGameWinner(string _winner) {
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(delegate {MatchController.Instance.Init();});
        nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play Again";
        resultPanel.SetActive(true);
        resultText.text = "WINNER\n"+_winner;
    }
}
