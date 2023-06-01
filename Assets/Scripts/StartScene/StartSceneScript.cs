using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneScript : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void PushStart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
