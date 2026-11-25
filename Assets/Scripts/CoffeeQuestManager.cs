
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;

public class CoffeeQuestManager : MonoBehaviour {
    public static CoffeeQuestManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject coffeeMachineObject;
    [SerializeField] private GameObject coffeeCupObject;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private GameObject coffeeCupInteractionObject;

    private bool coffeeBagCollected;
    private bool coffeeMade;
    private bool machineSelected;
    private bool previousTriggerPressed;
    private XRSimpleInteractable coffeeMachine;
    private CollectibleItem coffeeCup;
    private XRSimpleInteractable coffeeCupInteraction;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (coffeeMachineObject != null) {
            coffeeMachine =
                coffeeMachineObject.GetComponent<XRSimpleInteractable>();

            if (coffeeMachine == null) {
                Debug.LogError(
                    "XR Simple Interactable wurde an der Kaffeemaschine nicht gefunden!"
                );
            }
        }

        if (coffeeCupObject != null) {
            coffeeCup = coffeeCupObject.GetComponent<CollectibleItem>();

            if (coffeeCup == null) {
                Debug.LogError(
                    "CollectibleItem wurde an der Kaffeetasse nicht gefunden!"
                );
            }
            else {
                coffeeCup.SetLocked(true);
            }
        }

        if (coffeeCupInteractionObject != null) {
            coffeeCupInteraction =
                coffeeCupInteractionObject
                    .GetComponent<XRSimpleInteractable>();
        }

        ShowHint(false, "");
    }

    private void OnEnable() {
        if (coffeeMachine != null) {
            coffeeMachine.selectEntered.AddListener(OnMachineSelected);
            coffeeMachine.selectExited.AddListener(OnMachineDeselected);
        }
        if (coffeeCupInteraction != null) {
            coffeeCupInteraction.selectEntered
                .AddListener(OnCupSelected);

            coffeeCupInteraction.selectExited
                .AddListener(OnCupDeselected);
        }
    }

    private void OnDisable() {
        if (coffeeMachine != null) {
            coffeeMachine.selectEntered.RemoveListener(OnMachineSelected);
            coffeeMachine.selectExited.RemoveListener(OnMachineDeselected);
        }
        if (coffeeCupInteraction != null) {
            coffeeCupInteraction.selectEntered
                .RemoveListener(OnCupSelected);

            coffeeCupInteraction.selectExited
                .RemoveListener(OnCupDeselected);
        }
    }

    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Update() {
        bool triggerPressed = IsRightTriggerPressed();
        bool triggerJustPressed =
            triggerPressed && !previousTriggerPressed;

        previousTriggerPressed = triggerPressed;

        if (!machineSelected) {
            return;
        }

        if (coffeeMade) {
            ShowHint(true, "Kaffee ist fertig! Nimm die Tasse.");
            return;
        }

        if (!coffeeBagCollected) {
            ShowHint(true, "Hole zuerst den Kaffee aus dem Lager.");
            return;
        }

        ShowHint(true, "Trigger drücken, um Kaffee zuzubereiten.");

        if (triggerJustPressed) {
            MakeCoffee();
        }
    }

    private void OnMachineSelected(SelectEnterEventArgs args) {
        machineSelected = true;

        if (coffeeMade) {
            ShowHint(true, "Kaffee ist fertig! Nimm die Tasse.");
        }
        else if (!coffeeBagCollected) {
            ShowHint(true, "Hole zuerst den Kaffee aus dem Lager.");
        }
        else {
            ShowHint(true, "Trigger drücken, um Kaffee zuzubereiten.");
        }
    }

    private void OnMachineDeselected(SelectExitEventArgs args) {
        machineSelected = false;
        ShowHint(false, "");
    }

    public void OnCoffeeBagCollected() {
        coffeeBagCollected = true;
    }
    public void OnLockedCupAttempt() {
        if (!coffeeMade) {
            ShowHint(true, "Du musst zuerst Kaffee zubereiten!");
        }
    }
    private void MakeCoffee() {
        if (coffeeMade || !coffeeBagCollected) {
            return;
        }

        coffeeMade = true;

        if (coffeeCupInteraction != null) {
            coffeeCupInteraction.enabled = false;
        }

        if (coffeeCup != null) {
            coffeeCup.SetLocked(false);
        }

        ShowHint(true, "Kaffee ist fertig! Nimm die Tasse.");
    }

    private bool IsRightTriggerPressed() {
        var rightHand =
            InputSystem.GetDevice<UnityEngine.InputSystem.XR.XRController>();

        var trigger =
            rightHand?.TryGetChildControl<AxisControl>("trigger");

        return trigger != null && trigger.ReadValue() > 0.8f;
    }

    private void ShowHint(bool visible, string message) {
        if (hintText == null) {
            return;
        }

        if (hintText.gameObject.activeSelf != visible) {
            hintText.gameObject.SetActive(visible);
        }

        if (visible) {
            hintText.text = message;
        }
    }
    private void OnCupSelected(SelectEnterEventArgs args) {
        if (!coffeeMade) {
            OnLockedCupAttempt();
        }
    }
    private void OnCupDeselected(SelectExitEventArgs args) {
        if (!coffeeMade) {
            ShowHint(false, "");
        }
    }
}
