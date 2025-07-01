using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SettingManager : MonoBehaviour 
{
    public GameObject settingPanel;
    public Toggle fullscreenToggle;
    public Text toggleLabel;
    public TMP_Dropdown resolutionDropdown;

    private Resolution[] availableResolutions;
    private Resolution defaultResolution;
    public Button defaultButton;
    public Button closeButton;


    public static SettingManager Instance { get; private set; }

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
        InitializeResolutions();
        fullscreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
        UpdateToggleLabel(fullscreenToggle.isOn);

        fullscreenToggle.onValueChanged.AddListener(SetDisplayMode);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        closeButton.onClick.AddListener(CloseSetting);
        defaultButton.onClick.AddListener(ResetSetting);

        settingPanel.SetActive(false);
    }
    public void ShowSettingPanel()
    {
        closeButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.CLOSE);
        defaultButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.RESET);
        UpdateToggleLabel(fullscreenToggle.isOn);
        settingPanel.SetActive(true);
    }
    void InitializeResolutions()
    {
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var resolutionMap = new Dictionary<string, Resolution>();
        int currentResolutionIndex = 0;

        foreach (var res in availableResolutions)
        {
            const float aspectRatio = 16f / 9f;
            const float epsilon = 0.01f;

            if (Mathf.Abs((float)res.width / res.height - aspectRatio) > epsilon)
                continue;

            string option = res.width + "x" + res.height;
            if (!resolutionMap.ContainsKey(option))
            {
                resolutionMap[option] = res;
                resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(option));
                if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = resolutionDropdown.options.Count - 1;
                    defaultResolution = res;
                }
            }
        }
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    void SetDisplayMode(bool isFullscreen)
    { 
    Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        UpdateToggleLabel(isFullscreen);
    }

    void UpdateToggleLabel(bool isFullscreen)
    {
        toggleLabel.text = isFullscreen ? LM.GLV(Constants.FULLSCREEN) : LM.GLV(Constants.WINDOWED);
    }
    void SetResolution(int index)
    {
        string[] dimensions = resolutionDropdown.options[index].text.Split('x');
        int width = int.Parse(dimensions[0].Trim());
        int height = int.Parse(dimensions[1].Trim());
        Screen.SetResolution(width, height, Screen.fullScreenMode);

    }

    public void CloseSetting()
    {
        //SaveSettings();
        settingPanel.SetActive(false);
    }

    /*void SaveSettings()
    {
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.Save();
    } */
    void ResetSetting()
    { 
    resolutionDropdown.value = resolutionDropdown.options.FindIndex(option =>
        option.text == $"{defaultResolution.width}x{defaultResolution.height}");
        fullscreenToggle.isOn = true;

    }
}
