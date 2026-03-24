
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;




public class EnnemyAI : CombatAI
{

    List<GameObject> listOfBuiltBuilding;

    public override void Start()
    {
        base.Start();
        listOfBuiltBuilding = StaticClass.Instance.listOfBuiltBuilding;
    }

    void AttackBuilding()
    {
        Building building = currentTarget.GetComponentInParent<Building>();

        if (Utilities.Distance2D(currentTarget.transform.position, transform.position) <= 0.1f)
        {
            if (currentAttackTimer >= attackTimer)
            {
                animator.SetTrigger("Attack2");
                currentAttackTimer = 0f;
                if (building.health.TakingDamage(damage))
                {
                    state = AIState.SeekingTarget;
                }
            }
            else currentAttackTimer += Time.deltaTime;
        }
        else
        {
            Vector3 destination = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y, transform.position.z);
            agent.SetDestination(destination);
        }
    }


    
    protected override void Attacks()
    {
        if (currentTarget.CompareTag("Ally")) 
        {
            AttacksCharacter();
        }
        else if (currentTarget.CompareTag("Building"))
        {
            AttackBuilding();
        }
        else if (currentTarget.CompareTag("Player"))
        {
            AttackPlayer();
        }
    }

    void AttackPlayer()
    {
        if (currentAttackTimer >= attackTimer)
        {
            animator.SetTrigger("Attack1");
            currentAttackTimer = 0f;
            ////TODO : move this into an animation controller to make it more realistic

        }
        else
        {

            currentAttackTimer += Time.deltaTime;
        }


    }

    protected override void FindTarget()
    {

        LayerMask enemyLayer = LayerMask.GetMask("Ally");
        Collider[] EnemyInRange = Physics.OverlapSphere(transform.position, DetectionRange, enemyLayer);
        currentTarget = StaticClass.Instance.player;
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
                    state = AIState.JoiningTarget;
                }
            }
        }
        GameObject closestBuilding = FindClosestBuilding();

        float targetDistance = Utilities.Distance2D(transform.position, currentTarget.transform.position);
        float buildingDistance = Utilities.Distance2D(transform.position, closestBuilding.transform.position);
        if(targetDistance >= buildingDistance)
        {
            currentTarget = closestBuilding;
            state = AIState.JoiningTarget;  
        }

        if (state != AIState.JoiningTarget) 
        {
            state = AIState.JoiningTarget;
            currentTarget = StaticClass.Instance.player;
        }

        if(state == AIState.JoiningTarget)
        {
            agent.SetDestination(currentTarget.transform.position);
        }

        
    }

    GameObject FindClosestBuilding()
    {
        if (listOfBuiltBuilding.Count == 0)
        {
            return StaticClass.Instance.player; ;
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

    protected override void SetUpForAttacks()
    {
        if (currentTarget.CompareTag("Player"))
        {
            agent.SetDestination(currentTarget.transform.position);
            if(Utilities.Distance2D(currentTarget.transform.position, transform.position) <= 0.5f)
            {
                AttackPlayer();
            }
        }
        else
        {
            base.SetUpForAttacks();
        }
    }
}
