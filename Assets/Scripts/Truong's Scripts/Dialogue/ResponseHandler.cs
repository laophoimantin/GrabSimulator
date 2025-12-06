using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform _responseBox;
    [SerializeField] private RectTransform _responseButtonTemplate;
    [SerializeField] private RectTransform _responseContainer;

    private DialogueUI _dialogueUI;
    private ResponseEvent[] _responseEvents;

    private List<GameObject> tempResponseButtons = new();

    void Start()
    {
        if (_dialogueUI == null)
            _dialogueUI = GetComponent<DialogueUI>();
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        _responseEvents = responseEvents;
    }

    public void ShowResponse(Response[] responses)
    {
        float resonseBoxHeight = 0;
        for (int i = 0; i < responses.Length; i++)
        {
            Response response = responses[i];
            int responseIndex = i;

            GameObject responseButton = Instantiate(_responseButtonTemplate.gameObject, _responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText;
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex));

            tempResponseButtons.Add(responseButton);

            resonseBoxHeight += _responseButtonTemplate.sizeDelta.y;
        }

        _responseBox.sizeDelta = new Vector2(_responseBox.sizeDelta.x, resonseBoxHeight);
        _responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response, int responseIndex)
    {
        _responseBox.gameObject.SetActive(false);

        foreach (GameObject responseButton in tempResponseButtons)
        {
            Destroy(responseButton);
        }

        if (_responseEvents != null && responseIndex < _responseEvents.Length)
        {
            _responseEvents[responseIndex].OnPickedResponse?.Invoke();
        }

        _responseEvents = null;

        if (response.DialogueObject != null)
        {
            _dialogueUI.ShowDialogue(response.DialogueObject);
        }
        else
        {
            _dialogueUI.CloseDialogueBox();
        }
    }
}