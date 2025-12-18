using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [SerializeField] private Transform _cargoSocket;
    private GameObject _currentPackage;

    public void GetPackage(GameObject package)
    {
        package.transform.position = _cargoSocket.position;
        package.transform.SetParent(_cargoSocket);
        _currentPackage = package;
    }

    public void DropPackage(Transform dropPoint)
    {
        _currentPackage.transform.position = dropPoint.position;
        _currentPackage.transform.SetParent(null);
        _currentPackage = null;
    }
}
