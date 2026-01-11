using UnityEngine;

public class Pickup : MonoBehaviour
{
	[SerializeField] private PickupData data;
	public PickupData Data => data;
}