using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerData : MainScript
{
    private int originalPop; // Guarda o valor da população no início do jogo, para comparações futuras.
    private int tax; // Rendimentos de impostos.
    private int upkeep; // Despesas de conservacao de cada mes. Variam de acordo com os incrementos recebidos.  
    bool goldWarning;

    private float[] indexes = new float[3]; // 0 = healthIndex, 1 = educationIndex, 2 = incomeIndex"

    // Variáveis para o Relatório Final:
    [HideInInspector] public float lfi;  // Índice de expectativa de vida. De 20 a 85 anos (0% a 100% da barra de saúde, respectivamente).    
    private float edi;
    private float inci;
    private float hdi; // valor do índice de desenvolvimento Humano. Calculado ao final dos 48 meses.
    private int goalsAchieved;
    private int solvedCrises;

    //Touch
    Vector2 posInit;
    Vector2 pos_;
    int fingerIndex = -1;

    [Header("Valor máximo que cada uma das barras índice pode alcançar.")]
    [SerializeField] private int indexesMaxValue = 200;

    [Header("Valores mín e máx para iniciar as barras de índice.")]
    [Tooltip(" 0 = valor mínimo; 1 valor máximo. Valores de 0f a 1f.")]
    [SerializeField] private float[] valuesIndexStart = new float[2];

    [Header("Total de Ouro.")]
    public int gold; // Dinheiro atual.

    [Header("Total da População.")]
    [SerializeField] private int population = 100; // População da cidade.
    [Header("Variáveis de alteração da População.")]
    public int[] popIncrease; // Incrementos (positivos ou negativos) da população.

    [Header("Limites dos indices da cidade (de 0 a 1).")]
    [Range(0f, 1f)] [SerializeField] private float[] indexlimits; // limites dos indices da cidade.

    [Header("Valores de alteração (pos/neg) nas despesas.")]
    [SerializeField] private int[] upkeepIncrease; // Incrementos (positivos ou negativos) nas despesas de conservacao.    


    // ELEMENTOS UI
    [Header("Elementos de UI:")]
    public Text uiGold;
    public Text uiPopulation;

    [Header("UI -> Index Sliders: health, education, income")]
    [Tooltip("0 = healthIndex, 1 = educationIndex, 2 = incomeIndex")]
    public Slider[] uiIndexes;
    public Image[] slidersGlow;

    [Header("UI -> Relatório Mensal:")]
    [Tooltip("Month Report UI Data")]
    public GameObject monthReportPanel;
    [Tooltip("Month Report UI Data")]
    public Text previousGoldTx;
    [Tooltip("Month Report UI Data")]
    public Text costsTx;
    [Tooltip("Month Report UI Data")]
    public Text incomeTx;
    [Tooltip("Month Report UI Data")]
    public Text previousPopTx;
    [Tooltip("Month Report UI Data")]
    public Text changePopTx;
    [Tooltip("Month Report UI Data")]
    public Text finalGoldTx;
    [Tooltip("Month Report UI Data")]
    public Text finalPopTx;

    [Header("UI -> Relatório FINAL DE JOGO:")]
    public GameObject finalReportPanel;
    public Text goalsAchievedTx;
    public Text crisesSolvedTx;
    public Text populationIncrement;
    public Text finalGold;
    public Text hdiTx;
    public Text hdiTx_extensive;


    [HideInInspector] public AudioSource audio;
    [Header("Sound Clips")]
    [SerializeField] private AudioClip displayPanel;
    public AudioClip getRates;
    public AudioClip finalBtSound;

    private void Start()
    {
        uiGold.text = gold.ToString();
        uiPopulation.text = population.ToString();
        originalPop = population;

        //Altera o valor máximo dos sliders de indíce (health, education, income) para o valor da variável "indexesMaxValue".
        foreach (Slider s in uiIndexes)
        {
            s.maxValue = indexesMaxValue;
        }

        // Preenche os 3 indicadores com valores aleatórios entre 10 e 45% do valor total
        for (int i = 0; i < indexes.Length; i++) // 0 = healthIndex; 1 = educationIndex; 2 = incomeIndex;
        {
            indexes[i] = indexesMaxValue * Random.Range(valuesIndexStart[0], valuesIndexStart[1]); // Padrão 0.15 a 0.3.
            uiIndexes[i].value = Mathf.CeilToInt(indexes[i]);

            ChangeIndexBarGlow(i);
        }

        audio = GetComponent<AudioSource>();
    }


    private void Update()
    {
        if (!finishGame)
        {
            // if (population == 0 || healthIndex == 0 || educationIndex == 0 || incomeIndex == 0)
            if (indexes[0] == 0 || indexes[1] == 0 || indexes[2] == 0)
            {
                print("YOU LOSE!!!!");
                finishGame = true;
                StopAllCoroutines();
                GameManager gm = FindObjectOfType<GameManager>();
                gm.EndGame();
            }

            if (gold < 0 && !goldWarning)
            {
                goldWarning = true;
                Debug.LogWarning("O valor de gold está menor do que 0!");
            }


            // Clicando sobre os prédios:
            if (!EventSystem.current.IsPointerOverGameObject()) // The instructions bellow will run only if the mouse is not over a UI object.
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    //print("origin " + mouseRay.origin + ", direction " + mouseRay.direction);
                    if (Physics.Raycast(mouseRay, out hit))
                    {
                        //print(hit.collider.name.ToString());
                        if (hit.collider.tag == "Build")
                        {
                            Buildings build = hit.collider.GetComponent<Buildings>();
                            build.CollectRates();
                        }
                    }
                }
            }
            //#if UNITY_ANDROID
            // Touch sobre os prédios.
            if (Input.touchCount > 0)
            {
                if (!EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                {
                    /*Touch t = Input.GetTouch(0);
                    fingerIndex = t.fingerId;
                    if (t.phase == TouchPhase.Began)
                    {
                        //posInit = Input.GetTouch(0).position;                
                        //print("Finger index: " + fingerIndex);
                        posInit = Input.GetTouch(fingerIndex).position;
                    }

                    if (t.phase == TouchPhase.Moved)
                    {
                        StartCoroutine(ReadGesture());
                    }

                    if (t.phase == TouchPhase.Ended)
                    {
                        StopAllCoroutines();
                    }*/

                }

            }
            //#endif
        }
    }


    IEnumerator ReadGesture()
    {
        yield return new WaitForSeconds(0.05f);
        pos_ = Input.GetTouch(fingerIndex).position;
        if (posInit.x < pos_.x)
        {
            print("Going to RIGHT!");
           
        }
        else if (posInit.x > pos_.x)
        {
            print("Going to LEFT!");
            
        }

        if (posInit.y < pos_.y)
        {
            print("Going to UP!");

        }
        else if (posInit.y > pos_.y)
        {
            print("Going to DOWN!");

        }
    }

    public bool TaxCalculation() // Calculo dos Impostos da cidade (com base na populacao).
    {
        tax = Mathf.CeilToInt(population * 0.1f);
        //Debug.Log("Tax: " + tax);
        return true;
    }

    public bool Lfi_Calculation()
    {
        float percentualBar = (indexes[0] / indexesMaxValue);
        print("Porcentagem barra saúde = " + percentualBar + ".");
        float lifeExpec = Mathf.Lerp(20f, 85f, percentualBar);
        print("Life Expec: " + lifeExpec + " anos.");
        lfi = (lifeExpec - 20f) / 65f;
        Debug.LogWarning("LFI = (" + lifeExpec + " - 20) / 65 = " + lfi);
        return true;
    }

    public bool Edi_Calculation()
    {
        float percentualBar = (indexes[1] / indexesMaxValue);
        print("Porcentagem barra educação = " + percentualBar + ".");

        // Média de educação.
        float edAvg = Mathf.Lerp(5, 15, percentualBar);
        Debug.Log("edAVG = " + edAvg);

        // Expectativa de educação.
        float edExp = Mathf.Lerp(2, 18, percentualBar);
        Debug.Log("edExp = " + edExp);

        edi = ((edAvg / 15) + (edExp / 18f)) / 2f;
        Debug.LogWarning("EDI = ((" + edAvg + " / 15) + (" + edExp + " / 18)) / 2 => " + edi);

        return true;
    }

    public bool Inci_Calculation()
    {
        inci = indexes[2] / indexesMaxValue;
        Debug.LogWarning("Indice de Renda = " + indexes[2] + " / " + indexesMaxValue + " => " + inci);
        return true;
    }

    public bool Hdi_Calculation()
    {
        hdi = Mathf.Pow((lfi * edi * inci), 1f / 3f);
        //Debug.Log("Hdi completo:" + hdi);
        hdi = (float)decimal.Round((decimal)hdi, 2);
        // Debug.Log("Hdi com duas casas: " + hdi);
        hdi *= 100f;
        Debug.Log("Hdi percentual: " + hdi);

        // HDI = (LFI x EDI x II) ^ (1/3)
        return true;
    }

    bool LimitsRange(float cityIndex) // healthIndex, educationIndex ou incomeIndex.
    {
        if (cityIndex < indexlimits[0])
        {
            //print("Entrou no limite 0. " + " cityIndex = " + cityIndex);
            population += popIncrease[0];
            upkeep += upkeepIncrease[0];
        }
        else if (cityIndex >= indexlimits[0] && cityIndex < indexlimits[1])
        {
            //print("Entrou no limite de 0 a 1." + " cityIndex = " + cityIndex);
            population += popIncrease[1];
            upkeep += upkeepIncrease[1];
        }
        else if (cityIndex >= indexlimits[1] && cityIndex < indexlimits[2])
        {
            // print("Entrou no limite de 1 a 2." + " cityIndex = " + cityIndex);
            population += popIncrease[2];
            upkeep += upkeepIncrease[2];
        }
        else if (cityIndex >= indexlimits[2])
        {
            //print("Entrou no limite acima de 2." + " cityIndex = " + cityIndex);
            population += popIncrease[3];
            upkeep += upkeepIncrease[3];
        }

        return true;
    }

    public void ChangeIndexValue(int n, float value)
    {
        indexes[n] += value;
        if (indexes[n] > indexesMaxValue)
        {
            indexes[n] = indexesMaxValue;
        }
        else if (indexes[n] < 0f)
        {
            indexes[n] = 0f;
        }
        uiIndexes[n].value = Mathf.CeilToInt(indexes[n]);
        //print("Index " + n + " with " + temp + ". Value: " + value + ". Total: " + indexes[n]);      

        ChangeIndexBarGlow(n);
    }

    // Mudando a cor do glow:
    void ChangeIndexBarGlow(int n)
    {
        float f = indexes[n] / indexesMaxValue;
        // print("float f = " + f);
        if (f < indexlimits[0])
        {
            slidersGlow[n].color = Color.red;
            // print("0, float f = " + f + " < " + indexlimits[0]);
        }
        else if (f >= indexlimits[0] && f < indexlimits[1])
        {
            slidersGlow[n].color = Color.yellow;
            // print("1, float f = " + indexlimits[0] + " <= " + f + " < " + indexlimits[1]);
        }
        else if (f >= indexlimits[1] && f < indexlimits[2])
        {
            slidersGlow[n].color = Color.cyan;
            //  print("2, float f = " + indexlimits[1] + " <= " + f + " < " + indexlimits[2]);
        }
        else if (f >= indexlimits[2])
        {
            slidersGlow[n].color = Color.green;
            //  print("3, float f = " + f + " >= " + indexlimits[2]);
        }
    }

    public bool GoldCalc(float value)
    {
        Debug.Log("gold += value -> " + gold + " + " + value + " = " + (gold + value));
        gold += (int)value;

        if (gold < 0)
        {
            gold = 0;
        }

        uiGold.text = gold.ToString();
        return true;
    }

    public IEnumerator MonthReport_Calculations(bool endGame)
    {
        // Inicia zerando a variavel de upkeep pois ela deve ser recalculada a cada mês.
        upkeep = 0;

        // Guarda os valores previos de ouro e população.
        int previousPop = population;
        int previousGold = gold;


        // Calcula os indices para verificar o incremento da população e o novo valor do upkeep:
        for (int i = 0; i < uiIndexes.Length; i++)
        {
            //Debug.Log(i + " -> Population Before: " + population + ", Upkeep Before: " + upkeep);
            bool b_ = false;
            float n = uiIndexes[i].value;
            b_ = LimitsRange(n / indexesMaxValue);
            //Debug.Log("Index " + i + " value on limits range " + (n / indexesMaxValue));
            yield return new WaitUntil(() => b_ == true);
            // Debug.Log("Population Now: " + population + ", Upkeep Now: " + upkeep);
        }
        Debug.Log("TOTAL Population Now: " + population + ", Upkeep Now: " + upkeep);


        // Calcula os impostos que devem ser recebidos:
        bool b = TaxCalculation();
        yield return new WaitUntil(() => b == true); // espera com que o calculo termine.


        // Calcula o novo valor de ouro após receber os impostos e retirar os custos de manutenção:
        b = false;
        b = GoldCalc(tax - upkeep);
        yield return new WaitUntil(() => b == true); // espera com que o calculo termine.

        uiPopulation.text = population.ToString();

        if (population <= 0)
        {
            finishGame = true;
        }

        if (!endGame && !finishGame)
        {
            MonthReport_UIData(previousGold, previousPop);
        }
        else // FINAL DO JOGO
        {
            StartCoroutine(Indexes_FinalCalc());
        }
    }

    void MonthReport_UIData(int previousGold, int previousPop)
    {
        audio.PlayOneShot(displayPanel);
        monthReportPanel.SetActive(true);

        previousGoldTx.text = previousGold.ToString();
        costsTx.text = upkeep.ToString();
        incomeTx.text = tax.ToString();
        finalGoldTx.text = gold.ToString();

        previousPopTx.text = previousPop.ToString();
        changePopTx.text = (population - previousPop).ToString();
        finalPopTx.text = population.ToString();
    }

    IEnumerator Indexes_FinalCalc()
    {
        // CALCULANDO OS INDICES://///////////////////////   
        yield return new WaitUntil(() => Lfi_Calculation() == true);

        yield return new WaitUntil(() => Edi_Calculation() == true);

        yield return new WaitUntil(() => Inci_Calculation() == true);
        ////////////////////////////////////////////////

        // CALCULANDO O HDI ////////////////////////////////
        yield return new WaitUntil(() => Hdi_Calculation() == true);
        ////////////////////////////////////////////////////

        // Mostrando a tela final:
        FinalReport_UIData();
    }

    public void FinalReport_UIData()
    {
        finalReportPanel.SetActive(true);
        audio.PlayOneShot(displayPanel);

        goalsAchievedTx.text = goalsAchieved.ToString();
        crisesSolvedTx.text = solvedCrises.ToString();
        populationIncrement.text = (population - originalPop).ToString();
        finalGold.text = gold.ToString();

        hdiTx.text = hdi.ToString() + "%";
        if (hdi < 30)
        {
            hdiTx.color = Color.red;
            hdiTx_extensive.color = Color.red;
        }
        else if (hdi >= 30 && hdi < 50)
        {
            hdiTx.color = new Color(1f, 0.51f, 0f, 1f); // Orange
            hdiTx_extensive.color = new Color(1f, 0.51f, 0f, 1f); // Orange
        }
        else if (hdi >= 50 && hdi < 80)
        {
            hdiTx.color = new Color(0f, 0.67f, 1f, 1f); // Azul claro
            hdiTx_extensive.color = new Color(0f, 0.67f, 1f, 1f); // Azul claro
        }
        else
        {
            hdiTx.color = Color.green;
            hdiTx_extensive.color = Color.green;
        }
    }

    public int GetPopulation()
    {
        return population;
    }

    public void SetGoalsAchieved()
    {
        goalsAchieved++;
    }

    public void SetSolvedCrises()
    {
        solvedCrises++;
    }

    public void PlayFinalBtSound()
    {
        audio.PlayOneShot(finalBtSound);
    }
}
