using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GridManager : MonoBehaviour
{
    public Rect Size;
    public GameObject GridGo;
    public Text GridText;
    public InputField WidthField;
    public InputField LengthField;

    public Vector2Int StartPos;
    public Vector2Int TargetPos;

    void Start()
    {
        for (int i = 0; i <= Size.width; i++)
        {
            var x = Size.x + i;
            for (int j = 0; j <= Size.height; j++)
            {
                var y = Size.y + j;
                CreateGrid(new Vector2Int((int)x,(int)y));
            }
        }
    }

    GameObject CreateGrid(Vector2Int pos)
    {
        GameObject go = GameObject.Instantiate(GridGo);
        go.name = string.Format("Pos({0},{1})",pos.x,pos.y);
        go.transform.parent = this.transform;
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        Rect rect = rectTransform.rect;
        go.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        go.transform.localPosition = new Vector3(pos.x * rect.width,pos.y * rect.height,0);

        var trigger = go.GetComponent<EventTrigger>() ?? go.AddComponent<EventTrigger>();

        UnityAction<BaseEventData> onPointerEnter = new UnityAction<BaseEventData>((data)=>{ OnPointerEnter(go,pos); });
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener(onPointerEnter);

        UnityAction<BaseEventData> onPointerClick = new UnityAction<BaseEventData>((data) => { OnPointerClick(go, pos); });
        EventTrigger.Entry pointerClickEntry = new EventTrigger.Entry();
        pointerClickEntry.eventID = EventTriggerType.PointerClick;
        pointerClickEntry.callback.AddListener(onPointerClick);

        trigger.triggers.Add(pointerEnterEntry);
        trigger.triggers.Add(pointerClickEntry);
        return go;
    }
    public void OnPointerEnter(GameObject gridGo, Vector2Int pos)
    {
        GridText.text = gridGo.name;
        if(StartPos!=null){
            TargetPos = pos;
            DrawLine(StartPos,TargetPos);
        }
    }
    public void OnPointerClick(GameObject gridGo,Vector2Int pos)
    {
        StartPos = pos;
        DrawLine(StartPos, StartPos);
    }

    public void SetColor(Vector2Int pos,Color color){
        var node = transform.Find(string.Format("Pos({0},{1})", pos.x, pos.y));
        if(node == null) return;
        node.GetComponent<Image>().color = color;
    }

    public void ResetAllColor(){
        foreach (var comp in transform.GetComponentsInChildren<Image>())
        {
            comp.color = Color.white;
        }
    }

    public void DrawLine(Vector2Int startPos,Vector2Int targetPos){
        ResetAllColor();
        int width = 1;
        int length = 0;
        try
        {
            width = int.Parse(WidthField.text);
        }
        catch
        {
            width = 1;
        }
        try
        {
            length = int.Parse(LengthField.text);
        }
        catch
        {
            length = 0;
        }

        foreach (var pos in GridLineTool.GetGridLinePoints(startPos, targetPos, width,length))
        {
            SetColor(pos,Color.blue);
        }
    }

}
