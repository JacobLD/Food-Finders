

using System.Collections.Generic;
using System;

public class NeuralNetwork{

    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;
    private float fitness;

    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for(int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        InitNeurons();
        InitWeights();
    }

    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }

        InitNeurons();
        InitWeights();

        CopyWeights(copyNetwork.weights);
    }

    private void CopyWeights(float[][][] copyWeights)
    {
        for(int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }

    private void InitWeights()
    {
        List<float[][]> weightList = new List<float[][]>();

        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> LayerWeightList = new List<float[]>();

            int neuronsInPerviousLayer = layers[i - 1];

            for(int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPerviousLayer];

                for(int k = 0; k < neuronsInPerviousLayer; k++)
                {
                    //give random weight
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                LayerWeightList.Add(neuronWeights);
            }

            weightList.Add(LayerWeightList.ToArray());
        }

        weights = weightList.ToArray();
    }

    private void InitNeurons()
    {
        List<float[]> neuronList = new List<float[]>();

        for(int i = 0; i < layers.Length; i++)
        {
            neuronList.Add(new float[layers[i]]);
        }

        neurons = neuronList.ToArray();
    }

    public float[] FeedForward(float[] inputs)
    {
        for(int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        for(int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0.25f;
                for (int k = 0; k < neurons[i-1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }

                neurons[i][j] = (float)Math.Tanh(value);
            }
        }

        return neurons[neurons.Length - 1];
    }

    public void Mutate()
    {
        for(int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j][k]; k++)
                {
                    float weight = weights[i][j][k];

                    //mutate weight
                    float randomNum = UnityEngine.Random.Range(0,1000);

                    if(randomNum <= 2f)
                    {
                        weight *= -1;
                    }
                    else if(randomNum <= 4f)
                    {
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if(randomNum <= 6f)
                    {
                        weight *= UnityEngine.Random.Range(0f, 1f) + 1f;
                    }
                    else if(randomNum <= 8f)
                    {
                        weight *= UnityEngine.Random.Range(0f, 1f);
                    }

                    weights[i][j][k] = weight;
                }
            }
        }
    }

    public void SetFitness(float fitness)
    {
        this.fitness = fitness;
    }

    public float GetFitness()
    {
        return fitness;
    }

    public override string ToString()
    {
        return weights.ToString();
    }

    public void Reset()
    {
        InitNeurons();
        InitWeights();
    }

    public float[][][] GetWeights()
    {
        return weights;
    }

    public void Splice(NeuralNetwork dominateWeights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    if(UnityEngine.Random.Range(0, 100) > 50)
                    {
                        weights[i][j][k] = dominateWeights.GetWeights()[i][j][k];
                    }
                }
            }
        }
    }

}
