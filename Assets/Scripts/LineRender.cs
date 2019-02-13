using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRender : MonoBehaviour {

    private BackgroundCreatureDriver controller;
    private Line[] linesToDraw;
    private Material greenLine;
    private Material redLine;

    private void Awake()
    {
        controller = GameObject.Find("ScriptDriver").GetComponent<BackgroundCreatureDriver>();
        greenLine = controller.greenLine;
        redLine = controller.redLine;
    }

    private void Update()
    {
        linesToDraw = controller.linesToDraw;
        if(linesToDraw != null)
        {
            DrawLines();
        }
    }




    public void DrawLines()
    {
        for (int i = 0; i < linesToDraw.Length; i++)
        {
            Color color = linesToDraw[i].GetColor();
            Vector3 start = linesToDraw[i].GetStartAsVector3();
            Vector3 end = linesToDraw[i].GetEndAsVector3();



            if (color == Color.green)
            {
                RenderLines(start, end, greenLine);
            }
            else if (color == Color.red)
            {
                RenderLines(start, end, redLine);
            }
            Debug.Log("Line Drawn");
        }

        GL.End();
    }

    void RenderLines(Vector3 start, Vector3 end, Material mat)
    {
        GL.Begin(GL.LINES);
        GL.LoadOrtho();
        mat.SetPass(0);
        GL.Color(mat.color);
        GL.Vertex(start);
        GL.Vertex(end);
    }
}
