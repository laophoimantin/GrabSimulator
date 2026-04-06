using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobBoardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _container;
    [SerializeField] private JobUIButton _prefab;
    [SerializeField] private Button _btnConfirmTakeJob; 

    private List<JobUIButton> _spawnedButtons = new();
    private JobUIButton _currentSelectedButton = null;

    void Awake()
    {
        for (int i = 0; i < 10; i++)
        {
            JobUIButton newBtn = Instantiate(_prefab, _container);
            newBtn.gameObject.SetActive(false);
            _spawnedButtons.Add(newBtn);
        }
    }
    private void OnEnable()
    {
        JobBoardManager.OnBoardUpdated += RefreshUI;
        _btnConfirmTakeJob.onClick.AddListener(ConfirmSelection);
        RefreshUI(); 
    }

    private void OnDisable()
    {
        JobBoardManager.OnBoardUpdated -= RefreshUI;
        _btnConfirmTakeJob.onClick.RemoveListener(ConfirmSelection);
    }

    private void RefreshUI()
    {
        List<Order> jobs = JobBoardManager.Instance.GetAllJobs();
        int maxCount = Mathf.Max(jobs.Count, _spawnedButtons.Count);

        Order activeOrder = DeliveryManager.Instance.GetCurrentOrder(); 

        for (int i = 0; i < maxCount; i++)
        {
            if (i < jobs.Count)
            {
                if (i >= _spawnedButtons.Count)
                {
                    JobUIButton newBtn = Instantiate(_prefab, _container);
                    _spawnedButtons.Add(newBtn);
                }

                _spawnedButtons[i].gameObject.SetActive(true);
                _spawnedButtons[i].Init(jobs[i], this); 

                if (activeOrder != null && jobs[i] == activeOrder)
                {
                    _spawnedButtons[i].SetSelected(true); 
                    _currentSelectedButton = _spawnedButtons[i]; 
                }
                else
                {
                    _spawnedButtons[i].SetSelected(false);
                }
            }
            else
            {
                _spawnedButtons[i].gameObject.SetActive(false);
            }
        }

        _btnConfirmTakeJob.interactable = false; 
    }

    public void SelectJob(JobUIButton selectedButton)
    {
        if (_currentSelectedButton == selectedButton) return;

        if (_currentSelectedButton != null)
        {
            _currentSelectedButton.SetSelected(false);
        }

        _currentSelectedButton = selectedButton;
        _currentSelectedButton.SetSelected(true);

        _btnConfirmTakeJob.interactable = true;
    }

    private void ConfirmSelection()
    {
        if (_currentSelectedButton == null) return;

        Order selectedOrder = _currentSelectedButton.GetOrder();
        JobBoardManager.Instance.TakeJob(selectedOrder);

        _currentSelectedButton = null;
        _btnConfirmTakeJob.interactable = false;
    }
}