using UnityEngine;


public class ScreenShotter : MonoBehaviour  
{
    /* 第一步: 獲取螢幕當前解析度尺寸 Screen.width和 Screen.height是Unity提供的屬性，功能是來獲取螢幕解析度
    這裡需要的原因是因為截圖的紋理大小需要與螢幕解析度相同，這樣才能確保截圖的清晰度和完整性。 */
    public Texture2D CaptureScreenshot()
    {
        int width = Screen.width;
        int height = Screen.height;

        /*第二步:創建渲染紋理
        創建一個臨時的RenderTexture(渲染紋理)
        參數解析:
        width和 height是螢幕的寬度和高度，等於螢幕尺寸。
        24:為深度緩衝區的位數，這裡設置為24位，表示使用24位深度緩衝區，通常足夠使用。
        RenderTexture是一種特殊紋理，用於儲存於相機的渲染輸出
        簡而言之可以理解為相機的畫布(Canvas)*/

        RenderTexture rt = RenderTexture.GetTemporary(width, height, 24);

        /*第三步:檢查主相機是否存在
        作用是確保場景中有主相機純在(Camera.main 是 Unity提供的快速獲取主相機的屬性)。
        為什麼需要檢查主相機是否存在？
        如果主相機不存在，渲染操作則無法完成
        這行代碼是通過Debug.LogError()方法輸出錯誤信息到控制台並返回null，防止程序崩潰。*/
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return null;
        }
        /*第四步:設置相機的渲染目標
        為什麼需要這些操作？
        默認情況下，Unity的相機會將渲染結果輸出到螢幕上。
        但在這裡，我們需要將渲染結果輸出到RenderTexture中，以便後續處理。
        這行代碼將主相機的渲染目標設置為剛剛創建的RenderTexture(rt)，這樣相機渲染的內容就會被輸出到這個RenderTexture中。*/
        mainCamera.targetTexture = rt; //告訴主相機將渲染結果輸出到rt(渲染紋理)中
        RenderTexture.active = rt; //將渲染紋理設置為當前活動的渲染目標，這樣後續的渲染操作就會輸出到這個RenderTexture中。
        mainCamera.Render(); //強制主相機立即渲染場景並輸出到rt

        /*第五步: 讀取像素數據
        參數解析:
        new Rect(0, 0, width, height)是用來定義一個矩形區域，這個矩形區域的左下角坐標為(0, 0)，從左下角開始，讀取整個螢幕。
        0, 0: 將像素數據寫入到目標Texture2D的初始位置
        為什麼需要Apply()方法？
        Texture2D的更改不會立即生效，必須調用Apply()後，修改才會實際更新到紋理*/
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        /*第六步:重製渲染目標並且釋放資源*/
        mainCamera.targetTexture = null; //將主相機的渲染目標設置為null，這樣主相機就會恢復到默認的渲染行為，即將渲染結果輸出到螢幕上。
        RenderTexture.active = null; //清除當前啟動的渲染紋理，這樣可以避免後續的渲染操作影響到這個RenderTexture。
        RenderTexture.ReleaseTemporary(rt); //釋放之前創建的RenderTexture資源，這樣可以避免記憶體泄漏。

        /*第七步: 縮小截圖
         調用ResizeTexture 方法，將截圖縮小到原來的1/6尺寸
        縮小的理由是，縮小截圖可以減少圖片的大小，節省存儲空間和傳輸帶寬。
        通常用於保淳遊戲純黨的縮略圖或者發送網路請求*/
        Texture2D resizedScreenshot = ResizeTexture(screenShot, width / 6, height / 6);

        /*第八步: 銷毀原始截圖，釋放記憶體*/
        Destroy(screenShot);
        return resizedScreenshot;
    }

    /*輔助方法:將輸入的Texture2D縮小到指定的寬度和高度
     返回值: 返回縮小後的Texture2D對象*/
    private Texture2D ResizeTexture(Texture2D original, int newWidth, int newHeight)
    {
        /*第一步:創建渲染紋理
        創建一個與目標解析度相同的渲染紋理，並啟動它*/
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight, 24);
        RenderTexture.active = rt;

        /*第二步:使用GPU縮放
         * 使用GPU的Graphics.Blit將original的像素數據複製並縮放到rt
        為什麼使用GPU?
        GPU操作比手動逐像素縮放更高效，適合即時操作*/
        Graphics.Blit(original, rt);

        /*第三步:讀取縮放後的數據
        讀取RederTexture的內容到新的Texture2D對象中，與截圖的邏輯類似*/
        Texture2D resized = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
        resized.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resized.Apply();

        /*第四步:清理資源
        清理臨時的RenderTexture資源，避免記憶體泄漏*/
        RenderTexture.active = null; //清除當前活動的渲染紋理
        RenderTexture.ReleaseTemporary(rt); //釋放之前創建的RenderTexture資源

        return resized; //返回縮小後的Texture2D對象
    }







}
