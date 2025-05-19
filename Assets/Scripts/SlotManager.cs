using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class SlotManager : MonoBehaviour
{
    [Header("Slot Settings")]
    public Image[] slotImages;
    public Button spinButton;
    public SymbolData[] symbols;
    public float spinDuration = 2f;
    public float spinSpeed = 0.05f;

    [Header("Win Control")]
    [Range(0f, 100f)] public float winChancePercent = 30f;

    [Header("Win UI")]
    public GameObject winPanel; 
    public TMP_Text winText;
    public Button continueButton;

    void Start()
    {
        spinButton.onClick.AddListener(SpinSlots);
        continueButton.onClick.AddListener(SpinSlots);
        InitRandomSymbols();
        winPanel.SetActive(false);
    }

    void InitRandomSymbols()
    {
        List<int> usedIndices = new List<int>();
        for (int i = 0; i < slotImages.Length; i++)
        {
            int index;
            do
            {
                index = Random.Range(0, symbols.Length);
            } while (usedIndices.Contains(index));
            usedIndices.Add(index);
            slotImages[i].sprite = symbols[index].sprite;
        }
    }

    void SpinSlots()
    {
        spinButton.interactable = false;
        winPanel.SetActive(false);
        StartCoroutine(SpinAllSlots());
    }

    IEnumerator SpinAllSlots()
    {
        bool isWin = Random.value <= winChancePercent / 100f;
        SymbolData winSymbol = null;

        if (isWin)
        {
            winSymbol = GetWeightedRandomSymbol();
        }

        Coroutine[] spinning = new Coroutine[slotImages.Length];

        for (int i = 0; i < slotImages.Length; i++)
        {
            spinning[i] = StartCoroutine(SpinSlot(slotImages[i], spinDuration + i * 0.2f, isWin ? winSymbol.sprite : null));
        }

        foreach (Coroutine c in spinning)
            yield return c;

        if (isWin)
        {
            ShowWinUI(winSymbol);
        }
        spinButton.interactable = true;
    }

    IEnumerator SpinSlot(Image slotImage, float duration, Sprite forceSprite = null)
    {
        float timer = 0f;
        while (timer < duration)
        {
            int index = Random.Range(0, symbols.Length);
            slotImage.sprite = symbols[index].sprite;
            timer += spinSpeed;
            yield return new WaitForSeconds(spinSpeed);
        }

        if (forceSprite != null)
        {
            slotImage.sprite = forceSprite;
        }
    }

    SymbolData GetWeightedRandomSymbol()
    {
        float totalWeight = symbols.Sum(s => s.winWeight);
        float roll = Random.Range(0, totalWeight);
        float accum = 0f;

        foreach (var symbol in symbols)
        {
            accum += symbol.winWeight;
            if (roll <= accum)
                return symbol;
        }

        return symbols[0];
    }

    void ShowWinUI(SymbolData symbol)
    {
        winPanel.SetActive(true);
        //winText.text = $"You Win! ({symbol.name})";
        //Debug.Log($"Win! Matched {symbol.name}, Score: {symbol.score}");
    }
}
