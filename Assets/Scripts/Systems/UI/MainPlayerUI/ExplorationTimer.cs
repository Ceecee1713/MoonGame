using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExplorationTimer : MonoBehaviour
{
    public float RemainingTime; 

    [SerializeField]
    private CorriosonValues corriosonValues;

    [SerializeField]
    private TextMeshProUGUI timerText;

    [SerializeField]
    private float maxDurationOfExplorationPhase; 

    private bool _doNotAllowTimerToCountDown = false; 

    private int _minutes = 0;
    private int _seconds = 0;

    private const int FIRST_MOON_PUZZLE_AREA_NUMBER = 1;
    private const int SECOND_MOON_PUZZLE_AREA_NUMBER = 2;
    private const int THIRD_MOON_PUZZLE_AREA_NUMBER = 3;

    void Start()
    {
        RemainingTime = maxDurationOfExplorationPhase;

        EventBus.Instance.Subscribe<PauseExplorationTimer>(PauseTimerCountdown);
        EventBus.Instance.Subscribe<ResetExplorationTimer>(ResetTimer);
    }

    void Update()
    {
        if(_doNotAllowTimerToCountDown == true)
            return;

        if(RemainingTime == 0)
        {
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthWhenTimerIsUp, FIRST_MOON_PUZZLE_AREA_NUMBER));
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthWhenTimerIsUp, SECOND_MOON_PUZZLE_AREA_NUMBER));
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthWhenTimerIsUp, THIRD_MOON_PUZZLE_AREA_NUMBER));

            _doNotAllowTimerToCountDown = true;
            return;
        }

        if(RemainingTime > 0)
            RemainingTime -= Time.deltaTime;

        else if (RemainingTime < 0)
            RemainingTime = 0;

        _minutes = Mathf.FloorToInt(RemainingTime / 60);
        _seconds = Mathf.FloorToInt(RemainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", _minutes, _seconds);
    }

    private void PauseTimerCountdown(PauseExplorationTimer pauseExplorationTimer)
    {
        _doNotAllowTimerToCountDown = pauseExplorationTimer.AllowCountdown;
    }

    private void ResetTimer(ResetExplorationTimer resetExplorationTimer)
    {
        RemainingTime = maxDurationOfExplorationPhase;
        _doNotAllowTimerToCountDown = false;
    }
}
