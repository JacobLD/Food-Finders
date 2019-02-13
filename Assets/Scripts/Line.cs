using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line{

    private Vector2 start;
    private Vector2 end;
    private Color _color;

    public Line(Vector2 start, Vector2 end, Color _color)
    {
        this.start = start;
        this.end = end;
        this._color = _color;
    }

    public void Zero()
    {
        start = Vector2.zero;
        end = Vector2.zero;
        _color = Color.clear;
    }

    public void SetColor(Color _color)
    {
        this._color = _color;
    }

    public void SetStart(Vector2 start)
    {
        this.start = start;
    }

    public void SetEnd(Vector2 end)
    {
        this.end = end;
    }

    public Vector3 GetStartAsVector3()
    {
        return new Vector3(start.x, start.y, 0f);
    }

    public Vector3 GetEndAsVector3()
    {
        return new Vector3(end.x, end.y, 0f);
    }

    public Color GetColor()
    {
        return _color;
    }

    public override string ToString()
    {
        return this.start.ToString() + " " + end.ToString();
    }
}
