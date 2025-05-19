using UnityEngine;

[System.Serializable]
public class SymbolData
{
    public string name;
    public Sprite sprite;
    public float winWeight; // Used when a win happens
    public int score; // Future scalability
}
