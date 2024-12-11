using UnityEngine;
using UnityEngine.UIElements;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation myConversation;
    
   private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F)) 
            {
                ConversationManager.Instance.StartConversation(myConversation);
            }
        }
    }
}
