using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        //Destroy(gameObject, 7); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
        
        if(transform.position.x < -10)
        {
            transform.position += new Vector3((GameObject.Find("GameManager").GetComponent<GameManager>().pipeCount) * 1.5f, 0, 0);
        }
    }
}
