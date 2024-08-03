using UnityEngine;
using TMPro;
using System.Collections;

public class messageTextManager : MonoBehaviour
{
    public float fadeDuration = 2.0f;
    public float moveOffscreenDelay = 1.0f;
    public float delayBeforeFade = 1.0f;  // New delay before fading starts
    public Vector2 offscreenPosition = new Vector2(2000, 2000);

    private TextMeshProUGUI messageText;
    private Coroutine currentCoroutine;

    private void Start()
    {
        // Get the TextMeshProUGUI component attached to this GameObject
        messageText = GetComponent<TextMeshProUGUI>();
        if (messageText == null)
        {
            Debug.LogError("TextMeshProUGUI component is not attached to the GameObject.");
        }
    }

    public void DisplayMessage(string message)
    {
        if (messageText == null)
        {
            Debug.LogError("TextMeshProUGUI component is not found.");
            return;
        }

        // Stop any currently running coroutine
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // Reset transparency to fully opaque
        messageText.color = new Color(messageText.color.r, messageText.color.g, messageText.color.b, 1f);

        // Set the new message text
        messageText.text = message;

        // Start the animation coroutine and store the reference
        currentCoroutine = StartCoroutine(AnimateTextMessage());
    }

    private IEnumerator AnimateTextMessage()
    {
        // Move text to the center of the screen
        RectTransform rectTransform = messageText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;

        // Wait for the specified delay before starting the fade
        yield return new WaitForSeconds(delayBeforeFade);

        // Fade the text to transparency
        yield return StartCoroutine(FadeOutText());

        // Wait for a while before moving the text offscreen
        yield return new WaitForSeconds(moveOffscreenDelay);

        // Move the text off the screen
        rectTransform.anchoredPosition = offscreenPosition;

        // Clear the current coroutine reference
        currentCoroutine = null;
    }

    private IEnumerator FadeOutText()
    {
        Color originalColor = messageText.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            messageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure the text is completely transparent
        messageText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
    }
}
