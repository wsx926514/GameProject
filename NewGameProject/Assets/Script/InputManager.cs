using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public GameObject inputPanel; 
    public TextMeshProUGUI promptText;       
    public TMP_InputField nameInputField;
    public Button confirmButton;


    public static InputManager Instance { get; private set; }

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        inputPanel.SetActive(false);
    }

    void OnConfirm()
    { 
        string playerName = nameInputField.text.Trim();
        if (IsInvalidName(playerName))
        {
            return;       
        }
        PlayerData.Instance.playerName = playerName;
        inputPanel.SetActive(false);
        MenuManager.Instance.StartGame(); 
    }
    bool IsInvalidName(string name)
    {
        return string.IsNullOrEmpty(name);
    }
    public void ShowInputPanel()
    {
        confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.CONFIRM); // 确认按钮文本
        promptText.text = LM.GLV(Constants.PROMPT_TEXT);
        nameInputField.text = "";
        inputPanel.SetActive(true);


    }


}
