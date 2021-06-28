using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{
    //bütün popülasyonun genleri
    private List<(List<float> weightList, float genomeFitness)> population = new List<(List<float> weightList, float fitness)>();
    //popülasyon boyutu
    int populationSize;
    //her kromozom için aðýrlýk sayýsý
    int chromoLength;
    //en iyi kromozomun listedeki yeri
    int fittestGenome;
    int generCounter;
    public int NumOfCopiesElite = 4;
    public int NumElite = 4;
    public float maxPerturbation = 0.3f;
    float totalFitness;
    float bestFitness;
    float worstFitness;
    float averageFitness;
    float mutationRate;
    float crossOverRate;


    public GeneticAlgorithm(int popSize, float mutationRate, float crossOverRate, int chromoLength)
    {
        this.populationSize = popSize;
        this.mutationRate = mutationRate;
        this.crossOverRate = crossOverRate;
        this.chromoLength = chromoLength;
        totalFitness = 0;
        generCounter = 0;
        fittestGenome = 0;
        bestFitness = 0;
        worstFitness = 99999999;
        averageFitness = 0;

        //fitness deðerleri 0'a eþit ve aðýrlýklarý random olarak oluþturulmuþ 
        //bir popülasyon oluþturur.
        for (int i = 0; i < popSize; ++i)
        {
            Population.Add((new List<float>(), 0.0f));

            for (int j = 0; j < chromoLength; ++j)
            {
                Population[i].weightList.Add(UnityEngine.Random.Range(-1.0f, 1.0f));
            }
        }
    }

    void Mutate(ref List<float> chromosome)
    {

        for (int i = 0; i < chromosome.Count; ++i)
        {
            //Mutasyon olasýlýðý gerçeklenirse
            if (Random.Range(0, 1.0f) < mutationRate)
            {
                //Aðýrlýða random bir deðer ekle veya çýkar
                chromosome[i] += (Random.Range(-maxPerturbation, maxPerturbation));
            }
        }

    }
    List<float> GetChromoRoulettte()
    {
        //toplam fitness ve 0 arasýnda random bir sayý üretir
        float slice = Random.Range(0f, 1f) * totalFitness;
        List<float> chosenOne = Population[0].weightList;
        float fitnessSoFar = 0;

        for (int i = 0; i < populationSize; ++i)
        {

            fitnessSoFar += Population[i].genomeFitness;
            //bu noktaya kadar populasyonun fittnes deðeri toplana toplana gelir
            //elde edilen toplam yukarýdaki random sayýdan büyük olduðu zaman o noktadaki gen seçilir.
            if (fitnessSoFar > slice)
            {
                chosenOne = Population[i].weightList;
                break;
            }

        }

        return chosenOne;
    }

    void CrossOver(ref List<float> mum, ref List<float> dad, ref List<float> baby1, ref List<float> baby2)
    {
        //anne babayla aynýysa veya üretilen random sayý crossover ihtimalinden büyükse
        //direk ebeveynleri yeni birey yap
        if ((Random.Range(0f, 1.0f) > crossOverRate) || (mum == dad))
        {
            baby1 = mum;
            baby2 = dad;
            return;

        }
        //cp=crossover point=crossover'ýn gerçekleþeceði nokta
        int crossoverPoint = Random.Range(0, chromoLength - 1);
        //anne babadan iki çocuk oluþturuyoruz
        for (int i = 0; i < crossoverPoint; ++i)
        {
            baby1.Add(mum[i]);
            baby2.Add(dad[i]);

        }
        for (int j = crossoverPoint; j < mum.Count; ++j)
        {
            baby1.Add(dad[j]);
            baby2.Add(mum[j]);
        }
        return;
    }

    //Ga'yý bir nesil boyu çalýþtýrýr. Yeni kromozom popülasyonu döndürür

    public List<(List<float> weightList, float genomeFitness)> Epoch(ref List<(List<float> weightList, float genomeFitness)> oldPopulation)
    {

        ////verilen popülasyonu sýnýfýn popülasyonuna eþitler
        Population = oldPopulation;
        Reset();
        //popülasyonu sort ediyoruz
        oldPopulation.Sort((x, y) => x.genomeFitness.CompareTo(y.genomeFitness));

        //en iyi, kötü ve averaj fitness'ý hesaplýyoruz
        CalculateBestWorstAvTot();
        //temp popülasyon
        List<(List<float> weightList, float genomeFitness)> newPopulation = new List<(List<float> weightList, float genomeFitness)>();
        //Çift sayý olmasý lazým yoksa rulet çalýþmaz
        if (NumOfCopiesElite * NumElite % 2 == 0)
        {
            GrabNBest(NumElite, NumOfCopiesElite, ref newPopulation);
        }
        //GA loopu
        //yeni popülasyon oluþana kadar tekrar et
        while (newPopulation.Count < populationSize)
        {
            //Ruletle iki kromozom seç
            List<float> mum = GetChromoRoulettte();
            List<float> dad = GetChromoRoulettte();
            List<float> baby1 = new List<float>();
            List<float> baby2 = new List<float>();
            //çaprazlamaya sok
            CrossOver(ref mum, ref dad, ref baby1, ref baby2);
            //mutasyon
            Mutate(ref baby1);
            Mutate(ref baby2);

            //yeni popülasyona kopyala
            newPopulation.Add((baby1, 0.0f));
            newPopulation.Add((baby2, 0.0f));
        }
        //yeni popülasyonu sýnýfýn popülasyonu olarak ata 
        Population = newPopulation;

        return Population;

    }

    //Elitisim için en iyi bireyin bulunmasý
    void GrabNBest(int nBest, int numberOfCopies, ref List<(List<float> weightList, float genomeFitness)> populationReference)
    {
        while (nBest > 0)
        {
            for (int i = 0; i < numberOfCopies; ++i)
            {
                populationReference.Add(Population[(populationSize - 1) - nBest]);
            }

            nBest--;
        }

    }

    void CalculateBestWorstAvTot()
    {
        totalFitness = 0;
        float highestSoFar = 0;
        float lowestSoFar = 9999999;
        Debug.Log(highestSoFar);
        for (int i = 0; i < populationSize; ++i)
        {
            if (Population[i].genomeFitness > highestSoFar)
            {
                highestSoFar = Population[i].genomeFitness;
                fittestGenome = i;
                bestFitness = highestSoFar;
            }

            if (Population[i].genomeFitness < lowestSoFar)
            {
                lowestSoFar = Population[i].genomeFitness;
                worstFitness = lowestSoFar;
            }
            totalFitness += Population[i].genomeFitness;

        }
        averageFitness = totalFitness / populationSize;

    }

    void Reset()
    {

        totalFitness = 0;
        bestFitness = 0;
        worstFitness = 9999999;
        averageFitness = 0;

    }

    //Accessors
    public List<(List<float> weightList, float genomeFitness)> Population { get => population; set => population = value; }
    public double AverageFitness() { return totalFitness / populationSize; }
    public double BestFitness() { return bestFitness; }

}
