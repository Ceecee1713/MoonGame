using UnityEngine;

/// <summary>
/// Manages the prompting of raising of the player's health and publishing an event to show tutorial dialogue 
/// </summary>
/// 
/// <remarks>
/// This script is used for only the first safe zone when no moon puzzles have been solved.
/// 
/// Safe Zones are the OnTrigger collisions that'll raise the player's health continuously.
/// 
/// This script works together with "PlayerHealth",  "MoonPuzzleDialogueText", "DialogueCanvas" scripts
/// See <see cref="PlayerHealth"/> for how they work together - prompting to raise the player's health
/// See <see cref="MoonPuzzleDialogueText"/> for how they work together - publishing the "NewMoonFragmentObtained" event this script listens to
/// See <see cref="DialogueCanvas"/> for how they work together - prompting to show the dialogue canvas and start dialogue
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

    //Destroy the Game Object attached to this script and set the replacement safe zone for the same area active
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
        EventBus.Instance.Publish(new FreezePlayer(true));
        EventBus.Instance.Publish(new TypeDialogueOnMainUI(corriosonZoneTutorial, NEW_EXPLORATION_PHASE, STARTING_THE_GAME)); //Publish to "DialogueCanvas"
    }
}
