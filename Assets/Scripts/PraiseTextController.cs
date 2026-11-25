
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class PraiseTextController : MonoBehaviour
{
    [SerializeField] private TMP_Text praiseText;
    [SerializeField] private float displayDuration = 3f;

    private Coroutine hideCoroutine;
    public event Action OnPraiseHidden;

    private void Awake() {
        if (praiseText != null) {
            praiseText.gameObject.SetActive(false);
        }
        else {
            Debug.LogError("PraiseTextController: PraiseText ist nicht zugewiesen!");
        }
    }

    public void ShowPraise(string message) {
        if (praiseText == null)
            return;

        praiseText.text = message;
        praiseText.gameObject.SetActive(true);

        if (hideCoroutine != null) {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay() {
        yield return new WaitForSeconds(displayDuration);

        praiseText.gameObject.SetActive(false);
        hideCoroutine = null;

        OnPraiseHidden?.Invoke();
    }
}
