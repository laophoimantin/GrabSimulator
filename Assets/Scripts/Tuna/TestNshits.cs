using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNshits : MonoBehaviour
{
    [SerializeField]
    public OrderInfoSO order1;
    public OrderInfoSO order2;
    public OrderInfoSO order3;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DeliveryManager.Instance.StartDelivery(order1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DeliveryManager.Instance.StartDelivery(order2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DeliveryManager.Instance.StartDelivery(order3);
        }
    }
}
