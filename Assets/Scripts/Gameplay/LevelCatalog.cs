using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Game/Level Catalog")]
public sealed class LevelCatalog : ScriptableObject
{
	public AssetReferenceT<LevelData>[] levels;

	public int Count => levels?.Length ?? 0;

	public int Clamp(int index)
	{
		return Count == 0 ? 0 : Mathf.Clamp(index, 0, Count - 1);
	}
}