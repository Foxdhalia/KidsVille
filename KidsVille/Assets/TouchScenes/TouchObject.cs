using System;
using UnityEngine;

public class TouchObject : MonoBehaviour
{
    public float speed = 0.25f;
    public Collider2D coll2D;
    public Collider collCube;
    bool beingTouch;
    bool isSprite;
    Vector2 pos;

    private void Start()
    {
        if(GetComponent<Collider2D>())
        {
            coll2D = GetComponent<Collider2D>();
            isSprite = true;
        }
       else
        {
            collCube = GetComponent<Collider>();
            isSprite = false;
        }
    }

    void Update()
    {
        if (beingTouch)
        {
            pos = new Vector2(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).x,
                Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position).y);

            transform.position = pos;
        }

        if (isSprite) // FOR SPRITE OBJECTS
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) //&& Input.GetTouch(0).phase == TouchPhase.Began)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), Vector2.zero);
                if (hit.collider != null)
                {
                    if (hit.collider == coll2D)
                    {
                        SpriteRenderer spriteRend = GetComponent<SpriteRenderer>();
                        spriteRend.color = Color.red;
                        print("touch cube");
                        beingTouch = true;
                    }
                }
            }
        }

        else // FOR 3D OBJECTS
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) //&& Input.GetTouch(0).phase == TouchPhase.Began)
            {                
                Ray myRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;
                if (Physics.Raycast(myRay, out hit))
                {
                    if (hit.collider == collCube)
                    {

                        Renderer rend = GetComponent<Renderer>(); ;
                        rend.material.color = Color.red;
                        beingTouch = true;

                    }
                }
            }
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            beingTouch = false;
            pos = new Vector2(0f, 0f);
            print("Touch = " + beingTouch);

            if (isSprite)
            {
                SpriteRenderer spriteRend = GetComponent<SpriteRenderer>();
                spriteRend.color = Color.white;
            }
            else
            {
                Renderer rend = GetComponent<Renderer>(); ;
                rend.material.color = Color.white;
            }
        }
    }
}
