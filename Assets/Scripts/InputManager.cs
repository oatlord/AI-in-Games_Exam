using UnityEngine;

public class InputManager : MonoBehaviour
{
    // private InputSystem_Actions inputActions;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        // inputActions = new InputSystem_Actions();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnEnable()
    {
        // inputActions.Enable();
    }

    void OnDisable()
    {
        // inputActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // if (inputActions.Player.Move.IsPressed())
        // {
            
        // } 
    }
}
