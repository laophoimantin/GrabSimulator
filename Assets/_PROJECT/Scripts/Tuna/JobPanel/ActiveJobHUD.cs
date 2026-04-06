using UnityEngine;
using TMPro;

public class ActiveJobHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _panel; 
    [SerializeField] private TMP_Text _txtStatus;
    [SerializeField] private TMP_Text _txtCargo;  
    [SerializeField] private TMP_Text _txtRoute; 
    [SerializeField] private TMP_Text _txtReward;   
    private void OnEnable()
    {
        DeliveryManager.OnDeliveryUpdated += RefreshHUD;
    }

    private void OnDisable()
    {
        DeliveryManager.OnDeliveryUpdated -= RefreshHUD;
    }

    private void Start()
    {
        RefreshHUD();
    }

    private void RefreshHUD()
    {
        Order currentOrder = DeliveryManager.Instance.GetCurrentOrder();

        if (currentOrder == null)
        {
            _panel.SetActive(false);
            return;
        }

        _panel.SetActive(true);
        _txtReward.text = $"Reward: <color=yellow>{currentOrder.Reward}</color> VNĐ";
        _txtCargo.text = $"Cargo: {currentOrder.CargoData.CargoName}";
        DeliveryState state = DeliveryManager.Instance.GetCurrentState();

        if (state == DeliveryState.Accepted)
        {
            _txtStatus.text = "<color=yellow>PICKING UP PACKAGE</color>";
            _txtRoute.text = $"Go to: {currentOrder.PickupLocID}";
        }
        else if (state == DeliveryState.CarryingPackage)
        {
            _txtStatus.text = "<color=green>DELIVERING PACKAGE</color>";
            _txtRoute.text = $"Deliver to: {currentOrder.DropLocID}";
        }
    }
}