using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GesturesTouch : MonoBehaviour
{
    public Text text;
    Vector2 posInit;
    Vector2 pos_;
    int fingerIndex = -1;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            fingerIndex = t.fingerId;
            if (t.phase == TouchPhase.Began)
            {
                //posInit = Input.GetTouch(0).position;                
                print("Finger index: " + fingerIndex);
                posInit = Input.GetTouch(fingerIndex).position;
            }

            if (t.phase == TouchPhase.Moved)
            {
                StartCoroutine(ReadGesture());
            }

            if (t.phase == TouchPhase.Ended)
            {
                StopAllCoroutines();
            }

        }


    }

    IEnumerator ReadGesture()
    {
        yield return new WaitForSeconds(0.05f);
        pos_ = Input.GetTouch(fingerIndex).position;
        if (posInit.x < pos_.x)
        {
            print("Going to RIGHT!");
            text.text = "Going to RIGHT!";
        }
        else if (posInit.x > pos_.x)
        {
            print("Going to LEFT!");
            text.text = "Going to LEFT!";
        }
    }

}
