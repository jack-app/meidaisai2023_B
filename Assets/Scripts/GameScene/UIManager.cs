using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel = null!;
    [SerializeField] private GameObject resultPanel = null!;
    [SerializeField] private Text scoreText = null!;
    [SerializeField] private Text resultScoreText = null!;
    [SerializeField] private Text speedText = null!;
    [SerializeField] private Slider fuelSlider = null!;
    private bool pause = false;
    private bool result = false;

    private void Pause()
    {
        Time.timeScale = 0f;
        pause = true;
        pausePanel.gameObject.SetActive(true);
    }

    private void Resume()
    {
        pausePanel.gameObject.SetActive(false);
        pause = false;
        Time.timeScale = 1f;
    }

    private void Continue()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    private void Finish()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    public void Result(long score)
    {
        Time.timeScale = 0f;
        resultPanel.gameObject.SetActive(true);
        resultScoreText.text = "Your score : " + score.ToString();
        result = true;
    }

    public void UIUpdate(long score, float fuel, float speed)
    {
        fuelSlider.value = fuel;
        scoreText.text = score.ToString();
        speedText.text = "Speed : " + speed.ToString();
        if (Input.GetKey(KeyCode.P))
        {
            Pause();
        }
        if (pause && Input.GetKey(KeyCode.R))
        {
            Resume();
        }
        if (result && Input.GetKey(KeyCode.C))
        {
            Continue();
        }
        if ((result || pause)&& Input.GetKey(KeyCode.F))
        {
            Finish();
        }
    }
}
