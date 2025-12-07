using UnityEngine;


[CreateAssetMenu(menuName = "Dialogue/Dialogue Object")]
public class DialogueObject : ScriptableObject
{
    [SerializeField, TextArea] private string[] _dialogue;
    [SerializeField] private Response[] _responses;
    public string[] Dialogue => _dialogue;
    
    public bool HasResponses => _responses != null &&  _responses.Length > 0;
    public Response[] Responses => _responses;
    
    
}

