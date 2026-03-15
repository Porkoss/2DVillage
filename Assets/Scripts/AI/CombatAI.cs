using UnityEngine;
using UnityEngine.AI;

public class CombatAI : MonoBehaviour
{
    //bool
    protected bool bIsInCombat = false;
    protected bool bHasTarget = false;

    //Timer
    protected float currentAttackTimer=0f;
    protected float attackTimer=1f; 
    float currentDetectionTimer = 0.3f;
    float DetectionTimer = 0.3f;

    //target
    protected GameObject currentTarget;
    protected GameObject chosenAttackPoint;

    //Component
    protected NavMeshAgent agent;
    protected Animator animator;
    public Health health;



    [Header("Combat")]
    [SerializeField] protected float AttackRange = 1.2f;
    [SerializeField] public GameObject leftAttackPoint;
    [SerializeField] public GameObject rightAttackPoint;
    [SerializeField] protected float damage = 1f;
    [SerializeField] protected float DetectionRange = 10f;


    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();
        //defaultPosition = Utilities.To2DVector(transform.position);
        currentTarget = gameObject;
    }
    void Update()
    {
        if (bIsInCombat)
        {
            Attacks();
        }
        else if (bHasTarget)
        {
            SetUpForAttacks();
        }
        else if(currentDetectionTimer>DetectionTimer)
        {
            FindTarget();
            currentDetectionTimer = 0f;
        }

        currentDetectionTimer += Time.deltaTime;
    }
    public void TakingAggro(CombatAI attacker)
    {
        
        bIsInCombat = true;
        currentTarget = attacker.gameObject;
        agent.isStopped = true;

    }

    protected virtual void Attacks()
    {
        AttacksCharacter();
    }
     protected void AttacksCharacter()
    {
        if (currentTarget == null)
        {
            bIsInCombat = false;
            bHasTarget = false;
            agent.isStopped = false;
        }
        CombatAI ennemyAI = currentTarget.GetComponent<CombatAI>();
        if (ennemyAI == null)
        {
            bIsInCombat = false;
            bHasTarget = false;
            agent.isStopped = false;
        }
        else
        {
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
                if (currentAttackTimer >= attackTimer)
                {

                    animator.SetTrigger("Attack1");
                    currentAttackTimer = 0f;
                    bool bTargetDestroyed = ennemyAI.health.TakingDamage(damage);

                    ennemyAI.TakingAggro(this);
                    if (bTargetDestroyed)
                    {
                        bIsInCombat = false;
                        bHasTarget = false;
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
    }

    void SetUpForAttacks()
    {
        GameObject leftAttackPoint = currentTarget.GetComponent<CombatAI>().leftAttackPoint;

        GameObject rightAttackPoint = currentTarget.GetComponent<CombatAI>().rightAttackPoint;
        //checking first is the attack point is reachable.
        if (!CheckIfReachable(rightAttackPoint))
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
                Vector3 destination = new Vector3(rightAttackPoint.transform.position.x, rightAttackPoint.transform.position.y, transform.position.z);
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
                Vector3 destination = new Vector3(leftAttackPoint.transform.position.x, leftAttackPoint.transform.position.y, transform.position.z);
                agent.SetDestination(destination);
                transform.forward = new Vector3(0, -1, 0);
                animator.transform.localScale = new Vector3(1, 1, 1);
                chosenAttackPoint = leftAttackPoint;

            }
            else
            {
                agent.stoppingDistance = 0;
                Vector3 destination = new Vector3(rightAttackPoint.transform.position.x, rightAttackPoint.transform.position.y, transform.position.z);
                agent.SetDestination(destination);
                animator.transform.localScale = new Vector3(-1, 1, 1);
                chosenAttackPoint = rightAttackPoint;


            }

        }
    }

    protected bool CheckIfReachable(GameObject attackPoint)
    {
        int mask = LayerMask.GetMask("Default");
        Vector3 attackPointProjected = new Vector3(attackPoint.transform.position.x, attackPoint.transform.position.y, 0);
        Collider[] hits = Physics.OverlapSphere(attackPointProjected, 0.2f, mask, QueryTriggerInteraction.Collide);

        foreach (Collider hit in hits)
        {
            //Debug.Log(hit.gameObject.transform.parent.gameObject.name);
            if (hit.gameObject.CompareTag("Building"))
            {

                return false;
            }
        }
        return true;
    }
    protected virtual void  FindTarget()
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
                    bHasTarget = true;
                }
            }
        }
        else
        {
            bHasTarget = false;
        }
    }
}