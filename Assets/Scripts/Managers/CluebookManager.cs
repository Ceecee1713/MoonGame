using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the cluebook for the moon puzzles
/// The cluebook is made up of 9 different clues, and each clue is split into two fragments, totalling 18 individual fragments. Each fragment will be unreadable gibberish.
/// The player will need to have collected both of the 2 individual fragments for one clue, then deciper that clue at the crafting table
/// </summary>
/// 
/// <remarks>
/// 
/// This script works together with scripts: "CraftManager", "DecipherClueButton", "NPC"
/// See <see cref="CraftManager"/> - Listening to "DecipherClue" event that "CraftManager publishes to decipher a clue,
/// and publishing "AllowToCraftClue" to return value to "CraftManager" if it can craft a clue
/// 
/// See <see cref="DecipherClueButton"/> - Listening to "CheckForCompleteClues" event that "DecipherClueButton" publishes to check for any complete, gibberish clues
/// See <see cref="NPC"/> - Listening to "FoundClueFragment" event that "NPC" publishes to change a clue's text and mark a clue fragment found
/// 
/// No other script should be controlling the cluebook's answers directly.
///</remarks>

[Serializable]
public struct Clue
{
    /// <summary>Text for holding the gibberish clue and solved clue. Assign in Inspector.</summary>
    public TextMeshProUGUI ClueText;

    /// <summary>Information about the first clue fragment's gibberish message. Assign in Inspector.</summary>
    /// <remarks>See <see cref="StorytellingDialogueData"/> for how dialogue messages are structured.</remarks>
    public StorytellingDialogueData FirstClueFragment;

    /// <summary>Information about the second clue fragment's gibberish message. Assign in Inspector.</summary>
    /// <remarks>See <see cref="StorytellingDialogueData"/> for how dialogue messages are structured.</remarks>
    public StorytellingDialogueData SecondClueFragment;

    /// <summary>Complete gibberish message built from both fragments at runtime. Do NOT assign in Inspector.</summary>
    [HideInInspector] public string FullGibberishClue;

    /// <summary>Complete deciphered message revealed after crafting. Assign in Inspector.</summary>
    public string FullDecipheredClue;

    /// <summary>Flags whether the first clue fragment has been found. Do NOT assign in Inspector.</summary>
    public bool FoundFirstClueFragment;

    /// <summary>Flags whether the second clue fragment has been found. Do NOT assign in Inspector.</summary>
    public bool FoundSecondClueFragment;
}

public class CluebookManager : MonoBehaviour
{
    [SerializeField]
    private Clue [] clueIndexes = new Clue [9]; //Contains all the nine clues for the cluebook

    private List <int> _clueIndexesDeciphered = new List <int>(); //Prevent solved, deciphered clues (indexes from "clueIndexes") from being look at

    private string _clueDialogueMessage; 
    private string _incompleteMessage = " (Search for the other clue fragment).";

    private bool _resolvedClue = false; //A completed clue in gibberish, not yet deciphered 

    private const int FIRST_MESSAGE_INDEX = 0;
    
    void Start()
    {
        //Assigning all nine clues for the cluebook based on the scriptable objects: "FirstClueFragment", "SecondClueFragment"
        for(int i = 0; i < clueIndexes.Length; i++) 
        {
            string firstClueFragmentMessage = clueIndexes[i].FirstClueFragment.Messages[FIRST_MESSAGE_INDEX].message;
            string secondClueFragmentMessage = clueIndexes[i].SecondClueFragment.Messages[FIRST_MESSAGE_INDEX].message;
            
            clueIndexes[i].FullGibberishClue = firstClueFragmentMessage + " " + secondClueFragmentMessage;
        }
            
        EventBus.Instance.Subscribe<FoundClueFragment>(CheckForMatchingClueFragments);
        EventBus.Instance.Subscribe<CheckForCompleteClues>(CheckForACompleteClue);
        EventBus.Instance.Subscribe<DecipherClue>(DecipherSingleClue);
    }

    //Receives a "FoundClueFragment" event with parameters:
    //(string) "ClueDialogue" - dialogue of the newly found clue fragment
    private void CheckForMatchingClueFragments(FoundClueFragment foundClueFragment) //Published by "NPC"
    {
        _clueDialogueMessage = foundClueFragment.ClueDialogue;

        for(int i = 0; i < clueIndexes.Length; i++)
        {
            string firstClueFragmentMessage = clueIndexes[i].FirstClueFragment.Messages[FIRST_MESSAGE_INDEX].message;
            string secondClueFragmentMessage = clueIndexes[i].SecondClueFragment.Messages[FIRST_MESSAGE_INDEX].message;

            if(_clueDialogueMessage == firstClueFragmentMessage) //Found first clue fragment
            {
                clueIndexes[i].ClueText.text = firstClueFragmentMessage + _incompleteMessage;
                clueIndexes[i].FoundFirstClueFragment = true;
            }

            if(_clueDialogueMessage == secondClueFragmentMessage) //Found second clue fragment
            {
                clueIndexes[i].ClueText.text = secondClueFragmentMessage + _incompleteMessage;
                clueIndexes[i].FoundSecondClueFragment = true;
            }

            if(clueIndexes[i].FoundFirstClueFragment == true && clueIndexes[i].FoundSecondClueFragment == true) //Both clue fragments found
                clueIndexes[i].ClueText.text = clueIndexes[i].FullGibberishClue; //Completed gibberish clue, not yet deciphered
        }
    }

    //"CheckForCompleteClues" is the name of an event. Empty event
    private void CheckForACompleteClue(CheckForCompleteClues checkForCompleteClues) //Published by DecipherClueButton
    {
        for(int i = 0; i < clueIndexes.Length; i++) 
        {
            //Check if a clue is fully complete in gibberish
            if(clueIndexes[i].ClueText.text == clueIndexes[i].FullGibberishClue)
            {
                _resolvedClue = true;
                break;
            }

            _resolvedClue = false;
        }

        EventBus.Instance.Publish(new AllowToCraftClue(_resolvedClue)); //Publish to "CraftManager"
    }

    //"DecipherClue" is the name of an event. Empty event
    private void DecipherSingleClue(DecipherClue decipherClue) //Published by "CraftManager"
    {
        for(int i = 0; i < clueIndexes.Length; i++)
        {
            if(clueIndexes[i].ClueText.text == clueIndexes[i].FullGibberishClue && !_clueIndexesDeciphered.Contains(i))
            {
                clueIndexes[i].ClueText.text = clueIndexes[i].FullDecipheredClue;
                _clueIndexesDeciphered.Add(i); //Don't look at this clue again now that it's been solved
                break;
            }
        }
    }
}
