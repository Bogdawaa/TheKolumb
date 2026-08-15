using UnityEngine;
using System;
using System.Collections.Generic;

namespace DialogueSystem
{
[CreateAssetMenu(fileName = "DialogueTree_", menuName = "Dialogue/Tree")]
public class DialogueTree : ScriptableObject
{
    // nodes - все доступные варианты (в тч которые скрыты и тд)
    // [HideInInspector]
     public List<DialogueNode> nodes = new List<DialogueNode>();

    // entryNodeId - id ноды с которой начинается диалог
    // [HideInInspector] 
    public string entryNodeId;

    public DialogueNode GetNodeById(string id)
    {
        return nodes.Find(n => n.id == id);
    }
}


[System.Serializable]
public class DialogueNode
{
    public string id = Guid.NewGuid().ToString();

    [Tooltip("Id того кто произносит фразу")]
    public string characterId;

    [Tooltip("Текст кнопки, который видит игрок")]
    [TextArea] public string text;

    // public List<DialogueAction> onEnterActions = []; // содержится в DialogueChoice

    [Tooltip("ID узла, куда перейти после выбора (оставьте пустым для завершения диалога)")]
    public string nextNodeId; // для автоматических переходов

    // Тип перехода для конкретного варианта диалога
    public TransitionType transitionType = TransitionType.Automatic;

    [Tooltip("Вариант выбора - используется для ветвления")]
    public List<DialogueChoice> choices; // интерактивные выборы
}

public enum TransitionType
{
    Automatic,      // Просто идём к ID в nextNodeId
    Random,         // Выбираем случайный ID из списка
    Conditional,    // Движок проверяет условия и выбирает подходящий
    Manual          // Ждём внешнего вызова (например, из кода)
}


// Cобытия которые выполняются при входе в ноду диалога
[System.Serializable]
public class DialogueAction
{
    public ActionType actionType;
    public string targetId;
    public int amount;

    public void execute()
    {
        // TODO: добавить остальные случаи
        switch (actionType)
        {
            case ActionType.GiveQuest:
                if (string.IsNullOrEmpty(targetId))
                {
                    Debug.LogError("TargetId is null or empty!");
                    return;
                }
                // QuestSystem.ActivateQuest(targetId);
                Debug.Log($"Execute action: {actionType}, target: {targetId}");
                break;

            default:
                Debug.Log($"Execute action: {actionType}");
                break;
        }
    }
}

public enum ActionType
{
    GiveQuest
    // CompleteQuest,
    // AddReputation,
    // GiveItem
}

// Это кнопка/вариант ответа, который видит игрок и который определяет, куда пойдёт диалог дальше.
[System.Serializable]
public class DialogueChoice
{
    // TODO: - можно сделать DialogueChoice интерфейсом и далее сделать раличные реализации
    // например ChoiceWithQuestReward, ReputationChoice и тд

    public string choiceText;

    //  Куда перейти после выбора
    public string nextNodeId;

    // Что сделать при выборе
    public List<DialogueAction> onChooseActions;

}


}