using UnityEngine;
using UnityEngine.AddressableAssets;

public sealed class DropSpawner : MonoBehaviour
{
	[SerializeField] private AssetReferenceGameObject[] pickups;
	private static readonly Quaternion PickupRotation = Quaternion.Euler(-90f, 0f, 0f);

	public void TryDrop(float dropChance, Vector3 position)
	{
		if (pickups == null || pickups.Length == 0) return;
		if (Random.value > dropChance) return;
		var index = Random.Range(0, pickups.Length);
		var pickupRef = pickups[index];
		pickupRef.InstantiateAsync(new Vector3(position.x, -0.5f, position.z), PickupRotation);
	}
}