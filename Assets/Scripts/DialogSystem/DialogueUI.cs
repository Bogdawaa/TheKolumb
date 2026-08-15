using UnityEngine;
using DialogueSystem;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DialogueUI : MonoBehaviour
{

    [Header("UI References")]
    public GameObject panel; // Всё окно диалога
    public Transform contentContainer; // Content из ScrollView
    public GameObject npcMessagePrefab; // Префаб для реплики NPC
    public GameObject playerChoicePrefab; // Префаб для варианта ответа (как текст с EventTrigger)


    [Header("Settings")]
    public float typingSpeed = 0.05f; // Скорость печати текста
    public Color npcTextColor = Color.white;
    public Color playerChoiceColor = new Color(0.7f, 0.7f, 0.8f);
    public Color playerChoiceHoverColor = new Color(1f, 0.7f, 0.2f);


    private DialogueEngine _engine;
    private AutoScroll _autoScroller;
    private bool _isWaitingForChoice = false;

    void Start()
    {
        _engine = DialogueEngine.Instance;
        _autoScroller = GetComponent<AutoScroll>();

        if (_engine != null)
        {
            _engine.OnNodeEntered += OnNodeEntered;
            _engine.OnDialogueEnded += OnDialogueEnded;
        }
    
        // скрыть панель
        panel.SetActive(false);

    }

    private void OnNodeEntered(DialogueNode node)
    {
        // показать панель, если скрыта
        if (!panel.activeSelf)
            panel.SetActive(true);

        // добавить текст в ленту диалога
        AddMessageText(node.text);

        // показать варианты ответов
        if (node.choices != null && node.choices.Count > 0)
        {
            ShowChoices(node.choices);
            _isWaitingForChoice = true;
        }
        else
        {
            _isWaitingForChoice = false;
        }

        // TODO: добавить переход к следующей ноде если она есть
        // callNextNode()
        
    }

    private void AddMessageText(string text)
    {
        GameObject messageObj = Instantiate(npcMessagePrefab, contentContainer);
        TextMeshProUGUI tmp = messageObj.GetComponent<TextMeshProUGUI>();

        tmp.color = npcTextColor;
        tmp.text = "";

        StartCoroutine(TypeText(tmp, text));
    }

    private void ShowChoices(List<DialogueChoice> choices)
    {
        ClearChoices();

        foreach (var choice in choices)
        {
            GameObject choiceObj = Instantiate(playerChoicePrefab, contentContainer);
            TextMeshProUGUI tmp = choiceObj.GetComponent<TextMeshProUGUI>();

            tmp.color = playerChoiceColor;
            tmp.text = choice.choiceText;

            // Добавляем EventTrigger для обработки клика и наведения
            AddChoiceEvents(choiceObj, choice);
        }

        if (_autoScroller != null) {
            _autoScroller.ScrollToBottom();
        }
    }

    private void AddChoiceEvents(GameObject choiceObj, DialogueChoice choice)
    {
        EventTrigger trigger = choiceObj.GetComponent<EventTrigger>();

        if (trigger == null)
            trigger = choiceObj.AddComponent<EventTrigger>();


        // Клик
        var clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener((data) => {
            OnChoiceSelected(choice);
        });
        trigger.triggers.Add(clickEntry);

        // Наведение (эффект подсветки как в Disco Elysium)
        var hoverEntry = new EventTrigger.Entry();
        hoverEntry.eventID = EventTriggerType.PointerEnter;
        hoverEntry.callback.AddListener((data) => {
            TextMeshProUGUI tmp = choiceObj.GetComponent<TextMeshProUGUI>();
            tmp.color = playerChoiceHoverColor;
            // TODO: Здесь можно показать тултип с проверкой навыка
        });
        trigger.triggers.Add(hoverEntry);

        // Уход мыши
        var exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => {
            TextMeshProUGUI tmp = choiceObj.GetComponent<TextMeshProUGUI>();
            tmp.color = playerChoiceColor;
        });
        trigger.triggers.Add(exitEntry);
    }

    // Создает эффект печатания текста
    private System.Collections.IEnumerator TypeText(TextMeshProUGUI tmp, string fullText)
    {
        tmp.text = "";
        foreach (char c in fullText)
        {
            tmp.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        if (!_isWaitingForChoice)
            StartCoroutine(WaitForNextNodeClick());
    }

    private System.Collections.IEnumerator WaitForNextNodeClick()
    {
        // Ждем любое нажатие клавиши или клик мыши
        while (!Input.GetMouseButtonDown(0) && !Input.anyKeyDown)
            yield return null;
        
        _engine.NextNode();
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        ClearChoices();
        _engine.MakeChoice(choice);
    }

    private void ClearChoices()
    {
        foreach (Transform child in contentContainer) {
            if (child.GetComponent<PlayerChoiceTag>() != null)
                Destroy(child.gameObject);
        }
    }

    private void OnDialogueEnded()
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        } 

        panel.SetActive(false);
        _isWaitingForChoice = false;
    }
}