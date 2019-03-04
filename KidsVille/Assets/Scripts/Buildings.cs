using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    public int taxGold;
    [SerializeField] private bool isTax;
    private Material material;
    private bool outlineEnable;
    private bool moveOutline;
    private PlayerData player;
    private GameManager gm;

    void Start()
    {
        material = GetComponent<Renderer>().material;
        //print(material.name);
        player = FindObjectOfType<PlayerData>();
        gm = FindObjectOfType<GameManager>();

        if (isTax)
        {
            TurnOnOutline(true);
        }
    }


    // Enable and Disable the outline.
    public void TurnOnOutline(bool b)
    {
        float turnOn = 0;
        if (b)
        {
            turnOn = 1;
            print("Turn on outline.");
        }
        
        material.SetFloat("TurnOnOutline", turnOn);
    }

    public bool IsTax()
    {
        return isTax;
    }
    public void SetTax(bool b)
    {
        isTax = b;
    }

    public void CollectRates()
    {
        if (isTax)
        {
            player.GoldCalc(taxGold);
            TurnOnOutline(false);
            gm.ManageBuildLists(this);
        }
    }
}
