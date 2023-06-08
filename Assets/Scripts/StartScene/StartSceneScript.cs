using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneScript : MonoBehaviour
{
    [SerializeField] private GameObject settingPanel = null!;
    private bool settingg;
    void Start()
    {
        Application.targetFrameRate = 50;
    }
    void Update()
    {
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

    public void PushStart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
