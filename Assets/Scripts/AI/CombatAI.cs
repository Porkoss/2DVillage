using UnityEngine;
using UnityEngine.AI;

public class CombatAI : MonoBehaviour
{
    //bool
    public AIState state = AIState.SeekingTarget;

    //Timer
    protected float currentAttackTimer=0f;
    protected float attackTimer=1f; 
    protected float currentDetectionTimer = 0.3f;
    protected float DetectionTimer = 0.3f;

    protected float resetTimer = 2f;
    protected float currentResetTimer = 0f;

    //target
    [SerializeField] protected GameObject currentTarget;
    protected GameObject chosenAttackPoint;

    //Component
    protected NavMeshAgent agent;
    protected Animator animator;
    public Health health;
   


    [Header("Combat")]
    [SerializeField] protected float AttackRange = 1.5f;
    [SerializeField] public GameObject leftAttackPoint;
    [SerializeField] public GameObject rightAttackPoint;
    [SerializeField] public float damage = 1f;
    [SerializeField] protected float DetectionRange = 10f;


    public enum AIState
    {
        SeekingTarget,
        JoiningTarget,
        InCombat,
        AttackingBuilding
    }

    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();
        //defaultPosition = Utilities.To2DVector(transform.position);
        currentTarget = gameObject;
    }
    public virtual void Update()
    {
        transform.forward = Vector3.down;
        switch (state)
        {
            case AIState.SeekingTarget:
                if (currentDetectionTimer > DetectionTimer)
                {
                    FindTarget();
                    currentDetectionTimer = 0f;
                }
                currentDetectionTimer += Time.deltaTime;
                break;

            case AIState.JoiningTarget:
                if(Utilities.Distance2D(transform.position, currentTarget.transform.position) < 5f)//check for shortest path to attack point only when relatively close
                {
                    SetUpForAttacks();
                }
                break;

            case AIState.InCombat:
                Attacks();
                break;
        }

        if (currentResetTimer > resetTimer)
        {
            currentResetTimer = 0f;
            state = AIState.SeekingTarget;
        }
        currentResetTimer += Time.deltaTime;

        if (currentTarget != null)
        {
            Vector3 vector = currentTarget.transform.position - transform.position;

            animator.transform.localScale = new Vector3(Mathf.Sign(vector.x), 1, 1);
        }
    }
    public void TakingAggro(CombatAI attacker)
    {
        if(state!= AIState.InCombat)
        {
            currentTarget = attacker.gameObject;
            SetUpForAttacks();
            //agent.isStopped = true;
        }

    }

    protected virtual void Attacks()
    {
        AttacksCharacter();
    }
     protected void AttacksCharacter()
    {
        if (currentTarget == null)
        {
            state = AIState.SeekingTarget;
            agent.isStopped = false;
            return;
        }
        CombatAI ennemyAI = currentTarget.GetComponent<CombatAI>();
        if (ennemyAI == null)
        {
            state = AIState.SeekingTarget;
            agent.isStopped = false;
            return;
        }
        else
        {
            //Debug.Log("trying to attack at distance " + (Utilities.Distance2D(chosenAttackPoint.transform.position, transform.position) <= 0.2f));
            float distance = Utilities.Distance2D(chosenAttackPoint.transform.position, transform.position);
            //Debug.Log(distance);
            if (distance <= 1f)
            {
               ennemyAI.TakingAggro(this);
            }
            if (distance <= 0.5f)
            {
               
                agent.isStopped = true;
                if (currentAttackTimer >= attackTimer)
                {

                    animator.SetTrigger("Attack1");
                    currentAttackTimer = 0f;
                    bool bTargetDestroyed = ennemyAI.health.TakingDamage(damage);

                    ennemyAI.TakingAggro(this);
                    if (bTargetDestroyed)
                    {
                        state = AIState.SeekingTarget;
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

    protected virtual void SetUpForAttacks()
    {
        agent.isStopped = false;
        GameObject leftAttackPoint;
        GameObject rightAttackPoint;
        if (currentTarget.CompareTag("Building"))
        {
            leftAttackPoint = currentTarget.GetComponent<Building>().leftAttackPoint;

            rightAttackPoint = currentTarget.GetComponent<Building>().rightAttackPoint;
        }
        else
        {
            leftAttackPoint = currentTarget.GetComponent<CombatAI>().leftAttackPoint;

            rightAttackPoint = currentTarget.GetComponent<CombatAI>().rightAttackPoint;
        }

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
        if(chosenAttackPoint == null)
        {
            chosenAttackPoint = currentTarget;
        }

        if ((Utilities.Distance2D(transform.position, currentTarget.transform.position) <= AttackRange && state != AIState.InCombat ) || 
            (Utilities.Distance2D(transform.position, chosenAttackPoint.transform.position) <= AttackRange && state != AIState.InCombat))
        {
            Debug.Log("EngagingOponent)");

            state = AIState.InCombat;
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
                    state=AIState.JoiningTarget;
                }
            }
        }
        else
        {
            state =AIState.SeekingTarget;
        }

        if(state == AIState.JoiningTarget)
        {
            agent.SetDestination(currentTarget.transform.position);
        }
    }
}