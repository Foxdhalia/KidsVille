using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpeedGame : MonoBehaviour
{
    private GameManager gm;
    private Text speedTx;
    private Button speedUp_bt;
    private Button speedDown_bt;


    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
        speedTx = transform.GetChild(0).GetComponent<Text>();
        speedUp_bt = transform.GetChild(1).GetComponent<Button>();
        speedDown_bt = transform.GetChild(2).GetComponent<Button>();
    }

    public void ChangeGameSpeed(bool isToSpeedUp)
    {
        float f;

        if (isToSpeedUp)
        {
            if (gm.GetDaySpeed() < 8f)
            {
                f = 2f;
                gm.SetDaySpeed(f);
                speedDown_bt.interactable = true;

                if (gm.GetDaySpeed() >= 8f)
                {
                    speedUp_bt.interactable = false;
                }

            }
        }
        else
        {           
            if (gm.GetDaySpeed() > 1f)
            {
                f = 0.5f;
                gm.SetDaySpeed(f);
                speedUp_bt.interactable = true;
                
                if (gm.GetDaySpeed() <= 1f)
                {
                    speedDown_bt.interactable = false;
                }
            }
        }
       
        speedTx.text = gm.GetDaySpeed().ToString() + "x";
        print("Day speed: " + gm.GetDaySpeed());
    }

}
