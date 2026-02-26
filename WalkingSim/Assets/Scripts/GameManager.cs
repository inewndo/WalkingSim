using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //static is a varible that belongs to class itself and not specific instance of that class
    public static GameManager instance;

    void Awake()
    {

        //if there is no game manager in a new scene (when switching to it), dont destroy. If there is another manager in a new scene then destroy this one
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("reload");
    }
}
