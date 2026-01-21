using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class Building : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    SpriteRenderer spriteRenderer;

    
    [SerializeField] private float startAlpha=0.4f;
    [SerializeField] private float startIntensity = 0.7f;
    [SerializeField] private GameObject scaffoldingPrefab;
    private Scaffolding scaffolding;
    public bool bBuilt = false;

    public GameObject leftAttackPoint;
    public GameObject rightAttackPoint;
  
    [Header("Ressources")]


    [Header("Canvas")]
    [SerializeField] private GameObject goldCanva;
    [SerializeField] private GameObject woodCanva;
    [SerializeField] private GameObject stoneCanva;

    private List<GameObject> canvas;

    private Animator playerAnimator;

    private Inventory inventory;

    void Start()
    {
        scaffolding = scaffoldingPrefab.GetComponent<Scaffolding>();
        if (StaticClass.Instance)
        {
            playerAnimator = StaticClass.Instance.player.GetComponent<Animator>();
            inventory = StaticClass.Instance.inventory;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        canvas = new List<GameObject>
        {
            goldCanva,
            woodCanva,
            stoneCanva
        };
        foreach (GameObject go in canvas)
        {
            UpdateCanva(go);
        }
        if (!bBuilt)
        {
            Color color = spriteRenderer.color;
            color.a = startAlpha;
            color.r = startIntensity;
            color.g = startIntensity;
            color.b = startIntensity;
            spriteRenderer.color = color;
        }
        else
        {
            EndBuild();
            foreach (GameObject go in canvas)
            {
                go.SetActive(false);
            }
        }
        //Make the Start value match in UI


        
    }

    private void BuildIterative()
    { 
        //This part is hard coded with value 0.6 and 0.8 as the start for startAlpha and startIntensity because it won't ever change lol ( TO DO: update if needed)


        Color color = spriteRenderer.color;
        color.a = color.a + 0.10f;
        color.r = color.r + 0.10f;
        color.g = color.g + 0.10f;
        color.b = color.b + 0.10f;
        spriteRenderer.color = color;
        if (scaffolding.bConstructionEnded)
        {
            EndBuild();
        }
        else
        {
            SoundManager.PlayRandomSoundFromType(SoundType.BuildStep, 1f);
        }

    }
    private void EndBuild()
    {
        scaffolding.EndConstruction();
        bBuilt = true;
        Color color = spriteRenderer.color;
        color.a = 1f;
        color.r = 1f;
        color.g = 1f;
        color.b = 1f;
        spriteRenderer.color = color;
        SoundManager.PlayRandomSoundFromType(SoundType.BuildOver, 1f);
        StaticClass.Instance.listOfBuiltBuilding.Add(gameObject);
    }


    public bool TryOneBrick()
    {
        //GL rereading that
        // going trought inventory to find resources that can be substracted from the building needs and removing it + handling UI then making a check if the building is over 
        // TO DO animate evolution bricks by brick
        for (int i = canvas.Count - 1; i >= 0; i--)
        {

            GameObject go = canvas[i];
            if (go.GetComponent<CanvaValue>().value <= 0)
            {
                Debug.Log("Should not have already zero value in looked canvas");
                return false;
            }
            if (inventory.Substract(go.GetComponent<CanvaValue>()))
            {
                if (go.GetComponent<CanvaValue>().value == 1)
                {
                    go.GetComponent<CanvaValue>().value--;
                    UpdateCanva(go);
                    go.SetActive(false);
                    canvas.RemoveAt(i);
                    ReplaceCanva(canvas);
                    //Play Construction step animation
                    scaffolding.StepByStep(); //scaffolding part
                    BuildIterative(); // transparcy part

                    return true;
                }
                else
                {
                    go.GetComponent<CanvaValue>().value--;
                    UpdateCanva(go);
                    //Play Standard construction animation
                    scaffolding.PlayVFX();
                    return true;
                }
            }
            else
            {
                continue;
            }
        }

        return false;
    }

    



    private void UpdateCanva(GameObject go)
    {
        TextMeshProUGUI text = go.GetComponentInChildren<TextMeshProUGUI>();
        text.text = go.GetComponent<CanvaValue>().value.ToString();
        //go.GetComponent<CanvaValue>().value = value; not needed anymore
    }

    private void ReplaceCanva(List<GameObject> GOs)
    {
        switch (GOs.Count)
        {
            case 3:
                
                GOs[0].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(-120, 0);
                GOs[1].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(0, 0);
                GOs[2].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(120, 0);
                break;              
            case 2:                 
                GOs[0].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(-60, 0);
                GOs[1].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(60, 0);
                break;             
            case 1:                 
                GOs[0].GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(0, 0);
                break;
            case 0:
                break;
            default:
                Debug.Log("Invalid number of canvas");
                break;
                
        }
    }
    
}
