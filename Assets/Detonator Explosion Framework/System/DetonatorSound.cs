using UnityEngine;
using System;

[RequireComponent (typeof (Detonator))]
[AddComponentMenu("Detonator/Sound")]
public class DetonatorSound : DetonatorComponent {
	
	public AudioClip[] nearSounds;
	public AudioClip[] farSounds;
	
	public float distanceThreshold = 50f; //threshold in m between playing nearSound and farSound
	public float minVolume = .4f;
	public float maxVolume = 1f;
	public float rolloffFactor = 0.5f;
	
	private AudioSource _soundComponent;
	private bool _delayedExplosionStarted = false;
	private float _explodeDelay;
	
	override public void Init()
	{
		_soundComponent = (AudioSource)gameObject.AddComponent <AudioSource>();
	}

	void Update()
	{
		if (_soundComponent == null) return;

		_soundComponent.pitch = Time.timeScale;
		
		if (_delayedExplosionStarted)
		{
			_explodeDelay = (_explodeDelay - Time.deltaTime);
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
	}
	
	private int _idx;
	override public void Explode()
	{
		if (detailThreshold > detail) return;
	
		if (!_delayedExplosionStarted)
		{
			_explodeDelay = explodeDelayMin + (UnityEngine.Random.value * (explodeDelayMax - explodeDelayMin));
		}		
		if (_explodeDelay <= 0) 
		{
			//		_soundComponent.minVolume = minVolume;
			//		_soundComponent.maxVolume = maxVolume;
			//		_soundComponent.rolloffFactor = rolloffFactor;

			Vector3 cameraPos;
			try
			{
				cameraPos = GameObject.Find("Main Camera").transform.position;
			}
			catch (NullReferenceException)
			{
				cameraPos = GameObject.Find("Sub Camera").transform.position;
			}
            if (Vector3.Distance(cameraPos, this.transform.position) < distanceThreshold)
			{
				_idx = (int)(UnityEngine.Random.value * nearSounds.Length);
				_soundComponent.PlayOneShot(nearSounds[_idx]);
			}
			else
			{
				_idx = (int)(UnityEngine.Random.value * farSounds.Length);
				_soundComponent.PlayOneShot(farSounds[_idx]);
			}	
			_delayedExplosionStarted = false;
			_explodeDelay = 0f;			
		}
		else
		{
			_delayedExplosionStarted = true;
		}
	}
	
	public void Reset()
	{
	}
}