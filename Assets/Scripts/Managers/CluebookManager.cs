using System;
using System.Collections.Generic;
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

    private List <int> _clueIndexesDeciphered = new List <int>(); 

    private Dialogue _clueDialogue;

    private bool _resolvedClue = false;
    private string _incompleteMessage = " (Search for the other clue fragment).";

    void Start()
    {
        for(int i = 0; i < clueIndexes.Length; i++)
            clueIndexes[i].FullCodedClue = clueIndexes[i].FirstClueFragment.Message + " " + clueIndexes[i].SecondClueFragment.Message;

        EventBus.Instance.Subscribe<FoundClueFragment>(CheckForMatchingClueFragments);
        EventBus.Instance.Subscribe<DecipherClue>(DecipherSingleClue);
        EventBus.Instance.Subscribe<CheckForFinishedClues>(CheckForResolvedClues);
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

    private void DecipherSingleClue(DecipherClue decipherClue)
    {
        for(int i = 0; i < clueIndexes.Length; i++)
        {
            if(clueIndexes[i].ClueText.text == clueIndexes[i].FullCodedClue && !_clueIndexesDeciphered.Contains(i))
            {
                clueIndexes[i].ClueText.text = clueIndexes[i].FullDecipheredClue;
                _clueIndexesDeciphered.Add(i);
                break;
            }
        }
    }

    private void CheckForResolvedClues(CheckForFinishedClues checkForFinishedClues)
    {
        for(int i = 0; i < clueIndexes.Length; i++)
        {
            if(clueIndexes[i].ClueText.text == clueIndexes[i].FullCodedClue)
            {
                _resolvedClue = true;
                break;
            }

            _resolvedClue = false;
        }

        EventBus.Instance.Publish(new AllowToCraftClue(_resolvedClue));
    }
}
