using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    [SerializeField] private GameObject victoryUI; // ʤ��UI����
    [SerializeField] private AudioClip victoryMusic; // ʤ������
    private AudioSource audioSource; // ���ڲ�������
    public List<EnemyController> allEnemies = new List<EnemyController>(); // �洢���е���

    private void Start()
    {
        // ��ʼ�� AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // ȷ�����ֲ����Զ�����
        audioSource.loop = false; // ����Ҫѭ������ʤ������

        // ��ȡ���������еĵ��ˣ�����ӵ� allEnemies �б���
        allEnemies.AddRange(FindObjectsOfType<EnemyController>());

        // Ϊ���е�����������¼�����
        foreach (var enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDied += HandleEnemyDeath; // ע�������¼�
            }
        }
    }

    private void HandleEnemyDeath(EnemyController deadEnemy)
    {
        // ���б����Ƴ������ĵ���
        if (allEnemies.Contains(deadEnemy))
        {
            allEnemies.Remove(deadEnemy);
        }

        // ����Ƿ����е��˶�����
        if (allEnemies.Count == 0)
        {
            ShowVictoryUI(); // ���е�����������ʾʤ��UI
        }
    }

    // ��ʾʤ��UI
    private void ShowVictoryUI()
    {
        if (victoryUI != null)
        {
            victoryUI.SetActive(true); // ��ʾʤ������
            Time.timeScale = 0f; // ��ͣ��Ϸ
        }

        PlayVictoryMusic(); // ����ʤ������

        // ����Э�̼�ⰴ������
        StartCoroutine(WaitForAnyKey());
    }

    // ����ʤ������
    private void PlayVictoryMusic()
    {
        if (victoryMusic != null)
        {
            audioSource.clip = victoryMusic; // ������Ƶ����
            audioSource.Play(); // ������Ƶ
        }
        else
        {
            Debug.LogWarning("Victory music not assigned!");
        }
    }

    // �ȴ��û����������
    private System.Collections.IEnumerator WaitForAnyKey()
    {
        yield return new WaitForSecondsRealtime(1f); // �ȴ�һ�룬���� UI ����˲�䱻����

        while (!Input.anyKeyDown)
        {
            yield return null; // �ȴ���һ֡
        }

        ReturnToMainMenu(); // �û�����������󷵻����˵�
    }

    // �������˵�����
    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // �ָ�ʱ������
        SceneManager.LoadScene("MainUI"); // �������˵�������ȷ������������ʵ��ƥ��
    }
}
