using UnityEngine;

public class OrderCounter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PotionOutput potion)) return;
        if (potion.givenToCustomer) return;

        Actions.FilledOrder?.Invoke();
        Actions.OnCheckCustomers?.Invoke(potion);
    }
}
