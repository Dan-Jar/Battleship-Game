using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BoardUnitInfo : MonoBehaviour
{
    public TMP_Text BoardLabel;
    public int Row, Col;
    public bool occupied;
    public bool attacked;
    void Awake()
    {
        if (BoardLabel != null)
            BoardLabel.text = "[0,0]";
        
    }
    public void UpdateUnitDisplay(int row, int col)
    {
        if(BoardLabel!=null)
            BoardLabel.text = $"[{row},{col}]";
        Row = row;
        Col = col;
    }
}
