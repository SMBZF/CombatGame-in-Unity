using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    [SerializeField] private GameObject victoryUI; // 胜利UI界面
    [SerializeField] private AudioClip victoryMusic; // 胜利音乐
    private AudioSource audioSource; // 用于播放音乐
    public List<EnemyController> allEnemies = new List<EnemyController>(); // 存储所有敌人

    private void Start()
    {
        // 初始化 AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // 确保音乐不会自动播放
        audioSource.loop = false; // 不需要循环播放胜利音乐

        // 获取场景中所有的敌人，并添加到 allEnemies 列表中
        allEnemies.AddRange(FindObjectsOfType<EnemyController>());

        // 为所有敌人添加死亡事件监听
        foreach (var enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDied += HandleEnemyDeath; // 注册死亡事件
            }
        }
    }

    private void HandleEnemyDeath(EnemyController deadEnemy)
    {
        // 从列表中移除死亡的敌人
        if (allEnemies.Contains(deadEnemy))
        {
            allEnemies.Remove(deadEnemy);
        }

        // 检查是否所有敌人都死亡
        if (allEnemies.Count == 0)
        {
            ShowVictoryUI(); // 所有敌人死亡，显示胜利UI
        }
    }

    // 显示胜利UI
    private void ShowVictoryUI()
    {
        if (victoryUI != null)
        {
            victoryUI.SetActive(true); // 显示胜利界面
            Time.timeScale = 0f; // 暂停游戏
        }

        PlayVictoryMusic(); // 播放胜利音乐

        // 启动协程检测按键输入
        StartCoroutine(WaitForAnyKey());
    }

    // 播放胜利音乐
    private void PlayVictoryMusic()
    {
        if (victoryMusic != null)
        {
            audioSource.clip = victoryMusic; // 设置音频剪辑
            audioSource.Play(); // 播放音频
        }
        else
        {
            Debug.LogWarning("Victory music not assigned!");
        }
    }

    // 等待用户按下任意键
    private System.Collections.IEnumerator WaitForAnyKey()
    {
        yield return new WaitForSecondsRealtime(1f); // 等待一秒，避免 UI 出现瞬间被跳过

        while (!Input.anyKeyDown)
        {
            yield return null; // 等待下一帧
        }

        ReturnToMainMenu(); // 用户按下任意键后返回主菜单
    }

    // 返回主菜单场景
    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // 恢复时间流动
        SceneManager.LoadScene("MainUI"); // 加载主菜单场景，确保场景名称与实际匹配
    }
}
