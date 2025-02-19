using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class MainMenu : MonoBehaviour
{
    public Button startButton; // Start按钮
    public Button quitButton;  // Quit按钮
    public Text actionText;    // 显示提示文字的UI文本组件
    private Button selectedButton = null; // 当前选中的按钮
    private Color defaultColor = Color.white; // 默认按钮颜色
    private Color selectedColor = new Color(1f, 0.5f, 0f); // 橙色 (RGB: 255, 128, 0)

    public AudioSource audioSource; // 音效播放源
    public AudioClip buttonSound;    // Start按钮音效

    private void Start()
    {
        // 禁用鼠标指针
        Cursor.visible = false;

        // 初始化按钮颜色和提示文本
        ResetButtonColors();
        UpdateActionText(null);
    }

    private void Update()
    {
        // 检测键盘输入切换选中状态
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectButton(startButton);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectButton(quitButton);
        }

        // 检测确认键触发点击事件
        if (Input.GetKeyDown(KeyCode.E) && selectedButton != null)
        {
            TriggerButtonAction(selectedButton); // 触发按钮点击事件
            audioSource.PlayOneShot(buttonSound);
        }
    }

    // 选中按钮的功能
    private void SelectButton(Button button)
    {
        // 如果当前按钮已经是选中状态，不重复设置
        if (selectedButton == button) return;

        // 恢复所有按钮的默认颜色
        ResetButtonColors();

        // 更新当前选中的按钮
        selectedButton = button;

        // 设置选中按钮的颜色
        var colors = selectedButton.colors;
        colors.normalColor = selectedColor; // 改变颜色为选中颜色
        selectedButton.colors = colors;

        // 更新提示文本
        UpdateActionText(button);
    }

    // 恢复所有按钮的默认颜色
    private void ResetButtonColors()
    {
        var startColors = startButton.colors;
        startColors.normalColor = defaultColor;
        startButton.colors = startColors;

        var quitColors = quitButton.colors;
        quitColors.normalColor = defaultColor;
        quitButton.colors = quitColors;
    }

    // 更新提示文本内容
    private void UpdateActionText(Button button)
    {
        if (button == null)
        {
            actionText.text = ""; // 清空提示
        }
        else
        {
            actionText.text = "[E] - SELECT"; // 显示提示
        }
    }

    // 触发按钮点击事件
    private void TriggerButtonAction(Button button)
    {
        if (button == startButton)
        {
            Debug.Log("Start Game Clicked!");
            StartCoroutine(LoadSceneAfterDelay("Test", 0.5f)); // 替换为你的游戏场景名称
        }
        else if (button == quitButton)
        {
            Debug.Log("Quit Game Clicked!");
            Application.Quit();

#if UNITY_EDITOR
            // 在编辑器模式下不会退出，用于调试
            Debug.Log("Application.Quit() called in Editor.");
#endif
        }
    }

    // Coroutine to load scene after a delay
    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified time
        SceneManager.LoadScene(sceneName); // Load the scene
    }
}
