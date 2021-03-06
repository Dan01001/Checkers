﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkers_Board : MonoBehaviour
{

    public Piece[,] pieces = new Piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    Vector3 boardOffset = new Vector3(-4f, 0, -4f);
    Vector3 pieceOffset = new Vector3(0.5f, 0, 0.5f);

    private Vector2 mouseOver;
    private Piece selectedPiece;
    private List<Piece> forcedPieces;
    private Vector2 startDrag;
    private Vector2 endDrag;
    private bool isWhiteTurn;
    public bool isWhite;
    private bool hasKilled;

    void Start()
    {
        isWhiteTurn = true;
        forcedPieces = new List<Piece>();
        GenerateBoard();
    }

    void Update()
    {
        UpdateMouseOver();
        if((isWhite)? isWhiteTurn :!isWhiteTurn)
        {
         
            if(selectedPiece != null)
            {
                UpdatePieceDrag(selectedPiece);
            }

            if (Input.GetMouseButtonDown(0))
            {
                SelectPiece((int)mouseOver.x, (int)mouseOver.y);
            }

            if (Input.GetMouseButtonUp(0))
            {
                TryMove((int)startDrag.x, (int)startDrag.y, (int)mouseOver.x, (int)mouseOver.y);
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
           
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }

    void UpdatePieceDrag(Piece p)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
    }

    void SelectPiece(int x, int y)
    {
        //out of bounds
        if(x < 0 || x >= 8 || y < 0 || y >= 8)
        {
            return;
        }

        Piece p = pieces[x, y];

        if(p != null && p.isWhite == isWhite)
        {
            if (forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
            }
            else
            {
                //look for the piece under our forced pieces list
                if(forcedPieces.Find(fp => fp == p) == null)
                {
                    return;
                }
                selectedPiece = p;
                startDrag = mouseOver;
            }

           
       
        }
    }

    void TryMove(int x1, int y1, int x2, int y2)
    {
        forcedPieces = ScanForPossibleMove();

        //multiplayer support
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        //out of bounds
        if (x2 < 0 || x2 > 8 || y2 < 0 || y2 >= 8)
        {
            if(selectedPiece != null)
            {
                MovePiece(selectedPiece, x1, y1);
            }
            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
        }
        
        if (selectedPiece != null)
        {
            //if it has not moved
            if(endDrag == startDrag)
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }

            //check if its a valid move
            if (selectedPiece.ValidMove(pieces, x1, y1, x2, y2))
            {
                // did we kill anything if this is a jump
                if (Mathf.Abs(x2 - x1) == 2)
                {
                    Piece p = pieces[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null)
                    {
                        pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                        Destroy(p.gameObject);
                        hasKilled = true;
                    }
                }

                //Were we supposed to kill anything
                if (forcedPieces.Count != 0 && !hasKilled)
                {
                    MovePiece(selectedPiece, x1, y1);
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    return;
                }

                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2);

                EndTurn();
            }
            else
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }


        }
    }

    void EndTurn()
    {

        int x = (int)endDrag.x;
        int y = (int)endDrag.y;

        if(selectedPiece != null)
        {
            if(selectedPiece.isWhite && !selectedPiece.isKing && y == 7)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);

            }
            else if(!selectedPiece.isWhite && !selectedPiece.isKing && y == 0)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.Rotate(Vector3.right * 180);
            }
        }
      
        selectedPiece = null;
        startDrag = Vector2.zero;

        if(ScanForPossibleMove(selectedPiece, x,y).Count != 0 && hasKilled)
        {
            return;
        }

        isWhiteTurn = !isWhiteTurn;
       // isWhite = !isWhite;
        hasKilled = false;
        CheckVictory();
    }

    void CheckVictory()
    {
        var ps = FindObjectsOfType<Piece>();
        bool hasWhite = false, hasBlack = false;
        for(int i = 0; i< ps.Length; i++)
        {
            if (ps[i].isWhite)
                hasWhite = true;
            else
                hasBlack = true;
        }

        if (!hasWhite)
            Victory(false);
        if (!hasBlack)
            Victory(true);
      
    }
    void Victory(bool isWhite)
    {
        if (isWhite)
            Debug.Log("White team has won");
        else
            Debug.Log("Black team has won");
    }
    List<Piece> ScanForPossibleMove(Piece p, int x, int y)
    {
        forcedPieces = new List<Piece>();
        if (pieces[x, y].IsForcedToMove(pieces, x, y))
        {
            forcedPieces.Add(pieces[x, y]);
        }
            

        return forcedPieces;
    }

    private List<Piece> ScanForPossibleMove()
    {
        forcedPieces = new List<Piece>();

        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (pieces[i, j] != null && pieces[i, j].isWhite == isWhiteTurn)
                    if (pieces[i, j].IsForcedToMove(pieces, i, j))
                        forcedPieces.Add(pieces[i, j]);

        return forcedPieces;
    }

    void GenerateBoard()
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
