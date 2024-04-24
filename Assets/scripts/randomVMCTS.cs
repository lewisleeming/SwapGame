using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class randomVMCTS
{
    public static int[] BoardPositionsArray;
    public static List<int> allBoardPositions;
    public static Dictionary<int, int> playerOneTiles;
    public static Dictionary<int, int> playerTwoTiles;
    public static List<int[]> playerOneMovesTaken;
    public static List<int[]> playerTwoMovesTaken;
    public static List<int[]> ValidMoves;
    public static List<int> FrozenPositions;
    public static Random rnd = new Random();
    public static gameState state;
    public static bool pass = false;
    public static Board board;
    public GameObject blueTile;
    public GameObject redTile;
    public GameObject redFrozen;
    public GameObject blueFrozen;
    public playerHUD playerHUD;
    public Sprite blueImage;
    public Sprite redImage;
    public randomScript2 rs;

    public int getBlueOne()
    {
        return playerOneTiles[1];
    }
    public int getBlueTwo()
    {
        return playerOneTiles[2];
    }
    public int getBlueThree()
    {
        return playerOneTiles[3];
    }
    public int getBlueFour()
    {
        return playerOneTiles[4];
    }

    public int getRedOne()
    {
        return playerTwoTiles[1];
    }
    public int getRedTwo()
    {
        return playerTwoTiles[2];
    }
    public int getRedThree()
    {
        return playerTwoTiles[3];
    }
    public int getRedFour()
    {
        return playerTwoTiles[4];
    }
    
    void Start()
    {
        //printValidMoves();
        //buttonStart();
        //randomVsRandom();
        playMCTSvRandom();
    }

    public void playMCTSvRandom()
    {
        List<object> BoardState = new List<object>();
        int randomNo = 0;
        bool isFinished = false;
        int length = 0;
        //BoardState = FirstBoard();
        while(!isFinished)
        {
            //BoardState = randomScript2.playerOneTurn(BoardState);
            var playerOne = rs.playerOneTurn(BoardState);
            BoardState = playerOne.BoardState;
            //var playerTwo = MCTSscript.
            
        }
    }


}