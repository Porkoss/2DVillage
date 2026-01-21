using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

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

    [Header("Attacks")]
    [SerializeField] float damage = 1f;
    float currentAttackTimer=0f;
    [SerializeField] float AttackTimer = 1f;

    Animator animator;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoRepath = true;
        listOfBuiltBuilding = StaticClass.Instance.listOfBuiltBuilding;
        animator = GetComponentInChildren<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        // need to change in order to adapt to going left or right
        
        currentTimer += Time.deltaTime;
        if ((currentTimer>=resetTrackingTimer) && !bDestroyingBuilding)
        {
            currentTarget = ClosestEnnemy();
            agent.stoppingDistance = stoppingDistance;
            agent.SetDestination(currentTarget.transform.position);
            
            currentTimer = 0f;
        }
        Vector3 vector = currentTarget.transform.position - transform.position;
        transform.forward = Vector3.down;
        transform.localScale = new Vector3(Mathf.Sign(vector.x), 1, 1);
        Attacking();
        
        
    }

    GameObject ClosestEnnemy()
    {
        if (listOfBuiltBuilding.Count ==0)
        {
            return StaticClass.Instance.player;
        }
        GameObject currentClosest = listOfBuiltBuilding[0];
        var currentDistance = Distance2D(transform.position, listOfBuiltBuilding[0].transform.position);
        for (int i = 1; i < listOfBuiltBuilding.Count; i++)
        {
            float iterationDistance = Distance2D(transform.position, listOfBuiltBuilding[i].transform.position);
            if (iterationDistance <= currentDistance)
            {
                currentClosest = listOfBuiltBuilding[i];
                currentDistance = iterationDistance;
            }
        }
        if (Distance2D(transform.position, StaticClass.Instance.player.transform.position) < currentDistance)
        {
            return StaticClass.Instance.player;
        }
        return currentClosest;
    }


    void Attacking()
    {

        
        if (Distance2D(transform.position,currentTarget.transform.position) <= attackRange)
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
        PlayerController playerController= currentTarget.GetComponent<PlayerController>();
        //SettingUpForAttack(playerController.leftAttackPoint.transform.position,playerController.rightAttackPoint.transform.position);
    }

    void AttackBuilding()
    {
        Building building = currentTarget.GetComponentInParent<Building>();
        
        if (!bDestroyingBuilding)
        {
            currentTarget = SettingUpForAttack(building.leftAttackPoint, building.rightAttackPoint);
            bDestroyingBuilding = true;
        }


        
        if(Distance2D(currentTarget.transform.position, transform.position) <= 0.1f ) 
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

    GameObject SettingUpForAttack(GameObject leftPoint,GameObject rightPoint)
    {

        if (Distance2D(transform.position, leftPoint.transform.position) <= Distance2D(transform.position, rightPoint.transform.position))
        {
            agent.stoppingDistance = 0;
            agent.SetDestination(leftPoint.transform.position);
            transform.forward = new Vector3(0, -1, 0);
            transform.localScale = new Vector3(1, 1, 1);
            return leftPoint;

        }
        else
        {
            agent.stoppingDistance = 0;
            agent.SetDestination(rightPoint.transform.position);
            transform.localScale = new Vector3(-1, 1, 1);
            return rightPoint;
        }
    }
    //need to update this
    private void Attacks()
    {

        Vector2 position2D = new Vector2(transform.position.x, transform.position.y);
        Vector2 right = new Vector2(transform.right.x, transform.right.y);
        RaycastHit2D[] hits = Physics2D.CircleCastAll(position2D + right * 0.2f, 0.3f, right, 0.5f);
        Debug.DrawLine(position2D + right * 0.2f, position2D + right * 0.85f, Color.red, 1f);
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

    float Distance2D(Vector3 first, Vector3 second)
    {
        return Mathf.Sqrt(Mathf.Pow((first.x - second.x),2) +  Mathf.Pow((first.y - second.y),2));
    }
}
