using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class Board : MCTSscript
{
    
    public int[] BoardPositionsArray;
    public List<int> allBoardPositions;
    public Dictionary<int, int> playerOneTiles;
    public Dictionary<int, int> playerTwoTiles;
    public List<int[]> playerOneMovesTaken;
    public List<int[]> playerTwoMovesTaken;
    public gameState state;
    public int[] move;
    public int[] removedMove;
    public bool GameOver;
    public string tileColour;
    public List<List<object>> availableMoves;

    public Board(List<object> b)
    {
        BoardPositionsArray = (int[])b[0];
        allBoardPositions = (List<int>)b[1];
        playerOneTiles = (Dictionary<int,int>)b[2];
        playerTwoTiles = (Dictionary<int,int>)b[3];
        playerOneMovesTaken = (List<int[]>)b[4];
        playerTwoMovesTaken = (List<int[]>)b[5];
        state = (gameState)b[6];
        move = (int[])b[7];
        removedMove = (int[])b[8];
        availableMoves = new List<List<object>>();
        availableMoves = GenerateAvailableMoves();
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
        move = (int[]) b[7];
        removedMove = (int[])b[8];
    }
    
    public List<List<object>> GenerateAvailableMoves()
    {
        availableMoves.Clear();
        List<List<object>> tempValidMoves = new List<List<object>>();
        List<List<object>> tempMovesForSwap = new List<List<object>>();
        Dictionary<int, int> playerTiles;
        List<int[]> playerMovesTaken;
        if (state == gameState.PLAYERONE)
        {
            playerTiles = new Dictionary<int, int>(playerOneTiles);
            playerMovesTaken = new List<int[]>(playerOneMovesTaken);
        }
        else
        {
            playerTiles = new Dictionary<int, int>(playerTwoTiles);
            playerMovesTaken = new List<int[]>(playerTwoMovesTaken);
        }
        if (isSwap2(playerTiles, BoardPositionsArray))
        {
            for (int i = 0; i < playerMovesTaken.Count; i++)
            {
                tempMovesForSwap = makeMoveSwap2(BoardPositionsArray, allBoardPositions,
                    playerOneTiles, playerTwoTiles,playerOneMovesTaken,
                    playerTwoMovesTaken, state, playerMovesTaken[i]);
                if (tempMovesForSwap != null)
                {
                    tempValidMoves.AddRange(tempMovesForSwap);
                }
            }
        }
        else
        {
            tempValidMoves = makeMove2(BoardPositionsArray, allBoardPositions,
                playerOneTiles, playerTwoTiles,playerOneMovesTaken,
                playerTwoMovesTaken, state);
        }
        
        return tempValidMoves;
    }
    
        public  List<List<object>> makeMove2(int[] BoardPositionsArray, List<int> allBoardPositions,
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
            int[] move = ValidMoves[i];
            //removes invalid moves based on board positions
            if (!ValidMoves[i].Intersect(allBoardPositions).Any())
            {
                //removes invalid tiles also
                if (invalidTiles[ValidMoves[i].Length - 1])
                {
                    if (swap == gameState.PLAYERONE)
                    {
                        tempTilesPlayerOne = new Dictionary<int, int>(playerOneTiles);
                        tempTilesPlayerOne[MCTSscript.ValidMoves[i].Length] -= 1;
                        tempTilesPlayerTwo = new Dictionary<int, int>(playerTwoTiles);
                        tempPlayerOneMovesTaken = new List<int[]>(playerOneMovesTaken) {MCTSscript.ValidMoves[i]};
                        tempPlayerTwoMovesTaken = new List<int[]>(playerTwoMovesTaken);
                    }
                    else
                    {
                        tempTilesPlayerOne = new Dictionary<int, int>(playerOneTiles);
                        tempTilesPlayerTwo = new Dictionary<int, int>(playerTwoTiles);
                        tempTilesPlayerTwo[MCTSscript.ValidMoves[i].Length] -= 1;
                        tempPlayerTwoMovesTaken = new List<int[]>(playerTwoMovesTaken) {MCTSscript.ValidMoves[i]};
                        tempPlayerOneMovesTaken = new List<int[]>(playerOneMovesTaken);
                    }
                    tempMove = new List<object>();
                    //boardPositionsarray to list
                    tempBoardArray = BoardPositionsArray.ToArray();
                    tempMove.Add(SwapGame.addMoveToboard2(MCTSscript.ValidMoves[i], tempBoardArray));
                    //boardPositions
                    tempBoardPositions = new List<int>(allBoardPositions);
                    tempBoardPositions.AddRange(MCTSscript.ValidMoves[i]);
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
                    //add move (7)
                    tempMove.Add(MCTSscript.ValidMoves[i]);
                    // no tile removed
                    tempMove.Add(null);
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
    
    public List<List<object>> makeMoveSwap2 (int[] BoardPositionsArray, List<int> allBoardPositions,
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
        int length = moveRemoved.Length - 1;
        bool[] invalidTiles;
        Dictionary<int, int> playerTiles2;
        gameState newState;
        int[] remove = moveRemoved.ToArray();
        int[] newBoardArray = BoardPositionsArray.ToArray();
        List<int> newBoardPositions = new List<int>(allBoardPositions);
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

        newBoardArray = SwapGame.removeFromBoard2(moveRemoved, newBoardArray);
        newBoardPositions = LargestTilesRandom.removeFromAllBoardPositions(newBoardPositions, moveRemoved);
        playerTiles2[moveRemoved.Length] += 1;

        //checks all possible moves
        //Debug.Log("the evil length is: " + length);
        for (int i = 0; i < ValidMoves.Count; i++)
        {
            //removes invalid moves based on board positions
            if (!ValidMoves[i].Intersect(newBoardPositions).Any())
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
                        tempTilesPlayerTwo[MCTSscript.ValidMoves[i].Length] -= 1;
                        tempPlayerTwoMovesTaken = new List<int[]>(playerTwoMovesTaken) {MCTSscript.ValidMoves[i]};
                        tempPlayerOneMovesTaken = new List<int[]>(playerOneMovesTaken);
                    }
                    tempMove = new List<object>();
                    //boardPositionsarray to list
                    tempBoardArray = newBoardArray.ToArray();
                    tempMove.Add(SwapGame.addMoveToboard2(MCTSscript.ValidMoves[i], tempBoardArray));
                    //boardPositions
                    tempBoardPositions = new List<int>(newBoardPositions);
                    tempBoardPositions.AddRange(MCTSscript.ValidMoves[i]);
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
                    //add move
                    tempMove.Add(ValidMoves[i]);
                    //add removed tile
                    tempMove.Add(remove);
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
        int smallestTile = LargestTilesRandom.GetSmallestTile(playerTiles);
        int length1 = getLargestSpaceHorizontal(BoardPositionsArray);
        int length2 = getLargestSpaceVertical(BoardPositionsArray);
        largestSpace = Math.Max(length1, length2);
        if (largestSpace < smallestTile)
        {
            return true;
        }
        return false;
    }

}