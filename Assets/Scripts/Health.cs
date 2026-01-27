using UnityEngine;

public class Health: MonoBehaviour
{
	[SerializeField] protected float currentHealth;
	[SerializeField] protected float maxHealth;
	//TO DO : herites this class in order to adapt to entities ?
    public virtual void Death()
	{
		Debug.Log(gameObject.name + " is dead"); //TO DO : update this code if necessary
		Destroy(gameObject);
		//TO DO : add death sound / animation
	}
	public virtual bool TakingDamage(float damage)
	{
		Debug.Log(gameObject.name + " is taking " + damage);
		currentHealth -= damage;
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
}