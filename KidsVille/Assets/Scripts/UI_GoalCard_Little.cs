using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GoalCard_Little : MonoBehaviour
{
    private GameManager gm;
    private CardController cardController;
    private RectTransform rt;
    private Vector2 normalSize;

    [SerializeField] private int cardPos;
    [SerializeField] private int cardId;
    [SerializeField] private Image cardIcon;

    // Prazos: 0 = curto, 1 = medio, 2 = longo
    private string cardTerm;


    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        cardController = FindObjectOfType<CardController>();
        rt = GetComponent<RectTransform>();
        normalSize = rt.anchorMax;
    }

    public void GrowCard()
    {
        rt.anchorMax *= 2.4f;
    }

    public void NormalSize()
    {
        rt.anchorMax = normalSize;
    }

    public void OpenCard()
    {
        gm.SetCheckingGoals(true); // Pausa a contagem de tempo do jogo.
        cardController.OpenGoalCard_UI(cardPos);  // Abre a respectiva carta clicada no painel maior.      
    }


    // GETTERS AND SETTERS: 
    public void SetCardSprite(Sprite icon)
    {
        cardIcon.sprite = icon;
    }


    public int GetCardPos()
    {
        return cardPos;
    }
    public void SetCardPos(int indexPos)
    {
        cardPos = indexPos;
    }

    public int GetCardId()
    {
        return cardId;
    }
    public void SetCardId(int id)
    {
        cardId = id;
    }


    public string GetCardTerm()
    {
        return cardTerm;
    }
    public void SetCardTerm(string deadline)
    {
        cardTerm = deadline;
    }
}
