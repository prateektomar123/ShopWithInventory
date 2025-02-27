using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private int gold = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log($"Currency started with {gold} gold.");
    }

    public bool CanAfford(int cost)
    {
        return gold >= cost;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        gold += amount;
        Debug.Log($"Gained {amount} gold. Total: {gold}");
    }

    public void RemoveGold(int amount)
    {
        if (amount <= 0 || !CanAfford(amount)) return;
        gold -= amount;
        Debug.Log($"Spent {amount} gold. Total: {gold}");
    }

    public int GetGold()
    {
        return gold;
    }
}