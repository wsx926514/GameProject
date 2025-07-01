using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.IO;
using Unity.VisualScripting;

public class SaveLoadManager : MonoBehaviour
{
    public GameObject saveLoadPanel;
    public TextMeshProUGUI panelTitle;
    public Button[] saveLoadButtons;
    public Button prevPageButton;  
    public Button nextPageButton;
    public Button backButton;

    private bool isSave;
    private int currentPage = Constants.DEFAULT_START_INDEX;
    private readonly int slotsPerPage = Constants.SLOTS_PER_PAGE;       
    private readonly int totalSlots = Constants.TOTAL_SLOTS;
    private System.Action<int> currentAction; // �Ω�O�s�Υ[�����^�ը��
    private System.Action menuAction; // �Ω��^��檺�^�ը��

    public static SaveLoadManager Instance { get; private set; }

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



    private void Start()
    {
        prevPageButton.onClick.AddListener(PrevPage);
        nextPageButton.onClick.AddListener(NextPage);
        backButton.onClick.AddListener(GoBack);
        saveLoadPanel.SetActive(false);
    }
    public void ShowSavePanel(System.Action<int> action)
    {
        isSave = true;
        ShowPanel(action);
    }
    public void ShowLoadPanel(System.Action<int> action,System.Action menuAction)
    {
        isSave = false;
        this.menuAction = menuAction;
        ShowPanel(action);
    }
    public void ShowSaveLoadUI(bool save)
    {
        isSave = save;
        panelTitle.text = isSave ? Constants.SAVE_GAME : Constants.LOAD_GAME;
        UpdateSaveLoadUI();
        saveLoadPanel.SetActive(true);
        LoadStorylineAndScreenshot();
    }
    private void ShowPanel(System.Action<int> action)
    { 
    panelTitle.text = LM.GLV(isSave ? Constants.SAVE_GAME : Constants.LOAD_GAME);
    prevPageButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV (Constants.PREV_PAGE);
    nextPageButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.NEXT_PAGE);
    backButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.BACK);
        currentAction = action;
        UpdateUI();
        saveLoadPanel.SetActive(true);

    }
    private void UpdateUI()
    {
        for (int i = 0; i < slotsPerPage; ++i)
        {
            int slotIndex = currentPage * slotsPerPage + i;
            if (slotIndex < totalSlots)
            {
                UpdateSaveLoadButtons(saveLoadButtons[i], slotIndex);
                LoadStorylineAndScreenshot(saveLoadButtons[i], slotIndex);
            }
            else
            {
                saveLoadButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void UpdateSaveLoadButtons(Button button, int index)
    { 
        button.gameObject.SetActive(true);
        button.interactable = true; // �T�O���s�i����

        var savePath = GenerateDataPath(index);
        var fileExists = File.Exists(savePath);

        if(!isSave && !fileExists)
        {
            button.interactable = false; // �p�G�O�[���B�ɮפ��s�b�A�h�T�Ϋ��s
        }
        
        var textComponents = button.GetComponentsInChildren<TextMeshProUGUI>();
        textComponents[0].text = null;
        textComponents[1].text = (index + 1) + LM.GLV(Constants.COLON) + LM.GLV(Constants.EMPLY_SLOT);
        button.GetComponentInChildren<RawImage>().texture = null;

        button.onClick.RemoveAllListeners(); 
        button.onClick.AddListener(() =>  OnButtonClick(button, index));
    }
    private void OnButtonClick(Button button, int index)
    {
        menuAction?.Invoke(); // ��^��檺�^�ը��
        currentAction?.Invoke(index); //currentAction?.Invoke(index);
        if (isSave)
        {
          LoadStorylineAndScreenshot(button, index);
        }
        else
        {
           GoBack(); // �p�G�O�[���A�h�������O
        }

    }


    private void UpdateSaveLoadUI()
    {
        for (int i = 0; i < slotsPerPage; i++)
        {
            int slotIndex = currentPage * slotsPerPage + i;
            if (slotIndex < totalSlots)
            {
                saveLoadButtons[i].gameObject.SetActive(true);
                saveLoadButtons[i].interactable = true; 

              //��s���s��r��Ϥ�
              var slotText = (slotIndex + 1) + LM.GLV(Constants.COLON) + LM.GLV(Constants.EMPLY_SLOT);
              var textComponents = saveLoadButtons[i].GetComponentsInChildren<TextMeshProUGUI>();
              textComponents[0].text = null;
              textComponents[1].text = slotText;
              saveLoadButtons[i].GetComponentInChildren<RawImage>().texture = null;
            }
            else
            {
                saveLoadButtons[i].gameObject.SetActive(false);
            }
        }
      
    }
    private void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateSaveLoadUI();
            LoadStorylineAndScreenshot();
        }
    }
    private void NextPage()
    {
        if ((currentPage + 1) * slotsPerPage < totalSlots)
        {
            currentPage++;
            UpdateSaveLoadUI();
            LoadStorylineAndScreenshot();
        }
    }
    private void GoBack()
    {
        saveLoadPanel.SetActive(false);
    }
    private void LoadStorylineAndScreenshot(Button button, int index)
    {
        var savePath = GenerateDataPath(index);
        if (File.Exists(savePath))
        {

           string json = File.ReadAllText(savePath);
           var saveData = JsonConvert.DeserializeObject<VNcontorll.SaveData>(json);
           if(saveData.savedScreenshotData != null) 
            {
                Texture2D screenshot = new Texture2D(2, 2);
                screenshot.LoadImage(saveData.savedScreenshotData);
                button.GetComponentInChildren<RawImage>().texture = screenshot;
            }
           if(saveData.savedSpeakingContent != null) 
            {
                var textComponents = button.GetComponentsInChildren<TextMeshProUGUI>();
                textComponents[0].text = saveData.savedSpeakingContent;
                textComponents[1].text = File.GetLastWriteTime(savePath).ToString("G");
            }

        }
    }
 
    private void LoadStorylineAndScreenshot()
    {
    }

    private string GenerateDataPath(int index)
    {
        return Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH, index + Constants.SAVE_FILE_EXTENSION);
    }
}

