using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AllSFXs", menuName = "Audio Scriptable Objects/AllSFXs")]
public class AllSFXs : ScriptableObject
{
    [Header ("UI Sounds")]
    public AudioClip ButtonSoftSFX;
    public AudioClip ChoiceButtonSFX;
    public AudioClip NextMessageSFX;
    public AudioClip TextAdventurePaperFlipSFX;
    public AudioClip MoonFragmentFoundSFX;

    [Header ("Environment Sounds")]
    public AudioClip PrayingToMoonStatueSFX;
    public AudioClip MoonPuzzleAreaSFX;

    [Header ("Item Sounds - Picking Up")]
    public AudioClip PickUpBerriesSFX;
    public AudioClip PickUpBookSFX;
    public AudioClip PickUpSpeedPotionSFX;
    public AudioClip PickUpFernSFX;
    public AudioClip OpenChestSFX;

    [Header ("Item Sounds - Picking Up and Dropping")]
    public AudioClip WoodSFX;
    public AudioClip SpecialRockSFX;
    public AudioClip RockSFX;

    [Header ("Item Sounds - Dropping")]
    public AudioClip DroppingBookSFX;
    public AudioClip DroppingChestSFX;
    public AudioClip DroppingFernSFX;
    public AudioClip DroppingSpeedPotionSFX;

    //Checking if "audioClip" exists in this scriptable object
    //And this AudioClip in this scriptable object isn't null
    //Called by AudioManager
    public bool IsValidClip(AudioClip audioClip)
    {
        return GetType()
            .GetFields()
            .Where(f => f.FieldType == typeof(AudioClip))
            .Any(f => {
                AudioClip soClip = (AudioClip)f.GetValue(this);
                return soClip != null && soClip == audioClip;
            });
    }
}
