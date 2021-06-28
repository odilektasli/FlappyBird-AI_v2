using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class SceneHandler : MonoBehaviour
{
    
    public InputField populationSize;
    // Start is called before the first frame update

    
    public void StartGame(string input)
    {
        SceneManager.LoadScene("Game");
        GameManager.birdNumber = int.Parse(populationSize.text);
        Debug.Log("Size" + populationSize.text);
    }

    public void GetPopulationSize()
    {
        
        //Debug.Log("Size" + populationSize.text);
    }
}
