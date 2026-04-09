using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine.Rendering.Universal;
using System.Diagnostics.CodeAnalysis;
[RequireComponent(typeof(Health))]
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
    [SerializeField] private GameObject VillagerCostCanva;
    [SerializeField] private GameObject GeneratedVillagerCanva;
    [SerializeField] private GameObject GeneratedArmyCanva;

    private List<GameObject> canvas;

    private Animator playerAnimator;

    private Inventory inventory;
    [HideInInspector]
    public Health health;

    [Header("Army")]
    [SerializeField] private GameObject soldierPrefab;
    private List<GameObject> towerArmy = new List<GameObject>();
    private int ArmyScale;
    [SerializeField] private List<GameObject> spawnPoints;

    [Header("Population")]
    [SerializeField] private int population;

    

    [Header("Destruction")]
    [SerializeField] private List<GameObject> fireVFX;
    public int destructionStep = 2; //3 step in destruction 2 => 0
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
        UpdateCanva(VillagerCostCanva);
        UpdateCanva(GeneratedVillagerCanva);
        UpdateCanva(GeneratedArmyCanva);
        if (!bBuilt)
        {
            AlphaStartState();
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

        health =GetComponent<Health>();
        
    }



    private void AlphaStartState()
    {
        Color color = spriteRenderer.color;
        color.a = startAlpha;
        color.r = startIntensity;
        color.g = startIntensity;
        color.b = startIntensity;
        spriteRenderer.color = color;
    }
    private void BuildIterative()
    { 
        //This part is hard coded with value 0.6 and 0.8 as the start for startAlpha and startIntensity because it won't ever change lol ( //TODO: update if needed)


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
        GenerateArmy();
        CreateVillager();
    }


    public bool TryOneBrick()
    {
        // going trought inventory to find resources that can be substracted from the building needs and removing it + handling UI then making a check if the building is over 
       
        if (TryToHeal())
        {
            return true;
        }
        //adding enough villager check and hiding canva.
        CanvaValue VillagerCanvaClass = VillagerCostCanva.GetComponent<CanvaValue>();
        if (inventory.villager >= VillagerCanvaClass.value)
        {
            inventory.villager -= VillagerCanvaClass.value;
            VillagerCanvaClass.value = 0;
            VillagerCostCanva.SetActive(false);
            
        }
        else
        {
            return false;
        }
        
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

    public bool TryToHeal()
    {
        if (bBuilt)
        {
            return health.HealingDamage(1);
            
        }
        else
        {
            return false;
        }
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

    #region Army
    protected  void GenerateArmy()
    {
        ArmyScale = spawnPoints.Count;
        for (int i = 0; i < ArmyScale; i++)
        {
            towerArmy.Add(Instantiate(soldierPrefab, spawnPoints[i].transform.position, Quaternion.identity));
        }

    }

    //call when wave is over;
    public void RestoreArmy()
    {
        for (int i =0; i < ArmyScale; ++i) 

        {
            GameObject army = towerArmy[i];
            if(army == null)
            {
                army = Instantiate(soldierPrefab, spawnPoints[i].transform.position, Quaternion.identity);
            }
            
        }
    }
    #endregion Army

    #region Population 
    public void CreateVillager()
    {
        inventory.villager += GeneratedVillagerCanva.GetComponent<CanvaValue>().maxValue;
        inventory.villagerMax += GeneratedVillagerCanva.GetComponent<CanvaValue>().maxValue;
        VillagerCostCanva.SetActive(false);
        GeneratedVillagerCanva.SetActive(false);
        GeneratedArmyCanva.SetActive(false);
    }
    public void RemoveVillager()
    {
        inventory.villager -= GeneratedVillagerCanva.GetComponent<CanvaValue>().maxValue;
        inventory.villagerMax -= GeneratedVillagerCanva.GetComponent<CanvaValue>().maxValue;
    }
    #endregion 


    #region Destruction

    //called by the health function ?
    public void StepByStepFireDestruction()
    {
        Debug.Log("Destruction step "+ destructionStep);
        fireVFX[destructionStep].SetActive(true);
        destructionStep--;
    
    }
    //allow player to extinguish fire, need to call for a healing method too on the building health class
    public void StepByStepFireReconstruction()
    {
        if(destructionStep < fireVFX.Count)
        {
            destructionStep++;
            fireVFX[destructionStep].SetActive(false);
        }
        if(destructionStep == 2)
        {
            ResetToRebuild();
        }
    }


    public void ResetToRebuild()
    {
        scaffolding.ResetToStart();
        AlphaStartState();
        foreach (GameObject canva in canvas)
        {
            canva.SetActive(true);
            canva.GetComponent<CanvaValue>().Reset();
        }
        foreach (GameObject fire in fireVFX)
        {
            fire.SetActive(false);
        }
        destructionStep = 2;
        health.Reset();
        bBuilt = false;
    }


    #endregion Destruction
}
