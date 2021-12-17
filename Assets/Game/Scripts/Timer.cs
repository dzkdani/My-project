using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Timer Component")]
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private float timerPerMatch;
    private float timeLeft;
    private bool timerOn;

    private void Start() {
        InitTimer();
    }

    public void InitTimer() {
        timeLeft = timerPerMatch;
        timerOn = true;
    }

    private void Update() {
        if (MatchController.Instance.IsPlaying)
        {
            if (timerOn)
            {
                timeLeft -= Time.deltaTime;
                DisplayTimeLeft(timeLeft);
                CheckTimer();
            }
        }
    }

    public void CheckTimer() {
        if (timeLeft <= 0)
        {
            timerOn = false;
            MatchController.Instance.MatchEnd("Draw");
        }
    }

    private void DisplayTimeLeft(float _timeLeft)
    {
        int minute = (int) _timeLeft/ 60;
        int second = (int) _timeLeft % 60;

        timerText.text = string.Format("Time Left\n{0:00}:{1:00}", minute, second);
    }
}
