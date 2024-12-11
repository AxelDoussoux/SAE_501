using UnityEngine;
using UnityEngine.UIElements;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation myConversation;
    private Label dialogueText;
    private VisualElement optionsContainer;

    private void Start()
    {
        var uiDocument = FindObjectOfType<UIDocument>();
        var root = uiDocument.rootVisualElement;

        dialogueText = root.Q<Label>("DialogueText");
        optionsContainer = root.Q<VisualElement>("OptionsContainer");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                StartDialogue();
            }
        }
    }

    private void StartDialogue()
    {
        ConversationManager.Instance.StartConversation(myConversation);

        // Example to update UI Toolkit dynamically
        dialogueText.text = "This is a sample dialogue";
        optionsContainer.Clear();

        // Adding options dynamically
        var option1 = new Button(() => OnOptionSelected(1)) { text = "Option 1" };
        var option2 = new Button(() => OnOptionSelected(2)) { text = "Option 2" };
        optionsContainer.Add(option1);
        optionsContainer.Add(option2);
    }

    private void OnOptionSelected(int optionIndex)
    {
        Debug.Log($"Option {optionIndex} selected.");
        // Implement logic for option handling here
    }
}
