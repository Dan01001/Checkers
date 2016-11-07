using UnityEngine;
using System.Collections;

public class TEST : MonoBehaviour
{

    public Piece[,] pieces = new Piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    public Vector3 BoardOffset = new Vector3(-4.0f, 0, -4.0f);

    void start()
    {
        Debug.Log("Start");
        GenerateBoard();
    }
    void update()
    {
        Debug.Log("yep");
    }
    private void GenerateBoard()
    {
        //Generate White Team
        for (int y = 0; y < 3; y++)
        {
            Debug.Log(y);
            for (int x = 0; x < 8; x += 2)
            {
                Debug.Log(x);
                //Generate out piece
                GeneratePiece(x, y);
            }
        }
    }

    private void GeneratePiece(int x, int y)
    {
        GameObject go = Instantiate(whitePiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }

    void MovePiece(Piece p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y);
    }
}
