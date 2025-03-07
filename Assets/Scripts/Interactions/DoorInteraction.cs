using UnityEngine;
using DG.Tweening;

public class DoorInteraction : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] private SFXLibrary newCustomerSfx;
    
    [Header("Door Object")]
    [SerializeField] private GameObject door;
    [SerializeField] private bool isEntrance;
    private int _customerCount;

    private void OpenDoor()
    {
        door.transform.DOKill();
        door.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f);
    }

    private void CloseDoor()
    {
        //Debug.Log("Closing Door");
        door.transform.DOKill();
        door.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Customer")) return;

        _customerCount++;
        if (isEntrance)
            AudioManager.instance.sfxManager.PlaySFX(SFX_Type.ShopSounds, newCustomerSfx.PickAudioClip(), true);

        OpenDoor();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Customer")) return;

        _customerCount--; // Decrease count when a customer exits
        _customerCount = Mathf.Max(_customerCount, 0); // Ensure it never goes below 0

        if (_customerCount == 0) // Close door only if no customers are left
        {
            CloseDoor();
        }
    }
}