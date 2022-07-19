using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player instance;

    public int coins;
    public int level;

    public int maxHpStatus;
    public int maxPowerStatus;

#if UNITY_EDITOR
    [MethodButton("SavePlayer", "LoadPlayer", "DeletePlayer", "ReloadScene")]
    [SerializeField] private bool editorFoldout;
#endif

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayer();
        }
        else
            Destroy(this);
    }

    //save / load / delete
    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        coins = data.coins;
        level = data.level;

        maxHpStatus = data.maxHpStatus;
        maxPowerStatus = data.maxPowerStatus;
    }

    public void DeletePlayer()
    {
        SaveSystem.DeletePlayer();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}