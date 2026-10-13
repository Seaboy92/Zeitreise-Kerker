
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CollectibleItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRGrabInteractable grabInteractable;
    [SerializeField] private GameObject affordanceCallout;
    [SerializeField] private bool isKey;
    public bool IsKey => isKey;
    private bool isHeld;
    private bool isCollected;

    private void Awake()
    {
        // Referenz automatisch finden, falls nicht gesetzt
        if (grabInteractable == null)
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
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
        // E sammelt den Schlüssel ein, solange er gehalten wird
        if (isHeld &&
            !isCollected &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Collect();
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isHeld = true;

        if (affordanceCallout != null)
        {
            affordanceCallout.SetActive(false);
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isHeld = false;

        if (!isCollected && affordanceCallout != null)
        {
            affordanceCallout.SetActive(true);
        }
    }

    public void Collect()
    {
        if (isCollected || !isHeld)
        {
            return;
        }

        if (CollectibleManager.Instance == null)
        {
            Debug.LogError(
                "Kein CollectibleManager in der Szene vorhanden!"
            );
            return;
        }

        isCollected = true;

        CollectibleManager.Instance.CollectItem(this);

        Destroy(gameObject);
    }
}
