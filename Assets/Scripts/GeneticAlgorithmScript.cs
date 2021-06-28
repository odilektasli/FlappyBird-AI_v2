using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{
    //b�t�n pop�lasyonun genleri
    private List<(List<float> weightList, float genomeFitness)> population = new List<(List<float> weightList, float fitness)>();
    //pop�lasyon boyutu
    int populationSize;
    //her kromozom i�in a��rl�k say�s�
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

        //fitness de�erleri 0'a e�it ve a��rl�klar� random olarak olu�turulmu� 
        //bir pop�lasyon olu�turur.
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
            //Mutasyon olas�l��� ger�eklenirse
            if (Random.Range(0, 1.0f) < mutationRate)
            {
                //A��rl��a random bir de�er ekle veya ��kar
                chromosome[i] += (Random.Range(-maxPerturbation, maxPerturbation));
            }
        }

    }
    List<float> GetChromoRoulettte()
    {
        //toplam fitness ve 0 aras�nda random bir say� �retir
        float slice = Random.Range(0f, 1f) * totalFitness;
        List<float> chosenOne = Population[0].weightList;
        float fitnessSoFar = 0;

        for (int i = 0; i < populationSize; ++i)
        {

            fitnessSoFar += Population[i].genomeFitness;
            //bu noktaya kadar populasyonun fittnes de�eri toplana toplana gelir
            //elde edilen toplam yukar�daki random say�dan b�y�k oldu�u zaman o noktadaki gen se�ilir.
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
        //anne babayla ayn�ysa veya �retilen random say� crossover ihtimalinden b�y�kse
        //direk ebeveynleri yeni birey yap
        if ((Random.Range(0f, 1.0f) > crossOverRate) || (mum == dad))
        {
            baby1 = mum;
            baby2 = dad;
            return;

        }
        //cp=crossover point=crossover'�n ger�ekle�ece�i nokta
        int crossoverPoint = Random.Range(0, chromoLength - 1);
        //anne babadan iki �ocuk olu�turuyoruz
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

    //Ga'y� bir nesil boyu �al��t�r�r. Yeni kromozom pop�lasyonu d�nd�r�r

    public List<(List<float> weightList, float genomeFitness)> Epoch(ref List<(List<float> weightList, float genomeFitness)> oldPopulation)
    {

        ////verilen pop�lasyonu s�n�f�n pop�lasyonuna e�itler
        Population = oldPopulation;
        Reset();
        //pop�lasyonu sort ediyoruz
        oldPopulation.Sort((x, y) => x.genomeFitness.CompareTo(y.genomeFitness));

        //en iyi, k�t� ve averaj fitness'� hesapl�yoruz
        CalculateBestWorstAvTot();
        //temp pop�lasyon
        List<(List<float> weightList, float genomeFitness)> newPopulation = new List<(List<float> weightList, float genomeFitness)>();
        //�ift say� olmas� laz�m yoksa rulet �al��maz
        if (NumOfCopiesElite * NumElite % 2 == 0)
        {
            GrabNBest(NumElite, NumOfCopiesElite, ref newPopulation);
        }
        //GA loopu
        //yeni pop�lasyon olu�ana kadar tekrar et
        while (newPopulation.Count < populationSize)
        {
            //Ruletle iki kromozom se�
            List<float> mum = GetChromoRoulettte();
            List<float> dad = GetChromoRoulettte();
            List<float> baby1 = new List<float>();
            List<float> baby2 = new List<float>();
            //�aprazlamaya sok
            CrossOver(ref mum, ref dad, ref baby1, ref baby2);
            //mutasyon
            Mutate(ref baby1);
            Mutate(ref baby2);

            //yeni pop�lasyona kopyala
            newPopulation.Add((baby1, 0.0f));
            newPopulation.Add((baby2, 0.0f));
        }
        //yeni pop�lasyonu s�n�f�n pop�lasyonu olarak ata 
        Population = newPopulation;

        return Population;

    }

    //Elitisim i�in en iyi bireyin bulunmas�
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
