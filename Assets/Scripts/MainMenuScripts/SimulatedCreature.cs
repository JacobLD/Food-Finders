using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulatedCreature : MonoBehaviour {

    static Material lineMaterial;
    public Toggle showRaysBool;
    public Slider probeLengthSlider;
    public Slider numberOfProbesSlider;
    int probeLength;
    int numberOfProbes;

	
	void Update () {
        probeLength = (int)probeLengthSlider.value;
        numberOfProbes = (int)numberOfProbesSlider.value;
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

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        if (showRaysBool.isOn)
        {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            GL.MultMatrix(transform.localToWorldMatrix);

            // Draw lines
            GL.Begin(GL.LINES);
            for (int i = 0; i < numberOfProbes; ++i)
            {
                float a = i / (float)(numberOfProbes - 1);
                float angle = a * Mathf.PI;
                // Vertex colors change from red to green
                GL.Color(Color.red);
                // One vertex at transform position
                GL.Vertex3(0, 0, 0);
                // Another vertex at edge of circle
                GL.Vertex3(Mathf.Cos(angle) * probeLength, Mathf.Sin(angle) * probeLength, 0);
            }
            GL.End();
            GL.PopMatrix();
        }
    }
}
