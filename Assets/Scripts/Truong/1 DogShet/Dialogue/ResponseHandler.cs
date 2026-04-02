using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform _responseBox;
    [SerializeField] private RectTransform _responseButtonTemplate;
    [SerializeField] private RectTransform _responseContainer;

    private DialogueUIOld _dialogueUI;
    private ResponseEvent[] _responseEvents;

    private readonly List<GameObject> _tempResponseButtons = new();

    void Start()
    {
        if (_dialogueUI == null)
            _dialogueUI = GetComponent<DialogueUIOld>();
    }

    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        _responseEvents = responseEvents;
        foreach (ResponseEvent responseEvent in responseEvents)
        {
            Debug.Log(responseEvent.Name);
        }
    }

    public void ShowResponse(Response[] responses)
    {
        float responseBoxHeight = 0;
        for (int i = 0; i < responses.Length; i++)
        {
            Response response = responses[i];
            int responseIndex = i;

            GameObject responseButton = Instantiate(_responseButtonTemplate.gameObject, _responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText;
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex));

            _tempResponseButtons.Add(responseButton);

            responseBoxHeight += _responseButtonTemplate.sizeDelta.y;
        }

        _responseBox.sizeDelta = new Vector2(_responseBox.sizeDelta.x, responseBoxHeight);
        _responseBox.gameObject.SetActive(true);
    }

    private void OnPickedResponse(Response response, int responseIndex)
    {
        _responseBox.gameObject.SetActive(false);

        foreach (GameObject responseButton in _tempResponseButtons)
        {
            Destroy(responseButton);
        }
        _tempResponseButtons.Clear();

        if (_responseEvents != null && responseIndex < _responseEvents.Length)
        {
            _responseEvents[responseIndex].OnPickedResponse?.Invoke();
            Debug.Log("Response picked");
            Debug.Log(_responseEvents[responseIndex].Name);
        }
        _responseEvents = null;

        if (response.DialogueObject != null)
        {
            _dialogueUI.StepToNode(response.DialogueObject);
        }
        else
        {
            _dialogueUI.CloseDialogueBox();
        }
    }
}