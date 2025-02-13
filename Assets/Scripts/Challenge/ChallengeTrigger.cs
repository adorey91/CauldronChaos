using UnityEngine;
using System;

public class ChallengeTrigger : MonoBehaviour
{
    public enum ChallengeType { None, SlipperyFloor, MovingCauldron, GoblinUnleashed, WindyDay, SnailSlimed }
    public ChallengeType challengeType;

    public static Action OnStartChallenge;
    [SerializeField] private GameObject[] floor;
    [SerializeField] private GameObject[] cauldron;
    [SerializeField] private GameObject goblin;

    [SerializeField] private PhysicMaterial slipperyMaterial;


    private void Start()
    {
        StartChallenge();
    }


    private void StartChallenge()
    {
        switch (challengeType)
        {
            case ChallengeType.None:
                break;
            case ChallengeType.SlipperyFloor:
                goblin.GetComponent<GoblinAI>().enabled = false;
                PlayerMovement.OnIceDay?.Invoke(true);
                foreach (GameObject floor in floor)
                {
                    floor.GetComponent<Collider>().material = slipperyMaterial;
                }
                break;
            case ChallengeType.MovingCauldron:
                break;
            case ChallengeType.GoblinUnleashed:
                break;
            case ChallengeType.WindyDay:
                break;
        }
    }
}
