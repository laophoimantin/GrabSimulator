using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [Header("Scripts")]
    [SerializeField] private DialogueSoundController _dialogueSoundController;

    [Header("F#cking around with dialogue pitch")]
    [Range(1, 5)][SerializeField] private float dialoguePitchValue;

    private List<Button> _activeButtons = new List<Button>();
    private Coroutine _typeRoutine;
    private bool _isTyping;
    
    private string _currentFullText;
    
    
    private InputAction _skipDialogue;

    private void Start()
    {
        _panel.SetActive(false);
        
        DialogueController.Instance.OnNodeStart += UpdateUI;
        DialogueController.Instance.OnDialogueEnd += CloseUI;
        
        if (InputManager.Instance != null && InputManager.Instance.InputActions != null)
        {
            _skipDialogue = InputManager.Instance.InputActions.Dialogue.SkipDialogue;
            _skipDialogue.performed += OnAdvanceInput;
        }
    }
    
    private void OnDestroy()
    {
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.OnNodeStart -= UpdateUI;
            DialogueController.Instance.OnDialogueEnd -= CloseUI;
        }
        
        if (_skipDialogue != null)
        {
            _skipDialogue.performed -= OnAdvanceInput;
        }
    }
    
    private void OnAdvanceInput(InputAction.CallbackContext ctx)
    {
        if (!_panel.activeSelf) return;

        if (_isTyping)
        {
            CompleteTextImmediately();
        }
        else
        {
            DialogueController.Instance.Next();
        }
    }

    // private void Update()
    // {
    //     // Handle Input here. 
    //     if (!_panel.activeSelf) return;
    //
    //     if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
    //     {
    //         if (_isTyping)
    //         {
    //             CompleteTextImmediately();
    //         }
    //         else
    //         {
    //             DialogueController.Instance.Next();
    //         }
    //     }
    // }
    //
    //
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
            
            _dialogueSoundController.DialogueSoundGenerator(c, dialoguePitchValue, 0.5f);

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