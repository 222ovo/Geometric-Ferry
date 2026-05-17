using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighLightColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeColor();
    }

    public void ChangeColor()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        if (Mathf.Abs(Vector3.Distance(mouseWorldPosition, this.transform.position)) <= 0.5f)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.red;
            }
            return;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }
}
