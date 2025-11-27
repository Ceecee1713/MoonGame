using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExplorationTimer : MonoBehaviour
{
    [SerializeField]
    private CorriosonValues corriosonValues;

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private float maxDurationOfExplorationPhase; 

    private float _remainingTime; 

    private bool _doNotAllowTimerToCountDown = false; 

    private int _minutes = 0;
    private int _seconds = 0;

    private const int FIRST_MOON_PUZZLE_AREA_NUMBER = 1;
    private const int SECOND_MOON_PUZZLE_AREA_NUMBER = 2;
    private const int THIRD_MOON_PUZZLE_AREA_NUMBER = 3;

    void Start()
    {
        _remainingTime = maxDurationOfExplorationPhase;

        EventBus.Instance.Subscribe<PauseExplorationTimer>(PauseTimerCountdown);
        EventBus.Instance.Subscribe<ResetExplorationTimer>(ResetTimer);
    }

    void Update()
    {
        if(_doNotAllowTimerToCountDown == true)
            return;

        if(_remainingTime == 0)
        {
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthWhenTimerIsUp, FIRST_MOON_PUZZLE_AREA_NUMBER));
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthWhenTimerIsUp, SECOND_MOON_PUZZLE_AREA_NUMBER));
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthWhenTimerIsUp, THIRD_MOON_PUZZLE_AREA_NUMBER));

            _doNotAllowTimerToCountDown = true;
            return;
        }

        if(_remainingTime > 0)
            _remainingTime -= Time.deltaTime;

        else if (_remainingTime < 0)
            _remainingTime = 0;

        _minutes = Mathf.FloorToInt(_remainingTime / 60);
        _seconds = Mathf.FloorToInt(_remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", _minutes, _seconds);
    }

    private void PauseTimerCountdown(PauseExplorationTimer pauseExplorationTimer)
    {
        _doNotAllowTimerToCountDown = pauseExplorationTimer.AllowCountdown;
    }

    private void ResetTimer(ResetExplorationTimer resetExplorationTimer)
    {
        _remainingTime = maxDurationOfExplorationPhase;
        _doNotAllowTimerToCountDown = false;
    }
}
