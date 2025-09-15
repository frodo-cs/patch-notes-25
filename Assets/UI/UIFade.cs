using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class UIFade : MonoBehaviour
{
    public UIDocument uiDocument;
    public float fadeDuration = 1f;
    public float delayBetween = 0.5f;

    void OnEnable()
    {
        var root = uiDocument.rootVisualElement;
        var labels = root.Query<Label>(className: "fade-label").ToList();
        StartCoroutine(FadeInSequence(labels));
    }

    IEnumerator FadeInSequence(List<Label> labels)
    {
        foreach (var label in labels)
        {
            yield return StartCoroutine(FadeInLabel(label));
            yield return new WaitForSeconds(delayBetween);
        }
    }

    IEnumerator FadeInLabel(Label label)
    {
        float elapsed = 0f;
        label.style.opacity = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            label.style.opacity = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
    }
}
