using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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

    [SerializeField] bool bDestroyingBuilding = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoRepath = true;
        listOfBuiltBuilding = StaticClass.Instance.listOfBuiltBuilding;
        
    }

    // Update is called once per frame
    void Update()
    {
        // need to change in order to adapt to going left or right
        
        Debug.Log(Vector3.down);
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
        var currentDistance = Vector3.Distance(transform.position, listOfBuiltBuilding[0].transform.position);
        for (int i = 1; i < listOfBuiltBuilding.Count; i++)
        {
            float iterationDistance = Vector3.Distance(transform.position, listOfBuiltBuilding[i].transform.position);
            if (iterationDistance <= currentDistance)
            {
                currentClosest = listOfBuiltBuilding[i];
                currentDistance = iterationDistance;
            }
        }
        if (Vector3.Distance(transform.position, StaticClass.Instance.player.transform.position) < currentDistance)
        {
            return StaticClass.Instance.player;
        }
        return currentClosest;
    }


    void Attacking()
    {

        
        if (Vector3.Distance(transform.position,currentTarget.transform.position) <= attackRange)
        {
            if (currentTarget.CompareTag("Player") || currentTarget.CompareTag("Ally"))
            {
                AttackUnit();
            }
            else if (currentTarget.CompareTag("Building"))
            {
                AttackBuilding();
            }
        }
    }

    void AttackUnit()
    {
        
    }

    void AttackBuilding()
    {
        Building building = currentTarget.GetComponent<Building>();
        bDestroyingBuilding = true;
        if (Vector3.Distance(transform.position, building.leftAttackPoint.transform.position) <= Vector3.Distance(transform.position, building.rightAttackPoint.transform.position))
        {
            agent.stoppingDistance = 0;
            agent.SetDestination(building.leftAttackPoint.transform.position);
            transform.forward =  new Vector3(0, -1, 0);
            transform.localScale = new Vector3(1, 1, 1);


        }
        else
        {
            agent.stoppingDistance = 0;
            agent.SetDestination(building.rightAttackPoint.transform.position);
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
