using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button))]
public class TextButtonColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI buttonText;
    public Color normalColor = Color.white;
    public Color highlightedColor = Color.yellow;
    public Color pressedColor = Color.red;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
            buttonText.color = highlightedColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonText.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonText.color = pressedColor;
    }

    public void ChangeButtonColor(bool isSelected)
    {
        buttonText.color = isSelected ? highlightedColor : normalColor;
    }
}
