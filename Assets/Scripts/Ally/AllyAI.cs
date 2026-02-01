
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
[RequireComponent(typeof(Health))]
public class AllyAI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Vector3 defaultPosition;

    [SerializeField] float DetectionRange = 20f;
   
    [SerializeField] float DetectionTimer = 0.3f;
    float currentDetectionTimer = 0f;
    Animator animator;
    NavMeshAgent agent;


    [Header("Combat")]
    [SerializeField] float AttackRange = 1.2f;
    
    [SerializeField] public GameObject leftAttackPoint;
    [SerializeField] public GameObject rightAttackPoint;
    float currentAttackTimer=0f;
    [SerializeField] float AttackTimer = 1f;
    [SerializeField] float damage = 1f;
    [SerializeField] bool bIsInCombat = false;
    [SerializeField] private GameObject currentTarget; // change this
     

    public bool bHasOpponent = false;

    public Health health;

    [SerializeField] GameObject chosenAttackPoint;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>(); 
        defaultPosition = Utilities.To2DVector(transform.position);
        currentTarget = gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        ///Base comportment => checking for opponing each x sec 
        ///
        ///when opponent is detected Engaging oponent
        ///if another opponent is closer => swith opponent 
        ///when oppenent is clsoe Attack him
        /// if opponent is destroyed  => find new oen
        /// if nothing => ResetDefaultPosition
        transform.forward = Vector3.down;
        if (!bIsInCombat)
        {
            if(currentDetectionTimer >= DetectionTimer)
            {
                currentDetectionTimer = 0f;
                if (CheckingForOpponent())
                {
                    agent.SetDestination(currentTarget.transform.position);
                }
                else
                {
                    ResetDefaultPosition();
                }
            }
            currentDetectionTimer += Time.deltaTime;
            if(bHasOpponent)
            {
                EngagingOpponent();// check if close enough to enter combat mode here cannot go out of combat mode outside of winning combat
            }
            

        }
        //if close enough and in combat => attacks
        else
        {
            Attacking();//deal damage to ennemy and if ennemy is killed, go out of combat mode
        }
        Vector3 vector = currentTarget.transform.position - transform.position;

        animator.transform.localScale = new Vector3(Mathf.Sign(vector.x), 1, 1);

    }



    private void ResetDefaultPosition()
    {
        agent.SetDestination(defaultPosition);
    }



    private bool CheckingForOpponent()
    {
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider[] EnemyInRange = Physics.OverlapSphere(transform.position, DetectionRange, enemyLayer);
        
        if (EnemyInRange.Length > 0)
        {
            float currentDistance = Utilities.Distance2D(transform.position, EnemyInRange[0].gameObject.transform.position); ;
            currentTarget = EnemyInRange[0].gameObject;
            foreach (Collider enemy in EnemyInRange)
            {

                float iterDistance = Utilities.Distance2D(transform.position, enemy.gameObject.transform.position);
                if (iterDistance <= currentDistance)
                {
                    /// add conditions if opponent is available for combat here // kinda useless mecanic, let there be a pack of mob attacking each other if relevant.
                    
                    currentDistance = iterDistance;
                    currentTarget = enemy.gameObject;
                }
            }
        }
        else
        {
            return false;
        }
        bHasOpponent = true;
        return true;
    }

    private void EngagingOpponent()
    {
        
        GameObject leftAttackPoint = currentTarget.GetComponent<EnnemyAI>().leftAttackPoint;
        NavMeshPath navMeshPath = new NavMeshPath();

        GameObject rightAttackPoint = currentTarget.GetComponent<EnnemyAI>().rightAttackPoint;
        //checking first is the attack point is reachable.
        if (!CheckIfReachable(rightAttackPoint) )
        {
            if (CheckIfReachable(leftAttackPoint))
            {
                agent.stoppingDistance = 0;
                Vector3 destination = new Vector3(leftAttackPoint.transform.position.x, leftAttackPoint.transform.position.y, transform.position.z);
                agent.SetDestination(destination);
                transform.forward = new Vector3(0, -1, 0);
                animator.transform.localScale = new Vector3(1, 1, 1);
                chosenAttackPoint = leftAttackPoint;
            }
            else
            {
                chosenAttackPoint = currentTarget;
            }
        }
        if (!CheckIfReachable(leftAttackPoint))
        {
            if (CheckIfReachable(rightAttackPoint))
            {
                agent.stoppingDistance = 0;
                Vector3 destination = new Vector3(rightAttackPoint.transform.position.x, leftAttackPoint.transform.position.y, transform.position.z);
                agent.SetDestination(destination);
                animator.transform.localScale = new Vector3(-1, 1, 1);
                chosenAttackPoint = rightAttackPoint;
            }
        }

        
        if (Utilities.Distance2D(transform.position, currentTarget.transform.position) <= AttackRange && !bIsInCombat)
        {
            Debug.Log("EngagingOponent)");
            bIsInCombat = true;
            if (Utilities.Distance2D(transform.position, leftAttackPoint.transform.position) <= Utilities.Distance2D(transform.position, rightAttackPoint.transform.position))
            {
                agent.stoppingDistance = 0;
                Vector3 destination = new Vector3(leftAttackPoint.transform.position.x,leftAttackPoint.transform.position.y, transform.position.z);
                agent.SetDestination(destination);
                transform.forward = new Vector3(0, -1, 0);
                animator.transform.localScale = new Vector3(1, 1, 1);
                chosenAttackPoint = leftAttackPoint;

            }
            else
            {
                agent.stoppingDistance = 0;
                Vector3 destination = new Vector3(rightAttackPoint.transform.position.x, leftAttackPoint.transform.position.y, transform.position.z);
                agent.SetDestination(destination);
                animator.transform.localScale = new Vector3(-1, 1, 1);
                chosenAttackPoint = rightAttackPoint;


            }

        }
    }

    private bool CheckIfReachable(GameObject attackPoint)
    {
        int mask = LayerMask.GetMask("Default");
        Vector3 attackPointProjected = new Vector3(attackPoint.transform.position.x, attackPoint.transform.position.y, 0);
        Collider[] hits = Physics.OverlapSphere(attackPointProjected,0.2f,mask,QueryTriggerInteraction.Collide);
        
        foreach (Collider hit in hits)
        {
            //Debug.Log(hit.gameObject.transform.parent.gameObject.name);
            if (hit.gameObject.CompareTag("Building"));
            {
                
                return false;
            }
        }
        return true;
    }

    private void Attacking()
    {
        try
        {
            EnnemyAI ennemyAI = currentTarget.GetComponent<EnnemyAI>();
            //Debug.Log("trying to attack at distance " + (Utilities.Distance2D(chosenAttackPoint.transform.position, transform.position) <= 0.2f));
            //TO DO : keep that in check ( maybe change logic when once in place => just attack)
            if (Utilities.Distance2D(chosenAttackPoint.transform.position, transform.position) <= 1f)
            {
                ennemyAI.TakingAggro(this);
            }
            if (Utilities.Distance2D(chosenAttackPoint.transform.position, transform.position) <= 0.2f)
            {

                


                Debug.Log("Attacking Opponent");
                agent.isStopped = true;
                if (currentAttackTimer >= AttackTimer)
                {

                    animator.SetTrigger("Attack1");
                    currentAttackTimer = 0f;
                    bool bTargetDestroyed = ennemyAI.health.TakingDamage(damage);
                    ennemyAI.TakingAggro(this);
                    if (bTargetDestroyed)
                    {
                        bIsInCombat = false;
                        bHasOpponent = false;
                        agent.isStopped = false;
                    }

                }
                else
                {

                    currentAttackTimer += Time.deltaTime;
                }


            }

            else
            {
                Vector3 destination = new Vector3(chosenAttackPoint.transform.position.x, chosenAttackPoint.transform.position.y, transform.position.z);
                agent.SetDestination(destination);
            }

            
        }
        catch
        {
            bIsInCombat = false;
            bHasOpponent = false;
            agent.isStopped=false;
        }

    }
}
