using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public struct Clue
{
    public TextMeshProUGUI ClueText; //Assign in inspector
    public string FirstClueFragment; //Assign in inspector
    public string SecondClueFragment; //Assign in inspector

    public string FullCodedClue; 
    public string FullDecipheredClue; //Assign in inspector

    public bool FoundFirstClueFragment;
    public bool FoundSecondClueFragment;
}

public class CluebookManager : MonoBehaviour
{
    [SerializeField]
    private Clue [] clueIndexes = new Clue [9];

    private List <int> _clueIndexesDeciphered = new List <int>(); 

    private string _clueDialogue;

    private bool _resolvedClue = false; //Clue that's complete but hasn't been deciphered
    private string _incompleteMessage = " (Search for the other clue fragment).";

    void Start()
    {
        for(int i = 0; i < clueIndexes.Length; i++)
            clueIndexes[i].FullCodedClue = clueIndexes[i].FirstClueFragment + " " + clueIndexes[i].SecondClueFragment;

        EventBus.Instance.Subscribe<FoundClueFragment>(CheckForMatchingClueFragments);
        EventBus.Instance.Subscribe<CheckForCompleteClues>(CheckForACompleteClue);
        EventBus.Instance.Subscribe<DecipherClue>(DecipherSingleClue);
    }

    private void CheckForMatchingClueFragments(FoundClueFragment foundClueFragment) //Event call passed from NPC
    {
        _clueDialogue = foundClueFragment.ClueDialogue;

        for(int i = 0; i < clueIndexes.Length; i++)
        {
            if(_clueDialogue == clueIndexes[i].FirstClueFragment) //Found first clue fragment
            {
                clueIndexes[i].ClueText.text = clueIndexes[i].FirstClueFragment + _incompleteMessage;
                clueIndexes[i].FoundFirstClueFragment = true;
            }

            if(_clueDialogue == clueIndexes[i].SecondClueFragment) //Found second clue fragment
            {
                clueIndexes[i].ClueText.text = clueIndexes[i].SecondClueFragment + _incompleteMessage;
                clueIndexes[i].FoundSecondClueFragment = true;
            }

            if(clueIndexes[i].FoundFirstClueFragment == true && clueIndexes[i].FoundSecondClueFragment == true)
            {
                clueIndexes[i].ClueText.text = clueIndexes[i].FullCodedClue; //Completed clue but not yet deciphered
                break;
            }
        }
    }

    //Checking for a complete code that's not deciphered yet, called by DecipherClueButton
    private void CheckForACompleteClue(CheckForCompleteClues checkForCompleteClues)
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

        //Allow for a clue to be crafted in the CraftManager
        EventBus.Instance.Publish(new AllowToCraftClue(_resolvedClue));
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
}
