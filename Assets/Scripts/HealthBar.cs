using UnityEngine;

public class HealthBar : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] RectTransform currentHealthRectTransform;
    float maxSize;
    float height;
    public Health health;
    void Start()
    {
        maxSize = currentHealthRectTransform.rect.width;
        health = GetComponentInParent<Health>();
        height = currentHealthRectTransform.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        currentHealthRectTransform.sizeDelta = new Vector2(health.GetHealthPercent() * maxSize, height);
    }
}
