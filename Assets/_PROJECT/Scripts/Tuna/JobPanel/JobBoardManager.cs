using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class JobBoardManager : Singleton<JobBoardManager>
{
    [Header("Config")]
    [SerializeField] private int _maxJobsOnBoard = 8;
    [SerializeField] private int _minLifespan = 1;
    [SerializeField] private int _maxLifespan = 4;

    private List<Order> _availableJobs = new();
    public List<Order> GetAllJobs() => _availableJobs;

    public static Action OnBoardUpdated;
    
    private void Start()
    {
        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            AddNewRandomOrder();
        }
        OnBoardUpdated?.Invoke();
    }

    private void AddNewRandomOrder()
    {
        if (_availableJobs.Count >= _maxJobsOnBoard) return;

        Order rawOrder = OrderGenerator.Instance.GenerateRandomOrder();
        if (rawOrder != null)
        {
            rawOrder.Lifespan = Random.Range(_minLifespan, _maxLifespan);
            _availableJobs.Add(rawOrder);
        }
    }

    public void TickTurn()
    {
        for (int i = _availableJobs.Count - 1; i >= 0; i--)
        {
            _availableJobs[i].Lifespan--;
            
            if (_availableJobs[i].Lifespan <= 0)
            {
                _availableJobs.RemoveAt(i);
            }
        }

        int newOrdersCount = Random.Range(3, 8); 
        for (int i = 0; i < newOrdersCount; i++)
        {
            AddNewRandomOrder();
        }

        OnBoardUpdated?.Invoke();
    }

    public void TakeJob(Order order)
    {
        DeliveryManager.Instance.AcceptOrder(order);
        OnBoardUpdated?.Invoke();
    }
    
    public void RemoveJob(Order order)
    {
        if (_availableJobs.Contains(order))
        {
            _availableJobs.Remove(order);
            OnBoardUpdated?.Invoke();
        }
    }
}