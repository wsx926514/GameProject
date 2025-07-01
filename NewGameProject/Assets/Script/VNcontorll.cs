using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text; 
using System.IO;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using System.ComponentModel;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using System;



public class VNcontorll : MonoBehaviour
{
    #region Variables
    public GameObject gamePanel; // Panel for the game UI
    public GameObject dialogueBox; // Panel for dialogue UI
    public TextMeshProUGUI speakerName;
    //public TextMeshProUGUI speakingContent;
    public TypewriterEffect typerwriterEffect;
    public ScreenShotter screenShotter;


    public Image avatarImage;
    public AudioSource vocalAudio;
    public Image backgroundImage;
    public AudioSource backgroundMusic;
    public Image characterImage1;
    public Image characterImage2;

    public GameObject choicePanel; // Optional: Panel for choices
    public Button choiceButton1; // Optional: Button for choices        
    public Button choiceButton2; // Optional: Button for choices


    public GameObject bottomButtons; // Optional: Panel for bottom buttons  
    public Button autoButton; // Optional: Button for auto mode
    public Button skipButton; // Optional: Button for next line
    public Button saveButton; // Optional: Button for save
    public Button loadButton; // Optional: Button for load
    public Button historyButton; // Optional: Button for history
    public Button settingsButton; // Optional: Button for settings
    public Button homeButton; // Optional: Button for home
    public Button closeButton; // Optional: Button for close

    public class historyData
    {
        public string chineseName;
        public string chineseContent;
        public string englishName;
        public string englishContent;
        public string japaneseName;
        public string japaneseContent;

    }


    
    private readonly string defaultStoryFileName = Constants.DEFAULT_STORY_FILE_NAME;
    private readonly int defaultStartLine = Constants.DEFAULT_START_LINE;
    private readonly string excelFileExtension = Constants.EXCEL_FILE_EXTENSION;

    private string saveFolderPath;
    private byte[] screenshotData; //儲存螢幕截圖的資料
    private string currentSpeakingContent; //儲存目前正在說的內容

    private List<ExcelReader.ExcelData> storyData;
    private int currentLine;
    private string currentStoryFileName;
    private float currentTypingSpeed = Constants.DEFAULT_TYPING_SPEED; // Default typing speed  
   
