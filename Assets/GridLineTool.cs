using System.Runtime.InteropServices.ComTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLineTool
{
    /// <summary>
    /// 计算两点间经过的格子
    /// </summary>
    public static List<Vector2Int> GetGridLinePoints(Vector2Int from, Vector2Int to, float width = 0, int maxLength = 0)
    {
        List<Vector2Int> touchedGrids = new List<Vector2Int>();
        foreach (var offset in GetTargetPosToZeroGridLinePoints(to - from, width, maxLength))
        {
            var pos = from + offset;
            if (!touchedGrids.Contains(pos)) touchedGrids.Add(pos);
        }
        return touchedGrids;
    }

    /// <summary>
    /// 计算目标位置到原点所经过的格子
    /// </summary>
    static List<Vector2Int> GetTargetPosToZeroGridLinePoints(Vector2Int target, float width = 0, int maxLength = 0)
    {
        List<Vector2Int> touched = new List<Vector2Int>();
        bool steep = Mathf.Abs(target.y) > Mathf.Abs(target.x);
        float x = steep ? target.y : target.x;
        float y = steep ? target.x : target.y;

        //斜率
        float l = Mathf.Sqrt(Mathf.Pow(target.x, 2) + Mathf.Pow(target.y, 2));
        float tan = (float)y / x;
        float sin = (float)y / l;
        float cos = (float)x / l;

        float delta = x > 0 ? 0.5f : -0.5f;

        List<Vector2Int> tempList;

        for (int i = 0; i <= 2 * Mathf.Abs(x); i++)
        {
            float tempX = i * delta;
            float tempY = tan * tempX;

            bool isOnXEdge = Mathf.Abs(tempX - Mathf.FloorToInt(tempX)) == 0.5f;
            bool isOnYEdge = Mathf.Abs(tempY - Mathf.FloorToInt(tempY)) == 0.5f;

            Vector2Int pos;

            tempList = new List<Vector2Int>();

            if (isOnXEdge && isOnYEdge) // 点位在格子交点处
            {
                // continue;
            }
            else if (isOnXEdge && !isOnYEdge) // X在格子边缘，Y在格子内部
            {
                // 左右格子均加入
                pos = new Vector2Int(Mathf.CeilToInt(tempX), Mathf.RoundToInt(tempY));
                if (!tempList.Contains(pos)) tempList.Add(pos);
                pos = new Vector2Int(Mathf.FloorToInt(tempX), Mathf.RoundToInt(tempY));
                if (!tempList.Contains(pos)) tempList.Add(pos);
            }
            else if (!isOnXEdge && isOnYEdge) // Y在格子内部，X在格子边缘
            {
                // 上下格子均加入
                pos = new Vector2Int(Mathf.RoundToInt(tempX), Mathf.CeilToInt(tempY));
                if (!tempList.Contains(pos)) tempList.Add(pos);
                pos = new Vector2Int(Mathf.RoundToInt(tempX), Mathf.FloorToInt(tempY));
                if (!tempList.Contains(pos)) tempList.Add(pos);
            }
            else if (!isOnXEdge && !isOnYEdge) // 点位在格子内部
            {
                // 当前格子加入
                pos = new Vector2Int(Mathf.RoundToInt(tempX), Mathf.RoundToInt(tempY));
                if (!tempList.Contains(pos)) tempList.Add(pos);
            }

            // 计算宽度
            width = width > 1 ? width : 1;
            if (width - 1 > 0)
            {
                int sideWidth = Mathf.CeilToInt((width - 1) / 2f);
                int num = 4;
                for (int j = 1; j <= sideWidth * num; j++)
                {
                    var w = j * 1f / num;
                    var npos = new Vector2(tempX + w * -sin, tempY + w * cos);
                    pos = Vector2ToVector2Int(npos);
                    var len = Mathf.Sqrt(Mathf.Pow(pos.x - tempX, 2) + Mathf.Pow(pos.y - tempY, 2));
                    if (len <= w)
                    {
                        if (!tempList.Contains(pos)) tempList.Add(pos);
                    }

                    npos = new Vector2(tempX + w * sin, tempY + w * -cos);
                    pos = Vector2ToVector2Int(npos);
                    len = Mathf.Sqrt(Mathf.Pow(pos.x - tempX, 2) + Mathf.Pow(pos.y - tempY, 2));
                    if (len <= w)
                    {
                        if (!tempList.Contains(pos)) tempList.Add(pos);
                    }
                }
            }

            // 判断最大长度
            foreach (var temp in tempList)
            {
                if (temp.x == Int32.MinValue || temp.y == Int32.MinValue) continue;
                if (maxLength <= 0 || Mathf.Max(Mathf.Abs(temp.x), Mathf.Abs(temp.y)) <= maxLength - 1)
                {
                    if (!touched.Contains(temp)) touched.Add(temp);
                }
            }


        }

        if (steep)
        {
            //镜像翻转 交换 X Y
            for (int i = 0; i < touched.Count; i++)
            {
                Vector2Int v = touched[i];
                v.x = v.x ^ v.y;
                v.y = v.x ^ v.y;
                v.x = v.x ^ v.y;

                touched[i] = v;
            }
        }

        return touched;
    }

    public static Vector2Int Vector2ToVector2Int(Vector2 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

}
