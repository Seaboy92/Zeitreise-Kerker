
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DoorController : MonoBehaviour
{
    [Header("Tür")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float animationDuration = 1f;

    [Header("Rückmeldung")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private AudioSource lockedSound;

    private XRSimpleInteractable interactable;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen;
    private bool isAnimating;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        if (doorPivot == null)
        {
            Debug.LogError("DoorPivot wurde nicht zugewiesen!", this);
            return;
        }

        closedRotation = doorPivot.localRotation;
        openRotation = closedRotation
            * Quaternion.Euler(0f, openAngle, 0f);

        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnInteract);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnInteract);
        }
    }

    private void OnInteract(SelectEnterEventArgs args)
    {
        if (isAnimating)
        {
            return;
        }

        if (CollectibleManager.Instance == null ||
            !CollectibleManager.Instance.KeyCollected)
        {
            StartCoroutine(ShowLockedFeedback());
            return;
        }

        StartCoroutine(AnimateDoor(!isOpen));
    }

    private IEnumerator ShowLockedFeedback()
    {
        if (isAnimating)
        {
            yield break;
        }

        isAnimating = true;

        if (messageText != null)
        {
            messageText.text = "Verschlossen";
        }

        if (lockedSound != null)
        {
            lockedSound.Play();
        }

        Quaternion startRotation = doorPivot.localRotation;
        float elapsed = 0f;
        float duration = 0.35f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float angle = Mathf.Sin(elapsed * 45f) * 4f;
            doorPivot.localRotation =
                startRotation * Quaternion.Euler(0f, angle, 0f);

            yield return null;
        }

        doorPivot.localRotation = startRotation;

        yield return new WaitForSeconds(1.5f);

        if (messageText != null)
        {
            messageText.text = "";
        }

        isAnimating = false;
    }

    private IEnumerator AnimateDoor(bool open)
    {
        isAnimating = true;

        Quaternion startRotation = doorPivot.localRotation;
        Quaternion targetRotation = open
            ? openRotation
            : closedRotation;

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsed / animationDuration
            );

            doorPivot.localRotation = Quaternion.Slerp(
                startRotation,
                targetRotation,
                progress
            );

            yield return null;
        }

        doorPivot.localRotation = targetRotation;
        isOpen = open;
        isAnimating = false;
    }
}
