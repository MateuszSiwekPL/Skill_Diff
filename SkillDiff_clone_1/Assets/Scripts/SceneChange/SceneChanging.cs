using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanging : MonoBehaviour
{
    [SerializeField] int sceneID;

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneID);
    }
}
