using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class randomVsLargest : SwapGame
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
    public static Random rnd = new Random();
    
    
}