using UnityEngine;

public class EnnemyAnimationHandler : MonoBehaviour
{
    public void Hitframe()
    {
        EnnemyAI ennemyAI = GetComponentInParent<EnnemyAI>();
        
        if (ennemyAI == null)
        {
            Debug.Log("Problem with the attack of the ennemy against Player, wrong AI TYPE");
            return;
        }
        float damage = ennemyAI.damage;
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
}


