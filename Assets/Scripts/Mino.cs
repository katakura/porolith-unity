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
    private static float lineOffset = 0.50f;

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

    void DrawLineSub(LineRenderer lnobj, Vector3[] vect)
    {
        lnobj.material = new Material(Shader.Find("Sprites/Default"));
        lnobj.startColor = Color.white;
        lnobj.endColor = Color.white;
        lnobj.startWidth = 0.15f;
        lnobj.endWidth = 0.15f;
        lnobj.SetPositions(vect);
        return;

    }

    private void DrawLineTop(GameObject obj)
    {
        int roundX = Mathf.RoundToInt(obj.transform.position.x);
        int roundY = Mathf.RoundToInt(obj.transform.position.y);

        var newobj = new GameObject("LineObjectTop");
        newobj.transform.parent = obj.transform;
        var lineRendererTop = newobj.AddComponent<LineRenderer>();
        var positions = new Vector3[] {
                        new Vector3(roundX - lineOffset ,roundY + lineOffset, 0),
                        new Vector3(roundX + lineOffset ,roundY + lineOffset, 0),
                    };
        DrawLineSub(lineRendererTop, positions);
    }

    private void DrawLineBottom(GameObject obj)
    {
        int roundX = Mathf.RoundToInt(obj.transform.position.x);
        int roundY = Mathf.RoundToInt(obj.transform.position.y);

        var newobj = new GameObject("LineObjectBottom");
        newobj.transform.parent = obj.transform;
        var lineRendererTop = newobj.AddComponent<LineRenderer>();
        var positions = new Vector3[] {
                        new Vector3(roundX - lineOffset ,roundY - lineOffset, 0),
                        new Vector3(roundX + lineOffset ,roundY - lineOffset, 0),
                    };
        DrawLineSub(lineRendererTop, positions);
    }

    private void DrawLineLeft(GameObject obj)
    {
        int roundX = Mathf.RoundToInt(obj.transform.position.x);
        int roundY = Mathf.RoundToInt(obj.transform.position.y);

        var newobj = new GameObject("LineObjectLeft");
        newobj.transform.parent = obj.transform;
        var lineRendererTop = newobj.AddComponent<LineRenderer>();
        var positions = new Vector3[] {
                        new Vector3(roundX - lineOffset ,roundY - lineOffset, 0),
                        new Vector3(roundX - lineOffset ,roundY + lineOffset, 0),
                    };
        DrawLineSub(lineRendererTop, positions);
    }
    private void DrawLineRight(GameObject obj)
    {
        int roundX = Mathf.RoundToInt(obj.transform.position.x);
        int roundY = Mathf.RoundToInt(obj.transform.position.y);

        var newobj = new GameObject("LineObjectRight");
        newobj.transform.parent = obj.transform;
        var lineRendererTop = newobj.AddComponent<LineRenderer>();
        var positions = new Vector3[] {
                        new Vector3(roundX + lineOffset ,roundY - lineOffset, 0),
                        new Vector3(roundX + lineOffset ,roundY + lineOffset, 0),
                    };
        DrawLineSub(lineRendererTop, positions);
    }

    void DrawLine(GameObject obj)
    {
        // ブロック(単品の座標)
        int roundX = Mathf.RoundToInt(obj.transform.position.x);
        int roundY = Mathf.RoundToInt(obj.transform.position.y);

        // 上
        if (roundY < height - 1)
        {
            if (grid[roundX, roundY + 1])
            {
                if (grid[roundX, roundY + 1].GetComponent<SpriteRenderer>().sprite != obj.GetComponent<SpriteRenderer>().sprite)
                {
                    Transform childGameObject = obj.transform.Find("LineObjectTop");
                    if (!childGameObject)
                    {
                        DrawLineTop(obj);
                    }
                }
                else
                {
                    Transform childGameObject = obj.transform.Find("LineObjectTop");
                    if (childGameObject)
                    {
                        Destroy(childGameObject.gameObject);

                    }
                }
            }
            else
            {
                Transform childGameObject = obj.transform.Find("LineObjectTop");
                if (!childGameObject)
                {
                    DrawLineTop(obj);
                }
            }
        }
        // 下
        if (roundY > 0)
        {
            if (grid[roundX, roundY - 1])
            {
                if (grid[roundX, roundY - 1].GetComponent<SpriteRenderer>().sprite != obj.GetComponent<SpriteRenderer>().sprite)
                {
                    Transform childGameObject = obj.transform.Find("LineObjectBottom");
                    if (!childGameObject)
                    {
                        DrawLineBottom(obj);
                    }
                }
                else
                {
                    Transform childGameObject = obj.transform.Find("LineObjectBottom");
                    if (childGameObject)
                    {
                        Destroy(childGameObject.gameObject);

                    }
                }
            }
            else
            {
                Transform childGameObject = obj.transform.Find("LineObjectBottom");
                if (!childGameObject)
                {
                    DrawLineBottom(obj);
                }
            }
        }
        // 右
        if (roundX < width - 1)
        {
            if (grid[roundX + 1, roundY])
            {
                if (grid[roundX + 1, roundY].GetComponent<SpriteRenderer>().sprite != obj.GetComponent<SpriteRenderer>().sprite)
                {
                    Transform childGameObject = obj.transform.Find("LineObjectRight");
                    if (!childGameObject)
                    {
                        DrawLineRight(obj);
                    }
                }
                else
                {
                    Transform childGameObject = obj.transform.Find("LineObjectRight");
                    if (childGameObject)
                    {
                        Destroy(childGameObject.gameObject);

                    }
                }
            }
            else
            {
                Transform childGameObject = obj.transform.Find("LineObjectRight");
                if (!childGameObject)
                {
                    DrawLineRight(obj);
                }
            }
        }
        // 左
        if (roundX > 0)
        {
            if (grid[roundX - 1, roundY])
            {
                if (grid[roundX - 1, roundY].GetComponent<SpriteRenderer>().sprite != obj.GetComponent<SpriteRenderer>().sprite)
                {
                    Transform childGameObject = obj.transform.Find("LineObjectLeft");
                    if (!childGameObject)
                    {
                        DrawLineLeft(obj);
                    }
                }
                else
                {
                    Transform childGameObject = obj.transform.Find("LineObjectLeft");
                    if (childGameObject)
                    {
                        Destroy(childGameObject.gameObject);

                    }
                }
            }
            else
            {
                Transform childGameObject = obj.transform.Find("LineObjectLeft");
                if (!childGameObject)
                {
                    DrawLineLeft(obj);
                }
            }
        }


    }

    void AddToGrid()
    {

        foreach (Transform children in transform)
        {
            int roundX = Mathf.RoundToInt(children.transform.position.x);
            int roundY = Mathf.RoundToInt(children.transform.position.y);
            grid[roundX, roundY] = children;
        }
        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, i])
                {
                    DrawLine(grid[j, i].gameObject);
                }
            }
        }
        DumpGrid();
    }

    // minoの移動範囲の制御
    bool ValidMovement()
    {

        foreach (Transform children in transform)
        {
            int roundX = Mathf.RoundToInt(children.transform.position.x);
            int roundY = Mathf.RoundToInt(children.transform.position.y);

            // minoがステージよりはみ出さないように制御
            if (roundX < 0 || roundX >= width || roundY < 0 || roundY >= height)
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
    private void DumpGrid()
    {
        for (int i = height - 1; i >= 0; i--)
        {
            string msg = "";
            for (int j = 0; j < width; j++)
            {
                if (grid[j, i] != null)
                {
                    msg += "□　";
                }
                else
                {
                    msg += "　　";
                }
            }
            //Debug.Log(msg);
        }
    }
}