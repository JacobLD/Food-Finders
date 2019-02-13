using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ValuesToPass : MonoBehaviour {

    public float probes;
    public float deltaAngle;
    public float speed;
    public float length;
    public float lifespan;
    public float populationSize;
    public bool showRays;
    private bool loadMain;
    private bool loadMenu;


    void Start () {
        DontDestroyOnLoad(transform.gameObject);
        probes = 0f;
        deltaAngle = 0f;
        speed = 0f;
        length = 0f;
        lifespan = 0f;
        populationSize = 0f;
        showRays = false;
        loadMain = false;
        loadMenu = false;
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "Menu")
        {
            speed = GameObject.Find("Speed Slider").GetComponent<Slider>().value;
            probes = GameObject.Find("number of probes slider").GetComponent<Slider>().value;
            deltaAngle = GameObject.Find("Angle Slider").GetComponent<Slider>().value;
            lifespan = GameObject.Find("Lifespan slider").GetComponent<Slider>().value;
            length = GameObject.Find("Probe Length Slider").GetComponent<Slider>().value;
            populationSize = GameObject.Find("Population Size slider").GetComponent<Slider>().value;
            showRays = GameObject.Find("Toggle").GetComponent<Toggle>().isOn;
            Button start = GameObject.Find("Button").GetComponent<Button>();
            start.onClick.AddListener(StartSimulation);

            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
        else if (SceneManager.GetActiveScene().name == "Main")
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                ToMainMenu();
            }
        }
    }

    IEnumerator LoadYourAsyncScene(string scene)
    {
        // The Application loads the Scene in the background at the same time as the current Scene.
        //This is particularly good for creating loading screens. You could also load the Scene by build //number.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        //Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private void StartSimulation()
    {
        if (!loadMain)
        {
            StartCoroutine(LoadYourAsyncScene("Main"));
            //SceneManager.UnloadSceneAsync("Menu");
            loadMain = true;
        }
    }

    private void ToMainMenu()
    {
        if (!loadMenu)
        {
            StartCoroutine(LoadYourAsyncScene("Menu"));
            //SceneManager.UnloadSceneAsync("Menu");
            loadMenu = true;
        }
    }
}
