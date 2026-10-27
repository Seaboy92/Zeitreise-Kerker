
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CollectibleItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRGrabInteractable grabInteractable;
    [SerializeField] private GameObject affordanceCallout;

    [Header("Collection")]
    [SerializeField] private bool isKey;
    [SerializeField] private bool startsLocked;
    [SerializeField] private bool isCoffeeBag;

    public bool IsKey => isKey;

    private bool isHeld;
    private bool isCollected;
    private bool isLocked;

    private void Awake()
    {
        if (grabInteractable == null)
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
        }

        isLocked = startsLocked;

        if (grabInteractable != null)
        {
            grabInteractable.enabled = !isLocked;
        }

        if (isLocked && affordanceCallout != null)
        {
            affordanceCallout.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (grabInteractable == null)
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
        }

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }

    private void Update()
    {
        var rightHand =
            InputSystem.GetDevice<UnityEngine.InputSystem.XR.XRController>();

        var trigger = rightHand?.TryGetChildControl<AxisControl>("trigger");

        if (isHeld &&
            !isCollected &&
            !isLocked &&
            trigger != null &&
            trigger.ReadValue() > 0.8f)
        {
            Collect();
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {

        if (isLocked)
        {
            ShowLockedHint();
            return;
        }

        isHeld = true;

        if (affordanceCallout != null)
        {
            affordanceCallout.SetActive(false);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;

        if (!isCollected && !isLocked &&
            affordanceCallout != null)
        {
            affordanceCallout.SetActive(true);
        }
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;

        if (grabInteractable != null)
        {
            grabInteractable.enabled = !locked;
        }

        if (affordanceCallout != null)
        {
            affordanceCallout.SetActive(!locked && !isHeld);
        }
    }

    public void Collect()
    {
        if (isCollected || isLocked || !isHeld)
        {
            return;
        }

        if (CollectibleManager.Instance == null)
        {
            Debug.LogError(
                "Kein CollectibleManager in der Szene vorhanden!");
            return;
        }

        isCollected = true;

        CollectibleManager.Instance.CollectItem(this);

        if (isCoffeeBag && CoffeeQuestManager.Instance != null)
        {
            CoffeeQuestManager.Instance.OnCoffeeBagCollected();
        }

        Destroy(gameObject);
    }
    public void ShowLockedHint()
    {
        if (isLocked && CoffeeQuestManager.Instance != null)
        {
            CoffeeQuestManager.Instance.OnLockedCupAttempt();
        }
    }
}
