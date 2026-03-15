
using System.Collections.Generic;
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
                bool bTargetDestroyed = building.health.TakingDamage(damage);

                if (bTargetDestroyed)
                {
                    bIsInCombat = false;
                    bHasTarget = false;
                }
                //TO DO : "Add lancer flamme aniamtion"
            }
            else
            {
                currentAttackTimer += Time.deltaTime;
            }
        }

    }

    protected override void Attacks()
    {
        if (currentTarget.CompareTag("Player")) // || currentTarget.CompareTag("Ally")  TO DO : update for ally
        {
            AttacksCharacter();
        }
        else if (currentTarget.CompareTag("Building"))
        {
            AttackBuilding();
        }
    }

    protected override void FindTarget()
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
        GameObject closestBuilding = FindClosestBuilding();

        float targetDistance = Utilities.Distance2D(transform.position, currentTarget.transform.position);
        float buildingDistance = Utilities.Distance2D(transform.position, closestBuilding.transform.position);
        if(targetDistance >= buildingDistance)
        {
            currentTarget = closestBuilding;
            bHasTarget = true;
        }

        if (!bHasTarget) 
        {
            bHasTarget = true;
            currentTarget = StaticClass.Instance.player;
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
}
