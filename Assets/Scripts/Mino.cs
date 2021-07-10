using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mino : MonoBehaviour
{
    public float previousTime;
    // minoの落ちる時間
    public float fallTime = 1f;

    // ステージの大きさ
    private static int width = 13;
    private static int height = 24;

    // mino回転
    public Vector3 rotationPoint;

    // grid
    private static Transform[,] grid = new Transform[width, height];

    // lineRendererのオフセット
    private static float lineOffset = 0.5f;

    void Update()
    {
        MinoMovememt();
    }

    private void MinoMovememt()
    {
        // 左矢印キーで左に動く
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);

            if (!ValidMovement())
            {
                transform.position -= new Vector3(-1, 0, 0);
            }

        }
        // 右矢印キーで右に動く
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);

            if (!ValidMovement())
            {
                transform.position -= new Vector3(1, 0, 0);
            }
        }
        // 自動で下に移動させつつ、下矢印キーでも移動する
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.Space) || Time.time - previousTime >= fallTime)
        {
            transform.position += new Vector3(0, -1, 0);

            if (!ValidMovement())
            {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                // 今回の追加
                CheckLines();
                this.enabled = false;
                FindObjectOfType<SpawnMino>().NewMino();
            }

            previousTime = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // ブロックの回転
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
        }
    }

    // 今回の追加 ラインがあるか？確認
    public void CheckLines()
    {
        for (int i = height - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);
            }
        }
    }

    // 今回の追加 列がそろっているか確認
    bool HasLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            if (grid[j, i] == null)
                return false;
        }
        return true;
    }

    // 今回の追加 ラインを消す
    void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
        }

    }

    // 今回の追加 列を下げる
    public void RowDown(int i)
    {
        for (int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, y] != null)
                {
                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

    void DrawLine(GameObject obj)
    {
        // 一旦LineRednererのある子オブジェクトを全て削除
        var childObjs = obj.GetComponentsInChildren<LineRenderer>();
        foreach (var item in childObjs)
        {
            Destroy(item.gameObject);
        }

        // ブロック(単品の座標)
        int roundX = Mathf.RoundToInt(obj.transform.position.x);
        int roundY = Mathf.RoundToInt(obj.transform.position.y);

        // 上
        if (roundY < height - 1)
        {
            if (grid[roundX, roundY - 1])
            {
                if (grid[roundX, roundY - 1].GetComponent<SpriteRenderer>().sprite != obj.GetComponent<SpriteRenderer>().sprite)
                {
                    var upobj = new GameObject("LineObject");
                    upobj.transform.parent = obj.transform;
                    var lineRenderer = upobj.AddComponent<LineRenderer>();
                    var positions = new Vector3[] {
                        new Vector3(roundX - lineOffset ,roundY + lineOffset, 0),
                        new Vector3(roundX + lineOffset ,roundY + lineOffset, 0),
                    };
                    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    lineRenderer.startColor = Color.yellow;
                    lineRenderer.endColor = Color.yellow;
                    lineRenderer.startWidth = 0.1f;
                    lineRenderer.endWidth = 0.1f;
                    lineRenderer.SetPositions(positions);

                }
            } else {
                       var upobj = new GameObject("LineObject");
                    upobj.transform.parent = obj.transform;
                    var lineRenderer = upobj.AddComponent<LineRenderer>();
                    var positions = new Vector3[] {
                        new Vector3(roundX - lineOffset ,roundY + lineOffset, 0),
                        new Vector3(roundX + lineOffset ,roundY + lineOffset, 0),
                    };
                    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    lineRenderer.startColor = Color.yellow;
                    lineRenderer.endColor = Color.yellow;
                    lineRenderer.startWidth = 0.1f;
                    lineRenderer.endWidth = 0.1f;
                    lineRenderer.SetPositions(positions);
             
            }

        }
        return;
        // 上
        var downobj = new GameObject("LineObject2");
        downobj.transform.parent = obj.transform;
        var downlineRenderer = downobj.AddComponent<LineRenderer>();
        var downpositions = new Vector3[] {
                    // 左
                    new Vector3(roundX + lineOffset ,roundY - lineOffset, 0),
                    new Vector3(roundX - lineOffset ,roundY - lineOffset, 0),
                };
        downlineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        downlineRenderer.startColor = Color.yellow;
        downlineRenderer.endColor = Color.yellow;
        downlineRenderer.startWidth = 0.1f;
        downlineRenderer.endWidth = 0.1f;
        downlineRenderer.SetPositions(downpositions);


    }

    void AddToGrid()
    {

        foreach (Transform children in transform)
        {
            int roundX = Mathf.RoundToInt(children.transform.position.x);
            int roundY = Mathf.RoundToInt(children.transform.position.y);
            grid[roundX, roundY] = children;
            DrawLine(children.gameObject);
        }

    }

    // minoの移動範囲の制御
    bool ValidMovement()
    {

        foreach (Transform children in transform)
        {
            int roundX = Mathf.RoundToInt(children.transform.position.x);
            int roundY = Mathf.RoundToInt(children.transform.position.y);

            // minoがステージよりはみ出さないように制御
            if (roundX < 0 || roundX >= width || roundY < 3 || roundY >= height)
            {
                return false;
            }
            if (grid[roundX, roundY] != null)
            {
                return false;
            }


        }
        return true;
    }
}