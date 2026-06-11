using UnityEngine;

/// <summary>
/// Manages the prompting of raising of the player's health and publishing an event to show tutorial dialogue 
/// </summary>
/// 
/// <remarks>
/// This script is used for only the first safe zone when no moon puzzles have been solved.
/// 
/// Safe Zones are the OnTrigger collisions that'll raise the player's health continuously.
/// This script works similarily to SafeZone with both being collisions that'll raise the player's health continuously.
/// See <see cref="SafeZone"/> for similarities and make sure they both work the same
/// 
/// This script works together with "PlayerHealth",  "MoonPuzzleDialogueText", "DialogueCanvas", "PlayerStateMachine" scripts
/// See <see cref="PlayerHealth"/> - Publishing "AlterPlayerHealth" to raise the player's health
/// See <see cref="MoonPuzzleDialogueText"/> - Listening to "NewMoonFragmentObtained" event that "MoonPuzzleDialogueText" publishes to change safe zone 
/// areas in the same area and delete the current safe zone in the area
/// 
/// 
/// See <see cref="DialogueCanvas"/> - Publishing "TypeDialogueOnMainUI" event to show the dialogue UI canvas and start dialogue
/// See <see cref="PlayerStateMachine"/> - Publishing "FreezePlayer" to freeze player
/// 
/// See <see cref="StorytellingDialogueData"/> for how the dialogue data is set up for the tutorial
/// 
/// </remarks>

public class FirstSafeZone : MonoBehaviour
{
    [SerializeField]
    private GameObject nextSafeZoneArea;
    //Game object that'll have a larger OnTrigger collision for the same area when a new moon puzzle is completed.
    //Game object should remain inactive as this script sets it active during runtime

    [Header ("For Tutorial Dialogue Message")]
    [SerializeField]
    private GameObject dialogueUI; //Dialogue UI that's layered ontop of the Main Player UI
    [SerializeField] 
    private StorytellingDialogueData corriosonZoneTutorial; 
    
    [SerializeField]
    private float speedToIncraseHealth = 1.2f;

    private bool _showMoonTutorial = false; //Flag controlling to show the moon tutorial ONCE
    private bool _playerCollisionDetected = false;  //Flag whether the player is inside the collision to raise their health

    private const bool RECOVER_HEALTH = true;
    private const bool NEW_EXPLORATION_PHASE = false;
    private const bool STARTING_THE_GAME = false; 

    void Start()
    {
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(SetNewSafeZoneCollision);
    }

    void OnDestroy()
    {
        if (EventBus.Instance != null)
            EventBus.Instance.Unsubscribe<NewMoonFragmentObtained>(SetNewSafeZoneCollision);
    }

    //"NewMoonFragmentObtained" is the name of an event. Empty event
    private void SetNewSafeZoneCollision(NewMoonFragmentObtained newMoonFragmentObtained) //Published by "MoonPuzzleDialogueText"
    {
        if(nextSafeZoneArea != null)
        {
            nextSafeZoneArea.SetActive(true);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider collider) //Raise Player Health
    {
        if (collider.gameObject.CompareTag("Player") && _playerCollisionDetected == false)
        {
            EventBus.Instance.Publish(new AlterPlayerHealth(RECOVER_HEALTH, speedToIncraseHealth)); //Publish to "PlayerHealth"
            _playerCollisionDetected = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = false; 

            if(_showMoonTutorial == false)
            {
                _showMoonTutorial = true;
                ShowTutorial();
            }
        }
    }

    private void ShowTutorial() //Show the tutorial dialogue and set the dialouge UI active
    {
        dialogueUI.SetActive(true);
        EventBus.Instance.Publish(new FreezePlayer(true)); //Publish to "PlayerStateMachine"
        EventBus.Instance.Publish(new TypeDialogueOnMainUI(corriosonZoneTutorial, NEW_EXPLORATION_PHASE, STARTING_THE_GAME)); //Publish to "DialogueCanvas"
    }
}
