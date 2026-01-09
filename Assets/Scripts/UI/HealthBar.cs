using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	[SerializeField] private Image fill;
	[SerializeField] private Health health;

	private void Awake()
	{
		health.Changed += HealthUpdate;
		health.Died += () => Destroy(gameObject);
	}

	private void HealthUpdate(int current, int max)
	{
		fill.fillAmount = Mathf.Clamp01((float)current / max);
	}
}