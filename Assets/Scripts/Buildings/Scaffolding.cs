using UnityEngine;

public class Scaffolding : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject UpStructure;
    [SerializeField] GameObject DownStructure;
    [SerializeField] GameObject Ladder;
    [SerializeField] GameObject Hammer;
    [SerializeField] GameObject VFX1;
    [SerializeField] GameObject VFX2;
    [SerializeField] GameObject VFX3;

    public bool bUpStructure = true;
    public bool bDownStructure = true;
    public bool bLadder = true;

    public bool bConstructionEnded = false;

    public void RemoveUpStructure()
    {
        UpStructure.SetActive(false);
        Hammer.SetActive(false);
        bUpStructure = false;
    }

    public void RemoveLadder()
    {
        Ladder.SetActive(false);
        bLadder = false;
    }

    public void RemoveDownStructure()
    {
        DownStructure.SetActive(false);
        bDownStructure = false;
    }
    

    public void PlayVFX()
    {
        VFX1.SetActive(true);
        VFX2.SetActive(true);
        VFX3.SetActive(true);
    }



    public void StepByStep()
    {
        if (bUpStructure)
        {
            RemoveUpStructure();
            PlayVFX();
            return;
        }
        if (bDownStructure)
        {
            RemoveDownStructure();
            PlayVFX();  
            return;
        }
        if (bLadder)
        {
            RemoveLadder();
            PlayVFX();
            bConstructionEnded = true;
        }
        bConstructionEnded = true;
    }

    public void EndConstruction()
    {
        //Fail Safe to finish Construction
        while (!bConstructionEnded)
        {
            StepByStep();
        }
    }
}
