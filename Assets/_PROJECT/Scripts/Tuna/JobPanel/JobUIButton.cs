using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class JobUIButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text _txtCargo;
    [SerializeField] private TMP_Text _txtFrom;
    [SerializeField] private TMP_Text _txtTo;
    [SerializeField] private TMP_Text _txtReward;
    
    [Header("Toggle Visuals")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Color _normalColor = Color.white; 
    [SerializeField] private Color _selectedColor = Color.green;
    [SerializeField] private Button _button;  

    private Order _myOrder;
    private JobBoardUI _myBoss;

    public void Init(Order order, JobBoardUI boss)
    {
        _myOrder = order;
        _myBoss = boss;

        _txtFrom.text = $"{order.PickupLocID}";
        _txtTo.text = $"{order.DropLocID}";

        _txtCargo.text = $"{order.CargoData.CargoName}";
        _txtReward.text = $"{order.Reward.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"))} VND";

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnPanelClicked);

        SetSelected(false);
    }

    private void OnPanelClicked()
    {
        _myBoss.SelectJob(this);
    }

    public void SetSelected(bool isSelected)
    {
        _backgroundImage.color = isSelected ? _selectedColor : _normalColor;
    }

    public Order GetOrder() => _myOrder;
}