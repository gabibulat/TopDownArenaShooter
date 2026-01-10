using UnityEngine;

[CreateAssetMenu(menuName = "Game/Difficulty Profile")]
public sealed class DifficultyProfile : ScriptableObject
{
	[SerializeField] private float healthStart, healthEnd;
	public AnimationCurve healthCurve = AnimationCurve.Linear(0, 0, 1, 1);

	[SerializeField] private float damageStart, damageEnd;
	public AnimationCurve damageCurve = AnimationCurve.Linear(0, 0, 1, 1);

	[SerializeField] private float speedStart, speedEnd;
	public AnimationCurve speedCurve = AnimationCurve.Linear(0, 0, 1, 1);

	public float Health(float normalizedProgress) => Evaluate(healthStart, healthEnd, normalizedProgress);

	public float Damage(float normalizedProgress) => Evaluate(damageStart, damageEnd, normalizedProgress);

	public float Speed(float normalizedProgress) => Evaluate(speedStart, speedEnd, normalizedProgress);

	private float Evaluate(float start, float end, float normalizedProgress)
	{
		return Mathf.Lerp(start, end, Mathf.Clamp01(healthCurve.Evaluate(Mathf.Clamp01(normalizedProgress))));
	}
}