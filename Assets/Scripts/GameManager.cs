using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public Text ScoreText;
    public Text GenarationNum;
    public Text AliveBirdCount;
    public GameObject birdPrefab, pipePrefab;
    public int score;
    public float bestScore;
    private int birdDeadCount = 0;
    public static int birdNumber = 50;
    public float height;
    public float time;
    public int timeScale; 
    [Space]
    [Space]
    [Header("Genetic Algorithm")]
    [SerializeField]
    private int populationSize;
    public float crossOverRatio;
    public float mutationRatio;

    private GameObject[] aliveBirds;
    public GameObject[] pipes;
    public GameObject closestPipe;
    private GeneticAlgorithm geneticAlgorithm;
    public int pipeCount = 10;
    private int closestIndex = 0;
    private bool pipesInstantiated, birdsInstantiated;
    private static float epoch = 1;


    // Start is called before the first frame update

    public GameManager()
    {

    }

    public void Awake()
    {
        
        populationSize = birdNumber;
        birdsInstantiated = false;
        pipesInstantiated = false;
        aliveBirds = new GameObject[birdNumber];
        pipes = new GameObject[PipeCount];
        ManageBirds();
        ManagePipes();
        geneticAlgorithm = new GeneticAlgorithm(populationSize, mutationRatio, crossOverRatio, aliveBirds[0].GetComponent<Birdy>().NeuralNetwork.NeuralWeightLength);
        ClosestPipe = pipes[closestIndex];
        
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if(ClosestPipe != null)
        {
            if (ClosestPipe.transform.position.x < 0.0f)
            {
                closestIndex = (closestIndex + 1) % PipeCount1;
                ClosestPipe = pipes[closestIndex];
            }

            if (BirdDeadCount == birdNumber)
            {

                Debug.Log("bs:" + BestScore);
                Epoch += 1.0f;
                Debug.Log("Epoch: " + epoch);
                GenarationNum.text = Epoch.ToString();
                DeployGeneticAlgorithm();
                ManagePipes();
                ManageBirds();
                score = 0;
                BirdDeadCount = 0;
                closestIndex = 0;
                ClosestPipe = pipes[closestIndex];
            }
            else
            {
                AliveBirdCount.text = (birdNumber - BirdDeadCount).ToString();
            }
        }

        

    }

    public void BirdDeadSignal(bool isDead)
    {
        if(!isDead)
        {
            BirdDeadCount+=1;
        } 
    }

    void ManageBirds()
    {
        
        if(!birdsInstantiated)
        {
            GenerateBirds();
            birdsInstantiated = true;
        }
        else
        {
            for (int index = 0; index < birdNumber; index++)
            {

                aliveBirds[index].gameObject.GetComponent<Birdy>().IsDead = false;
                aliveBirds[index].transform.position = Vector3.zero;
                aliveBirds[index].gameObject.GetComponent<SpriteRenderer>().enabled = true;
                aliveBirds[index].gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;         

            }

        }

    }

    void GenerateBirds()
    {

        for (int index = 0; index < birdNumber; index++)
        {
            aliveBirds[index] = Instantiate(birdPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }

    public void  UpdateScore()
    {
        BestScore++;
        ScoreText.text = BestScore.ToString();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void ManagePipes()
    {
        float xPosition = 1.5f;
        if(!pipesInstantiated)
        {
            for (int pipeIndex = 0; pipeIndex < PipeCount; pipeIndex++, xPosition += 1.5f)
            {
                pipes[pipeIndex] = InstantiatePipes(xPosition);
            }
            pipesInstantiated = true;
        }
        else
        {
            for (int pipeIndex = 0; pipeIndex < PipeCount; pipeIndex++, xPosition += 1.5f)
            {
                pipes[pipeIndex].transform.position = new Vector3(xPosition, Random.Range(-height, height), 0);
            }
            
        }
    }

    private GameObject InstantiatePipes(float xPosition)
    {
        return Instantiate(pipePrefab, new Vector3(xPosition, Random.Range(-height, height), 0), Quaternion.identity);
    }



    void DeployGeneticAlgorithm()
    {
        //Get Current population 
        var currentPopulation = GetPopulation();
        //Debug.Log("Best Fitness:" + geneticAlgorithm.BestFitness());
        //Iterate GA 1 time
        var newPopulation = geneticAlgorithm.Epoch(ref currentPopulation);
        //Deploy new weights to birds

        for (int individualIndex = 0; individualIndex < populationSize; individualIndex++)
        {
            var currentIndividiual = aliveBirds[individualIndex].GetComponent<Birdy>();
            currentIndividiual.NeuralNetwork.PutWeights(newPopulation[individualIndex].weightList);
            currentIndividiual.IndividualScore = newPopulation[individualIndex].genomeFitness;

        }
    }

    List<(List<float> weightList, float fitness)> GetPopulation()
    {
        List<(List<float> weightList, float fitness)> currentPopulation = new List<(List<float> weightList, float fitness)>();
        for (int individualIndex = 0; individualIndex < populationSize; individualIndex++)
        {
            var currentIndividiual = aliveBirds[individualIndex].GetComponent<Birdy>();
            currentPopulation.Add((currentIndividiual.NeuralNetwork.GetWeights(), currentIndividiual.IndividualScore));
        }
        return currentPopulation;
    }


    public int PipeCount { get => PipeCount1; set => PipeCount1 = value; }
    public int BirdDeadCount { get => birdDeadCount; set => birdDeadCount = value; }
    public GameObject ClosestPipe { get => closestPipe; set => closestPipe = value; }
    public static float Epoch { get => epoch; set => epoch = value; }
    
    public int PipeCount1 { get => pipeCount; set => pipeCount = value; }
    public float BestScore { get => bestScore; set => bestScore = value; }
}
