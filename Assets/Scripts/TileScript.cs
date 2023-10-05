using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TileScript : MonoBehaviour
{

    public bool isStart = false;
    public bool isEnd = false;
    public bool isTraversed = false;
    public bool isBlocked = false;
    public Color originalColor = Color.white;
    public GameObject Master;
    public int x;
    public int y;

    public static Color START_COLOR = Color.red;
    public static Color END_COLOR = Color.blue;
    public static Color TRAVERSED_COLOR = Color.green;
    public static Color BLOCKED_COLOR = Color.black;
    // Start is called before the first frame update
    void Start()
    {
        setClear();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setClear()
    {
        isStart = false;
        isEnd = false;
        isTraversed = false;
        isBlocked = false;
        gameObject.GetComponent<SpriteRenderer>().color = originalColor;
    }

    public void setStart()
    {
        isStart = true;
        isEnd = false;
        isTraversed = false;
        isBlocked = false;
        gameObject.GetComponent<SpriteRenderer>().color = START_COLOR;
        Master.GetComponent<MasterScript>().startNode = this.gameObject;
    }

    public void setEnd()
    {
        isStart = false;
        isEnd = true;
        isTraversed = false;
        isBlocked = false;
        gameObject.GetComponent<SpriteRenderer>().color = END_COLOR;
        Master.GetComponent<MasterScript>().endNode = this.gameObject;
    }

    public void setBlocked()
    {
        isStart = false;
        isEnd = false;
        isTraversed = false;
        isBlocked = true;
        gameObject.GetComponent<SpriteRenderer>().color = BLOCKED_COLOR;
    }
    public void setTraversed()
    {
        isStart = false;
        isEnd = false;
        isTraversed = true;
        isBlocked = true;
        gameObject.GetComponent<SpriteRenderer>().color = TRAVERSED_COLOR;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isBlocked)
            {
                setClear();
            }
            else
            {
                setBlocked();
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
           
            if (isStart)
            {
                setClear();
                Master.GetComponent<MasterScript>().startNode = null;
            }
            else if (isEnd)
            {
                setClear();
                Master.GetComponent<MasterScript>().endNode = null;
            }
            else if(Master.GetComponent<MasterScript>().startNode != null)
            {
                setEnd();
            }
            else
            {
                setStart();
            }
        }
    }
    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0))
        {
            if (isBlocked)
            {
                setClear();
            }
            else
            {
                setBlocked();
            }
        }
    }
}
