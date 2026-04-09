using UnityEngine;

public class HealthBuilding : Health
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int DestructionStep=2;
    private Building building;

    private void Start()
    {
        building = GetComponent<Building>();
    }
    public override void Death()
    {
        Debug.Log(gameObject.name + " is dead");
        //in order to avoid restore Army : 
        StaticClass.Instance.listOfBuiltBuilding.Remove(gameObject);
        //in order to avoid using village from destroyed building
        building.RemoveVillager();
    }

    public override bool TakingDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth<= (DestructionStep/3.0f) * maxHealth)
        {
            building.StepByStepFireDestruction();
            DestructionStep--;
        }

        
        if (currentHealth <= 0)
        {
            Death();
            return true;
        }
        return false;
    }

    public override void Reset()
    {
        DestructionStep = 2;
        base.Reset();
    }
    
    public override bool HealingDamage(float damageHealed)
    {
        if (currentHealth > (DestructionStep / 3.0f) * maxHealth)
        {
            building.StepByStepFireReconstruction();
            DestructionStep++;
        }
        return base.HealingDamage(damageHealed);
    }

}
