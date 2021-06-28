using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Birdy : MonoBehaviour
{
    [SerializeField]
    private bool isDead;

    public float velocity = 1f;
    private Rigidbody2D rb2D;
    NeuralNetwork neuralNetwork;
    List<float> inputList;
    float individualScore = 0;
    public bool changeSpeed;
    public int timeScale = 1;



    private void Awake()
    {
        
        neuralNetwork = new NeuralNetwork();
        inputList = new List<float>();
        neuralNetwork.InitializeNetwork(new int[] { 3, 6, 3, 1});
        rb2D = GetComponent<Rigidbody2D>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = GameObject.Find("GameManager").GetComponent<GameManager>().timeScale;
    }
    // Update is called once per frame
    void Update()
    {
        Time.timeScale = GameObject.Find("GameManager").GetComponent<GameManager>().timeScale;
        if (IsDead)
        {
            rb2D.velocity = Vector3.zero;
        }
        else
        {
            rb2D.velocity = Vector2.up * neuralNetwork.CalculateOutput(CreateInput()).ElementAt(0);
        }

    }

    List<float> CreateInput()
    {
        inputList.Clear();
        Vector3 pipeData = GameObject.Find("GameManager").GetComponent<GameManager>().ClosestPipe.transform.Find("ScoreArea").GetComponent<BoxCollider2D>().transform.position;
        inputList.Add(transform.position.y);
        inputList.Add(pipeData.y);
        inputList.Add(pipeData.x - transform.position.x);

        return inputList;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.name.Equals("ScoreArea"))
        {
            IndividualScore += 1.0f;
            
        }
        if(GameObject.Find("GameManager").GetComponent<GameManager>().BestScore < IndividualScore)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().UpdateScore();
        }
        Debug.Log("Score:" + IndividualScore);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag.Equals("DeathArea"))
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().BirdDeadSignal(IsDead);
            IsDead = true;
            this.gameObject.transform.position = new Vector3(-25, -25, 0);
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }        
        else 
        {
            Debug.Log(collision);
        }
    }
    public float IndividualScore { get => individualScore; set => individualScore = value; }
    public NeuralNetwork NeuralNetwork { get => neuralNetwork; set => neuralNetwork = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
}

public class NeuralNetwork
{
    private List<float[ , ]> weights = new List<float[ , ]>();
    private float[] output;
    int[] neuralLayers;
    int neuralWeightLength = 0;   

    //[5,6,3,1]
    public void InitializeNetwork(int[] layers)
    {
        neuralLayers = layers;
        for (int layerIndex = 0; layerIndex < layers.Length - 1; layerIndex++)
        {
            //+1 for bias
            var layerWeights = new float[layers[layerIndex + 1] , layers[layerIndex] + 1];
            for (int rowIndex = 0; rowIndex < layers[layerIndex + 1]; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < layers[layerIndex] + 1; columnIndex++)
                {
                    layerWeights[rowIndex, columnIndex] = UnityEngine.Random.Range(-1f, 1f);
                    NeuralWeightLength++;
                }
            }

            weights.Add(layerWeights);
        }

    }

    public List<float> GetWeights()
    {
        List<float> flattenedWeights = new List<float>();

        for (int layerIndex = 0; layerIndex < neuralLayers.Length - 1; layerIndex++)
        {
            int rowCount = weights.ElementAt(layerIndex).GetLength(0);
            int columnCount = weights.ElementAt(layerIndex).GetLength(1);

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
                {
                    flattenedWeights.Add(weights.ElementAt(layerIndex)[rowIndex, columnIndex]);
                }                
            }

        }

        return flattenedWeights;
    }

    public void PutWeights(List<float> flattenedWeights)
    {
        int flattenIndex = 0;
        for (int layerIndex = 0; layerIndex < neuralLayers.Length - 1; layerIndex++)
        {
            int rowCount = weights.ElementAt(layerIndex).GetLength(0);
            int columnCount = weights.ElementAt(layerIndex).GetLength(1);

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) 
            {
                for (int columnIndex = 0; columnIndex < columnCount; columnIndex++) 
                {
                    weights.ElementAt(layerIndex)[rowIndex, columnIndex] = flattenedWeights[flattenIndex++];
                }
            }

        }
    }

    public List<float> CalculateOutput(List<float> inputs)
    {
        List<float> outputs = new List<float>();
        for (int layerIndex = 0; layerIndex < neuralLayers.Length - 1; layerIndex++)
        {
            int rowCount = weights.ElementAt(layerIndex).GetLength(0);
            int columnCount = weights.ElementAt(layerIndex).GetLength(1);

            for (int rowIndex = 0; rowIndex < neuralLayers[layerIndex + 1]; rowIndex++) // bir nöron
            {
                float weightedSum = 0;
                for (int columnIndex = 0; columnIndex < neuralLayers[layerIndex] - 1; columnIndex++) // o nörona gelen aðýrlýklar
                {
                    weightedSum += weights.ElementAt(layerIndex)[rowIndex, columnIndex] * inputs.ElementAt(columnIndex);
                }
                    //bias
                    weightedSum -= weights.ElementAt(layerIndex)[rowIndex, columnCount - 1];
                outputs.Add((float)System.Math.Tanh(weightedSum));
                //outputs.Add(Sigmoid(weightedSum));
            }
            inputs = outputs;


        }

        return outputs;

    }

    float Sigmoid(float netInput, float response = 1.0f)
    {
        return (1.0f / (1.0f + (float)System.Math.Exp(-netInput / response)));
    }

    public int NeuralWeightLength { get => neuralWeightLength; set => neuralWeightLength = value; }
}
