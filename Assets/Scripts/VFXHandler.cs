using Unity.VisualScripting;
using UnityEngine;

public class VFXHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator animator;
    public bool bIsLooping=false;
    public bool bIsInstantiable = false;

    void OnEnable()
    {
        animator = GetComponent<Animator>();

    }



    public void OnAnimationEnded()
    {
        if (!bIsLooping)
        {
            gameObject.SetActive(false);
        }
        if (bIsInstantiable)
        {
            Destroy(gameObject);
        }
        
    }

}
