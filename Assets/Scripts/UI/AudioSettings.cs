using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
	[SerializeField] private AudioMixer mixer;
	[SerializeField] private Slider masterSlider;
	[SerializeField] private Slider musicSlider;
	[SerializeField] private Slider sfxSlider;

	private const string MasterKey = "vol_master";
	private const string MusicKey = "vol_music";
	private const string SfxKey = "vol_sfx";

	private const string MasterParam = "MasterVolume";
	private const string MusicParam = "MusicVolume";
	private const string SfxParam = "SfxVolume";

	private float _master01  = 1f;
	private float _music01  = 1f;
	private float _sfx01 = 1f;

	private void Awake()
	{
		Load();
		ApplyAll();
		InitSlidersFromSavedValues();
		HookSliderEvents();
	}
	
	private void Start()
	{
		ApplyAll();
		StartCoroutine(ApplyNextFrame());
	}

	private IEnumerator ApplyNextFrame()
	{
		yield return null;
		ApplyAll();
	}

	private void InitSlidersFromSavedValues()
	{
		if (masterSlider) masterSlider.SetValueWithoutNotify(_master01);
		if (musicSlider) musicSlider.SetValueWithoutNotify(_music01);
		if (sfxSlider) sfxSlider.SetValueWithoutNotify(_sfx01);
	}

	private void HookSliderEvents()
	{
		if (masterSlider) masterSlider.onValueChanged.AddListener(SetMaster);
		if (musicSlider) musicSlider.onValueChanged.AddListener(SetMusic);
		if (sfxSlider) sfxSlider.onValueChanged.AddListener(SetSfx);
	}

	private void SetMaster(float value01)
	{
		_master01 = Mathf.Clamp01(value01);
		SetMixerVolume(MasterParam, _master01);
		PlayerPrefs.SetFloat(MasterKey, _master01);
	}

	private void SetMusic(float value01)
	{
		_music01 = Mathf.Clamp01(value01);
		SetMixerVolume(MusicParam, _music01);
		PlayerPrefs.SetFloat(MusicKey, _music01);
	}

	private void SetSfx(float value01)
	{
		_sfx01 = Mathf.Clamp01(value01);
		SetMixerVolume(SfxParam, _sfx01);
		PlayerPrefs.SetFloat(SfxKey, _sfx01);
	}

	private void Load()
	{
		_master01 = PlayerPrefs.GetFloat(MasterKey, 1f);
		_music01 = PlayerPrefs.GetFloat(MusicKey, 1f);
		_sfx01 = PlayerPrefs.GetFloat(SfxKey, 1f);
	}

	private void ApplyAll()
	{
		SetMixerVolume(MasterParam, _master01);
		SetMixerVolume(MusicParam, _music01);
		SetMixerVolume(SfxParam, _sfx01);
	}

	private void SetMixerVolume(string exposedParam, float value01)
	{
		var dB = Mathf.Log10(Mathf.Max(value01, 0.0001f)) * 20f;
		mixer.SetFloat(exposedParam, dB);
	}
}