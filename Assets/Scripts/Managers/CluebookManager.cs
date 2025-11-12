using System;
using UnityEngine;
using TMPro;

[Serializable]
public struct Clue
{
    public TextMeshProUGUI ClueText;
    public Dialogue FirstClueFragment;
    public Dialogue SecondClueFragment;

    public string FullCodedClue;
    public string FullDecipheredClue;

    public bool FoundFirstClueFragment;
    public bool FoundSecondClueFragment;
}

public class CluebookManager : MonoBehaviour
{
    [SerializeField]
    private Clue [] clueIndexes = new Clue [9];

    private Dialogue _clueDialogue;

    private string _incompleteMessage = " (Search for the other clue fragment).";

    void Start()
    {
        for(int i = 0; i < clueIndexes.Length; i++)
            clueIndexes[i].FullCodedClue = clueIndexes[i].FirstClueFragment.Message + " " + clueIndexes[i].SecondClueFragment.Message;

        EventBus.Instance.Subscribe<FoundClueFragment>(CheckForMatchingClueFragments);
    }

    private void CheckForMatchingClueFragments(FoundClueFragment foundClueFragment)
    {
        _clueDialogue = foundClueFragment.ClueDialogue;

        for(int i = 0; i < clueIndexes.Length; i++)
        {
            if(_clueDialogue == clueIndexes[i].FirstClueFragment)
            {
                clueIndexes[i].ClueText.text = clueIndexes[i].FirstClueFragment.Message + _incompleteMessage;
                clueIndexes[i].FoundFirstClueFragment = true;
            }

            if(_clueDialogue == clueIndexes[i].SecondClueFragment)
            {
                clueIndexes[i].ClueText.text = clueIndexes[i].SecondClueFragment.Message + _incompleteMessage;
                clueIndexes[i].FoundSecondClueFragment = true;
            }

            if(clueIndexes[i].FoundFirstClueFragment == true && clueIndexes[i].FoundSecondClueFragment == true)
            {
                clueIndexes[i].ClueText.text = clueIndexes[i].FullCodedClue;
                break;
            }
        }
    }
}
