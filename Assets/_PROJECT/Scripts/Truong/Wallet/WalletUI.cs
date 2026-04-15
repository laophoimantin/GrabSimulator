using UnityEngine;
using TMPro; 

public class WalletUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text _coinText;

    [Header("Settings")]
    [SerializeField] private string _prefix = "VND ";
    [SerializeField] private string _suffix = ""; 

    private void Start()
    {
        if (WalletSystem.Instance != null)
        {
            UpdateUI(WalletSystem.Instance.Coins);
        }
    }

    private void OnEnable()
    {
        if (WalletSystem.Instance != null)
        {
            WalletSystem.Instance.OnCoinsChanged += UpdateUI;
        }
    }

    private void OnDisable()
    {
        if (WalletSystem.Instance != null)
        {
            WalletSystem.Instance.OnCoinsChanged -= UpdateUI;
        }
    }

    private void UpdateUI(int currentCoins)
    {
        _coinText.text = $"{_prefix}{currentCoins:N0}{_suffix}";
    }
}