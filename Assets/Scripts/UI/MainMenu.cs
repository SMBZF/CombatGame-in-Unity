using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class MainMenu : MonoBehaviour
{
    public Button startButton; // Start��ť
    public Button quitButton;  // Quit��ť
    public Text actionText;    // ��ʾ��ʾ���ֵ�UI�ı����
    private Button selectedButton = null; // ��ǰѡ�еİ�ť
    private Color defaultColor = Color.white; // Ĭ�ϰ�ť��ɫ
    private Color selectedColor = new Color(1f, 0.5f, 0f); // ��ɫ (RGB: 255, 128, 0)

    public AudioSource audioSource; // ��Ч����Դ
    public AudioClip buttonSound;    // Start��ť��Ч

    private void Start()
    {
        // �������ָ��
        Cursor.visible = false;

        // ��ʼ����ť��ɫ����ʾ�ı�
        ResetButtonColors();
        UpdateActionText(null);
    }

    private void Update()
    {
        // �����������л�ѡ��״̬
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectButton(startButton);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectButton(quitButton);
        }

        // ���ȷ�ϼ���������¼�
        if (Input.GetKeyDown(KeyCode.E) && selectedButton != null)
        {
            TriggerButtonAction(selectedButton); // ������ť����¼�
            audioSource.PlayOneShot(buttonSound);
        }
    }

    // ѡ�а�ť�Ĺ���
    private void SelectButton(Button button)
    {
        // �����ǰ��ť�Ѿ���ѡ��״̬�����ظ�����
        if (selectedButton == button) return;

        // �ָ����а�ť��Ĭ����ɫ
        ResetButtonColors();

        // ���µ�ǰѡ�еİ�ť
        selectedButton = button;

        // ����ѡ�а�ť����ɫ
        var colors = selectedButton.colors;
        colors.normalColor = selectedColor; // �ı���ɫΪѡ����ɫ
        selectedButton.colors = colors;

        // ������ʾ�ı�
        UpdateActionText(button);
    }

    // �ָ����а�ť��Ĭ����ɫ
    private void ResetButtonColors()
    {
        var startColors = startButton.colors;
        startColors.normalColor = defaultColor;
        startButton.colors = startColors;

        var quitColors = quitButton.colors;
        quitColors.normalColor = defaultColor;
        quitButton.colors = quitColors;
    }

    // ������ʾ�ı�����
    private void UpdateActionText(Button button)
    {
        if (button == null)
        {
            actionText.text = ""; // �����ʾ
        }
        else
        {
            actionText.text = "[E] - SELECT"; // ��ʾ��ʾ
        }
    }

    // ������ť����¼�
    private void TriggerButtonAction(Button button)
    {
        if (button == startButton)
        {
            Debug.Log("Start Game Clicked!");
            StartCoroutine(LoadSceneAfterDelay("Test", 0.5f)); // �滻Ϊ�����Ϸ��������
        }
        else if (button == quitButton)
        {
            Debug.Log("Quit Game Clicked!");
            Application.Quit();

#if UNITY_EDITOR
            // �ڱ༭��ģʽ�²����˳������ڵ���
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
