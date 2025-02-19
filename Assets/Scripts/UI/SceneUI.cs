using UnityEngine;

public class GamePause : MonoBehaviour
{
    public GameObject pauseUI; // ����ͣUI����
    public CameraController cameraController; // �󶨿���������Ľű�
    public MonoBehaviour playerController; // ����ҿ��ƽű���������ҵ��ƶ��򹥻����ƣ�
    public MonoBehaviour enemyAIController; // �󶨵���AI�ű�������У�
    public Animator playerAnimator; // ����ҵ�Animator���
    public Animator enemyAnimator; // �󶨵��˵�Animator���

    private bool isPaused = false;

    void Update()
    {
        // ��Esc���л���ͣ״̬
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                // �ָ���Ϸ
                ResumeGame();
            }
            else
            {
                // ��ͣ��Ϸ
                PauseGame();
            }
        }
    }

    // ��ͣ��Ϸ
    void PauseGame()
    {
        isPaused = true;
        pauseUI.SetActive(true); // ��ʾ��ͣUI

        // ����������Ŀ��ƽű�
        if (cameraController != null)
        {
            cameraController.enabled = false;
        }

        // ������ҿ��ƽű�
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // ���õ���AI�ű�������У�
        if (enemyAIController != null)
        {
            enemyAIController.enabled = false;
        }

        // ֹͣ��ҵĶ���
        if (playerAnimator != null)
        {
            playerAnimator.speed = 0f; // ��ͣ��ҵĶ���
        }

        // ֹͣ���˵Ķ���
        if (enemyAnimator != null)
        {
            enemyAnimator.speed = 0f; // ��ͣ���˵Ķ���
        }
    }

    // �ָ���Ϸ
    void ResumeGame()
    {
        isPaused = false;
        pauseUI.SetActive(false); // ������ͣUI

        // ����������Ŀ��ƽű�
        if (cameraController != null)
        {
            cameraController.enabled = true;
        }

        // ������ҿ��ƽű�
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // ���õ���AI�ű�������У�
        if (enemyAIController != null)
        {
            enemyAIController.enabled = true;
        }

        // �ָ���ҵĶ���
        if (playerAnimator != null)
        {
            playerAnimator.speed = 1f; // �ָ���ҵĶ�������
        }

        // �ָ����˵Ķ���
        if (enemyAnimator != null)
        {
            enemyAnimator.speed = 1f; // �ָ����˵Ķ�������
        }
    }
}
