using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcceptOrder : MonoBehaviour
{
    public OrderInfoSO order;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnDeliveryAccepted);
    }

    private void OnDeliveryAccepted()
    {
        DeliveryManager.Instance.StartDelivery(order);
    }

}
