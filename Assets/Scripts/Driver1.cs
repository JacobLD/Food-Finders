using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Driver1 : MonoBehaviour {

    float speed;
    float angle;
    float rayLength;
    float lifeSpan;
    int NumberOfProbes;
    static Material lineMaterial;
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
    int populationSize;
    public int numberOfFoods;
    public Text bf;
    public Text blfd;
    public Text gen;
    public int currentGen;
    private ValuesToPass vp;
    private bool showRays;
    


    private void Awake()
    {
        AssignMenuValues();
        brain = new int[] { NumberOfProbes, NumberOfProbes, 1 };
        BuildPopulation();
        currentGen = 1;
    }

    private void AssignMenuValues()
    {
        vp = GameObject.Find("ValuesToPassToSimulation").GetComponent<ValuesToPass>();
        speed = vp.speed;
        angle = vp.deltaAngle;
        rayLength = vp.length;
        lifeSpan = vp.lifespan;
        NumberOfProbes = (int)vp.probes;
        populationSize = (int)vp.populationSize;
        showRays = vp.showRays;
    }

    public void BuildPopulation()
    {
        creatureReferences = new GameObject[populationSize];
        for (int i = 0; i < populationSize; i++)
        {
            creatureReferences[i] = Instantiate(creatureTemplate);
        }

        pop = new PopulationController(populationSize, creatureReferences, lifeSpan, brain, speed, angle, rayLength, inititalX, initialY, NumberOfProbes, walls, food, showRays);
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
        UpdateFitnessText();
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

    public void UpdateFitnessText()
    {
        bf.text = "Best Fitness: " + pop.GetTopFitness();
        blfd.text = "Delta-F: " + pop.GetTopFitnessDifference();
        gen.text = "Gen: " + currentGen;
    }

    public void FixedUpdate()
    {
        pop.CallForVelocity();
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        if (showRays)
        {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);

            
            for(int i = 0; i < populationSize; i++)
            {
                GL.PushMatrix();
                // Set transformation matrix for drawing to
                // match our transform
                GL.MultMatrix(transform.localToWorldMatrix);

                // Draw lines
                GL.Begin(GL.LINES);
                Creature currentCreature = pop.GetCreatureByIndex(i);
                Debug.Log("Starting Lines for Creature " + i);

                if (currentCreature.alive)
                {
                    for (int j = 0; j < NumberOfProbes; j++)
                    {
                        Line currentLine = currentCreature.GetLines()[j];
                        Vector3 start = currentLine.GetStartAsVector3();
                        Vector3 end = currentLine.GetEndAsVector3();
                        // Vertex colors change from red to green
                        GL.Color(currentLine.GetColor());
                        // One vertex at transform position
                        GL.Vertex3(start.x, start.y, 0f);
                        // Another vertex at edge of circle
                        GL.Vertex3(start.x, start.y, 0f);
                    }
                }
                GL.End();
                GL.PopMatrix();
            }
        }
    }

    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }
}
