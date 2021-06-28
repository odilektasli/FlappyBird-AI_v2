using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Birdy BirdScript;
    public GameObject Pipes;

    public float height;

    public float time;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnObject(time));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator SpawnObject(float time)
    {
        while (!BirdScript.IsDead)
        {
            Instantiate(Pipes, new Vector3(1.5f, Random.Range(-height, height), 0), Quaternion.identity);

            yield return new WaitForSeconds(time);
        }
    }
}
