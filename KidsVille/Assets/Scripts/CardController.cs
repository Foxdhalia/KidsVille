using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    private GameManager gm;
    private PlayerData player;

    // ELEMENTOS DE CARTAS:
    private List<CardEvent> allEventsList = new List<CardEvent>(); // Todas as cartas de eventos do jogo.
    private List<CardEvent> backupEventsList = new List<CardEvent>(); // Backup de todas as cartas de eventos do jogo.
    private CardEvent[] tempEventCards = new CardEvent[2]; // Cartas que são sorteadas quando evento ocorre.
    private List<CardBonus> allBonusList = new List<CardBonus>(); // Todas as cartas de BONUS do jogo.
    private List<CardBonus> backupBonusList = new List<CardBonus>(); //  Backup de todas as cartas de BONUS do jogo.
    private CardBonus tempBonusCard;
    // Cartas objetivo (promessas) do jogo:
    private List<CardGoal> shortGoalsList = new List<CardGoal>(); // Cartas objetivo CURTAS.
    private List<CardGoal> mediumGoalsList = new List<CardGoal>(); // Cartas objetivo MÉDIAS.
    private List<CardGoal> longGoalsList = new List<CardGoal>(); // Cartas objetivo LONGAS.
    private List<CardGoal> allGoalsList = new List<CardGoal>(); // Todas as cartas objetivo da pilha.
    [HideInInspector] public List<CardGoal> activeGoalCards = new List<CardGoal>(); // Cartas objetivo (promessas) sorteadas do jogador.


    private int[] eventRaflle = new int[2];
    private int bonusRaffle;

    // Variaveis não expostas das Cartas de PROMESSA:
    private int displayedGoal;
    private Button[] goalArrows = new Button[2]; // 0 = back/left arrow; 1 = next/rigth arrow.


    [SerializeField] private int newGoalCardPrice;
    [Range(0f, 1f)] [SerializeField] private float penalty_ForPass;
    [Range(0f, 1f)] [SerializeField] private float penalty_NoGold;

    // ELEMENTOS DE UI: 
    [SerializeField] private GameObject warningPanel;

    [Header("UI -> Cartas de Evento (2).")]
    [SerializeField] private GameObject eventCardsPanel;
    [SerializeField] private TextMeshProUGUI[] tittleEvent = new TextMeshProUGUI[2];
    [SerializeField] private Text[] descriptionEvent = new Text[2];
    [SerializeField] private TextMeshProUGUI[] healthEvent = new TextMeshProUGUI[2];
    [SerializeField] private TextMeshProUGUI[] educationEvent = new TextMeshProUGUI[2];
    [SerializeField] private TextMeshProUGUI[] incomeEvent = new TextMeshProUGUI[2];
    [SerializeField] private TextMeshProUGUI[] costEvent = new TextMeshProUGUI[2];
    [SerializeField] private GameObject[] buyEvent_BoxBt = new GameObject[2];
    private Button[] buyEvent_Bt = new Button[2];

    [Header("UI -> Cartas de Promessas.")]
    [SerializeField] private GameObject goalCard_MainPanel;
    [SerializeField] private Button buyGoal_Bt;
    [SerializeField] private GameObject goalCard_LittlePanel;
    [SerializeField] private List<UI_GoalCard_Little> goalCards_UI = new List<UI_GoalCard_Little>();
    [SerializeField] private TextMeshProUGUI tittleGoal;
    [SerializeField] private Text descriptionGoal;
    [SerializeField] private TextMeshProUGUI healthGoal;
    [SerializeField] private TextMeshProUGUI educationGoal;
    [SerializeField] private TextMeshProUGUI incomeGoal;
    [SerializeField] private TextMeshProUGUI costGoal;
    [SerializeField] private HorizontalLayoutGroup layoutHorizontal;
    [SerializeField] private Button buyNewGoalButton;

    [Header("Sprites das Cartas de Promessas")]
    [Tooltip("0 = Saúde, 1 = Educação, 2 = Renda, 3 = Mix")]
    [SerializeField] private Sprite[] goalIcons = new Sprite[4];

    [Header("UI -> Carta de Bônus.")]
    [SerializeField] private GameObject bonusCardPanel;
    [SerializeField] private Text descriptionBonus;
    [SerializeField] private TextMeshProUGUI healthBonus;
    [SerializeField] private TextMeshProUGUI educationBonus;
    [SerializeField] private TextMeshProUGUI incomeBonus;
    [SerializeField] private TextMeshProUGUI costBonus;

    void Start()
    {
        // Acessa os arquivos *.csv na pasta Resources para extrair as informacoes em cartas de Objetivos/Metas.
        TextAsset goalsAsset = Resources.Load<TextAsset>("goalsCards");
        string[] lines = goalsAsset.text.Split(new char[] { '\n' });

        for (int i = 2; i < lines.Length - 1; i++)
        {
            string[] row = lines[i].Split(new char[] { '|' });
            CardGoal c = new CardGoal();
            c.type = row[0];
            c.id = int.Parse(row[1]);
            c.tittle = row[2];
            c.description = row[3];
            c.health = float.Parse(row[4]);
            c.education = float.Parse(row[5]);
            c.income = float.Parse(row[6]);
            c.cost = float.Parse(row[7]);
            switch (row[0])
            {
                case "Curto":
                    shortGoalsList.Add(c);
                    break;

                case "Médio":
                    mediumGoalsList.Add(c);
                    break;

                case "Longo":
                    longGoalsList.Add(c);
                    break;
            }
        }

        // Acessa os arquivos *.csv na pasta Resources para extrair as informacoes em cartas de Evento.
        TextAsset eventAsset = Resources.Load<TextAsset>("kvCards");
        lines = eventAsset.text.Split(new char[] { '\n' });
        for (int i = 2; i < lines.Length - 1; i++)
        {
            string[] row = lines[i].Split(new char[] { '|' });
            CardEvent c = new CardEvent();
            c.id = int.Parse(row[0]);
            c.tittle = row[1];
            c.description = row[2];
            c.health = float.Parse(row[3]);
            c.education = float.Parse(row[4]);
            c.income = float.Parse(row[5]);
            c.cost = float.Parse(row[6]);
            allEventsList.Add(c);
            backupEventsList.Add(c);
        }


        // Acessa os arquivos *.csv na pasta Resources para extrair as informacoes em cartas de BONUS.
        TextAsset bonusAsset = Resources.Load<TextAsset>("bonusCards");
        lines = eventAsset.text.Split(new char[] { '\n' });
        for (int i = 2; i < lines.Length - 1; i++)
        {
            string[] row = lines[i].Split(new char[] { '|' });
            CardBonus c = new CardBonus();
            c.id = int.Parse(row[0]);
            c.tittle = row[1];
            c.description = row[2];
            c.health = float.Parse(row[3]);
            c.education = float.Parse(row[4]);
            c.income = float.Parse(row[5]);
            c.cost = float.Parse(row[6]);
            allBonusList.Add(c);
            backupBonusList.Add(c);
        }

        // Escolhe 10 cartas de objetivo (goalCards) para o jogador ir cumprindo durante o jogo.
        ChoosingGoalCards();

        gm = GetComponent<GameManager>();
        player = FindObjectOfType<PlayerData>();

        layoutHorizontal.enabled = false;

        goalArrows[0] = goalCard_MainPanel.transform.GetChild(0).transform.GetChild(3).GetComponent<Button>();
        goalArrows[1] = goalCard_MainPanel.transform.GetChild(0).transform.GetChild(2).GetComponent<Button>();

        for (int i = 0; i < 2; i++)
        {
            buyEvent_Bt[i] = buyEvent_BoxBt[i].transform.GetChild(0).transform.GetChild(1).GetComponent<Button>();
        }
    }


    // Removendo uma carta de sua respectiva lista.
    public void RemoveCards(int listIndex, string cardType)
    {
        if (cardType == "event")
            allEventsList.RemoveAt(listIndex);
        else if (cardType == "bonus")
            allBonusList.RemoveAt(listIndex);
        else if (cardType == "goals")
            activeGoalCards.RemoveAt(listIndex);
        else if (cardType == "short goals")
            shortGoalsList.RemoveAt(listIndex);
        else if (cardType == "medium goals")
            mediumGoalsList.RemoveAt(listIndex);
        else if (cardType == "long goals")
            longGoalsList.RemoveAt(listIndex);
    }

    // Aplicando Penalidade por NÃO comprar NENHUMA carta (Evento):
    public void Penalty() // 0 ou 1 (carta da esquerda e da direita, respectivamente)
    {
        for (int n = 0; n < 2; n++)
        {
            if (tempEventCards[n].health > 0)
            {
                player.ChangeIndexValue(0, -tempEventCards[n].health);
            }
            if (tempEventCards[n].education > 0)
            {
                player.ChangeIndexValue(1, -tempEventCards[n].education);
            }
            if (tempEventCards[n].income > 0)
            {
                player.ChangeIndexValue(2, (-tempEventCards[n].income));
            }
        }
    }

    // Aplicar Penalidade por NÃO comprar uma carta (Evento e Bonus):
    void Penalty(int n) // 0 ou 1 (carta da esquerda e da direita, respectivamente)
    {        
        if (tempEventCards[n].health > 0)
        {
            player.ChangeIndexValue(0, -tempEventCards[n].health);
        }
        if (tempEventCards[n].education > 0)
        {
            player.ChangeIndexValue(1, -tempEventCards[n].education);
        }
        if (tempEventCards[n].income > 0)
        {
            player.ChangeIndexValue(2, (-tempEventCards[n].income));
        }        
    }




    //////////// EVENTOS  ////////////////////////////////////////////////////////////////////////////////////
    public void ChoosingEventCards()
    {
        gm.SetTimePassing(false);

        if (allEventsList.Count <= 5)
        {
            Debug.LogWarning("Backup: " + backupEventsList.Count);
            allEventsList.Clear();
            foreach (CardEvent c in backupEventsList)
            {
                allEventsList.Add(c);
            }
            Debug.LogWarning("Cartas de Evento recolocadas! All Even Cards: " + allEventsList.Count);

        }

        if (allEventsList.Count >= 5)
        {
            // Sorteia duas cartas de evento para serem mostradas ao jogador e as remove da lista "allEventsList", para que elas não se repitam.
            eventRaflle[0] = UnityEngine.Random.Range(1, allEventsList.Count);
            tempEventCards[0] = allEventsList[eventRaflle[0]];

            do
            {
                eventRaflle[1] = UnityEngine.Random.Range(1, allEventsList.Count);
                //Debug.Log(gm.PrintDate() + ". Cartas evento sorteadas: " + eventRaflle[0] + " e " + eventRaflle[1] + ".");
            } while (eventRaflle[1] == eventRaflle[0]);

            tempEventCards[1] = allEventsList[eventRaflle[1]];


            //Ativa o painel onde as cartas de evento aparecem e o preenche com os dados contidos nelas.
            eventCardsPanel.SetActive(true);
            for (int i = 0; i < 2; i++)
            {
                tittleEvent[i].text = tempEventCards[i].tittle;
                descriptionEvent[i].text = tempEventCards[i].description;
                healthEvent[i].text = tempEventCards[i].health.ToString();
                educationEvent[i].text = tempEventCards[i].education.ToString();
                incomeEvent[i].text = tempEventCards[i].income.ToString();
                costEvent[i].text = tempEventCards[i].cost.ToString();

                // Ativa/Desativa o botão para comprar a carta com base no ouro do jogador.
                if (player.gold >= tempEventCards[i].cost && !buyEvent_Bt[i].interactable)
                {
                    buyEvent_Bt[i].interactable = true;
                    buyEvent_Bt[i].GetComponent<Image>().color = Color.white;

                    buyEvent_Bt[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 1f;

                    buyEvent_BoxBt[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = Color.white;
                    buyEvent_BoxBt[i].transform.GetChild(1).GetComponent<Image>().color = Color.white;

                    costEvent[i].alpha = 1f;
                }
                else if (player.gold < tempEventCards[i].cost && buyEvent_Bt[i].interactable)// Desativa o botão para comprar a carta caso o jogador não tenha ouro suficiente. 
                {
                    buyEvent_Bt[i].interactable = false;
                    buyEvent_Bt[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 0.6f;

                    Color c = Color.white;
                    c = new Color(c.r, c.g, c.b, 0.6f);
                    buyEvent_Bt[i].GetComponent<Image>().color = c;

                    buyEvent_BoxBt[i].transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().color = c;
                    buyEvent_BoxBt[i].transform.GetChild(1).GetComponent<Image>().color = c;

                    costEvent[i].alpha = 0.6f;
                }
            }
        }
        else
        {
            gm.SetTimePassing(true);
        }
        gm.SetEventDay(false);
    }


    public void ApplyCardEvent(int n) // 0 ou 1 (carta da esquerda e da direita, respectivamente)
    {
        if (tempEventCards[n].health != 0)
        {
            player.ChangeIndexValue(0, tempEventCards[n].health);
        }
        if (tempEventCards[n].education != 0)
        {
            player.ChangeIndexValue(1, tempEventCards[n].education);
        }
        if (tempEventCards[n].income != 0)
        {
            player.ChangeIndexValue(2, tempEventCards[n].income);
        }
        player.GoldCalc(-tempEventCards[n].cost);

        // Aplicar penalidade sobre a carta não comprada:
        if (n == 0)
        {
            Penalty(1);
        }
        else
        {
            Penalty(0);
        }

        // Remove a carta de evento comprada da lista de cartas.
        RemoveCards(n, "event");
    }



    //////////// BÔNUS //////////////////////////////////////////////////////////////////////////////
    public void ChoosingBonusCards()
    {
        gm.SetTimePassing(false); // Pausa a contagem de dias da cidade.
        bonusRaffle = Random.Range(1, allBonusList.Count); // Sorteia um número entre as cartas bônus disponíveis
        tempBonusCard = allBonusList[bonusRaffle];

        bonusCardPanel.SetActive(true);
        descriptionBonus.text = tempBonusCard.description;
        healthBonus.text = tempBonusCard.health.ToString();
        educationBonus.text = tempBonusCard.education.ToString();
        incomeBonus.text = tempBonusCard.income.ToString();
        costBonus.text = tempBonusCard.cost.ToString();

        if (allBonusList.Count < 2)
        {
            allBonusList.Clear();
            foreach (CardBonus c in backupBonusList)
            {
                allBonusList.Add(c);
            }
            Debug.LogWarning("Cartas Bonus recolocadas! All Bonus Cards: " + allBonusList.Count);
        }
    }

    public void ApplyCardBonus()
    {
        if (tempBonusCard.health != 0)
        {
            player.ChangeIndexValue(0, tempBonusCard.health);
        }
        if (tempBonusCard.education != 0)
        {
            player.ChangeIndexValue(1, tempBonusCard.education);
        }
        if (tempBonusCard.income != 0)
        {
            player.ChangeIndexValue(2, tempBonusCard.income);
        }
        player.GoldCalc(-tempBonusCard.cost);

        RemoveCards(bonusRaffle, "bonus"); // Remove a carta de evento comprada da lista de cartas.
    }




    //////////// PROMESSAS (GOALS) ///////////////////////////////////////////////////////////////
    // Escolhe 10 cartas de objetivo (goalCards) para o jogador ir cumprindo durante o jogo.
    private void ChoosingGoalCards()
    {
        for (int i = 0; i < 4; i++) // 4 cartas de CURTO prazo.
        {
            int rnd = Random.Range(1, shortGoalsList.Count);
            CardGoal c = shortGoalsList[rnd];
            activeGoalCards.Add(c);
            RemoveCards(rnd, "short goals");
        }

        for (int i = 0; i < 4; i++) // 4 cartas de MÉDIO prazo.
        {
            int rnd = Random.Range(1, mediumGoalsList.Count);
            CardGoal c = mediumGoalsList[rnd];
            activeGoalCards.Add(c);
            RemoveCards(rnd, "medium goals");
        }

        for (int i = 0; i < 2; i++) // 2 cartas de LONGO prazo.
        {
            int rnd = Random.Range(1, longGoalsList.Count);
            CardGoal c = longGoalsList[rnd];
            activeGoalCards.Add(c);
            RemoveCards(rnd, "long goals");
        }

        // Posiciona as cartas de promessas na "mesa".
        DisplayGoalCardsHand();

        // Coloca todas as cartas de promessas em uma lista só, esvaziando as especificas.
        foreach (CardGoal c in shortGoalsList)
        {
            allGoalsList.Add(c);
        }
        foreach (CardGoal c in mediumGoalsList)
        {
            allGoalsList.Add(c);
        }
        foreach (CardGoal c in longGoalsList)
        {
            allGoalsList.Add(c);
        }
        shortGoalsList.Clear();
        mediumGoalsList.Clear();
        longGoalsList.Clear();
    }

    void DisplayGoalCardsHand()
    {
        layoutHorizontal.enabled = true;

        for (int i = 0; i < activeGoalCards.Count; i++)
        {
            //print("Display. activeGoalCards count: " + activeGoalCards.Count);
            goalCards_UI[i].SetCardPos(i);
            goalCards_UI[i].SetCardId(activeGoalCards[i].id);
            goalCards_UI[i].SetCardTerm(activeGoalCards[i].type);

            // Posicionando o símbolo do meio:
            if (activeGoalCards[i].tittle.Contains("Saude"))
            {
                goalCards_UI[i].SetCardSprite(goalIcons[0]);
            }
            else if (activeGoalCards[i].tittle.Contains("Educação"))
            {
                goalCards_UI[i].SetCardSprite(goalIcons[1]);
            }
            else if (activeGoalCards[i].tittle.Contains("Renda"))
            {
                goalCards_UI[i].SetCardSprite(goalIcons[2]);
            }
            else
            {
                goalCards_UI[i].SetCardSprite(goalIcons[3]);
            }

            //Debug.Log("Card " + i + ", Tittle " + activeGoalCards[i].tittle + ", id " + activeGoalCards[i].id);
        }
        layoutHorizontal.enabled = false;
    }

    void DisplayGoalCardsHand(int alteredCard)
    {
        layoutHorizontal.enabled = true;

        for (int i = 0; i < activeGoalCards.Count; i++)
        {
            //print("Display. activeGoalCards count: " + activeGoalCards.Count);
            goalCards_UI[i].SetCardPos(i);
            goalCards_UI[i].SetCardId(activeGoalCards[i].id);
            goalCards_UI[i].SetCardTerm(activeGoalCards[i].type);

            // Posicionando o símbolo do meio:
            if (activeGoalCards[i].tittle.Contains("Saude"))
            {
                goalCards_UI[i].SetCardSprite(goalIcons[0]);
            }
            else if (activeGoalCards[i].tittle.Contains("Educação"))
            {
                goalCards_UI[i].SetCardSprite(goalIcons[1]);
            }
            else if (activeGoalCards[i].tittle.Contains("Renda"))
            {
                goalCards_UI[i].SetCardSprite(goalIcons[2]);
            }
            else
            {
                goalCards_UI[i].SetCardSprite(goalIcons[3]);
            }

            //Debug.Log("Card " + i + ", Tittle " + activeGoalCards[i].tittle + ", id " + activeGoalCards[i].id);
        }

        if (alteredCard > 0)
        {
            goalCards_UI[activeGoalCards.Count - 1].gameObject.SetActive(true);

        }
        else if (alteredCard < 0 && activeGoalCards.Count < 10)
        {
            for (int i = 9; i > activeGoalCards.Count - 1; i--)
            {
                goalCards_UI[i].gameObject.SetActive(false);
            }
        }
        layoutHorizontal.enabled = false;
    }

    public void OpenGoalCard_UI(int cardIndexPos)
    {
        goalCard_MainPanel.SetActive(true);

        displayedGoal = cardIndexPos;

        tittleGoal.text = activeGoalCards[cardIndexPos].tittle;
        descriptionGoal.text = activeGoalCards[cardIndexPos].description;
        healthGoal.text = activeGoalCards[cardIndexPos].health.ToString();
        educationGoal.text = activeGoalCards[cardIndexPos].education.ToString();
        incomeGoal.text = activeGoalCards[cardIndexPos].income.ToString();
        costGoal.text = activeGoalCards[cardIndexPos].cost.ToString();

        if (displayedGoal <= 0) // Desabilita o botão seta "Back" (left).
        {
            goalArrows[0].interactable = false;
            goalArrows[1].interactable = true;
        }
        else if (displayedGoal >= (activeGoalCards.Count - 1)) // Desabilita o botão seta "Next" (right).
        {
            goalArrows[0].interactable = true;
            goalArrows[1].interactable = false;
        }
        else if (activeGoalCards.Count == 1) // Desbilita ambas as setas caso só haja uma carta.
        {
            goalArrows[0].interactable = false;
            goalArrows[1].interactable = false;
        }
        else
        {
            goalArrows[0].interactable = true;
            goalArrows[1].interactable = true;
        }

        // Ativa/Desativa o botão para comprar a carta de promessa.
        if (buyGoal_Bt.interactable && activeGoalCards[displayedGoal].cost > player.gold)
        {
            Color a = new Color(1f, 1f, 1f, 0.6f);
            buyGoal_Bt.interactable = false;
            buyGoal_Bt.GetComponent<Image>().color = a;
            buyGoal_Bt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 0.6f;

            Transform goParent = buyGoal_Bt.transform.parent;
            goParent.transform.GetChild(0).GetComponent<Image>().color = a; // Base do botão.
            goParent.transform.parent.transform.GetChild(1).GetComponent<Image>().color = a;
            goParent.transform.parent.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 0.6f;
        }
        else if (!buyGoal_Bt.interactable && activeGoalCards[displayedGoal].cost <= player.gold)
        {
            Color a = new Color(1f, 1f, 1f, 1f);
            buyGoal_Bt.interactable = true;
            buyGoal_Bt.GetComponent<Image>().color = a;
            buyGoal_Bt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 1f;

            Transform goParent = buyGoal_Bt.transform.parent;
            goParent.transform.GetChild(0).GetComponent<Image>().color = a;
            goParent.transform.parent.transform.GetChild(1).GetComponent<Image>().color = a;
            goParent.transform.parent.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 1f;
        }
    }

    // Para as setas no Painel que mostra cada uma das Cartas Promessas por vez.
    public void OtherGoalCard_UI(int operatorCalc) // Valor negativo ou positivo.
    {
        if ((displayedGoal <= 0 && operatorCalc == -1) || (displayedGoal >= (activeGoalCards.Count - 1) && operatorCalc == 1))
        {
            return;
        }

        displayedGoal += operatorCalc;

        tittleGoal.text = activeGoalCards[displayedGoal].tittle;
        descriptionGoal.text = activeGoalCards[displayedGoal].description;
        healthGoal.text = activeGoalCards[displayedGoal].health.ToString();
        educationGoal.text = activeGoalCards[displayedGoal].education.ToString();
        incomeGoal.text = activeGoalCards[displayedGoal].income.ToString();
        costGoal.text = activeGoalCards[displayedGoal].cost.ToString();

        if (displayedGoal <= 0) // Desabilita o botão seta "Back".
        {
            goalArrows[0].interactable = false;
        }
        else if (displayedGoal >= (activeGoalCards.Count - 1)) // Desabilita o botão seta "Next".
        {
            goalArrows[1].interactable = false;
        }

        // Ativa/Desativa o botão para comprar a carta de promessa.
        if (buyGoal_Bt.interactable && activeGoalCards[displayedGoal].cost > player.gold)
        {
            Color a = new Color(1f, 1f, 1f, 0.6f);
            buyGoal_Bt.interactable = false;
            buyGoal_Bt.GetComponent<Image>().color = a;
            buyGoal_Bt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 0.6f;

            Transform goParent = buyGoal_Bt.transform.parent;
            goParent.transform.GetChild(0).GetComponent<Image>().color = a; // Base do botão.
            goParent.transform.parent.transform.GetChild(1).GetComponent<Image>().color = a;
            goParent.transform.parent.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 0.6f;
        }
        else if (!buyGoal_Bt.interactable && activeGoalCards[displayedGoal].cost <= player.gold)
        {
            Color a = new Color(1f, 1f, 1f, 1f);
            buyGoal_Bt.interactable = true;
            buyGoal_Bt.GetComponent<Image>().color = a;
            buyGoal_Bt.transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 1f;

            Transform goParent = buyGoal_Bt.transform.parent;
            goParent.transform.GetChild(0).GetComponent<Image>().color = a;
            goParent.transform.parent.transform.GetChild(1).GetComponent<Image>().color = a;
            goParent.transform.parent.transform.GetChild(1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 1f;
        }
    }

    public void ApplyCardGoal()
    {
        // Verifica se o jogador pode comprar a carta.
        if (activeGoalCards[displayedGoal].cost <= player.gold)
        {
            if (activeGoalCards[displayedGoal].health != 0)
            {
                player.ChangeIndexValue(0, activeGoalCards[displayedGoal].health);
            }
            if (activeGoalCards[displayedGoal].education != 0)
            {
                player.ChangeIndexValue(1, activeGoalCards[displayedGoal].education);
            }
            if (activeGoalCards[displayedGoal].income != 0)
            {
                player.ChangeIndexValue(2, activeGoalCards[displayedGoal].income);
            }
            player.GoldCalc(-activeGoalCards[displayedGoal].cost);


            // Remove a carta Promessa aplicada da lista de cartas de promessas ativa.
            RemoveCards(displayedGoal, "goals"); // Remove a carta da lista activeGoalCards.        

            // Remove a carta Promessa aplicada da lista de cartas de promessas UI.
            layoutHorizontal.enabled = true;
            DisplayGoalCardsHand(-1); // Retira a carta aplicada.
        }
    }

    public void BuyCardGoal()
    {
        // Verifica se o jogador pode comprar a nova carta de promessa.
        if (player.gold >= newGoalCardPrice && allGoalsList.Count > 0)
        {
            print("Cartas ativas antes da compra: " + activeGoalCards.Count + ", Cartas da pilha: " + allGoalsList.Count);
            if (activeGoalCards.Count < 10 && player.gold >= newGoalCardPrice)
            {
                int rdm = Random.Range(0, allGoalsList.Count);
                activeGoalCards.Add(allGoalsList[rdm]);
                allGoalsList.RemoveAt(rdm);
                DisplayGoalCardsHand(1); // Adiciona a carta comprada na UI.
                player.GoldCalc(-newGoalCardPrice);

                if (allGoalsList.Count <= 0)
                {
                    buyNewGoalButton.interactable = false;
                }
            }
            else if (activeGoalCards.Count >= 10)
            {
                warningPanel.SetActive(true);
                warningPanel.transform.GetChild(1).GetComponent<Text>().text = "Você já tem o máximo de promessas permitidas!";
            }
        }
        else if (player.gold < newGoalCardPrice && allGoalsList.Count > 0)
        {
            warningPanel.SetActive(true);
            warningPanel.transform.GetChild(1).GetComponent<Text>().text = "Você não tem recursos suficientes para compra!";
        }

        // Verifica se ainda existe cartas de promessa para serem compradas. Em caso negativo, desabilita o monte.
        if (allGoalsList.Count <= 0)
        {
            Debug.Log("All goal cards already shopped!");
            buyNewGoalButton.gameObject.SetActive(false);
            buyNewGoalButton.transform.parent.transform.parent.transform.parent.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
        }
    }
}

public class CardEvent
{
    public int id;
    public string tittle;
    public string description;
    public float health;
    public float education;
    public float income;
    public float cost;

}

public class CardGoal
{
    public string type; // Curto, Médio, Longo.
    public int id;
    public string tittle;
    public string description;
    public float health;
    public float education;
    public float income;
    public float cost;

}

public class CardBonus
{
    public int id;
    public string tittle;
    public string description;
    public float health;
    public float education;
    public float income;
    public float cost;
}