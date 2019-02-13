using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationController{

    private int populationSize;
    private Creature[] pop;
    private GameObject[] creatureReferences;
    private GameObject[] lineRendererReferences;
    private int[] fitnessRatings;
    private List<float> topFitnesses;
    private float topFitnessDifference;
    private bool showRays;
    private float meanFitness;
    private float lowestFitness;
    private float highestFitness;




	public PopulationController(int populationSize, GameObject[] creatureReferences, float startingHealth, int[] brainLayers, float constantSpeed, float changeInAngle, float rayLength, float x, float y, int probes, LayerMask walls, LayerMask food, bool showRays)
    {
        this.populationSize = populationSize;
        pop = new Creature[populationSize];
        this.creatureReferences = creatureReferences;
        Populate(startingHealth, brainLayers,constantSpeed,changeInAngle,rayLength,x,y,probes,walls,food);
        fitnessRatings = new int[populationSize];
        topFitnesses = new List<float>();
        topFitnessDifference = 0f;
        this.showRays = showRays;
    }

    public PopulationController(int populationSize, GameObject[] creatureReferences, float startingHealth, int[] brainLayers, float constantSpeed, float changeInAngle, float rayLength, float x, float y, int probes, LayerMask walls, LayerMask food, bool showRays, GameObject[] lineRendererReferences)
    {
        this.populationSize = populationSize;
        pop = new Creature[populationSize];
        this.creatureReferences = creatureReferences;
        Populate(startingHealth, brainLayers, constantSpeed, changeInAngle, rayLength, x, y, probes, walls, food);
        fitnessRatings = new int[populationSize];
        topFitnesses = new List<float>();
        topFitnessDifference = 0f;
        this.showRays = showRays;
        this.lineRendererReferences = lineRendererReferences;
        AssignLineRenderers();
    }

    public PopulationController(int populationSize, GameObject[] creatureReferences, float startingHealth, int[] brainLayers, float constantSpeed, float changeInAngle, float rayLength, float x, float y, int probes, LayerMask walls, LayerMask food)
    {
        this.populationSize = populationSize;
        pop = new Creature[populationSize];
        this.creatureReferences = creatureReferences;
        Populate(startingHealth, brainLayers, constantSpeed, changeInAngle, rayLength, x, y, probes, walls, food);
        fitnessRatings = new int[populationSize];
        topFitnesses = new List<float>();
        topFitnessDifference = 0f;
        this.showRays = false;
    }

    private void Populate(float startingHealth, int[] brainLayers, float constantSpeed, float changeInAngle, float rayLength, float x, float y, int probes, LayerMask walls, LayerMask food)
    {
        for(int i = 0; i < pop.Length; i++)
        {
            pop[i] = new Creature(startingHealth, brainLayers, constantSpeed, changeInAngle, rayLength, x, y, creatureReferences[i], probes, walls, food);
        }
    }

    public void CallForRotation()
    {
        for(int i = 0; i < pop.Length; i++)
        {
            pop[i].CallForRotation();
            
        }
    }

    public void CallForVelocity()
    {
        for (int i = 0; i < pop.Length; i++)
        {
            pop[i].CallForVelocity();
        }
    }

    public void AssignLineRenderers()
    {
        for(int i = 0; i < pop.Length; i++)
        {
            List<GameObject> lineList = new List<GameObject>();
            for(int j = 0; j < pop[i].GetNumberOfProbes(); j++)
            {
                int currentRenderer = i * pop[0].GetNumberOfProbes() + j;
                lineList.Add(lineRendererReferences[currentRenderer]);

            }
            pop[i].SetRenderers(lineList.ToArray());
        }
    }

    public void Restart()
    {

        PurgeAndMutate();

        for (int i = 0; i < pop.Length; i++)
        {
            pop[i].ResetCreature();
        }
    }

    public void PurgeAndMutate()
    {
        GaugeFitness();

        float range = populationSize * .3f;
        int startingIndex = (int)range;

        for (int i = startingIndex; i > 0; i--)
        {
            pop[i].GetBrain().Splice(pop[0].GetBrain());
        }

        Mutate();
    }

    private void Mutate()
    {
        float range = populationSize * .3f;
        range = populationSize - range;
        int startingIndex = (int)range;
        for (int i = startingIndex; i < populationSize; i++)
        {
            if (Random.Range(0, 100) > 80)
            {
                pop[i].MakeNewBrain();
            }
        }

        for (int i = 2; i < pop.Length; i++)
        {
            pop[i].MutateBrain();
        }
    }

    public void GaugeFitness()
    {
        for(int i = 0; i <populationSize; i++)
        {
            fitnessRatings[i] = -1;
        }
        int index = 0;
        float savedValue = 0f;

        for(int j = 0; j < populationSize; j++)
        {
            for(int i = 0; i < populationSize; i++)
            {
                if(fitnessRatings[i] == -1)
                {
                    if(pop[i].GetFitness() > savedValue)
                    {
                        savedValue = pop[i].GetFitness();
                        index = i;
                    }
                }
            }
            fitnessRatings[j] = index;
        }

        highestFitness = fitnessRatings[0];
        lowestFitness = fitnessRatings[fitnessRatings.Length - 1];
        meanFitness = Mean(pop);

        ManageTopFitnessValues();
    }

    private void ManageTopFitnessValues()
    {
        topFitnesses.Add(pop[fitnessRatings[0]].GetFitness());

        if (topFitnesses.Count > 1)
        {
            topFitnessDifference = topFitnesses[topFitnesses.Count - 1] - topFitnesses[topFitnesses.Count - 2];
        }
    }

    public bool StillAlive()
    {
        for(int i = 0; i < pop.Length; i++)
        {
            if (pop[i].GetStatus())
            {
                return true;
            }
        }

        return false;
    }

    public float GetTopFitness()
    {
        return topFitnesses[topFitnesses.Count - 1];
    }

    public float GetTopFitnessDifference()
    {
        return topFitnessDifference;
    }

    public override string ToString()
    {
        return "Population Size: " + populationSize + " | pop.length = " + pop.Length;
    }

    public Creature GetCreatureByIndex(int index)
    {
        return pop[index];
    }

    public void SetCreatureSortingLayer(string layerName)
    {
        for(int i = 0; i < pop.Length; i++)
        {
            pop[i].GetGameObject().GetComponent<SpriteRenderer>().sortingLayerName = layerName;
        }
    }

    public Line[] GetAllLines()
    {
        List<Line> linesList = new List<Line>();

        for(int i = 0; i < pop.Length; i++)
        {
            for(int j = 0; j < pop[i].GetLines().Length; j++)
            {
                linesList.Add(pop[i].GetLines()[j]);
            }
        }

        return linesList.ToArray();
    }

    //helpers

    private float Mean(Creature[] x)
    {
        float sum = 0f;
        for(int i = 0; i < x.Length; i++)
        {
            sum += x[i].GetFitness();
        }

        return (sum / x.Length);
    }

}
