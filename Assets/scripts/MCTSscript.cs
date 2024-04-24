using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class MCTSscript: MonoBehaviour
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
    private bool playerOnePass;


    private static double Cp = 1/Math.Sqrt(2);
    public int numExpansions = 100;
    private static double[,] lookupTable = new double[1500,1500];

    private static int numLookups = 0;
    private static int numNoLookups = 0;
    
    public static void setup()
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
        
        for (int i = 1; i < 1500; ++i)
        for (int j = i; j < 1500; ++j)
            lookupTable[i,j] = (Cp * Math.Sqrt((Math.Log((double)i)) / (double)j ));
    }
    
    void Start()
    {
        setup();
        //printValidMoves();
        //buttonStart();
        //randomVsRandom();
        MCTSPlay();
    }
    
    public static double getRHS(int n, int nj){
        if (n < 1500){
            numLookups ++;
            return lookupTable[n, nj];
        }
        numNoLookups++;
        return (2*Cp * Math.Sqrt((2*(Math.Log((double)n))) / (double)nj ));
    }

    public void MCTSPlay()
    {
        List<object> BoardState = new List<object>();
        int randomNo = 0;
        bool isFinished = false;
        int length = 0;
        BoardState = FirstBoard();
        while(!isFinished)
        {
            var playerOne = computerMove(BoardState);
            BoardState = playerOne.move;
            playerOnePass = playerOne.GameOver;
            printBoardState(BoardState);
            if ((int[]) BoardState[8]!= null)
            {
                int[] move = (int[]) BoardState[8];
                Destroy(GameObject.Find("randomScript" + move[0]));
                PutMoveOnBoard((int[])BoardState[7], blueFrozen);
            }
            else
            {
                PutMoveOnBoard((int[])BoardState[7], blueTile);
            }
            var playerTwo = computerMove(BoardState);
            BoardState = playerTwo.move;
            if ((int[]) BoardState[8]!= null)
            {
                int[] move = (int[]) BoardState[8];
                Destroy(GameObject.Find("randomScript" + move[0]));
                PutMoveOnBoard((int[])BoardState[7], redFrozen);
            }
            else
            {
                PutMoveOnBoard((int[])BoardState[7], redTile);
            }
            printBoardState(BoardState);
            checkFinishedMain(playerOnePass, playerTwo.GameOver);
        }
        //randomScript.getWinner(playerOnePass,playerTwoPass);
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
    
    public static void printBoardState(List<object> boardState)
    {
        SwapGame.printPositions((int[])boardState[0]);
    }


    public static string printList(List<int> boardPositions)
    {
        string str = "";
        foreach (var pos in boardPositions)
        {
            str += pos;
        }
        return str;
    }
    public static (List<object> move,bool GameOver) computerMove(List<object> BoardState)
    {
        List<object> bestMove = new List<object>();
        NodeMCTS rootNode = new NodeMCTS(BoardState);

        if (rootNode.board.availableMoves == null)
        {
            return (BoardState,true);
        }

        for (int i = 0; i < 10; i++)
        {
            NodeMCTS n = TreePolicy(rootNode);
            n.Backup(Simulate(n));
        }
        NodeMCTS maxNode = null;
        //Debug.Log ("maxnode set");
        double maxVal = double.NegativeInfinity;

        foreach (NodeMCTS node in rootNode.children) {
            if(node.timesVisited == 0){
                continue;
            }
            if((double)node.score/(double)node.timesVisited > maxVal){
                maxNode = new NodeMCTS(node);
                maxVal = (double)node.score/(double)node.timesVisited;
            }
        }

        if (maxNode == null)
        {
            Debug.Log("fail");
        }
        bestMove = maxNode.move;
        //printBoardState(bestMove);
        return (bestMove,false);
    }
    
    public static void PutMoveOnBoard(int[] move, GameObject tile)
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
        }
        else
        {
            //vertical
            middleOfTile = moveCoords.y + ((float)distance / 2);
            GameObject newTile = Instantiate(tile, new Vector3(moveCoords.x, middleOfTile, 0), Quaternion.identity);
            newTile.transform.localScale = new Vector3(1, move.Length, 1);
            newTile.name = "randomScript"+ move[0];
            newTile.layer = 1;
        }
        //convert move values to coords -
    }
    public static Vector2 getCoords(int move)
    {
        double xpos = move % 6;
        double ypos = move / 6;
        xpos -= 2.5;
        ypos -= 2.5;
        return new Vector2((float)xpos, (float)ypos);
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
                k += 5;
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

    private static NodeMCTS TreePolicy(NodeMCTS n)
    {
        NodeMCTS v = n;

        while (v.board.availableMoves.Count != 0)
        {
            v.addAvailableMoveNodes(v.board.availableMoves);
            if (v.availableMoveNodes.Count != 0)
            {
                return v.Expand();
            }
            else
            {
                v = v.BestChild();
            }
        }
        return v;
    }

    public static int Simulate(NodeMCTS n)
    {
        var initBoardPos = (int[]) n.move[0];
        BoardPositionsArray = initBoardPos.ToArray();
        var initAllBoardPos = (List<int>) n.move[1];
        allBoardPositions = new List<int>(initBoardPos);
        var pOneTiles = (Dictionary<int, int>) n.move[2];
        playerOneTiles = new Dictionary<int, int>(pOneTiles);
        var pTwoTiles = (Dictionary<int, int>) n.move[3];
        playerTwoTiles = new Dictionary<int, int>(pTwoTiles);
        playerOneMovesTaken = (List<int[]>) n.move[4];
        playerOneMovesTaken = new List<int[]>(playerOneMovesTaken);
        playerTwoMovesTaken = (List<int[]>) n.move[5];
        playerTwoMovesTaken = new List<int[]>(playerTwoMovesTaken);
        var initState = (gameState) n.move[6];
        state = initState;

        int randomNo = 0;
        bool isFinished = false;
        bool playerOnePass = false;
        bool playerTwoPass = false;
        bool firstIteration = true;
        while (!isFinished)
        {
            if (firstIteration && state == gameState.PLAYERTWO)
            {
                playerOnePass = pass;
                playerTwoPass = playerTwoTurn();
                firstIteration = false;
            }
            else
            {
                playerOnePass = playerOneTurn();
                playerTwoPass = playerTwoTurn();
            }

            isFinished = checkFinished(playerOnePass, playerTwoPass);
        }

        return getWinner(playerOnePass, playerTwoPass);
    }
    
    public static bool checkFinished(bool playerOne, bool playerTwo)
    {
        if (playerOne || playerTwo)
        {
            return true;
        }
        return false;
    }
    
    

    public static int getWinner(bool playerOne, bool playerTwo)
    {
        if (playerOne && playerTwo)
        {
            return 0;
        }else if (playerOne)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
    

    public static bool playerOneTurn()
    {
        int[] move;
        if (isSwap2(playerOneTiles))
        {
            int removeLength = PickTileRemoval(playerOneMovesTaken,playerOneTiles);
            move = makeMoveSwap(playerOneTiles, removeLength);
            if (move == null)
            {
                return true;
            }
            FrozenPositions.AddRange(move);
        }
        else //regular move
        {
            move = makeMove(playerOneTiles);
            if (move == null)
            {
                return true;
            }
            playerOneMovesTaken.Add(move);
        }
        addMoveToboard(move);
        playerOneTiles[move.Length] -= 1;
        state = gameState.PLAYERTWO;
        return false;
    }

    public static bool playerTwoTurn()
    {
        int[] move;
        if (isSwap2(playerTwoTiles))
        {
            int removeLength = PickTileRemoval(playerTwoMovesTaken,playerTwoTiles);
            move = makeMoveSwap(playerTwoTiles, removeLength);
            if (move == null)
            {
                return true;
            }
            FrozenPositions.AddRange(move);
        }
        else //regular move
        {
            move = makeMove(playerTwoTiles);
            if (move == null)
            {
                return true;
            }
            playerTwoMovesTaken.Add(move);
        }
        addMoveToboard(move);
        playerTwoTiles[move.Length] -= 1;
        state = gameState.PLAYERONE;
        return false;
    }
    public List<object> FirstBoard()
    {
        int[] placeholderArray = new[] {-1};
        var tempBoardState = new List<object>();
        tempBoardState.Add(BoardPositionsArray);
        tempBoardState.Add(allBoardPositions);
        tempBoardState.Add(playerOneTiles);
        tempBoardState.Add(playerTwoTiles);
        tempBoardState.Add(playerOneMovesTaken);
        tempBoardState.Add(playerTwoMovesTaken);
        tempBoardState.Add(gameState.PLAYERTWO);
        tempBoardState.Add(placeholderArray);
        tempBoardState.Add(null);
        return tempBoardState;
    }
    
    public static bool isSwap2(Dictionary<int,int> playerTiles)
    {
        int largestSpace = 0;
        //get smallest tile
        //check if largest space < smallest tile
        int smallestTile = lineScript.GetSmallestTile(playerTiles);
        int length1 = getLargestSpaceHorizontal(BoardPositionsArray);
        int length2 = getLargestSpaceVertical(BoardPositionsArray);
        largestSpace = Math.Max(length1, length2);
        if (largestSpace < smallestTile)
        {
            return true;
        }
        return false;
    }

    public static int PickTileRemoval(List<int[]> playerMovesTaken, Dictionary<int,int> playerTiles)
    {
        //gets move from taken moves list
        int randomNo = rnd.Next(playerMovesTaken.Count - 1);
        int length = playerMovesTaken[randomNo].Length;
        removeFromBoard(playerMovesTaken[randomNo]);
        playerTiles[length] += 1;
        return length;
    }
    
    public static void removeFromBoard(int[] move)
    {
        for (int i = 0; i < move.Length; i++)
        {
            BoardPositionsArray[move[i]] = 0;
        }
    }
    public static void addMoveToboard(int[] move)
    {
        for (int i = 0; i < move.Length; i++)
        {
            BoardPositionsArray[move[i]] = 1;
        }
        allBoardPositions.AddRange(move);
    }
    
    public static int[] makeMoveSwap(Dictionary<int,int> playerTiles, int length)
    {
        bool[] invalidTiles = Get0Tiles(playerTiles);
        List<int[]> tempValidMoves = new List<int[]>();
        //checks all possible moves
        for (int i = 0; i < ValidMoves.Count; i++)
        {
            //removes invalid moves based on board positions
            if (!ValidMoves[i].Intersect(allBoardPositions).Any())
            {
                if (invalidTiles[ValidMoves[i].Length - 1] && ValidMoves[i].Length != length)
                {
                    tempValidMoves.Add(ValidMoves[i]);
                }
            }
        }
        if (tempValidMoves.Count == 0)
        {
            return null;
        }
        int random = rnd.Next(tempValidMoves.Count - 1);
        FrozenPositions.AddRange(tempValidMoves[random]);
        return tempValidMoves[random];
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
    public static int[] makeMove(Dictionary<int,int> playerTiles)
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
        if (tempValidMoves.Count == 0)
        {
            return null;
        }
        int random = rnd.Next(tempValidMoves.Count - 1);
        return tempValidMoves[random];
    }

    public static int getBlueOne()
    {
        return playerOneTiles[1];
    }
    public static int getBlueTwo()
    {
        return playerOneTiles[2];
    }
    public static int getBlueThree()
    {
        return playerOneTiles[3];
    }
    public static int getBlueFour()
    {
        return playerOneTiles[4];
    }

    public static int getRedOne()
    {
        return playerTwoTiles[1];
    }
    public static int getRedTwo()
    {
        return playerTwoTiles[2];
    }
    public static int getRedThree()
    {
        return playerTwoTiles[3];
    }
    public static int getRedFour()
    {
        return playerTwoTiles[4];
    }
    
}