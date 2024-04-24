using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class playerHUD : MonoBehaviour
{

    public TextMeshProUGUI oneTile;
    public TextMeshProUGUI twoTile;
    public TextMeshProUGUI threeTile;
    public TextMeshProUGUI fourTile;

    public void SetHUD(int four, int three, int two, int one, Sprite tilesImage)
    {
        gameObject.GetComponent<Image>().overrideSprite = tilesImage;
        oneTile.text = "X " + one;
        twoTile.text = "X " + two;
        threeTile.text = "X " + three;
        fourTile.text = "X " + four;
    }


}
