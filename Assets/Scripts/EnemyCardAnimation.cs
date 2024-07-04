using System.Collections;
using UnityEngine;
using System;

public class EnemyCardAnimation : MonoBehaviour
{
    public float animationDuration = .5f;
    private Vector3 targetPosition;
    public bool AnimationComplete { get; private set; } = false;
    public event Action onAnimationComplete;

    void Start()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        targetPosition = Camera.main.ScreenToWorldPoint(screenCenter);
        targetPosition.z = 0;

        StartCoroutine(MoveCardToCenter());
    }

    private IEnumerator MoveCardToCenter()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < animationDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / animationDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        AnimationComplete = true;
        StartCoroutine(HoldAtCenter());
    }

    private IEnumerator HoldAtCenter()
    {
        yield return new WaitForSeconds(.65f); // Hold at the center for 1 second
        onAnimationComplete?.Invoke();
        Destroy(this);
    }
}
