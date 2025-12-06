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
        private List<GameObject> tempResponseButtons = new();

        void Start()
        {
            if (_dialogueUI == null)
                _dialogueUI = GetComponent<DialogueUI>();
        }
        
        public void ShowResponse(Response[] responses)
        {
            float resonseBoxHeight = 0;
            foreach (Response response in responses)
            {
                GameObject responseButton = Instantiate(_responseButtonTemplate.gameObject, _responseContainer);
                responseButton.gameObject.SetActive(true);
                responseButton.GetComponent<TMP_Text>().text = response.ResponseText;
                responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response));
                
                tempResponseButtons.Add(responseButton);
                
                resonseBoxHeight += _responseButtonTemplate.sizeDelta.y;
            }
            
            _responseBox.sizeDelta = new Vector2(_responseBox.sizeDelta.x, resonseBoxHeight);
            _responseBox.gameObject.SetActive(true);
        }

        private void OnPickedResponse(Response response)
        {
            _responseBox.gameObject.SetActive(false);

            foreach (GameObject responseButton in tempResponseButtons)
            {
                Destroy(responseButton);
            }
            _dialogueUI.ShowDialogue(response.DialogueObject);
        }
  
    }
