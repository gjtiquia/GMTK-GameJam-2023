using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void ReloadScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}