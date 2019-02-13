using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundCreatureDriver : MonoBehaviour {

    public bool drawLines;
    public float speed;
    public float angle;
    public float rayLength;
    public float lifeSpan;
    public int NumberOfProbes;
    public LayerMask walls;
    public LayerMask food;
    public float inititalX;
    public float initialY;
    private int[] brain;
    public GameObject creatureTemplate;
    private GameObject[] creatureReferences;
    private PopulationController pop;
    public int populationSize;
    public int numberOfFoods;
    public int currentGen;
    public Material redLine;
    public Material greenLine;
    public Line[] linesToDraw;
    private GameObject[] lineRenderers;
    public GameObject lineRendererReference;


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
        lineRenderers = new GameObject[populationSize * NumberOfProbes];
        for (int i = 0; i < populationSize * NumberOfProbes; i++)
        {
            lineRenderers[i] = Instantiate(lineRendererReference);
        }

        SetRenderLayerOrderToBLUR();

        pop = new PopulationController(populationSize, creatureReferences, lifeSpan, brain, speed, angle, rayLength, inititalX, initialY, NumberOfProbes, walls, food);
        pop.SetCreatureSortingLayer("ThingsOnTopOfBackground");
    }

    public void Update()
    {
        pop.CallForRotation();
        linesToDraw = pop.GetAllLines();
        UpdateRenderers();

        if (!pop.StillAlive())
        {
            Restart();
        }

        if (Input.GetButtonDown("Reset"))
        {
            Restart();
        }
    }

    private void UpdateRenderers()
    {
        for(int i = 0; i < linesToDraw.Length; i++)
        {
            LineRenderer lr = lineRenderers[i].GetComponent<LineRenderer>();
            Vector3[] currentLine = new Vector3[] {linesToDraw[i].GetStartAsVector3(), linesToDraw[i].GetEndAsVector3()};
            Color color = linesToDraw[i].GetColor();
            lr.enabled = true;

            if(color == Color.green)
            {
                lr.SetPositions(currentLine);
                lr.material = greenLine;
            }
            else if(color == Color.red)
            {
                lr.SetPositions(currentLine);
                lr.material = redLine;
            }
            else
            {
                lr.enabled = false;
            }
        }
    }

    private void SetRenderLayerOrderToBLUR()
    {
        for(int i = 0; i < lineRenderers.Length; i++)
        {
            lineRenderers[i].GetComponent<LineRenderer>().sortingOrder = SortingLayer.GetLayerValueFromName("Background");
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

