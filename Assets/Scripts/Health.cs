using UnityEngine;

public class Health: MonoBehaviour
{
	[SerializeField] protected float currentHealth;
	[SerializeField] protected float maxHealth;

	[SerializeField] protected float regenTimer = 10f;
	 protected float runningRegenTimer = 0f;
	//TO DO : herites this class in order to adapt to entities ?
    public virtual void Death()
	{
		Debug.Log(gameObject.name + " is dead"); //TO DO : update this code if necessary
        SoundManager.PlayRandomSoundFromType(SoundType.Die, 0.4f);
        Destroy(gameObject);
		//TO DO : add death sound / animation
	}
	public virtual bool TakingDamage(float damage)
	{
		Debug.Log(gameObject.name + " is taking " + damage);
		currentHealth -= damage;
		runningRegenTimer = 0;
		if (currentHealth <= 0)
		{	
			Death();
			return true;
		}
		return false;
	}

	public virtual bool HealingDamage(float damageHealed)
	{
		if (currentHealth >= maxHealth)
		{
			return false;
		}
		else
		{
			currentHealth += damageHealed;
			return true;
		}
	}

	public virtual void Reset()
	{
		currentHealth = maxHealth;
	}

	public float GetHealthPercent()
	{
		return currentHealth/maxHealth;
	}
	//TO DO: don't forget to remove this from other place in the game 
	public void passiveRegen()
    {
        if (runningRegenTimer > regenTimer)
        {
            runningRegenTimer = 0;
            currentHealth = maxHealth;
        }
        runningRegenTimer += Time.deltaTime;

    }

    public void Update()
    {
		passiveRegen();
    }
}