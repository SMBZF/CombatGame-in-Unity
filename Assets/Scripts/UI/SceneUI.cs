using UnityEngine;

public class GamePause : MonoBehaviour
{
    public GameObject pauseUI; // 绑定暂停UI对象
    public CameraController cameraController; // 绑定控制摄像机的脚本
    public MonoBehaviour playerController; // 绑定玩家控制脚本（例如玩家的移动或攻击控制）
    public MonoBehaviour enemyAIController; // 绑定敌人AI脚本（如果有）
    public Animator playerAnimator; // 绑定玩家的Animator组件
    public Animator enemyAnimator; // 绑定敌人的Animator组件

    private bool isPaused = false;

    void Update()
    {
        // 按Esc键切换暂停状态
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                // 恢复游戏
                ResumeGame();
            }
            else
            {
                // 暂停游戏
                PauseGame();
            }
        }
    }

    // 暂停游戏
    void PauseGame()
    {
        isPaused = true;
        pauseUI.SetActive(true); // 显示暂停UI

        // 禁用摄像机的控制脚本
        if (cameraController != null)
        {
            cameraController.enabled = false;
        }

        // 禁用玩家控制脚本
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // 禁用敌人AI脚本（如果有）
        if (enemyAIController != null)
        {
            enemyAIController.enabled = false;
        }

        // 停止玩家的动画
        if (playerAnimator != null)
        {
            playerAnimator.speed = 0f; // 暂停玩家的动画
        }

        // 停止敌人的动画
        if (enemyAnimator != null)
        {
            enemyAnimator.speed = 0f; // 暂停敌人的动画
        }
    }

    // 恢复游戏
    void ResumeGame()
    {
        isPaused = false;
        pauseUI.SetActive(false); // 隐藏暂停UI

        // 启用摄像机的控制脚本
        if (cameraController != null)
        {
            cameraController.enabled = true;
        }

        // 启用玩家控制脚本
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // 启用敌人AI脚本（如果有）
        if (enemyAIController != null)
        {
            enemyAIController.enabled = true;
        }

        // 恢复玩家的动画
        if (playerAnimator != null)
        {
            playerAnimator.speed = 1f; // 恢复玩家的动画播放
        }

        // 恢复敌人的动画
        if (enemyAnimator != null)
        {
            enemyAnimator.speed = 1f; // 恢复敌人的动画播放
        }
    }
}
