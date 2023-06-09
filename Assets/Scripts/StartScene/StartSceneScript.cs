using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneScript : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel = null!;
    [SerializeField] private GameObject sceneChangePanel;
    [SerializeField] private AudioSource BGMSource;
    private Image fadeAlpha;
    private bool settingg;
    private bool fadeinFlag = true; 
    private bool fadeoutFlag = false;
    private float alpha = 1.0f;
    void Start()
    {
        Application.targetFrameRate = 50;
        fadeAlpha = sceneChangePanel.GetComponent<Image>();
        alpha = fadeAlpha.color.a;
    }
    void Update()
    {
        Fade();
        if (Input.GetKey(KeyCode.S))
        {
            settingg = true;
            settingPanel.gameObject.SetActive(true);
        }
        if (settingg && Input.GetKey(KeyCode.B))
        {
            settingg = false;
            settingPanel.gameObject.SetActive(false);
        }
    }
    private void Fade()
    {
        if (fadeinFlag)
        {
            alpha -= 0.02f;
            if (alpha <= 0)
            {
                fadeinFlag = false;
                alpha = 0;
            }
            fadeAlpha.color = new Color(0, 0, 0, alpha);
        }
        else if (fadeoutFlag)
        {

            alpha += 0.02f;
            if (alpha >= 1)
            {
                SceneManager.LoadScene("GameScene");
                fadeoutFlag = false;
                alpha = 1;
            }
            BGMSource.volume = BGMSource.volume - 0.02f;
            fadeAlpha.color = new Color(0, 0, 0, alpha);
        }
    }

    public void PushStart()
    {
        fadeoutFlag = true;
        Debug.Log("called");
    }
}
