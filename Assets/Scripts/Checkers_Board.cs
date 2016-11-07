﻿using UnityEngine;
using System.Collections;

public class Checkers_Board : MonoBehaviour
{

    public Piece[,] pieces = new Piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    Vector3 boardOffset = new Vector3(-4f, 0, -4f);
    Vector3 pieceOffset = new Vector3(0.5f, 0, 0.5f);

    private Vector2 mouseOver;
    private Piece selectedPiece;
    private Vector2 startDrag;
    private Vector2 endDrag;


    void Start()
    {
        GenerateBoard();
    }

    void Update()
    {
        UpdateMouseOver();
        //if it is my turn
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if (Input.GetMouseButtonDown(0))
            {
                SelectPiece(x, y);
            }

            if (Input.GetMouseButtonUp(0))
            {
                TryMove((int)startDrag.x, (int)startDrag.y, x, y);
            }
        }
    }

    void UpdateMouseOver()
    {
        //if its my turn
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
            //Debug.Log(mouseOver);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    void SelectPiece(int x, int y)
    {
        //out of bounds
        if(x < 0 || x >= pieces.Length || y < 0 || y >= pieces.Length)
        {
            return;
        }

        Piece p = pieces[x, y];

        if(p != null)
        {
            selectedPiece = p;
            startDrag = mouseOver;
            Debug.Log(selectedPiece.name);
        }
    }

    void TryMove(int x1, int y1, int x2, int y2)
    {
        //multiplayer support
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        MovePiece(selectedPiece, x2, y2);
    }

    private void GenerateBoard()
    {
        //Generate White Team
        for (int y = 0; y < 3; y++)
        {
            bool oddRow = (y % 2 == 0);     
            for (int x = 0; x < 8; x += 2)
            {
                //Generate out piece
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }

        // Generate black Team
        for (int y = 7; y > 4; y--)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                //Generate out piece
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }
    }

    private void GeneratePiece(int x, int y)
    {
        bool isPieceWhite = (y > 3) ? false : true;
        GameObject go = Instantiate((isPieceWhite) ?  whitePiecePrefab : blackPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }

    void MovePiece(Piece p, int x, int y)
    {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * y) + boardOffset + pieceOffset;
    }
}