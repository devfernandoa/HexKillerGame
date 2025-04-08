using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private PowerUp powerUp;

    public void Setup(PowerUp powerUp)
    {
        this.powerUp = powerUp;
        nameText.text = powerUp.name;
        descriptionText.text = powerUp.description;
        if (iconImage != null && powerUp.icon != null)
        {
            iconImage.sprite = powerUp.icon;
        }
    }

    public void SetPosition(int index)
    {
        float[] xPositions = { 0f, 193f, -193f };
        if (index >= 0 && index < xPositions.Length)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xPositions[index], rectTransform.anchoredPosition.y);
        }
    }

    public void OnClick()
    {
        XpManager.Instance.SelectPowerUp(powerUp);
    }
}