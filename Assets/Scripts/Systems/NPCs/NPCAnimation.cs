using System.Collections;
using UnityEngine;

public class NPCAnimation : MonoBehaviour
{
    [Header("Y Position Settings")]
    [SerializeField] 
    private float minimumValue = -1f;
    [SerializeField] 
    private float maximumValue = 1f;
    [SerializeField] 
    private float durationOfAnimationLoop = 1f;

    private Coroutine _loopCoroutine;

    void Start()
    {
        _loopCoroutine = StartCoroutine(LoopY());
    }

    private IEnumerator LoopY()
    {
        while (true)
        {
            yield return LerpY(minimumValue, maximumValue);
            yield return LerpY(maximumValue, minimumValue);
        }
    }

    private IEnumerator LerpY(float startingValue, float targetValue)
    {
        float elapsed = 0f;

        while (elapsed < durationOfAnimationLoop)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / durationOfAnimationLoop);
            Vector3 currentPosition = transform.position;
            currentPosition.y = Mathf.Lerp(startingValue, targetValue, t);
            transform.position = currentPosition;
            yield return null;
        }

        Vector3 finalPosition = transform.position;
        finalPosition.y = targetValue;
        transform.position = finalPosition;
    }
}
