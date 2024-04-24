using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class SwapGame : MonoBehaviour
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
    
    public static List<int[]> addValidMoves()
    {
        List<int[]> TempValidMoves = new List<int[]>();
        int startX = 0;
        int startY = 0;
        int length = 0;
        
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                startX = x + (y * 6);
                startY = y + (x * 6);
                length = x;
                if (length < 3)
                {
                    TempValidMoves.Add(new int[] {startX, startX + 1, startX + 2, startX + 3});
                    TempValidMoves.Add(new int[] {startY, startY + 6, startY + 12, startY + 18});

                    TempValidMoves.Add(new int[] {startX, startX + 1, startX + 2});
                    TempValidMoves.Add(new int[] {startY, startY + 6, startY + 12});

                    TempValidMoves.Add(new int[] {startX, startX + 1});
                    TempValidMoves.Add(new int[] {startY, startY + 6});

                    TempValidMoves.Add(new int[] {startX});
                    TempValidMoves.Add(new int[] {startY});

                }
                else if (length == 3)
                {

                    TempValidMoves.Add(new int[] {startX, startX + 1, startX + 2});
                    TempValidMoves.Add(new int[] {startY, startY + 6, startY + 12});

                    TempValidMoves.Add(new int[] {startX, startX + 1});
                    TempValidMoves.Add(new int[] {startY, startY + 6});

                    TempValidMoves.Add(new int[] {startX});
                    TempValidMoves.Add(new int[] {startY});

                }
                else if (length == 4)
                {
                    TempValidMoves.Add(new int[] {startX, startX + 1});
                    TempValidMoves.Add(new int[] {startY, startY + 6});

                    TempValidMoves.Add(new int[] {startX});
                    TempValidMoves.Add(new int[] {startY});
                }
                else
                {

                    TempValidMoves.Add(new int[] {startX});
                    TempValidMoves.Add(new int[] {startY});
                }
            }
        }

        return TempValidMoves;
    }
    
    public static void addTiles(Dictionary<int,int> playerOneTiles, Dictionary<int,int> playerTwoTiles)
    {
        playerOneTiles.Add(1, 2);
        playerOneTiles.Add(2, 3);
        playerOneTiles.Add(3, 3);
        playerOneTiles.Add(4, 1);
        playerTwoTiles.Add(1, 2);
        playerTwoTiles.Add(2, 3);
        playerTwoTiles.Add(3, 3);
        playerTwoTiles.Add(4, 1);
    }
    
    //checks if all tiles are 0, dont think i need this anymore
    public static bool checkGameFinished(Dictionary<int,int> playerTiles)
    {
        for (int i = 1; i <= playerTiles.Count; i++)
        {
            if (playerTiles[i] != 0)
            {
                return false;
            }
        }
        return true;
    }

    //adds move to board array
    public static void addMoveToboard(int[] move)
    {
        for (int i = 0; i < move.Length; i++)
        {
            BoardPositionsArray[move[i]] = 1;
        }
        allBoardPositions.AddRange(move);
    }
    
    public static int[] addMoveToboard2(int[] move, int[] boardArray)
    {
        for (int i = 0; i < move.Length; i++)
        {
            boardArray[move[i]] = 1;
        }
        return boardArray;
    }

    //removes move from board
    public static void removeFromBoard(int[] move)
    {
        for (int i = 0; i < move.Length; i++)
        {
            BoardPositionsArray[move[i]] = 0;
        }
    }
    
    //removes move from board
    public static int[] removeFromBoard2(int[] move, int[] boardArray)
    {
        for (int i = 0; i < move.Length; i++)
        {
            boardArray[move[i]] = 0;
        }
        return boardArray;
    }
    
    //returns array of boolean values
    //true means tiles available
    //false means no tiles available
    public static bool[] Get0TilesSwap(Dictionary<int,int> playerTiles, int length)
    {
        int count = 0;
        bool[] invalidTiles = new bool[4];
        for (int i = 1; i <= 4; i++)
        {
            if (playerTiles[i] <= 0 || i == length)
            {
                invalidTiles[i-1] = false;
                count++;
            }
            else
            {
                invalidTiles[i-1] = true;
            }
        }

        if (count == 4)
        {
            return null;
        }
        return invalidTiles;
    }
    
    public static bool[] Get0Tiles(Dictionary<int,int> playerTiles)
    {
        bool[] invalidTiles = new bool[4];
        for (int i = 1; i <= 4; i++)
        {
            if (playerTiles[i] == 0)
            {
                invalidTiles[i-1] = false;
            }
            else
            {
                invalidTiles[i-1] = true;
            }
        }
        return invalidTiles;
    }

    //---------------------print methods ---------------------------------
    
    //prints all board positions
    public static void printPositions()
    {
        string str = "";
        for (int i = 0; i < BoardPositionsArray.Length; i++)
        {
            str += BoardPositionsArray[i];
        }
        Debug.Log(str);
    }
    public static void printPositions(int[] board)
    {
        string str = "Positionssss: ";
        for (int i = 0; i < board.Length; i++)
        {
            str += board[i];
        }
        Debug.Log(str);
    }
    
    //prints number of tiles player has for each length
    public static void printTiles(Dictionary<int, int> playerTiles)
    {
        string str = "";
        foreach (KeyValuePair<int, int> kvp in playerTiles)
        {
            //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            str += " Value = " + kvp.Value;
        }
        Debug.Log(str);
    }
    
    //prints all items in a move
    public static void printMove(int[] move)
    {
        int tileLength = move.Length;
        string str = "";
        for (int i = 0; i < tileLength; i++)
        {
            str += move[i] + ", ";
        }
        Debug.Log(str);
    }
    
    //prints all valid moves
    public static void printValidMoves()
    {
        int tileLength = 0;
        foreach (int[] move in ValidMoves)
        {
            tileLength = move.Length;
            switch (tileLength)
            {
                case 1:
                    Debug.Log("p1 " + move[0]);
                    break;
                case 2:
                    Debug.Log("p1 " + move[0] + " p2 " + move[1]);
                    break;
                case 3:
                    Debug.Log("p1 " + move[0] + " p2 " + move[1] + " p3 " + move[2]);
                    break;
                case 4:
                    Debug.Log("p1 " + move[0] + " p2 " + move[1] + " p3 " + move[2] + " p4 " + move[3]);
                    break;
            }

        }
    }
}