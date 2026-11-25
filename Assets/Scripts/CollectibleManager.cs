
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    [Header("Sammlung")]
    [SerializeField] private int totalItems = 10;
    [SerializeField] private TMP_Text progressText;

    private readonly HashSet<CollectibleItem> collectedItems = new();
    private bool keyCollected;
    public bool KeyCollected => keyCollected;
    private bool coffeeCupCollected;
    public bool CoffeeCupCollected => coffeeCupCollected;
    public int CollectedCount => collectedItems.Count;

    [Header("Spielabschluss")]
    [SerializeField] private GameObject endGameMenu;
    [SerializeField] private PraiseTextController praiseTextController;

    private bool gameCompleted;

    private void Awake() {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Es gibt mehrere CollectibleManager!");
            return;
        }

        if (endGameMenu != null)
        {
            endGameMenu.SetActive(false);
        }

        if (praiseTextController != null)
        {
            praiseTextController.OnPraiseHidden += HandlePraiseHidden;
        }

        Instance = this;
        UpdateProgressDisplay();
    }

    public void CollectItem(CollectibleItem item) {
        // Verhindert, dass derselbe Gegenstand doppelt zählt
        if (!collectedItems.Add(item))
        {
            return;
        }

        if (item.IsKey)
        {
            keyCollected = true;
            Debug.Log("Schlüssel eingesammelt!");
        }

        if (item.IsCoffeeCup)
        {
            coffeeCupCollected = true;
            Debug.Log("Kaffeetasse eingesammelt!");
        }
        UpdateProgressDisplay();

        Debug.Log(
            $"Gegenstand gesammelt: {collectedItems.Count}/{totalItems}"
        );

    }

    private void UpdateProgressDisplay() {
        if (progressText != null)
        {
            progressText.text =
                $"Gesammelt: {collectedItems.Count}/{totalItems}";
        }
    }

    private void HandlePraiseHidden()
    {
        if (collectedItems.Count >= totalItems && !gameCompleted)
        {
            gameCompleted = true;

            if (endGameMenu != null)
            {
                endGameMenu.SetActive(true);
            }
        }
    }
}
