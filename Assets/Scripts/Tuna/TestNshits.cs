using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNshits : MonoBehaviour
{
    [SerializeField]
    public OrderInfoSO[] orders;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var random = Random.Range(0, orders.Length);
            DeliveryManager.Instance.StartDelivery(orders[random]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DeliveryManager.Instance.CompleteDelivery();
        }
    }
}
