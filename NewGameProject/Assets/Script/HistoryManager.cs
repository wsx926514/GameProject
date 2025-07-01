using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using JetBrains.Annotations;

public class HistoryManager: MonoBehaviour 
{
    public Transform historyContent;
    public GameObject historyItemPrefab;
    public GameObject historyScrollView;
    public Button closeButton;

    private LinkedList<VNcontorll.historyData> historyRecords;


    public static HistoryManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
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
        historyScrollView.SetActive(false);
        closeButton.onClick.AddListener(CloseHistory);
    }

    public void ShowHistory(LinkedList<VNcontorll.historyData> records)
    { 
    closeButton.GetComponentInChildren<TextMeshProUGUI>().text = LM.GLV(Constants.CLOSE);
        // Clear existing history items
        foreach (Transform child in historyContent)
        {
            Destroy(child.gameObject);
        }
        historyRecords = records;
        LinkedListNode<VNcontorll.historyData> currentNode = historyRecords.Last;
        while (currentNode != null)
        {
            var name = currentNode.Value.chineseName;
            var content = currentNode.Value.chineseContent;
            switch (MenuManager.Instance.currentLanguageIndex)
            {
                case 0:
                    break;
                case 1:
                    name = currentNode.Value.englishName;
                    content = currentNode.Value.englishContent;
                    break;
                case 2:
                    name = currentNode.Value.japaneseName;
                    content = currentNode.Value.japaneseContent;
                    break;
            }
            AddHistoryItem(name + LM.GLV(Constants.COLON) + content);
            currentNode = currentNode.Previous;

        }
        historyContent.GetComponent<RectTransform>().localPosition = Vector3.zero;
        historyScrollView.SetActive(true);
    }


    public void CloseHistory()
        {
            historyScrollView.SetActive(false);
        }

    private void AddHistoryItem(string text)
    { 
    GameObject historyItem = Instantiate(historyItemPrefab, historyContent);
      historyItem.GetComponentInChildren<TextMeshProUGUI>().text = text;
        historyItem.transform.SetAsFirstSibling();
    }

    }
