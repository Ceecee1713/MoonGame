using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public struct Clue
{
    public TextMeshProUGUI ClueText; //Assign in inspector
    //public string FirstClueFragment; //Assign in inspector
    //public string SecondClueFragment; //Assign in inspector
    public StorytellingDialogueData FirstClueFragment;
    public StorytellingDialogueData SecondClueFragment;

    [HideInInspector]
    public string FullCodedClue; //Don't assign in inspector
    public string FullDecipheredClue; //Assign in inspector

    public bool FoundFirstClueFragment;
    public bool FoundSecondClueFragment;
}

public class CluebookManager : MonoBehaviour
{
    [SerializeField]
    private Clue [] clueIndexes = new Clue [9];

    private List <int> _clueIndexesDeciphered = new List <int>(); //Prevent deciphered clues (indexes from "clueIndexes") from being solved/look at again

    private string _clueDialogueMessage;
    private string _incompleteMessage = " (Search for the other clue fragment).";

    private bool _resolvedClue = false; //Bool to represent a clue that's complete but hasn't been deciphered

    private const int FIRST_MESSAGE = 0;
    
    void Start()
    {
        for(int i = 0; i < clueIndexes.Length; i++)
            //clueIndexes[i].FullCodedClue = clueIndexes[i].FirstClueFragment + " " + clueIndexes[i].SecondClueFragment;
            clueIndexes[i].FullCodedClue = clueIndexes[i].FirstClueFragment.Messages[FIRST_MESSAGE].message + " " + clueIndexes[i].SecondClueFragment.Messages[FIRST_MESSAGE].message;

        EventBus.Instance.Subscribe<FoundClueFragment>(CheckForMatchingClueFragments);
        EventBus.Instance.Subscribe<CheckForCompleteClues>(CheckForACompleteClue);
        EventBus.Instance.Subscribe<DecipherClue>(DecipherSingleClue);
    }

    private void CheckForMatchingClueFragments(FoundClueFragment foundClueFragment) //Published by NPC
    {
        _clueDialogueMessage = foundClueFragment.ClueDialogue;

        for(int i = 0; i < clueIndexes.Length; i++)
        {
            if(_clueDialogueMessage == clueIndexes[i].FirstClueFragment.Messages[FIRST_MESSAGE].message) //Found first clue fragment
            {
                clueIndexes[i].ClueText.text = clueIndexes[i].FirstClueFragment.Messages[FIRST_MESSAGE].message + _incompleteMessage;
                clueIndexes[i].FoundFirstClueFragment = true;
            }

            if(_clueDialogueMessage == clueIndexes[i].SecondClueFragment.Messages[FIRST_MESSAGE].message) //Found second clue fragment
            {
                clueIndexes[i].ClueText.text = clueIndexes[i].SecondClueFragment.Messages[FIRST_MESSAGE].message + _incompleteMessage;
                clueIndexes[i].FoundSecondClueFragment = true;
            }

            if(clueIndexes[i].FoundFirstClueFragment == true && clueIndexes[i].FoundSecondClueFragment == true)
                clueIndexes[i].ClueText.text = clueIndexes[i].FullCodedClue; //Completed clue but not yet deciphered
        }
    }

    private void CheckForACompleteClue(CheckForCompleteClues checkForCompleteClues) //Published by DecipherClueButton
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
                _clueIndexesDeciphered.Add(i); //Don't look at this clue again now that it's been solved
                break;
            }
        }
    }
}
