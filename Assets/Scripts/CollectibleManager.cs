
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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Es gibt mehrere CollectibleManager!");
            return;
        }

        Instance = this;
        UpdateProgressDisplay();
    }

    public void CollectItem(CollectibleItem item)
    {
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

        UpdateProgressDisplay();

        Debug.Log(
            $"Gegenstand gesammelt: {collectedItems.Count}/{totalItems}"
        );

    }

    private void UpdateProgressDisplay()
    {
        if (progressText != null)
        {
            progressText.text =
                $"Gesammelt: {collectedItems.Count}/{totalItems}";
        }
    }
}
