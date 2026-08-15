using UnityEngine;
using DialogueSystem;

public class NPCInteractions : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public DialogueTree dialogueTree;
    
    [Header("Interaction Settings")]
    public float interactionRange = 5f;
    
    void OnMouseDown()
    {
        // Проверяем, не слишком ли далеко игрок (опционально)
        if (!IsPlayerInRange())
            return;
        
        // Запускаем диалог
        if (DialogueEngine.Instance != null && dialogueTree != null)
        {
            DialogueEngine.Instance.StartDialog(dialogueTree);
        }
        else if (DialogueEngine.Instance == null)
        {
            Debug.LogError("DialogueEngine.Instance = null! Добавьте DialogueEngine в сцену.");
        }
        else if (dialogueTree == null)
        {
            Debug.LogError("dialogueTree = null! Перетащите DialogueTree в инспектор на NPC.");
        }
    }
    
    private bool IsPlayerInRange()
    {
        // Находим игрока по тегу "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return true; // Если игрока нет, разрешаем диалог
        
        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance <= interactionRange;
    }
    
    // Визуализация радиуса взаимодействия в Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
