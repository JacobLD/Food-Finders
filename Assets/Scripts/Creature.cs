using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature{

    GameObject body;
    Collider2D collider;
    Rigidbody2D rb;
    Vector3 startingPos;
    NeuralNetwork brain;
    public float currentHealth;
    public float startingHealth;
    public bool alive;
    public float constantSpeed;
    public float changeInAngle;
    public float rayLength;
    //Layers
    LayerMask walls;
    LayerMask food;
    //for fitness
    private float fitness;
    private int foodEaten;
    private float timeAlive;
    private float timeStart;
    private float timeEnd;
    int[] backupBrain;
    //Probes
    private int probes;
    private int gaps;
    private float probeOffest;
    public float[] rayDistances;
    //public float[] rayBools;
    private bool inContactWithFood;
    //Line arrays for line rendering
    private Line[] lines;
    private GameObject[] lineRenderers;





    public Creature(float startingHealth, int[] brainLayers, float constantSpeed, float changeInAngle, float rayLength, float x, float y, GameObject creatureTemplate, int probes, LayerMask walls, LayerMask food)
    {
        //body has to be first for refrences
        //-------------------------------------
        body = creatureTemplate;
        collider = body.GetComponent<Collider2D>();
        startingPos = new Vector3(x, y);
        rb = body.GetComponent<Rigidbody2D>();
        body.transform.position = startingPos;
        body.transform.Rotate(new Vector3(0f, 0f, Random.Range(0f, 2 * Mathf.PI)));
        fitness = 0f;
        foodEaten = 0;
        timeStart = 0f;
        timeEnd = 0f;
        timeAlive = 0f;
        brain = new NeuralNetwork(brainLayers);
        backupBrain = brainLayers;
        currentHealth = startingHealth;
        alive = true;
        rayDistances = new float[probes];
        //rayBools = new float[probes];

        for(int i = 0; i < probes; i++)
        {
            rayDistances[i] = 0f;
            //rayBools[i] = 0f;
        }

        this.constantSpeed = constantSpeed;
        this.rayLength = rayLength;
        this.startingHealth = startingHealth;
        this.changeInAngle = changeInAngle;
        this.probes = probes;
        this.gaps = probes - 1;
        this.walls = walls;
        this.food = food;
        this.inContactWithFood = false;
        lines = new Line[probes];
    }

    //for regular update
    public void CallForRotation()
    {
        if (alive)
        {
            UpdateRotation();
            Hunger();
            CheckCollisions();
        }
    }

    //call for fixedUpdate
    public void CallForVelocity()
    {
        if (alive)
        {
            ForwardMotion();
        }
    }

    private void CheckCollisions()
    {
        if (collider.IsTouchingLayers(walls))
        {
            Die();
        }

        //structured so we only eat food once to build fitness correctly
        if (collider.IsTouchingLayers(food) && !inContactWithFood)
        {
            inContactWithFood = true;
        }
        if (!collider.IsTouchingLayers(food) && inContactWithFood)
        {
            inContactWithFood = false;
            Eat();
            //Debug.Log("Nom");
        }
    }

    private void Eat()
    {
        currentHealth = startingHealth;
        foodEaten++;
    }

    private void Hunger()
    {
        currentHealth -= Time.deltaTime;
        AdjustColor();

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void AdjustColor()
    {
        body.GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.Lerp(Color.red, Color.green, currentHealth/startingHealth));
    }

    private void Die()
    {
        alive = false;
        body.SetActive(false);
        timeEnd = Time.time;
        timeAlive = timeEnd - timeStart;
        CalculateFitness();
        SetLinesZero();
        //Debug.Log("Fitness: " + fitness);
    }

    private void SetLinesZero()
    {
        for(int i = 0; i < lines.Length; i++)
        {
            lines[i].Zero();
        }
    }

    public void ResetCreature()
    {
        //Debug.Log("Steve has been reset");
        alive = true;
        currentHealth = startingHealth;
        body.SetActive(true);
        body.transform.position = startingPos;
        foodEaten = 0;
        timeStart = Time.time;
    }

    public void CalculateFitness()
    {
        fitness = timeAlive * foodEaten;
    }

    public void UpdateRotation()
    {
        SetDistancesAndDrawGizmos();
        body.transform.Rotate(new Vector3(0f, 0f, brain.FeedForward(rayDistances)[0] * changeInAngle) * Time.deltaTime * 2);
        //CombineArrays(rayDistances, rayBools)
    }

    private float[] CombineArrays(float[] x, float[] y)
    {
        float[] inputs = new float[x.Length + y.Length];
        for(int i = 0; i < x.Length; i++)
        {
            inputs[i] = x[i];
            inputs[x.Length + i] = y[i];
        }

        return inputs;
    }

    private void SetDistancesAndDrawGizmos()
    {
        Vector2 pos = new Vector2(body.transform.position.x, body.transform.position.y);

        for (int i = 0; i < probes; i++)
        {
            Vector2 normalizedRay = RadianToVector2((Mathf.PI * i / gaps) + body.transform.rotation.ToEuler().z).normalized;
            RaycastHit2D hitWall = Physics2D.Raycast(pos, normalizedRay, rayLength, walls);
            RaycastHit2D hitFood = Physics2D.Raycast(pos, normalizedRay, rayLength, food);

            if (hitFood.collider != null)
            {
                rayDistances[i] = Map(hitFood.distance, 0, rayLength, 0, 1);
                Debug.DrawRay(pos, normalizedRay * hitFood.distance, Color.green);
                lines[i] = new Line(pos, (normalizedRay * hitFood.distance) + pos, Color.green);
                //Debug.Log(lines[i].ToString() + "   " + pos + "  " + (normalizedRay * hitFood.distance));
            }
            else if (hitWall.collider != null)
            {
                rayDistances[i] = Map(hitWall.distance, 0, rayLength, 0, 1) * -1;//-1 added for new calcs
                Debug.DrawRay(pos, normalizedRay * hitWall.distance, Color.red);
                lines[i] = new Line(pos, (normalizedRay * hitWall.distance) + pos, Color.red);
            }
            else
            {
                rayDistances[i] = 1;
                lines[i] = new Line(pos, normalizedRay, Color.clear);
                Debug.DrawRay(pos, normalizedRay * rayLength, Color.red);
            }
        }
    }

    public void ForwardMotion()
    {
        Vector2 theWhere = RadianToVector2(body.transform.rotation.ToEuler().z + Mathf.PI / 2) * constantSpeed;
        Vector2 pos = new Vector2(body.transform.position.x, body.transform.position.y);
        rb.velocity = theWhere;
    }

    public void MakeNewBrain()
    {
        brain = new NeuralNetwork(backupBrain);
    }

    public void MutateBrain()
    {
        brain.Mutate();
    }

    public float GetFitness()
    {
        return fitness;
    }

    public bool GetStatus()
    {
        return alive;
    }

    public Line[] GetLines()
    {
        return lines;
    }

    public void SetBrain(NeuralNetwork newBrain)
    {
        brain = new NeuralNetwork(newBrain);
    }

    public NeuralNetwork GetBrain()
    {
        return brain;
    }

    public GameObject GetGameObject()
    {
        return body;
    }

    public int GetNumberOfProbes()
    {
        return probes;
    }

    public void SetRenderers(GameObject[] lrs)
    {
        lineRenderers = lrs;
    }

    //Helper Functions
    //--------------------------

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
    //decimal res = 2.Map(1, 3, 0, 10);
    // res will be 5
}
