using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MainScript
{
    private CardController cardController;
    private PlayerData player;

    public static  bool continueGame; // Para no futuro gerenciar se o jogador já entrou ou não no jogo antes.
    //public static  bool timePassing; // Para gerenciar se a contagem dos dias está correndo ou não.
    public static bool checkingGoals; // Pausa a contagem do tempo quando uma carta objetivo estiver aberta na tela.

    private float dayTimer; // variavel interna do código para contagem do tempo do dia.

    private float eventChance; // Chance de sair o evento. Pode ser incrementada.
    private int countDays; // Contagem de dias para gerenciar o incremento da chance de sair o evento.

    // Usada para aguardar o momento em que aparecem 2 eventos para o jogador escolher:
    private bool eventDay;

    // Usada para aguardar o momento em que o jogador deve pegar/passar o bônus (todo dia 30):
    private int bonusDay; // 0 = defaul; 1 = jogador pegou o bônus; 2 = jogador PASSOU o bônus.

    // Usada para aguardar o momento em que os cálculos mensais são apurados (todo dia 5):
    private bool monthReportDay;

    //Taxas recolhidas dos prédios da cidade:
    private List<Buildings> buildings = new List<Buildings>();
    private List<Buildings> buildingsTaxed = new List<Buildings>();
    private int countDaysTax;
    

    [Header("Duração do jogo (máximo de anos):")]
    [SerializeField] private int maxYears = 4;

    [Header("Duração de cada dia:")]
    [SerializeField] private float daySpeed; // Permite acelerar ou diminuir a passagem do tempo.
    [SerializeField] private float dayDuration; // É a duracao maxima do dia.

    [Header("Variáveis para os Eventos.")]
    [SerializeField] private int eventChanceStart = 0;
    [SerializeField] private int daysToIncrement_eventChance = 5;

    [Header("Variáveis para taxas dos prédios.")]
    [Tooltip("Valor da taxa dos prédios")]
    [SerializeField] private int taxValue = 10;
    [Tooltip("Dias para incremento da taxa dos prédios")]
    [SerializeField] private int daysToIncrement_buildsTax = 5;    
    [Tooltip("Chance inicial para a taxa dos prédios (em %)")]
    [SerializeField] private float taxChanceStart = 30f; // porcentagem

    // ELEMENTOS DE UI:
    [Header("UI - > Calendário: Ano, Mês, Dia.")]
    [SerializeField] private Text[] calendar = new Text[3];
        

    void Start()
    {
        eventChance = eventChanceStart;
        cardController = GetComponent<CardController>();
        player = FindObjectOfType<PlayerData>();
        //timePassing = true;
        NewYear();

        Buildings[] tempBuild = FindObjectsOfType<Buildings>();
        foreach(Buildings build in tempBuild)
        {
            buildings.Add(build);
        }
        Debug.Log("Buildings found: " + buildings.Count);
    }

    void Update()
    {
        if (!finishGame)
        {
            if (timePassing && !checkingGoals)
            {
                dayTimer += Time.deltaTime * daySpeed;
                try
                {
                    if (dayTimer > dayDuration)
                    {
                        if (day < 30)
                        {
                            //print("Dia " + day + " terminado com tempo de " + dayTimer + ".");                    
                            day++;
                            calendar[2].text = day.ToString();
                            dayTimer = 0;

                            if ((day == 5 && year > 1) || (day == 5 && month > 1 && year == 1))
                            {
                                MonthReportDay();
                            }
                            else if (day == 30 && month == 12)
                            {
                                StartCoroutine(BonusDay());
                            }
                            else
                            {
                                CheckForNewTaxes();
                                EventChance();
                            }
                        }
                        else
                        {
                            NewMonth();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("ALGO DE ERRADO NÃO ESTÁ CERTO!");
                }
            }
        }
    }

    private void CheckForNewTaxes()
    {
        if (buildings.Count > 0)
        {
            if (countDaysTax >= daysToIncrement_buildsTax)
            {
                Debug.Log("Dia de verificar se há taxa. " + day + " / " + month + " / " + year);
                countDaysTax = 0;

                float taxChance = (float)player.GetPopulation() * 0.1f;
                taxChance += taxChanceStart;
                if (taxChance >= 100f)
                {
                    taxChance = 100f;
                }
                float raffle = UnityEngine.Random.Range(0f, 100f);
                Debug.Log("Número sorteado para taxa = " + raffle + ". Chance da taxa = " + taxChance + "%");

                if (raffle <= taxChance)
                {
                    int r = UnityEngine.Random.Range(0, buildings.Count);
                    Debug.Log("Prédio " + r + " com taxas.");
                    buildings[r].SetTax(true);
                    //buildings[r].TurnOnOutline(true);
                    buildings[r].Blink(true);
                    buildingsTaxed.Add(buildings[r]);
                    buildings.RemoveAt(r);
                }
            }
            else
            {
                countDaysTax++;
            }
        }
    }

    private bool EventChance() // Controla a chance diaria de sair algum evento.
    {
        if(day < 5 && month == 1 && year == 1) // Para o evento não sair log de cara.
        {
            return true;
        }

        timePassing = false;

        float raffle = UnityEngine.Random.Range(1f, 100f);
        //print("Número sorteado: " + raffle + ". Chance de sair o evento: " + eventChance + "% .");

        if (raffle <= eventChance)
        {
            eventDay = true;
            cardController.ChoosingEventCards();
            eventChance = eventChanceStart; // 0
            countDays = 0;
        }
        else if (countDays >= daysToIncrement_eventChance) // daysToIncrement_eventChance = 5
        {
            eventDay = false;
            eventChance++;
            countDays = 0;
           // print("Chance de sair o evento incrementada para: " + eventChance + "% .");
            timePassing = true;
        }
        else if (countDays < daysToIncrement_eventChance) // daysToIncrement_eventChance = 5
        {
            eventDay = false;
            countDays++;
            timePassing = true;
        }
        
        return true;
    }

    private void MonthReportDay() // Todo o dia 5.
    {
        timePassing = false;
        StartCoroutine(player.MonthReport_Calculations(false));
    }

    public void EndGame()
    {
        timePassing = false;
        StartCoroutine(player.MonthReport_Calculations(true));       
    }

    private IEnumerator BonusDay() // Todo dia 30 de dezembro (12).
    {
        timePassing = false;        
        cardController.ChoosingBonusCards();
        print("Aguardando o jogador pegar ou passar o bônus do dia " + day + " do mês " + month + " do ano " + year + ".");
        yield return new WaitUntil(() => bonusDay != 0);
        if (bonusDay == 1) // Se o bonusDay retornar como 1, significa que o jogador comprou o bônus.
        {
            cardController.ApplyCardBonus();
            bonusDay = 0;
        }
        else if (bonusDay > 1) // Se o bonusDay retornar como 2, significa que o jogador NÃO comprou o bônus (passou ou não tinha dinheiro).
        {
            bonusDay = 0;
        }
        timePassing = true;

        // Sortendo a possibilidade de evento no dia 30/12.
        /*bool b = false;
        b = EventChance();
        yield return new WaitUntil(() => b == true);
        if (eventDay)
        {
            timePassing = false;
            eventDay = false;
        }
        else
        {
            timePassing = true;
        }*/
    }

    private void NewYear()
    {
        print("ANO " + year + " terminado. Mês " + month + " terminado. Dia " + day + " terminado com tempo de " + dayTimer + ".");
        day = 1;
        month = 1;
        year++;
        calendar[2].text = day.ToString();
        calendar[1].text = month.ToString();
        calendar[0].text = year.ToString();
        dayTimer = 0;
        EventChance();
    }

    private void NewMonth()
    {
        if (month < 12)
        {
            print("Mês " + month + " terminado. Dia " + day + " terminado com tempo de " + dayTimer + ".");
            day = 1;
            month++;
            calendar[2].text = day.ToString();
            calendar[1].text = month.ToString();
            dayTimer = 0;
            EventChance();
        }
        else
        {
            if (year < maxYears)
            {
                NewYear();
            }
            else
            {
                timePassing = false;
                EndGame();
                print("Finish GAME!");
                print("ANO " + year + " terminado. Mês " + month + " terminado. Dia " + day + " terminado com tempo de " + dayTimer + ".");
            }
        }
    }

    // Buildings List:
    public void ManageBuildLists(Buildings build)
    {
        buildings.Add(build);
        buildingsTaxed.Remove(build);
    }

    // GETTERS AND SETTERS
    public void SetCheckingGoals(bool areCheckingGoals)
    {
        checkingGoals = areCheckingGoals;
    }

    public void SetBonusDay(int n)
    {
        bonusDay = n;
    }

    public float GetDaySpeed()
    {
        return daySpeed;
    }

    public void SetDaySpeed(float multiplier)
    {
        daySpeed *= multiplier;
    }


    public void SetEventDay(bool b)
    {
        eventDay = b;
    }

    public string PrintDate()
    {
        string s = day + "/ " + month + "/ ano " + year;
        return s;
    }

    public int GetTaxValue()
    {
        return taxValue;
    }


    // Volta para o menu inicial:
    public void ReturnToMainMenu()
    {
        player.PlayFinalBtSound();
        SceneManager.LoadScene("MenuScene");
        //StartCoroutine(ReturnMenu());
    }
    IEnumerator ReturnMenu()
    {
        player.PlayFinalBtSound();
        yield return new WaitForSeconds(player.finalBtSound.length);
        SceneManager.LoadScene("MenuScene");
    }
}
