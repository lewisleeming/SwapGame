using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class MCTSbyclicks : MCTSscript
{
    public new int[] BoardPositionsArray;
    public new List<int> allBoardPositions;
    public new Dictionary<int, int> playerOneTiles;
    public new Dictionary<int, int> playerTwoTiles;
    public new List<int[]> playerOneMovesTaken;
    public new List<int[]> playerTwoMovesTaken;
    public new List<int[]> ValidMoves;
    public new List<int> FrozenPositions;
    public new gameState state;
    public int[] move;
    public int[] removeMove;

    private bool playerOnePass;

    private List<object> BoardState;

    public void setup()
    {
        //initialise all variables
        BoardPositionsArray = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        allBoardPositions = new List<int>();
        playerOneTiles = new Dictionary<int, int>();
        playerTwoTiles = new Dictionary<int, int>();
        playerOneMovesTaken = new List<int[]>();
        playerTwoMovesTaken = new List<int[]>();
        ValidMoves = new List<int[]>();
        FrozenPositions = new List<int>();

        int startX = 0;
        int startY = 0;
        int length = 0;

        playerOneTiles.Add(1, 2);
        playerOneTiles.Add(2, 3);
        playerOneTiles.Add(3, 3);
        playerOneTiles.Add(4, 1);
        playerTwoTiles.Add(1, 2);
        playerTwoTiles.Add(2, 3);
        playerTwoTiles.Add(3, 3);
        playerTwoTiles.Add(4, 1);

        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                startX = x + (y * 6);
                startY = y + (x * 6);
                length = x;
                if (length < 3)
                {
                    ValidMoves.Add(new int[] { startX, startX + 1, startX + 2, startX + 3 });
                    ValidMoves.Add(new int[] { startY, startY + 6, startY + 12, startY + 18 });

                    ValidMoves.Add(new int[] { startX, startX + 1, startX + 2 });
                    ValidMoves.Add(new int[] { startY, startY + 6, startY + 12 });

                    ValidMoves.Add(new int[] { startX, startX + 1 });
                    ValidMoves.Add(new int[] { startY, startY + 6 });

                    ValidMoves.Add(new int[] { startX });
                    ValidMoves.Add(new int[] { startY });

                }
                else if (length == 3)
                {
                    ValidMoves.Add(new int[] { startX, startX + 1, startX + 2 });
                    ValidMoves.Add(new int[] { startY, startY + 6, startY + 12 });

                    ValidMoves.Add(new int[] { startX, startX + 1 });
                    ValidMoves.Add(new int[] { startY, startY + 6 });

                    ValidMoves.Add(new int[] { startX });
                    ValidMoves.Add(new int[] { startY });

                }
                else if (length == 4)
                {
                    ValidMoves.Add(new int[] { startX, startX + 1 });
                    ValidMoves.Add(new int[] { startY, startY + 6 });

                    ValidMoves.Add(new int[] { startX });
                    ValidMoves.Add(new int[] { startY });
                }
                else
                {

                    ValidMoves.Add(new int[] { startX });
                    ValidMoves.Add(new int[] { startY });

                }
            }
        }
        state = gameState.PLAYERONE;
        playerHUD.SetHUD(getBlueFour(), getBlueThree(), getBlueTwo(), getBlueOne(), blueImage);
    }
    public void Start()
    {
        MCTSscript.setup();
        BoardPositionsArray = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        allBoardPositions = new List<int>();
        playerOneMovesTaken = new List<int[]>();
        playerTwoMovesTaken = new List<int[]>();
        ValidMoves = new List<int[]>(MCTSscript.ValidMoves);
        playerOneTiles = new Dictionary<int, int>(MCTSscript.playerOneTiles);
        playerTwoTiles = new Dictionary<int, int>(MCTSscript.playerOneTiles);
        state = gameState.PLAYERONE;
        BoardState = FirstBoard();
        ApplyMove(BoardState);
    }
    public void ApplyMove(List<object> b)
    {
        BoardPositionsArray = (int[])b[0];
        allBoardPositions = (List<int>)b[1];
        playerOneTiles = (Dictionary<int, int>)b[2];
        playerTwoTiles = (Dictionary<int, int>)b[3];
        playerOneMovesTaken = (List<int[]>)b[4];
        playerTwoMovesTaken = (List<int[]>)b[5];
        state = (gameState)b[6];
        move = (int[])b[7];
        removeMove = (int[])b[8];
    }
    public new List<object> FirstBoard()
    {
        var tempBoardState = new List<object>();
        tempBoardState.Add(BoardPositionsArray);
        tempBoardState.Add(allBoardPositions);
        tempBoardState.Add(playerOneTiles);
        tempBoardState.Add(playerTwoTiles);
        tempBoardState.Add(playerOneMovesTaken);
        tempBoardState.Add(playerTwoMovesTaken);
        tempBoardState.Add(state);
        tempBoardState.Add(null);
        tempBoardState.Add(null);
        return tempBoardState;
    }

    public void MCTSbutton()
    {
        int randomNo = 0;
        bool isFinished = false;
        int length = 0;
        if (state == gameState.PLAYERONE)
        {
            var playerOne = computerMove(BoardState);
            BoardState = playerOne.move;
            playerOnePass = playerOne.GameOver;
            ApplyMove(BoardState);
            SwapGame.printPositions(BoardPositionsArray);
            SwapGame.printTiles(playerOneTiles);
            if ((int[])BoardState[8] != null)
            {
                int[] move = (int[])BoardState[8];
                Destroy(GameObject.Find("randomScript" + move[0]));
                PutMoveOnBoard((int[])BoardState[7], blueFrozen);
            }
            else
            {
                PutMoveOnBoard((int[])BoardState[7], blueTile);
            }
            //playerone.gameover is true when no moves can be made
            playerHUD.SetHUD(getRedFour(), getRedThree(), getRedTwo(), getRedOne(), redImage);
        }
        else
        {
            var playerTwo = computerMove(BoardState);
            BoardState = playerTwo.move;
            if ((int[])BoardState[8] != null)
            {
                int[] move = (int[])BoardState[8];
                Destroy(GameObject.Find("randomScript" + move[0]));
                PutMoveOnBoard((int[])BoardState[7], redFrozen);
            }
            else
            {
                PutMoveOnBoard((int[])BoardState[7], redTile);
            }
            ApplyMove(BoardState);
            printBoardState(BoardState);
            playerHUD.SetHUD(getBlueFour(), getBlueThree(), getBlueTwo(), getBlueOne(), blueImage);
            //playerone.gameover is true when no moves can be made
            checkFinishedMain(playerOnePass, playerTwo.GameOver);
        }
    }

    public bool checkFinishedMain(bool playerOne, bool playerTwo)
    {
        if (playerOne && playerTwo)
        {
            Debug.Log("Draw");
            return true;
        }
        if (playerOne)
        {
            Debug.Log("Player Two wins");
            return true;
        }

        if (playerTwo)
        {
            Debug.Log("player One wins");
            return true;
        }
        return false;
    }

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

}