using System.Collections.Generic;
using System.Net.Mail;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


[RequireComponent(typeof(Health))]
public class EnnemyAI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    NavMeshAgent agent;
    
    float currentTimer = 0.3f;
    float resetTrackingTimer = 0.3f;

    [SerializeField] float stoppingDistance = 0.1f;
    [SerializeField] float attackRange = 2f;

    GameObject currentTarget;
    List<GameObject> listOfBuiltBuilding;

    public bool bDestroyingBuilding = false; //TO DO: go out of destroying building loop (even if the building is not destroyed)

    [Header("Combat")]
    [SerializeField] float damage = 1f;
    float currentAttackTimer=0f;
    [SerializeField] float AttackTimer = 1f;

    Animator animator;

    public GameObject leftAttackPoint;
    public GameObject rightAttackPoint;
    public Health health;
    [SerializeField] bool bIsInCombat = false;

    public bool bIsAvalaibleForCombat = true;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoRepath = true;
        listOfBuiltBuilding = StaticClass.Instance.listOfBuiltBuilding;
        animator = GetComponentInChildren<Animator>(); 
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        // will only react to attacks from ally not engange
        ///if got aggro will do 
        if (bIsInCombat)
        {
            AttackingAlly();
        }
        else // attacking Building / player loop
        {
            currentTimer += Time.deltaTime;
            if ((currentTimer >= resetTrackingTimer) && !bDestroyingBuilding)
            {
                currentTarget = ClosestEnnemy();
                agent.stoppingDistance = stoppingDistance;
                agent.SetDestination(currentTarget.transform.position);

                currentTimer = 0f;
            }

            Attacking();

        }
        Vector3 vector = currentTarget.transform.position - transform.position;
        transform.forward = Vector3.down;
        animator.transform.localScale = new Vector3(Mathf.Sign(vector.x), 1, 1);

    }

    GameObject ClosestEnnemy()
    {
        if (listOfBuiltBuilding.Count ==0)
        {
            return StaticClass.Instance.player;
        }
        GameObject currentClosest = listOfBuiltBuilding[0];
        var currentDistance = Utilities.Distance2D(transform.position, listOfBuiltBuilding[0].transform.position);
        for (int i = 1; i < listOfBuiltBuilding.Count; i++)
        {
            float iterationDistance = Utilities.Distance2D(transform.position, listOfBuiltBuilding[i].transform.position);
            if (iterationDistance <= currentDistance)
            {
                currentClosest = listOfBuiltBuilding[i];
                currentDistance = iterationDistance;
            }
        }
        if (Utilities.Distance2D(transform.position, StaticClass.Instance.player.transform.position) < currentDistance)
        {
            return StaticClass.Instance.player;
        }
        return currentClosest;
    }


    void Attacking()
    {

        
        if (Utilities.Distance2D(transform.position,currentTarget.transform.position) <= attackRange)
        {
            if (currentTarget.CompareTag("Player")) // || currentTarget.CompareTag("Ally")  TO DO : update for ally
            {
                AttackPlayer();
            }
            else if (currentTarget.CompareTag("Building") )
            {
                AttackBuilding();
            }
        }
    }

    void AttackPlayer()
    {
        if (currentAttackTimer >= AttackTimer)
        {
            animator.SetTrigger("Attack1");
            //TO DO : move this into an animation controller to make it more realistic
            Vector2 position2D = new Vector2(transform.position.x, transform.position.y);
            Vector2 right = new Vector2(transform.right.x, transform.right.y);

            RaycastHit2D[] hits = Physics2D.CircleCastAll(position2D + right * 0.2f, 0.3f, right, 0.5f);
            Debug.DrawLine(position2D + right * 0.2f, position2D + right * 0.85f, Color.red, 1f);

            foreach (RaycastHit2D hit in hits)
            {

                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    hit.collider.gameObject.GetComponent<Health>().TakingDamage(damage);
                }
            }
        }
        else
        {

            currentAttackTimer += Time.deltaTime;
        }

        
    }

    void AttackBuilding()
    {
        Building building = currentTarget.GetComponentInParent<Building>();
        
        if (!bDestroyingBuilding)
        {
            currentTarget = SettingUpForAttack(building.leftAttackPoint, building.rightAttackPoint);
            bDestroyingBuilding = true;
        }


        
        if(Utilities.Distance2D(currentTarget.transform.position, transform.position) <= 0.1f ) 
        {
            
            if (currentAttackTimer >= AttackTimer)
            {
                
                animator.SetTrigger("Attack2");
                currentAttackTimer = 0f;
                bool bTargetDestroyed = building.health.TakingDamage(damage);
                if (bTargetDestroyed)
                {
                    bDestroyingBuilding = false;
                }
                //TO DO : "Add lancer flamme aniamtion"
            }
            else
            {

                currentAttackTimer += Time.deltaTime;
            }
        }

    }
    //rework this with waiting list for attackpoint
    GameObject SettingUpForAttack(GameObject leftPoint,GameObject rightPoint)
    {

        if (Utilities.Distance2D(transform.position, leftPoint.transform.position) <= Utilities.Distance2D(transform.position, rightPoint.transform.position))
        {
            agent.stoppingDistance = 0;
            agent.SetDestination(leftPoint.transform.position);
            transform.forward = new Vector3(0, -1, 0);
            animator.transform.localScale = new Vector3(1, 1, 1);
            return leftPoint;

        }
        else
        {
            agent.stoppingDistance = 0;
            agent.SetDestination(rightPoint.transform.position);
            animator.transform.localScale = new Vector3(-1, 1, 1);
            return rightPoint;
        }
    }
    //need to update this

    //turnign arround according to aggro
    public void TakingAggro(AllyAI attacker)
    {
        bDestroyingBuilding = false;
        bIsInCombat = true;
        currentTarget = attacker.gameObject;
        agent.isStopped = true;

    }

    private void AttackingAlly()
    {
        try
        {
            AllyAI allyAI = currentTarget.GetComponent<AllyAI>();

            if (currentAttackTimer >= AttackTimer)
            {

                animator.SetTrigger("Attack1");
                currentAttackTimer = 0f;
                bool bTargetDestroyed = allyAI.health.TakingDamage(damage);
                if (bTargetDestroyed)
                {
                    bIsInCombat = false;
                    agent.isStopped = false;
                }

            }
            else
            {

                currentAttackTimer += Time.deltaTime;
            }
            
        }
        catch
        {
            bIsInCombat = false;
        }

    }

    public bool CheckForCombatPlace()
    {
        return false;
    }
}
