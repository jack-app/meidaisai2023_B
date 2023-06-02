using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel = null!;
    [SerializeField] private GameObject resultPanel = null!;
    [SerializeField] private Text score = null!;

    public void Pause()
    {
        Time.timeScale = 0f;
        pausePanel.gameObject.SetActive(true);
    }

    public void Resume()
    {
        pausePanel.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Continue()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void PushFinish()
    {
        SceneManager.LoadScene("StartScene");
    }
}
