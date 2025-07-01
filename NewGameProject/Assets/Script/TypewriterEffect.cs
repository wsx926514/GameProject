using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textDisplay; //用來顯示打字動畫文字的 TextMeshPro 元件

    private float waitingSeconds; //// 每個字元顯示的間隔秒數，控制打字速度
    private Coroutine typingCoroutine; // 用來儲存打字動畫的 Coroutine，方便控制打字過程
    private bool isTyping; // 用來標記是否正在打字

    public void StartTyping(string text, float speed) // 初始化打字動畫
    {
        waitingSeconds = speed; // 每個字元顯示的間隔秒數
        if (typingCoroutine != null) // 如果已經有打字動畫在進行，則停止它
        {
            StopCoroutine(typingCoroutine);
        }
    
        typingCoroutine = StartCoroutine(TypeLine(text)); // 開始新的打字動畫
    }

    private IEnumerator TypeLine(string text) // 開始打字動畫的協程
    {
        isTyping = true; // 標記為正在打字
        textDisplay.text = text; // 設定要顯示的文字
        textDisplay.maxVisibleCharacters = 0; // 初始化時不顯示任何字元

        for (int i = 0; i <= text.Length; i++) // 循環每個字元
                                              // 這裡的 i 代表目前要顯示的字元數量
                                              // 每次循環都會增加一個字元到可見字元數量中
        {
            textDisplay.maxVisibleCharacters = i ; // 顯示目前的字元數量
            yield return new WaitForSeconds(waitingSeconds);  // 等待指定的時間
        }
        isTyping = false; 
    }
    public void CompleteLine() // 完成打字動畫，顯示所有字元
    {
        if (typingCoroutine != null) // 如果有打字動畫正在進行，則停止它
        {
            StopCoroutine(typingCoroutine);  

        }

        textDisplay.maxVisibleCharacters = textDisplay.text.Length; // 顯示所有字元
        isTyping = false; 
    }
    public bool IsTyping() => isTyping; // 檢查是否正在打字
}