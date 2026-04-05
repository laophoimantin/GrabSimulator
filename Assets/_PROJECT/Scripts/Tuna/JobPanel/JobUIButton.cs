using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JobUIButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text _txtCargo;
    [SerializeField] private TMP_Text _txtRoute;
    [SerializeField] private TMP_Text _txtReward;
    
    [Header("Toggle Visuals")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Sprite _normalSprite; 
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Button _btnClickArea;  

    private Order _myOrder;
    private JobBoardUI _myBoss;

    public void Init(Order order, JobBoardUI boss)
    {
        _myOrder = order;
        _myBoss = boss;

        _txtCargo.text = order.CargoData.CargoName;
        _txtRoute.text = $"{order.PickupLocID} -> {order.DropLocID}";
        _txtReward.text = $"{order.Reward} VNĐ";

        _btnClickArea.onClick.RemoveAllListeners();
        _btnClickArea.onClick.AddListener(OnPanelClicked);

        SetSelected(false);
    }

    private void OnPanelClicked()
    {
        _myBoss.SelectJob(this);
    }

    public void SetSelected(bool isSelected)
    {
        _backgroundImage.sprite = isSelected ? _selectedSprite : _normalSprite;
    }

    public Order GetOrder() => _myOrder;
}