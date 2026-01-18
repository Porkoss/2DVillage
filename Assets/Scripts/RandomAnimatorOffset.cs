using Unity.VisualScripting;
using UnityEngine;

public class RandomAnimatorOffset : MonoBehaviour
{
    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetFloat("Offset", Random.value);
    }
}
