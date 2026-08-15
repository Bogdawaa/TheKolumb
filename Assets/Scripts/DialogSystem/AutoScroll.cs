using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    private ScrollRect scrollRect;
    
    void Start()
    {
        if (scrollRect == null)
        {
            scrollRect = GetComponent<ScrollRect>();

            if (scrollRect == null)
                Debug.LogError("AutoScrollToBottom: Нет компонента ScrollRect на объекте " + gameObject.name);
        }
    }

    public void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
