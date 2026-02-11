using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private TextMeshProUGUI _speakerText;
    [SerializeField] private TextMeshProUGUI _bodyText;
    [SerializeField] private float _typeSpeed = 0.04f;

    [Header("Dynamic Choices")]
    [SerializeField] private Transform _choiceContainer;
    [SerializeField] private Button _choiceButtonPrefab; 

    private List<Button> _activeButtons = new List<Button>();
    private Coroutine _typeRoutine;
    private bool _isTyping;
    
    private string _currentFullText;

    private void Start()
    {
        _panel.SetActive(false);
        
        DialogueController.Instance.OnNodeStart += UpdateUI;
        DialogueController.Instance.OnDialogueEnd += CloseUI;
    }
    
    private void OnDestroy()
    {
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.OnNodeStart -= UpdateUI;
            DialogueController.Instance.OnDialogueEnd -= CloseUI;
        }
    }

    private void Update()
    {
        // Handle Input here. 
        if (!_panel.activeSelf) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (_isTyping)
            {
                CompleteTextImmediately();
            }
            else
            {
                DialogueController.Instance.Next();
            }
        }
    }

    private void UpdateUI(DialogueNode node)
    {
        _panel.SetActive(true);
        _speakerText.text = node.SpeakerName;
        
        _currentFullText = node.Text;
        _bodyText.text = "";
        
        // Handle Text Typewriter
        if (_typeRoutine != null) StopCoroutine(_typeRoutine);
        _typeRoutine = StartCoroutine(TypeText(node.Text));

        // Handle Choices
        CreateChoiceButtons(node.Choices);
    }

    private void CreateChoiceButtons(List<DialogueChoice> choices)
    {
        // Clear old buttons
        foreach (var btn in _activeButtons) 
            Destroy(btn.gameObject);
        
        _activeButtons.Clear();

        if (choices == null || choices.Count == 0) return;

        // Create new buttons
        for (int i = 0; i < choices.Count; i++)
        {
            int index = i;
            Button btn = Instantiate(_choiceButtonPrefab, _choiceContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choices[i].ChoiceText;
            
            btn.onClick.AddListener(() => DialogueController.Instance.SelectChoice(index));
            
            _activeButtons.Add(btn);
        }
    }

    private IEnumerator TypeText(string text)
    {
        _isTyping = true;
        _bodyText.text = "";
        foreach (char c in text)
        {
            _bodyText.text += c;
            yield return new WaitForSeconds(_typeSpeed);
        }
        _isTyping = false;
    }

    private void CompleteTextImmediately()
    {
        if (_typeRoutine != null) StopCoroutine(_typeRoutine);
        _bodyText.text = _currentFullText;
        _isTyping = false;
    }

    private void CloseUI()
    {
        _panel.SetActive(false);
    }
}