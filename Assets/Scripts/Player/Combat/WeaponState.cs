[System.Serializable]
public class WeaponState
{
	public int ammoInMag;
	public int ammoReserve;

	public WeaponState(int ammoInMag, int ammoReserve)
	{
		this.ammoInMag = ammoInMag;
		this.ammoReserve = ammoReserve;
	}
}