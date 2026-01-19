using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Rigidbody2D rigidBody;
    
    public float speed;

    public float damage = 1f;

    [Header("Inputs")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private InputActionReference pauseAction;

    private float timerAttack=1f;
    private float runningTimerAttack=0f;

    private Animator animator;

    [SerializeField] private GameObject pauseMenu;

    [Header("Error")]
    [SerializeField] private GameObject ErrorPrefab;
    private float currentTimerError=0f;
    private bool bErrorShowing = false;
   
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Moves();

        //recolts : TODO refactor this
        if (attackAction.action.IsPressed() && runningTimerAttack>=timerAttack)
        {
            runningTimerAttack = 0f;
            AttackCollectable();
        }
        if (runningTimerAttack <= timerAttack)
        {
            runningTimerAttack += Time.deltaTime;
        }

        //build
        if(interactAction.action.WasPressedThisFrame()&& runningTimerAttack >= timerAttack)
        {
            runningTimerAttack = 0f;
            TriesToBuild();
        }

        //pause 

        Pause();
        
        ShowErrorPrefab();
    }
    #region InputAction
    private void OnEnable()
    {
        moveAction.action.Enable();
        attackAction.action.Enable();
        interactAction.action.Enable();
        pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        attackAction.action.Disable();
        interactAction.action.Disable();
        pauseAction.action.Disable();
    }

    #endregion InputAction
    private void Moves()
    {
        Vector2 move = moveAction.action.ReadValue<Vector2>();
        rigidBody.linearVelocity = move*speed;
        if(move.x != 0f)
        {
            
            transform.forward = new Vector3(0, 0, move.x);// may need to change after TODO
        }

        animator.SetBool("IsRunning", move != Vector2.zero);

         

    }

    private void AttackCollectable()
    {
        Vector2 position2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 right = new Vector2(transform.right.x,transform.right.y);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(position2D+right*0.2f, 0.3f,right, 0.5f);
        Debug.DrawLine(position2D + right * 0.2f, position2D + right*0.85f, Color.red,1f);
        foreach (RaycastHit2D hit in hits)
        {
            //add logic on collectable here with tag
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Collectable"))
            {
                
                GameObject go = hit.collider.gameObject;
                Debug.Log(go.name);
                go.GetComponent<Collectable>().TakeDamage(damage);
            }
        }
        
    }

    private void TriesToBuild()
    {
        Vector2 position2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 right = new Vector2(transform.right.x, transform.right.y);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(position2D + right * 0.2f, 0.3f, right, 0.5f);
        Debug.DrawLine(position2D + right * 0.2f, position2D + right * 0.85f, Color.red, 1f);
        foreach (RaycastHit2D hit in hits)
        {
            //add logic on collectable here with tag
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Building"))
            {
                GameObject go = hit.collider.gameObject;
                if(go.GetComponent<Building>().TryOneBrick())
                {
                    animator.SetTrigger("Build");
                }
                else
                {
                    currentTimerError = 0;
                    ErrorPrefab.SetActive(true);
                    bErrorShowing = true;
                    //TO DO : play sound
                }
                
            }
        }
    }


    private void ShowErrorPrefab()
    {
        if (bErrorShowing)
        {
            if (currentTimerError >= 1f)
            {
                currentTimerError = 0f;
                ErrorPrefab.SetActive(false);
            }
            else
            {
                currentTimerError += Time.deltaTime;
            }
        }

    }

    //collects items
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        if (go.CompareTag("Loot"))
        {
            Loot loot = go.GetComponent<Loot>();
            loot.Collected();
            SoundManager.PlayRandomSoundFromType(SoundType.Gather, 0.4f);
        }
    }

    //pause

    private void Pause()
    {
        if (pauseAction.action.WasPressedThisFrame())
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }


}
