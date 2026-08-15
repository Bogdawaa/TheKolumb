using UnityEngine;
using DialogueSystem;
using System.Collections.Generic;
using System.Linq;

public class DialogueEngine : MonoBehaviour
{
    public static DialogueEngine Instance { get; private set; }

    private DialogueTree _currentDialogue;
    private DialogueNode _currentNode;
    private Dictionary<string, DialogueNode> _nodeCache;

    // События для UI (подпишется диалоговое окно)
    public System.Action<DialogueNode> OnNodeEntered;
    public System.Action OnDialogueEnded;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialog(DialogueTree dialogueTree)
    {
        _currentDialogue = dialogueTree;
        _nodeCache = dialogueTree.nodes.ToDictionary(n => n.id, n => n);

        // начиная с первой ноды диалога
        var startNode = GetNodeById(dialogueTree.entryNodeId);
        EnterNode(startNode);
    }

    public void MakeChoice(DialogueChoice choice)
    {
        var nextNode = GetNodeById(choice.nextNodeId);

         if (nextNode != null)
            EnterNode(nextNode);
        else
            EndDialogue();
    }

    public void NextNode()
    {
        // Если есть выборы — ничего не делаем, ждём выбора игрока
        if (_currentNode.choices != null && _currentNode.choices.Count > 0)
            return;
        
        // Если есть автоматический переход
        if (_currentNode.nextNodeId != null)
        {
            string nextId = _currentNode.nextNodeId;
            var nextNode = GetNodeById(nextId);
            
            if (nextNode != null)
                EnterNode(nextNode);
            else
                EndDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    private DialogueNode GetNodeById(string id)
    {
        return _nodeCache.GetValueOrDefault(id);
    }

    private void EnterNode(DialogueNode node)
    {
        _currentNode = node;
        OnNodeEntered?.Invoke(node);
    }

    private void EndDialogue()
    {
        _currentDialogue = null;
        _currentNode = null;

        OnDialogueEnded?.Invoke();
    }
}
