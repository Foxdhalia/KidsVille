using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    public int taxGold;
    [SerializeField] private bool isTax;
    [SerializeField] private GameObject goldImg;
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

        taxGold = gm.GetTaxValue();

        if (isTax)
        {
            Blink(true);
        }
    }


    // Enable and Disable the outline.
    public void Blink(bool b)
    {
        float turnOn = 0;
        if (b)
        {
            player.audio.PlayOneShot(player.getRates);
            turnOn = 1;
            goldImg.SetActive(true);
            //print("Turn on outline.");
        }

        material.SetFloat("RampActive", turnOn);
    }

    // Enable and Disable the outline.
    public void TurnOnOutline(bool b)
    {
        float turnOn = 0;
        if (b)
        {
            turnOn = 1;
            goldImg.SetActive(true);
            //print("Turn on outline.");
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
            isTax = false;
            //player.audio.PlayOneShot(player.getRates);
            player.GoldCalc(taxGold);
            //TurnOnOutline(false);
            Blink(false);
            goldImg.SetActive(false);
            gm.ManageBuildLists(this);
        }
    }
}
