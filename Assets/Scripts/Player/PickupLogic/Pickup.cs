using UnityEngine;

public sealed class Pickup : MonoBehaviour
{
	[SerializeField] private PickupData data;
	public PickupData Data => data;
}