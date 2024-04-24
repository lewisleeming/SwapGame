using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class randomScript : SwapGame
{

    public int playerOneWinCount = 0;
    public int playerTwoWinCount = 0;
    public int drawCount = 0;
    public GameObject blueTile;
    public GameObject redTile;
    public GameObject redFrozen;
    public GameObject blueFrozen;
    public playerHUD playerHUD;
    public Sprite blueImage;
    public Sprite redImage;
    public int currTiles;
    //what i need to add today - 
    //where should win condition be? after swap move?
    //first thought - if all tiles are frozen - probs a draw
    //if zero possible moves after swap chosen
    //might have to create list of liquid placements

    public void setup()
    {
        //initialise all variables
        BoardPositionsArray = new [] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
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
                    ValidMoves.Add(new int[] {startX, startX + 1, startX + 2, startX + 3});
                    ValidMoves.Add(new int[] {startY, startY + 6, startY + 12, startY + 18});

                    ValidMoves.Add(new int[] {startX, startX + 1, startX + 2});
                    ValidMoves.Add(new int[] {startY, startY + 6, startY + 12});

                    ValidMoves.Add(new int[] {startX, startX + 1});
                    ValidMoves.Add(new int[] {startY, startY + 6});

                    ValidMoves.Add(new int[] {startX});
                    ValidMoves.Add(new int[] {startY});

                }
                else if (length == 3)
                {

                    ValidMoves.Add(new int[] {startX, startX + 1, startX + 2});
                    ValidMoves.Add(new int[] {startY, startY + 6, startY + 12});

                    ValidMoves.Add(new int[] {startX, startX + 1});
                    ValidMoves.Add(new int[] {startY, startY + 6});

                    ValidMoves.Add(new int[] {startX});
                    ValidMoves.Add(new int[] {startY});

                }
                else if (length == 4)
                {
                    ValidMoves.Add(new int[] {startX, startX + 1});
                    ValidMoves.Add(new int[] {startY, startY + 6});

                    ValidMoves.Add(new int[] {startX});
                    ValidMoves.Add(new int[] {startY});
                }
                else
                {

                    ValidMoves.Add(new int[] {startX});
                    ValidMoves.Add(new int[] {startY});

                }
            }
        }
        state = gameState.PLAYERONE;
        playerHUD.SetHUD(getBlueFour(), getBlueThree(), getBlueTwo(), getBlueOne(), blueImage);
    }

    void Start()
    {
        setup();
        //printValidMoves();
        //buttonStart();
        randomVsRandom();
    }
    public void randomVsRandom()
    {
        printPositions();
        int randomNo = 0;
        bool isFinished = false;
        int length = 0;
        bool playerOnePass = false;
        bool playerTwoPass = false;
        while(!isFinished) 
        {
            playerOnePass = playerOneTurn();
            playerTwoPass = playerTwoTurn();
            isFinished = checkFinished(playerOnePass, playerTwoPass);
        }
        getWinner(playerOnePass,playerTwoPass);
    }

    public void getWinner(bool playerOne, bool playerTwo)
    {
        if (playerOne && playerTwo)
        {
            Debug.Log("Draw");
            drawCount++;
        }else if (playerOne)
        {
            Debug.Log("playerTwo wins");
            playerTwoWinCount++;
        }
        else
        {
            Debug.Log("playerOne wins");
            playerOneWinCount++;
        }
    }
    public static bool checkFinished(bool playerOne, bool playerTwo)
    {
        if (playerOne || playerTwo)
        {
            return true;
        }
        return false;
    }

    public bool playerOneTurn()
    {
        int[] move;
        if (isSwap(playerOneTiles))
        {
            Debug.Log("swap move needed");
            int removeLength = PickTileRemoval(playerOneMovesTaken,playerOneTiles);
            move = makeMoveSwap(playerOneTiles, removeLength);
            if (move == null)
            {
                return true;
            }
            FrozenPositions.AddRange(move);
            PutMoveOnBoard(move,blueFrozen);
        }
        else //regular move
        {
            move = makeMove(playerOneTiles);
            if (move == null)
            {
                return true;
            }
            playerOneMovesTaken.Add(move);
            PutMoveOnBoard(move,blueTile);
        }
        addMoveToboard(move);
        Debug.Log("prior: playertiles: "+ move.Length +":"+playerOneTiles[move.Length]);
        playerOneTiles[move.Length] -= 1;
        Debug.Log("move length: " + move.Length);
        printTiles(playerOneTiles);
        printPositions();
        state = gameState.PLAYERTWO;
        playerHUD.SetHUD(getRedFour(), getRedThree(), getRedTwo(), getRedOne(), redImage);
        //should return move
        return false;
    }

    public bool playerTwoTurn()
    {
        int[] move;
        if (isSwap(playerTwoTiles))
        {
            Debug.Log("swap move needed");
            int removeLength = PickTileRemoval(playerTwoMovesTaken,playerTwoTiles);
            move = makeMoveSwap(playerTwoTiles, removeLength);
            if (move == null)
            {
                return true;
            }
            FrozenPositions.AddRange(move);
            PutMoveOnBoard(move,redFrozen);
        }
        else //regular move
        {
            move = makeMove(playerTwoTiles);
            if (move == null)
            {
                return true;
            }
            playerTwoMovesTaken.Add(move);
            PutMoveOnBoard(move,redTile);
        }
        addMoveToboard(move);
        Debug.Log("prior: playertiles: "+move.Length+":"+ playerTwoTiles[move.Length]);
        playerTwoTiles[move.Length] -= 1;
        Debug.Log("move length: " + move.Length);
        printTiles(playerTwoTiles);
        printPositions();
        state = gameState.PLAYERONE;
        playerHUD.SetHUD(getBlueFour(), getBlueThree(), getBlueTwo(), getBlueOne(), blueImage);
        return false;
    }

    public void PutMoveOnBoard(int[] move, GameObject tile)
    {
        float middleOfTile;
        Vector2 moveCoords = getCoords(move[0]);
        int distance = move.Length-1;
        Debug.Log(move[0]+ "cmon"+ distance);

        //checks if single tile or horizontal
        if (move.Length == 1 || move[1] == move[0] + 1)
        {
            //horizontal
            middleOfTile = moveCoords.x + ((float)distance / 2);
            GameObject newTile = Instantiate(tile, new Vector3(middleOfTile, moveCoords.y, 0), Quaternion.identity);
            newTile.transform.localScale = new Vector3(move.Length, 1, 1);
            newTile.name = "randomScript"+ move[0];
            newTile.layer = 1;
            currTiles++;
        }
        else
        {
            //vertical
            middleOfTile = moveCoords.y + ((float)distance / 2);
            GameObject newTile = Instantiate(tile, new Vector3(moveCoords.x, middleOfTile, 0), Quaternion.identity);
            newTile.transform.localScale = new Vector3(1, move.Length, 1);
            newTile.name = "randomScript"+ move[0];
            newTile.layer = 1;
            currTiles++;
        }
        //convert move values to coords -
    }

    public Vector2 getCoords(int move)
    {
        double xpos = move % 6;
        double ypos = move / 6;
        xpos -= 2.5;
        ypos -= 2.5;
        return new Vector2((float)xpos, (float)ypos);
    }

    public void removeTilesFromBoard(int[] move)
    {
        for (int i = 0; i < move.Length; i++)
        {
            allBoardPositions.Remove(move[i]);
        }
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
    public int[] makeMove(Dictionary<int,int> playerTiles)
    {
        bool[] invalidTiles = Get0Tiles(playerTiles);
        List<int[]> tempValidMoves = new List<int[]>();
        //checks all possible moves
        for (int i = 0; i < ValidMoves.Count; i++)
        {
            //removes invalid moves based on board positions
            if (!ValidMoves[i].Intersect(allBoardPositions).Any())
            {
                //removes invalid tiles also
                if (invalidTiles[ValidMoves[i].Length - 1])
                {
                    tempValidMoves.Add(ValidMoves[i]);
                }
            }
        }
        Debug.Log("Total Moves: "+tempValidMoves.Count);
        if (tempValidMoves.Count == 0)
        {
            return null;
        }
        int random = rnd.Next(tempValidMoves.Count - 1);
        Debug.Log(" move starts at "+ tempValidMoves[random][0] +" length: " + tempValidMoves[random].Length);
        return tempValidMoves[random];
    }

    public bool isSwap(Dictionary<int,int> playerTiles)
    {
        int largestSpace = 0;
        //get smallest tile
        //check if largest space < smallest tile
        int smallestTile = GetSmallestTile(playerTiles);
        int length1 = getLargestSpaceHorizontal();
        int length2 = getLargestSpaceVertical();
        largestSpace = Math.Max(length1, length2);
        if (largestSpace < smallestTile)
        {
            return true;
        }
        return false;
    }

    public int GetSmallestTile(Dictionary<int, int> playerTiles)
    {
        for (int i = 1; i <= 4; i++)
        {
            if (playerTiles[i] != 0)
            {
                return i;
            }
        }
        return 7;
    }

    //same as make move but excludes length taken away
    //returns random move from available moves
    public int[] makeMoveSwap(Dictionary<int,int> playerTiles, int length)
    {
        bool[] invalidTiles = Get0TilesSwap(playerTiles,length);
        List<int[]> tempValidMoves = new List<int[]>();
        //checks all possible moves
        Debug.Log("the evil length is: " + length);
        
        for (int i = 0; i < ValidMoves.Count; i++)
        {
            //removes invalid moves based on board positions
            if (!ValidMoves[i].Intersect(allBoardPositions).Any())
            {
                if (invalidTiles[ValidMoves[i].Length - 1])
                {
                    tempValidMoves.Add(ValidMoves[i]);
                }
            }
        }

        Debug.Log("Total Moves: "+tempValidMoves.Count);
        if (tempValidMoves.Count == 0)
        {
            return null;
        }
        int random = rnd.Next(tempValidMoves.Count - 1);
        Debug.Log(" move starts at "+ tempValidMoves[random][0] + " length: " + tempValidMoves[random].Length);
        FrozenPositions.AddRange(tempValidMoves[random]);
        return tempValidMoves[random];
    }
    public int getLargestSpaceVertical()
    {
        int tmp = 0;
        int largestSpace = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int k = i; k < 36; k++)
            {
                if(BoardPositionsArray[k] == 0)
                {
                    tmp++;
                }
                else
                {
                    tmp = 0;
                }
                if (tmp > largestSpace)
                {
                    largestSpace = tmp;
                }
                k += 6;
            }
            tmp = 0;
        }

        return largestSpace;
    }
    public int testLargestSpaceVertical(int[] testArray)
    {
        int tmp = 0;
        int largestSpace = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int k = i; k < 36; k++)
            {
                if(BoardPositionsArray[k] == 0)
                {
                    tmp++;
                }
                else
                {
                    tmp = 0;
                }
                if (tmp > largestSpace)
                {
                    largestSpace = tmp;
                }
                k += 6;
            }
            tmp = 0;
        }

        return largestSpace;
    }
    public int getLargestSpaceHorizontal()
    {
        int tmp = 0;
        int index = 0;
        int largestSpace = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                index = j + (i * 6);
                if (BoardPositionsArray[index] == 0)
                {
                    tmp++;
                }
                else
                {
                    tmp = 0;
                }

                if (tmp > largestSpace)
                {
                    largestSpace = tmp;
                }
            }
            tmp = 0;
        }
        return largestSpace;
    }
    
    
    public int PickTileRemoval(List<int[]> playerMovesTaken, Dictionary<int,int> playerTiles)
    {
        //gets move from taken moves list
        //valid removals are:
        //in the player moves taken list
        //result has big enough whole for placement
        
        int randomNo = rnd.Next(playerMovesTaken.Count - 1);
        int length = playerMovesTaken[randomNo].Length;
        Debug.Log("removing tile: "+ length);
        removeFromBoard(playerMovesTaken[randomNo]);
        destroyTile(playerMovesTaken[randomNo]);
        playerTiles[length] += 1;
        return length;
    }

    public void destroyTile(int[] move)
    {
        Destroy(GameObject.Find("randomScript" + move[0]));
    }
    
    public int PickTileRemoval2(List<int[]> playerMovesTaken, Dictionary<int,int> playerTiles)
    {
        //gets move from taken moves list
        int randomNo = rnd.Next(playerMovesTaken.Count - 1);
        int length = playerMovesTaken[randomNo].Length;
        removeFromBoard(playerMovesTaken[randomNo]);
        playerTiles[length] += 1;
        return length;
    }
}