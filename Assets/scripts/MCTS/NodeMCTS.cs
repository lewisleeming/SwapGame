using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class NodeMCTS
{
    public int score;
    public int timesVisited;
    
    public NodeMCTS parent;
    public List<NodeMCTS> children;
    public List<NodeMCTS> availableMoveNodes;
    public List<object> BoardState;
    public List<object> SavedBoard;
    public List<List<object>> possibleMoves;
    public bool GameOver;
    public static List<int[]> ValidMoves;
    public List<List<object>> movesPlaced;
    public List<object> stored;
    public Board board;
    public List<object> move;

    //variables for representing game state

    /*public NodeMCTS()
    {
        score = 0;
        timesVisited = 0;
        parent = null;
        children = new List<NodeMCTS> ();
        availableMoves = new List<NodeMCTS>();
        boardArray = new [] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        playerOneMovesTaken = new List<int>();
        playerTwoMovesTaken = new List<int>();
        allBoardPositions = new List<int>();
        playerOneTiles = new Dictionary<int, int>();
        playerTwoTiles = new Dictionary<int, int>();
        //ValidMoves = new List<int[]>();
        //randomScript.addValidMoves(ValidMoves);
        state = gameState.PLAYERONE;
        AddAvailableMoves(boardArray,allBoardPositions,
            playerOneMovesTaken,playerTwoMovesTaken,
            playerOneTiles, playerTwoTiles, state);
    }*/

    public NodeMCTS(List<object> b)
    {
        score = 0;
        timesVisited = 0;
        parent = null;
        children = new List<NodeMCTS>();
        availableMoveNodes = new List<NodeMCTS>();
        move = b;
        board = new Board(b);
        addAvailableMoveNodes(board.availableMoves);
    }
    /*boardArray,allBoardPositions,
    playerOneMovesTaken,playerTwoMovesTaken,
    playerOneTiles, playerTwoTiles, state
    
    var BoardPositionsArray = BoardState[0];
        var allBoardPositions = BoardState[1];
        var playerOneTiles = BoardState[2];
        var playerTwoTiles = BoardState[3];
        var playerOneMovesTaken = BoardState[4];
        var playerTwoMovesTaken = BoardState[5];
        var state = BoardState[6];
    */
    
    public NodeMCTS(NodeMCTS Parent, List<object> b)
    {
        score = 0;
        timesVisited = 0;
        parent = Parent;
        children = new List<NodeMCTS> ();
        availableMoveNodes = new List<NodeMCTS> (parent.availableMoveNodes);
        move = b;
        board = new Board(parent.move);
        board.applyMove(b);
        board.GenerateAvailableMoves();
        //AddAvailableMoves(board.availableMoves.Keys.ToList());
    }
    
    public NodeMCTS(NodeMCTS n)
    {
        score = n.score;
        timesVisited = n.timesVisited;
        parent = n.parent;
        children = new List<NodeMCTS>(n.children);
        availableMoveNodes = new List<NodeMCTS>(n.availableMoveNodes);
        board = new Board(n.move);
        move = n.move;
    }
    
    /*List<int[]> tempMoves = new List<int[]>();
    List<int[]> tempBoardArrays = new List<int[]>();
    List<Dictionary<int,int>> tempTiles = new List<Dictionary<int, int>>();
    List<List<int>> tempBoardPositions = new List<List<int>>();
    
    //sets variables needed for makeMove methods
    List<int[]> playerMovesTaken2;
    Dictionary<int, int> playerTiles2;
    gameState nextPlayer;
    
    
    if (state2 == gameState.PLAYERONE)
    {
        playerMovesTaken2 = playerOneMovesTaken2;
        playerTiles2 = playerOneTiles2;
        nextPlayer = gameState.PLAYERTWO;
    }
    else
    {
        playerMovesTaken2 = playerTwoMovesTaken2;
        playerTiles2 = playerTwoTiles2;
        nextPlayer = gameState.PLAYERONE;
    }*/

    public void addAvailableMoveNodes(List<List<object>> boardMoves)
    {
        foreach (List<object> bm in boardMoves)
        {
            availableMoveNodes.Add(new NodeMCTS(this, bm));
        }
    }
    
    public NodeMCTS Expand()
    {
        if (availableMoveNodes.Count > 0) {
            NodeMCTS ret = availableMoveNodes[0];
            AddChild(ret);
            availableMoveNodes.Remove(ret);
            return ret;
        }
        return null;
    }

    public void AddChild(NodeMCTS child)
    {
        children.Add(child);
    }
    
    public NodeMCTS BestChild()
    {
        double bestVal = double.MinValue;
        NodeMCTS bestChild = null;

        foreach(NodeMCTS node in children)
        {
            double utc = ((double)node.score / (double)node.timesVisited) + MCTSscript.getRHS(timesVisited, node.timesVisited);

            if (utc > bestVal){
                bestChild = node;
                bestVal = utc;
            }
        }
        return bestChild;
    }
    public void Backup(int val)
    {
        score += val;
        timesVisited++;
        if (parent != null) {
            parent.Backup(val);
        }
    }

}