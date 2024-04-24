using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class LargestTilesRandom : MonoBehaviour
{
    public static int[] BoardPositionsArray;
    public static List<int> allBoardPositions;
    public static Dictionary<int, int> playerOneTiles;
    public static Dictionary<int, int> playerTwoTiles;
    public static List<int[]> playerOneMovesTaken;
    public static List<int[]> playerTwoMovesTaken;
    public static List<int[]> ValidMoves;
    public static List<int> FrozenPositions;
    public static gameState state;

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
    public static Random rnd = new Random();
    
    
    private List<object> BoardState;

    private bool playerOneGameOver;

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
        bool playerOnePass = false;
        bool playerTwoPass = false;
        BoardState = FirstBoard();
        applyMove(BoardState);
        //randomButton();
    }

    public void applyMove(List<object> b)
    {
        BoardPositionsArray = (int[])b[0];
        allBoardPositions = (List<int>)b[1];
        playerOneTiles = (Dictionary<int,int>)b[2];
        playerTwoTiles = (Dictionary<int,int>)b[3];
        playerOneMovesTaken = (List<int[]>)b[4];
        playerTwoMovesTaken = (List<int[]>)b[5];
        state = (gameState)b[6];
    }

    public void randomButton()
    {
        int randomNo = 0;
        bool isFinished = false;
        int length = 0;
        if (state == gameState.PLAYERONE)
        {
            var playerOne = playerOneTurn(BoardState);
            BoardState = playerOne.BoardState;
            applyMove(BoardState);
            SwapGame.printPositions(BoardPositionsArray);
            SwapGame.printTiles(playerOneTiles);
            //playerone.gameover is true when no moves can be made
            playerOneGameOver = playerOne.GameOver;
            playerHUD.SetHUD(getBlueFour(), getBlueThree(), getBlueTwo(), getBlueOne(), blueImage);
        }
        else
        {
            var playerTwo = playerTwoTurn(BoardState);
            BoardState = playerTwo.BoardState;
            applyMove(BoardState);
            MCTSscript.printBoardState(BoardState);
            SwapGame.printTiles(playerTwoTiles);
            playerHUD.SetHUD(getRedFour(), getRedThree(), getRedTwo(), getRedOne(), redImage);
            //playerone.gameover is true when no moves can be made
            checkFinished(playerOneGameOver, playerTwo.GameOver);
        }
    }

    public  void randomVsRandom()
    {
        int randomNo = 0;
        bool isFinished = false;
        int length = 0;
        //BoardState = FirstBoard();
        while(!isFinished)
        {
            var playerOne = playerOneTurn(BoardState);
            BoardState = playerOne.BoardState;
            MCTSscript.printBoardState(BoardState);
            SwapGame.printTiles(playerOneTiles);
            
            var playerTwo = playerTwoTurn(BoardState);
            BoardState = playerTwo.BoardState;
            MCTSscript.printBoardState(BoardState);
            SwapGame.printTiles(playerTwoTiles);
            isFinished = checkFinished(playerOne.GameOver, playerTwo.GameOver);
        }
        //randomScript.getWinner(playerOnePass,playerTwoPass);
    }

    public bool checkFinished(bool playerOne, bool playerTwo)
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
    
    

    public (List<object> BoardState, bool GameOver) playerOneTurn(List<object> b)
    {
        List<object> moveFound = new List<object>();
        List<List<object>> tempMoves = new List<List<object>>();
        GameObject tileColour;
        BoardPositionsArray = (int[])b[0];
        allBoardPositions = (List<int>)b[1];
        playerOneTiles = (Dictionary<int,int>)b[2];
        playerTwoTiles = (Dictionary<int,int>)b[3];
        playerOneMovesTaken = (List<int[]>)b[4];
        playerTwoMovesTaken = (List<int[]>)b[5];
        state = (gameState)b[6];
        if (isSwap2(playerOneTiles, BoardPositionsArray))
        {
            int[] move = chooseLargestTile(playerOneMovesTaken);
            tempMoves = makeMoveSwap2(BoardPositionsArray, allBoardPositions, playerOneTiles, playerTwoTiles,
                playerOneMovesTaken, playerTwoMovesTaken, state,move);
            tileColour = blueFrozen;
        }
        else
        {
            tempMoves = makeMove2(BoardPositionsArray, allBoardPositions, playerOneTiles, playerTwoTiles, 
                playerOneMovesTaken, playerTwoMovesTaken, state);
            tileColour = blueTile;
        }
        if (tempMoves==null)
        {
            return (BoardState: b, GameOver:true);
        }
        int random = rnd.Next(tempMoves.Count - 1);
        moveFound = tempMoves[random];
        MCTSscript.printBoardState(moveFound);
        PutMoveOnBoard((int[])moveFound[7], tileColour);
        playerHUD.SetHUD(getRedFour(), getRedThree(), getRedTwo(), getRedOne(), redImage);
        return (BoardState: moveFound, GameOver: false);
    }

    public int[] chooseLargestTile(List<int[]> playerMovesTaken)
    {
        int max = 0;
        int biggestTileLength = 0;
        int[] biggestTile = new int[0];
        foreach (int[] move in playerMovesTaken)
        {
            if (move.Length > biggestTile.Length)
            {
                biggestTile = move.ToArray();
            }
        }
        return biggestTile;
    }
    
    public (List<object> BoardState, bool GameOver) playerTwoTurn(List<object> b)
    {
        List<object> moveFound = new List<object>();
        List<List<object>> tempMoves = new List<List<object>>();
        GameObject tileColour;
        BoardPositionsArray = (int[])b[0];
        allBoardPositions = (List<int>)b[1];
        playerOneTiles = (Dictionary<int,int>)b[2];
        playerTwoTiles = (Dictionary<int,int>)b[3];
        playerOneMovesTaken = (List<int[]>)b[4];
        playerTwoMovesTaken = (List<int[]>)b[5];
        state = (gameState)b[6];
        if (isSwap2(playerTwoTiles, BoardPositionsArray))
        {
            int[] move = chooseLargestTile(playerTwoMovesTaken);
            tempMoves = makeMoveSwap2(BoardPositionsArray, allBoardPositions, playerOneTiles, playerTwoTiles,
                playerOneMovesTaken, playerTwoMovesTaken, state,move);
            tileColour = redFrozen;
        }
        else
        {
            tempMoves = makeMove2(BoardPositionsArray, allBoardPositions, playerOneTiles, playerTwoTiles, playerOneMovesTaken,
                playerTwoMovesTaken, state);
            tileColour = redTile;
        }
        if (tempMoves == null)
        {
            return (BoardState: b, GameOver:true);
        }
        int random = rnd.Next(tempMoves.Count - 1);
        moveFound = tempMoves[random];
        MCTSscript.printBoardState(moveFound);
        PutMoveOnBoard((int[])moveFound[7], tileColour);
        playerHUD.SetHUD(getBlueFour(), getBlueThree(), getBlueTwo(), getBlueOne(), blueImage);
        return (BoardState: moveFound, GameOver: false);
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
    
    public static List<List<object>> makeMoveSwap2 (int[] BoardPositionsArray, List<int> allBoardPositions,
        Dictionary<int,int> playerOneTiles, Dictionary<int,int> playerTwoTiles, List<int[]> playerOneMovesTaken, 
        List<int[]> playerTwoMovesTaken, gameState swap, int[] moveRemoved)
    {
        List<List<object>> allMoves = new List<List<object>>();
        List<object> tempMove;
        Dictionary<int, int> tempTiles;
        int[] tempBoardArray;
        List<int> tempBoardPositions;
        List<int[]> tempPlayerMovesTaken;
        Dictionary<int, int> tempTilesPlayerOne;
        Dictionary<int, int> tempTilesPlayerTwo;
        List<int[]> tempPlayerOneMovesTaken;
        List<int[]> tempPlayerTwoMovesTaken;
        int length = moveRemoved.Length-1;
        bool[] invalidTiles;
        Dictionary<int, int> playerTiles2;
        gameState newState;
        if (swap == gameState.PLAYERONE)
        {
            invalidTiles = SwapGame.Get0TilesSwap(playerOneTiles, length);
            playerTiles2 = new Dictionary<int, int>(playerOneTiles);
            newState = gameState.PLAYERTWO;
        }
        else
        {
            invalidTiles = SwapGame.Get0TilesSwap(playerTwoTiles,length);
            playerTiles2 = new Dictionary<int, int>(playerTwoTiles);
            newState = gameState.PLAYERONE;
        }

        BoardPositionsArray = SwapGame.removeFromBoard2(moveRemoved, BoardPositionsArray);
        allBoardPositions = removeFromAllBoardPositions(allBoardPositions, moveRemoved);
        Destroy(GameObject.Find("randomScript" + moveRemoved[0]));
        playerTiles2[moveRemoved.Length] += 1;

        //checks all possible moves
        //Debug.Log("the evil length is: " + length);
        for (int i = 0; i < ValidMoves.Count; i++)
        {
            //removes invalid moves based on board positions
            if (!ValidMoves[i].Intersect(allBoardPositions).Any())
            {
                if (invalidTiles[ValidMoves[i].Length - 1])
                {
                    if (swap == gameState.PLAYERONE)
                    {
                        tempTilesPlayerOne = new Dictionary<int, int>(playerOneTiles);
                        tempTilesPlayerOne[ValidMoves[i].Length] -= 1;
                        tempTilesPlayerTwo = new Dictionary<int, int>(playerTwoTiles);
                        tempPlayerOneMovesTaken = new List<int[]>(playerOneMovesTaken) {ValidMoves[i]};
                        tempPlayerTwoMovesTaken = new List<int[]>(playerTwoMovesTaken);
                    }
                    else
                    {
                        tempTilesPlayerOne = new Dictionary<int, int>(playerOneTiles);
                        tempTilesPlayerTwo = new Dictionary<int, int>(playerTwoTiles);
                        tempTilesPlayerTwo[ValidMoves[i].Length] -= 1;
                        tempPlayerTwoMovesTaken = new List<int[]>(playerTwoMovesTaken) {ValidMoves[i]};
                        tempPlayerOneMovesTaken = new List<int[]>(playerOneMovesTaken);
                    }
                    tempMove = new List<object>();
                    //boardPositionsarray to list
                    tempBoardArray = BoardPositionsArray.ToArray();
                    tempMove.Add(SwapGame.addMoveToboard2(ValidMoves[i], tempBoardArray));
                    //boardPositions
                    tempBoardPositions = new List<int>(allBoardPositions);
                    tempBoardPositions.AddRange(ValidMoves[i]);
                    tempMove.Add(tempBoardPositions);
                    //player tiles 1
                    tempMove.Add(tempTilesPlayerOne);
                    //temp tiles 2
                    tempMove.Add(tempTilesPlayerTwo);
                    //player moves taken 1
                    tempMove.Add(tempPlayerOneMovesTaken);
                    //other players tiles and moves taken
                    tempMove.Add(tempPlayerTwoMovesTaken);
                    //add new state
                    tempMove.Add(newState);
                    //add move positions
                    tempMove.Add(ValidMoves[i]);
                    //Finally add tempMove to list allMoves
                    allMoves.Add(tempMove);
                }
            }
        }
        
        if (allMoves.Count == 0)
        {
            return null;
        }
        return allMoves;
    }
    public bool isSwap2(Dictionary<int,int> playerTiles, int[] BoardPositionsArray)
    {
        int largestSpace = 0;
        //get smallest tile
        //check if largest space < smallest tile
        int smallestTile = GetSmallestTile(playerTiles);
        int length1 = getLargestSpaceHorizontal(BoardPositionsArray);
        int length2 = getLargestSpaceVertical(BoardPositionsArray);
        largestSpace = Math.Max(length1, length2);
        if (largestSpace < smallestTile)
        {
            return true;
        }
        return false;
    }
    
    public static int GetSmallestTile(Dictionary<int, int> playerTiles)
    {
        for (int i = 1; i <= 4; i++)
        {
            if (playerTiles[i] != 0)
            {
                return i;
            }
        }
        //number that is too big, change so less hardcoded
        return 7;
    }

    public static List<int> removeFromAllBoardPositions(List<int> allBoardPositions, int[] move)
    {
        foreach (int pos in move)
        {
            allBoardPositions.Remove(pos);
        }

        return allBoardPositions;
    }

    public static int getLargestSpaceVertical(int[] BoardPositionsArray)
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
    
    public static int getLargestSpaceHorizontal(int[] BoardPositionsArray)
    {
        int tmp = 0;
        int index = 0;
        int largestTile = 0;
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

                if (tmp > largestTile)
                {
                    largestTile = tmp;
                }
            }
            tmp = 0;
        }
        return largestTile;
    }
    
    public static List<List<object>> makeMove2(int[] BoardPositionsArray, List<int> allBoardPositions,
        Dictionary<int,int> playerOneTiles, Dictionary<int,int> playerTwoTiles, List<int[]> playerOneMovesTaken, 
        List<int[]> playerTwoMovesTaken, gameState swap)
    {
        List<List<object>> allMoves = new List<List<object>>();
        List<object> tempMove;
        int[] tempBoardArray;
        List<int> tempBoardPositions;
        Dictionary<int, int> tempTilesPlayerOne;
        Dictionary<int, int> tempTilesPlayerTwo;
        List<int[]> tempPlayerOneMovesTaken;
        List<int[]> tempPlayerTwoMovesTaken;
        bool[] invalidTiles;
        gameState newState;
        if (swap == gameState.PLAYERONE)
        {
            invalidTiles = SwapGame.Get0Tiles(playerOneTiles);
            newState = gameState.PLAYERTWO;
        }
        else
        {
            invalidTiles = SwapGame.Get0Tiles(playerTwoTiles);
            newState = gameState.PLAYERONE;
        }

        //checks all possible moves
        for (int i = 0; i < ValidMoves.Count; i++)
        {
            //removes invalid moves based on board positions
            if (!ValidMoves[i].Intersect(allBoardPositions).Any())
            {
                //removes invalid tiles also
                if (invalidTiles[ValidMoves[i].Length - 1])
                {
                    if (swap == gameState.PLAYERONE)
                    {
                        tempTilesPlayerOne = new Dictionary<int, int>(playerOneTiles);
                        tempTilesPlayerOne[ValidMoves[i].Length] -= 1;
                        tempTilesPlayerTwo = new Dictionary<int, int>(playerTwoTiles);
                        tempPlayerOneMovesTaken = new List<int[]>(playerOneMovesTaken) {ValidMoves[i]};
                        tempPlayerTwoMovesTaken = new List<int[]>(playerTwoMovesTaken);
                    }
                    else
                    {
                        tempTilesPlayerOne = new Dictionary<int, int>(playerOneTiles);
                        tempTilesPlayerTwo = new Dictionary<int, int>(playerTwoTiles);
                        tempTilesPlayerTwo[ValidMoves[i].Length] -= 1;
                        tempPlayerTwoMovesTaken = new List<int[]>(playerTwoMovesTaken) {ValidMoves[i]};
                        tempPlayerOneMovesTaken = new List<int[]>(playerOneMovesTaken);
                    }
                    tempMove = new List<object>();
                    //boardPositionsarray to list
                    tempBoardArray = BoardPositionsArray.ToArray();
                    tempMove.Add(SwapGame.addMoveToboard2(ValidMoves[i], tempBoardArray));
                    //boardPositions
                    tempBoardPositions = new List<int>(allBoardPositions);
                    tempBoardPositions.AddRange(ValidMoves[i]);
                    tempMove.Add(tempBoardPositions);
                    //player tiles 1
                    tempMove.Add(tempTilesPlayerOne);
                    //temp tiles 2
                    tempMove.Add(tempTilesPlayerTwo);
                    //player moves taken 1
                    tempMove.Add(tempPlayerOneMovesTaken);
                    //other players tiles and moves taken
                    tempMove.Add(tempPlayerTwoMovesTaken);
                    //add state
                    tempMove.Add(newState);
                    //add move positions
                    tempMove.Add(ValidMoves[i]);
                    //Finally add tempMove to list allMoves
                    allMoves.Add(tempMove);
                }
            }
        }
        if (allMoves.Count == 0)
        {
            return null;
        }
        
        //int random = rnd.Next(tempValidMoves.Count - 1);
        //Debug.Log(" move starts at "+ tempValidMoves[random][0] +" length: " + tempValidMoves[random].Length);
        return allMoves;
    }
    
    public static List<object> FirstBoard()
    {
        var tempBoardState = new List<object>();
        tempBoardState.Add(BoardPositionsArray);
        tempBoardState.Add(allBoardPositions);
        tempBoardState.Add(playerOneTiles);
        tempBoardState.Add(playerTwoTiles);
        tempBoardState.Add(playerOneMovesTaken);
        tempBoardState.Add(playerTwoMovesTaken);
        tempBoardState.Add(state);
        return tempBoardState;
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