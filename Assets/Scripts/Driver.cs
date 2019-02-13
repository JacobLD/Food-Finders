using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Driver : MonoBehaviour {

    public float speed;
    public float angle;
    public float rayLength;
    public float lifeSpan;
    public int NumberOfProbes;
    public static GameObject StaticCreatureTemplate;
    public LayerMask walls;
    public LayerMask food;
    public float inititalX;
    public float initialY;
    private int[] brain;
    public GameObject creatureTemplate;
    private GameObject[] creatureReferences;
    private PopulationController pop;
    private Creature steve;
    public int populationSize;
    public int numberOfFoods;
    public int currentGen;


    private void Awake()
    {
        brain = new int[] { NumberOfProbes, NumberOfProbes, 1 };
        BuildPopulation();
        currentGen = 1;
    }

    public void BuildPopulation()
    {
        creatureReferences = new GameObject[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            creatureReferences[i] = Instantiate(creatureTemplate);
        }

        pop = new PopulationController(populationSize, creatureReferences, lifeSpan, brain, speed, angle, rayLength, inititalX, initialY, NumberOfProbes, walls, food);
    }

    public void Update()
    {
        pop.CallForRotation();

        if (!pop.StillAlive())
        {
            Restart();
        }

        if (Input.GetButtonDown("Reset"))
        {
            Restart();
        }
    }

    public void Restart()
    {
        ResetFood();
        pop.Restart();
        ++currentGen;
    }

    public void ResetFood()
    {
        for(int i = 0; i < numberOfFoods; i++)
        {
            string currentFoodName = "Food (" + i + ")";
            GameObject.Find(currentFoodName).GetComponent<BoxCollider2D>().enabled = true;
            GameObject.Find(currentFoodName).GetComponent<SpriteRenderer>().enabled = true;
        }
    }


    public void FixedUpdate()
    {
        pop.CallForVelocity();
    }
}