    private bool isAutoPlay = false; // Flag for auto play mode
    private bool isSkip = false; // Flag for skip mode
    private bool isLoad = false; // Flag for load mode
    private int maxReachedLineIndex = 0; // Track the maximum reached line index
    private Dictionary<string, int> globalMaxReachedLineIndices = new Dictionary<string, int>(); //儲存每個故事檔案的最大行索引
    private LinkedList<historyData> historyRecords; //儲存歷史紀錄
    public HashSet<string> unlockedBackgrounds = new HashSet<string>(); //儲存已解鎖的背景圖片
    public static VNcontorll Instance { get; private set; }
    #endregion
    #region Lifecycle 
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
        InitializeSaveFilePath();
        bottomButtonsAddListener();
        Debug.Log(Application.persistentDataPath);
    }
    void Update()
    {
        if (!MenuManager.Instance.menuPanel.activeSelf &&
            !SaveLoadManager.Instance.saveLoadPanel.activeSelf &&
            !HistoryManager.Instance.historyScrollView.activeSelf &&
            !SettingManager.Instance.settingPanel.activeSelf &&
            gamePanel.activeSelf)
        {
            if(Input.GetMouseButtonDown(0)|| Input.GetKeyDown(KeyCode.Space))
            { 
            if (!dialogueBox.activeSelf)
            {
                OpenUI();
            }
            else if (!IsHittingBottomButtons())
            {  
                    DisplayNextLine();
            }
           }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (dialogueBox.activeSelf)
                {
                    CloseUI();
                }
                else
                {
                    OpenUI();
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                Debug.Log("按下Ctrl");
                CtrlSkip();
            }
        }
    }
    #endregion
    #region Intialization
    void InitializeSaveFilePath()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, Constants.SAVE_FILE_PATH);
        if(!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath); // Create the save folder if it doesn't exist
        }
    }
    void bottomButtonsAddListener()
    {
        skipButton.onClick.AddListener(OnSkipButtonClick);
        autoButton.onClick.AddListener(OnAutoButtonClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        loadButton.onClick.AddListener(OnLoadButtonClick);
        historyButton.onClick.AddListener(OnHistoryButtonClick);
        settingsButton.onClick.AddListener(OnSettingButtonClick);
        homeButton.onClick.AddListener(OnHomeButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }
    public void StartGame(string fileName , int lineNumber)
    {
        InitializeAndLoadStory(defaultStoryFileName, defaultStartLine);
    }
    void InitializeAndLoadStory(string fileName,int lineNumber)
    {
        Initialize(lineNumber);
        LoadStoryFromFile(fileName);
        if (isLoad)
        {
            RecoverLastBackgroundAndCharacter();
            isLoad = false;
        }
        DisplayThisLine();
    }
    void Initialize(int lineNumber)
    {
        currentLine = lineNumber;

        backgroundImage.gameObject.SetActive(false);
        backgroundMusic.gameObject.SetActive(false);
        
        avatarImage.gameObject.SetActive(false);
        vocalAudio.gameObject.SetActive(false);

        characterImage1.gameObject.SetActive(false);
        characterImage2.gameObject.SetActive(false);

        choicePanel.SetActive(false);
        historyRecords = new LinkedList<historyData>();
    }
    void LoadStoryFromFile(string fileName)
    {
        currentStoryFileName = fileName;
        string filePath =Path.Combine(Application.streamingAssetsPath,
                                      Constants.STORY_PATH,
                                      fileName + excelFileExtension);
        storyData = ExcelReader.ReadExcel(filePath);
        if (storyData == null || storyData.Count == 0)
        {
            Debug.LogError(Constants.NO_DATA_FOUND);
        }
        if (globalMaxReachedLineIndices.ContainsKey(currentStoryFileName))
        {
            maxReachedLineIndex = globalMaxReachedLineIndices[currentStoryFileName];
        }
        else
        {
            maxReachedLineIndex = 0; // Initialize to 0 if not found
            globalMaxReachedLineIndices[currentStoryFileName] = maxReachedLineIndex;
        }
    }

    public void ReloadStoryLine()
    {
        historyRecords.RemoveLast();
        currentLine--;
        DisplayNextLine();
    }
    #endregion
    #region Display
    private void DisplayNextLine()
    {
        if (currentLine > maxReachedLineIndex)
        {
            maxReachedLineIndex = currentLine;
            globalMaxReachedLineIndices[currentStoryFileName] = maxReachedLineIndex;
        }
        // 先檢查特殊指令
        if (storyData[currentLine].speakerName == Constants.CHOICE)
        {
            ShowChoices();
            return;
        }
        if (storyData[currentLine].speakerName == Constants.GOTO)
        {
            InitializeAndLoadStory(storyData[currentLine].speakingContent, defaultStartLine);
            return;
        }
        // 處理故事結尾
        if (currentLine >= storyData.Count - 1)
        {
            if (isAutoPlay)
            {
                isAutoPlay = false;
                UpdateButtonImage(Constants.AUTO_OFF, autoButton);
            }
            if (storyData[currentLine].speakerName == Constants.END_OF_STORY)
            {
                Debug.Log(Constants.END_OF_STORY);
            }
            return;
        }
        // 顯示普通對話行
        if (typerwriterEffect.IsTyping())
        {
            typerwriterEffect.CompleteLine();
        }
        else
        {
            DisplayThisLine();
        }
    }


    void DisplayThisLine()
    {
        var data = storyData[currentLine];

        string playerName = PlayerData.Instance.playerName; // Get the player's name from PlayerData
        string chineseName = data.speakerName.Replace(Constants.NAME_PLACEHOLDER, playerName);
        string chineseContent = data.speakingContent.Replace(Constants.NAME_PLACEHOLDER, playerName);
        string englishName = data.englishName.Replace(Constants.NAME_PLACEHOLDER, playerName);
        string englishContent = data.englishContent.Replace(Constants.NAME_PLACEHOLDER, playerName);
        string japaneseName = data.japaneseName.Replace(Constants.NAME_PLACEHOLDER, playerName);
        string japaneseContent = data.japaneseContent.Replace(Constants.NAME_PLACEHOLDER, playerName);

        switch(MenuManager.Instance.currentLanguageIndex)
        {
            case 0: // Chinese
                speakerName.text = chineseName;
                currentSpeakingContent = chineseContent;
                break;
            case 1: // English
                speakerName.text = englishName;
                currentSpeakingContent = englishContent;
                break;
            case 2: // Japanese
                speakerName.text = japaneseName;
                currentSpeakingContent = japaneseContent;
                break;
            default:
                speakerName.text = chineseName; // Default to Chinese if index is out of range
                currentSpeakingContent = chineseContent;
                break;
        }
        /*string speaker = data.speakerName.Replace(Constants.NAME_PLACEHOLDER,playerName);
        string content = data.speakingContent.Replace(Constants.NAME_PLACEHOLDER, playerName);
        speakerName.text = speaker;
        currentSpeakingContent = content;*/
        //speakerName.text = data.speakerName;
        //currentSpeakingContent = data.speakingContent;
        typerwriterEffect.StartTyping(currentSpeakingContent,currentTypingSpeed);

        RecordHistory(chineseName, chineseContent,
                      englishName, englishContent,
                      japaneseName, japaneseContent);

        if (NotNNullNorEmpty(data.avatarImageFileName))
        {
            UpdateAvatarImage(data.avatarImageFileName);
        }
        else
        {
            avatarImage.gameObject.SetActive(false); // Hide the avatar image if no file name is provided   
        }
        if (NotNNullNorEmpty(data.vocalAudioName))
        {
            PlayVocalAudio(data.vocalAudioName);
        }
        if (NotNNullNorEmpty(data.backgroundImageFileName))
        {
            UpdateBackgroundImage(data.backgroundImageFileName);
        }
        if (NotNNullNorEmpty(data.backgroundMusicFileName))
        {
            PlayBackgroundMusic(data.backgroundMusicFileName);
        }
        if (NotNNullNorEmpty(data.character1Action))
        {
            UpdateCharacterImage(data.character1Action, data.character1ImageFileName,
                characterImage1, data.coordinateX1);
        }
        if (NotNNullNorEmpty(data.character2Action))
        {
            UpdateCharacterImage(data.character2Action, data.character2ImageFileName,
                characterImage2, data.coordinateX2);
        }
        currentLine++;
    }

    void RecordHistory(string chineseName,string chineseContent,
                       string englishName,string englishContent,
                       string japaneseName,string japaneseContent)
    {
       var historyRecord = new historyData
       {
           chineseName = chineseName,
           chineseContent = chineseContent,
           englishName = englishName,
           englishContent = englishContent,
           japaneseName = japaneseName,
           japaneseContent = japaneseContent
       };
        if (historyRecords.Count >= Constants.MAX_LENGTH)
        {
            historyRecords.RemoveFirst(); // Remove the oldest record if the limit is exceeded
        }
        historyRecords.AddLast(historyRecord); // Add the new record to the history
    }


    void RecoverLastBackgroundAndCharacter()
    { 
        var data = storyData[currentLine];
        if(NotNNullNorEmpty(data.lastBackgroundImage))
        {
            UpdateBackgroundImage(data.lastBackgroundImage);
        }
        if (NotNNullNorEmpty(data.lastBackgroundMusic))
        {
            PlayBackgroundMusic(data.lastBackgroundMusic);
        }
        if(data.character1Action != Constants.APPEAR_AT &&
           NotNNullNorEmpty(data.lastCoordinateX1))
        {
            UpdateCharacterImage(Constants.APPEAR_AT, data.character1ImageFileName,
                characterImage1, data.lastCoordinateX1);
        }
        if (data.character2Action != Constants.APPEAR_AT &&
           NotNNullNorEmpty(data.lastCoordinateX2))
        {
            UpdateCharacterImage(Constants.APPEAR_AT, data.character2ImageFileName,
                characterImage2, data.lastCoordinateX2);
        }
  }
    bool NotNNullNorEmpty(string str)
    {
        return !string.IsNullOrEmpty(str);

    }
    #endregion
    #region Choices
    void ShowChoices()
    {
        var data = storyData[currentLine];
        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();
        choicePanel.SetActive(true); // Show the choice panel
        choiceButton1.GetComponentInChildren<TextMeshProUGUI>().text = data.speakingContent;
        choiceButton1.onClick.AddListener(() => InitializeAndLoadStory(data.avatarImageFileName,defaultStartLine));
        choiceButton2.GetComponentInChildren<TextMeshProUGUI>().text = data.vocalAudioName;
        choiceButton2.onClick.AddListener(() => InitializeAndLoadStory(data.backgroundImageFileName,defaultStartLine));

    }
    #endregion
    #region Audios
    void PlayVocalAudio(string audioFileName)
    {
        string audioPath = Constants.VOCAL_PATH + audioFileName;
        PlayAudio(audioPath, vocalAudio, false);
    }
   
    void PlayBackgroundMusic(string musicFileName)
    {
        string musicPath = Constants.MUSIC_PATH + musicFileName;
        PlayAudio(musicPath, backgroundMusic, true);
    }

    void PlayAudio(string audioPath, AudioSource audioSource, bool isloop)
    {
        AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.gameObject.SetActive(true);
            audioSource.loop = isloop;
            audioSource.Play();
        }
        else
        {
            if (audioSource == vocalAudio)
            {
                Debug.LogError(Constants.AUDIO_LOAD_FAILED + audioPath);
            }
            else if (audioSource == backgroundMusic)
            {
                Debug.LogError(Constants.AUDIO_LOAD_FAILED + audioPath);
            }
        }
    }
    #endregion
    #region Images
    void UpdateBackgroundImage(string imagFileName)
    {
        string imagePath = Constants.BACKGROUND_PATH + imagFileName;
        UpdateImage(imagePath, backgroundImage);
        //紀錄背景圖片已出現
        if (!unlockedBackgrounds.Contains(imagFileName))
        {
            unlockedBackgrounds.Add(imagFileName); // Add the background image to the unlocked set
        }
    }
    void UpdateAvatarImage(string imageFileName)
    {
        var imagePath = Constants.AVATAR_PATH + imageFileName;
        UpdateImage(imagePath, avatarImage);
    }
    void UpdateCharacterImage(string action, string imgeFileName, Image characterImage, string x)
    {
        if (action.StartsWith(Constants.APPEAR_AT)) //解析立繪出現位置及動作在的(x.y) 
        {
            string imagePath = Constants.CHARACTER_PATH + imgeFileName;
            if (NotNNullNorEmpty(x))
            {
                UpdateImage(imagePath, characterImage);
                var newPosition = new Vector2(float.Parse(x), characterImage.rectTransform.anchoredPosition.y);
                characterImage.rectTransform.anchoredPosition = newPosition; // Set the position of the character image
                characterImage.DOFade(1,((isLoad || action == Constants.APPEAR_AT_INSTANTLY)? 0 :Constants.DURATION_TIME)).From(0);
            }
            else
            {
                Debug.LogError(Constants.COORDINATE_MISSING);

            }
        }
        else if (action == Constants.DISAPPEAR)
        {
            characterImage.DOFade(0, Constants.DURATION_TIME).OnComplete(() => characterImage.gameObject.SetActive(false)); // Hide the character image
        }
        else if (action.StartsWith(Constants.MOVE_TO))
        {
            if (NotNNullNorEmpty(x))
            {
                characterImage.rectTransform.DOAnchorPosX(float.Parse(x), Constants.DURATION_TIME);

            }
            else
            {
                Debug.LogError(Constants.COORDINATE_MISSING);
            }
        }
    }

    void UpdateImage(string imagePath, Image image)
    {
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null)
        {
            image.sprite = sprite;
            image.gameObject.SetActive(true); // Show the character image
        }
        else
        {
            Debug.LogError(Constants.IMAGE_LOAD_FAILED + imagePath);

        }
    }

    void UpdateButtonImage(string imageFileName, Button button)
    {
        string imagePath = Constants.BUTTON_PATH + imageFileName;

        UpdateImage(imagePath, button.image);
    }
    #endregion
    #region Buttons
    #region Buttom
    bool IsHittingBottomButtons()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
              bottomButtons.GetComponent<RectTransform>(),
              Input.mousePosition,
              Camera.main
              );
    }
    #endregion
    #region Auto
    void OnAutoButtonClick()
    {
        isAutoPlay = !isAutoPlay;
        UpdateButtonImage((isAutoPlay ? Constants.AUTO_OFF : Constants.AUTO_ON), autoButton);
        if (isAutoPlay)
        {
            StartCoroutine(StartAutoPlay());
        }
    }

    private IEnumerator StartAutoPlay()
    {
        while (isAutoPlay)
        {
            if (!typerwriterEffect.IsTyping())
            {
                DisplayNextLine();
            }
            yield return new WaitForSeconds(Constants.DEFAULT_AUTO_WAITING_SECONDS);
        }
    }
    #endregion
    #region Skip
    void OnSkipButtonClick()
    {
        if (!isSkip && CanSkip())
        {
            StartSkip();
        }
        else if (isSkip)
        {
            StopCoroutine(SkipToMaxReachedLine());
            EndSkip();
        }
    }
    bool CanSkip()
    {
        return currentLine < maxReachedLineIndex;
    }
    void StartSkip()
    {
        isSkip = true;
        UpdateButtonImage(Constants.SKIP_OFF, skipButton);
        currentTypingSpeed = Constants.SKIP_MODE_TYPING_SPEED;
        StartCoroutine(SkipToMaxReachedLine());
    }

    private IEnumerator SkipToMaxReachedLine()
    {
        while (isSkip)
        {
            if (CanSkip())
            {
                DisplayThisLine();
            }
            else
            {
                EndSkip();
            }
            yield return new WaitForSeconds(Constants.DEFAULT_SKIP_WAITING_SECONDS);

        }
    }
    void EndSkip()
    {
        isSkip = false;
       currentTypingSpeed = Constants.DEFAULT_TYPING_SPEED; // Reset typing speed
        UpdateButtonImage(Constants.SKIP_ON, skipButton);
    }
    void CtrlSkip()
    { 
     currentTypingSpeed = Constants.SKIP_MODE_TYPING_SPEED; // Set typing speed to skip mode
        StartCoroutine(SkipWhilePressingCtrl());
    }

    private IEnumerator SkipWhilePressingCtrl()
    {
        while (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
                DisplayNextLine();
            yield return new WaitForSeconds(Constants.DEFAULT_SKIP_WAITING_SECONDS); // Wait for the next frame
        }
    }
    #endregion
    #region Save
    void OnSaveButtonClick()
    {
        CloseUI();
        Texture2D screenshot = screenShotter.CaptureScreenshot();
        screenshotData = screenshot.EncodeToPNG(); // Convert screenshot to PNG format
        SaveLoadManager.Instance.ShowSavePanel(SaveGame);
        OpenUI();// Show save UI
    }

    void SaveGame(int slotIndex)
    {
        var saveData = new SaveData
        {
            savedStoryFileName = currentStoryFileName,
            savedLine = currentLine,
            savedSpeakingContent =currentSpeakingContent,
            savedScreenshotData = screenshotData,
            savedHistoryRecords = historyRecords,
            savedPlayerName = PlayerData.Instance.playerName // Get the player's name from PlayerData
        };
        string savePath = Path.Combine(saveFolderPath, slotIndex + Constants.SAVE_FILE_EXTENSION);
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(savePath, json);
    }
    public class SaveData
    {
        public string savedStoryFileName; //儲存的故事檔案名稱
        public int savedLine; //儲存的行號
        public string savedSpeakingContent; //儲存的說話內容
        public byte[] savedScreenshotData; //儲存的螢幕截圖資料
        public LinkedList<historyData> savedHistoryRecords; //儲存的歷史紀錄
        public string savedPlayerName; //儲存的玩家名稱
    }
    #endregion
    #region Load
    void OnLoadButtonClick()
    {
        ShowLoadPanel(null);
    }
    public void ShowLoadPanel(Action action)
    {
        SaveLoadManager.Instance.ShowLoadPanel(LoadGame, action);
    }
    void LoadGame(int slotIndex)
    {
        string savePath = Path.Combine(saveFolderPath, slotIndex + Constants.SAVE_FILE_EXTENSION);
        if (File.Exists(savePath))
        {
            isLoad = true; // Set the load flag to true
            string json = File.ReadAllText(savePath);
            var saveData = JsonConvert.DeserializeObject<SaveData>(json);
            historyRecords = saveData.savedHistoryRecords;
            historyRecords.RemoveLast();

            PlayerData.Instance.playerName = saveData.savedPlayerName; // Load the player's name from PlayerData

            var lineNumber = saveData.savedLine - 1; //因為DISPLAY_NEXT_LINE會增加1
            InitializeAndLoadStory(saveData.savedStoryFileName, lineNumber);
        }

    }
    #endregion
    #region History
    void OnHistoryButtonClick()
    {
        HistoryManager.Instance.ShowHistory(historyRecords);
    }
    #endregion
    #region Settings
    void OnSettingButtonClick()
    {
        SettingManager.Instance.ShowSettingPanel();
    }
    #endregion

    #region Home
    void OnHomeButtonClick()
    {
    gamePanel.SetActive(false); // Hide the game panel
        MenuManager.Instance.menuPanel.SetActive(true); // Show the menu panel
        }
    #endregion
    #region Close
    void OnCloseButtonClick()
{
        CloseUI();
}
    void OpenUI()
{
    dialogueBox.SetActive(true);
    bottomButtons.SetActive(true);

}
void CloseUI()
{
    dialogueBox.SetActive(false);
    bottomButtons.SetActive(false);
}
    #endregion
    #endregion
}


