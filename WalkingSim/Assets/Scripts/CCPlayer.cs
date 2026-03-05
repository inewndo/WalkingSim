using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CCPlayer : MonoBehaviour
{
    #region VARIABLES
    [Header("Movement")]
    public float walkSpeed = 5;
    public float sprintSpeed = 7;
    public float jumpHeight = 5;
    bool isSprinting;
    bool isJumping;

    public Transform cameraTransform;
    public float lookSensativity = 1f;

    private CharacterController cc;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float verticalVelocity; //current upward/downward speed
    private float gravity = -20f; //constant downward acceleration

    private float pitch; //up/down

    //interaction variables
    private GameObject currrentTarget; //who the player is looking at
    public Image reticleImage;
    private bool interactPressed;

    public static event Action<NPCData> OnDialogueRequested;
    private Interactable currrentInteractable;

    #endregion

    private void Awake()
    {
        cc = GetComponent<CharacterController>();

        //optional lock cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        //find reticle
        reticleImage = GameObject.Find("Reticle").GetComponent<Image>();
        reticleImage.color = new Color(r: 0, g: 0, b: 0, a: 7f); //slightly transparent black
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleLook();
        HandleMovement();
        CheckInteract();
        HandleInteract();
    }

    #region HANDLES
    private void HandleLook()
    {
        //horiz rotate player
        float yaw = lookInput.x * lookSensativity;
        //vert rotate cam
        float pitchDelta = lookInput.y * lookSensativity;

        transform.Rotate(eulers: Vector3.up * yaw);

        //accumulate vert rotation
        pitch -= pitchDelta;
        //clamp to prevent flipping upside down
        pitch = Mathf.Clamp(pitch, min: -90, max: 90);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }
    private void HandleMovement()
    {
        //updating player's bool to be true or false whether th eplayer is grounded or not
        bool grounded = cc.isGrounded;
        //Debug.Log("is Grounded" + grounded);

        //keep cc's snapped to ground
        if(grounded && verticalVelocity <= 0)
        {
            verticalVelocity = -2f;
        }

        float currentSpeed = walkSpeed;
        if (isSprinting)
        {
            currentSpeed = sprintSpeed;
        }
        else if(!isSprinting)
        {
            currentSpeed = walkSpeed;
        }

        Vector3 move = transform.right * moveInput.x * currentSpeed + transform.forward * moveInput.y * currentSpeed;

        if(isJumping && grounded)
        {
            verticalVelocity = Mathf.Sqrt(f: jumpHeight * -2f * gravity);
        }
        else
        {
            isJumping = false;
        }

        //apply gravity to every frame
        verticalVelocity += gravity * Time.deltaTime;

        //convert verticalVelocity into movement vector
        Vector3 velocity = Vector3.up * verticalVelocity;
        cc.Move(motion: (move + velocity) * Time.deltaTime); //finally actually moving the player
    }
    #endregion 

    void CheckInteract()
    {
        //reset reticle image to normal color first
        if (reticleImage != null) reticleImage.color = new Color(0, 0, 0, .7f);
        //make a ray that goes straight out of the camera(center of screen)
        //players eyesight
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        //RaycastHit hit;
        //asking unity if it hit something within 3 units
        //hit stores what we hit like the collider
        //bool didHit = Physics.Raycast(ray, out hit, 3);
        //if (!didHit) return;//if we didn't hit anything start here
        //if we hit something tagged interactable
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            currrentInteractable = hit.collider.GetComponent<Interactable>();
            if(currrentInteractable != null && reticleImage != null)
            {
                reticleImage.color = Color.red;
                Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 3, Color.blue);

            }
            else
            {
                Debug.DrawRay(cameraTransform.position, cameraTransform.forward * 3, Color.blue);
            }
        }

        
    }
    void HandleInteract()
    {
        //if the player did not press interact this frame do nothing
        if (!interactPressed) return;
        //consume the input so one click only triggers one interactions
        //this changes next frame
        interactPressed = false;
        if (currrentInteractable == null) return;
        currrentInteractable.Interact(this);
    }

    #region PLAYERINPUT
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //jumpin=true only w=the moment key is pressed
        if (context.performed) isJumping = true;
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed) interactPressed = true;
        Debug.Log("OnInteract fired.performed=" + context.performed);
    }
    #endregion

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("CC Collided with: " + hit.gameObject.name);
    }

    public void RequestDialogue(NPCData npcData)
    {
        OnDialogueRequested?.Invoke(npcData);
    }

}
