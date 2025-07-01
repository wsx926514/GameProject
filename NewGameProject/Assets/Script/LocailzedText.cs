using UnityEngine;
using TMPro;
public class LocailzedText :MonoBehaviour
{
    public string key;

    private TextMeshProUGUI textComponent;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();

        LocalizationManager.Instance.LanguageChanged += UpdateText;

        UpdateText();
    }

    void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.LanguageChanged -= UpdateText;
        }
    }

    private void UpdateText()
    {
        if (textComponent != null)
        {
            textComponent.text = LocalizationManager.Instance.GetLocalizedValue(key);
        }
    }
}

