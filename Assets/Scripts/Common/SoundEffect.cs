using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour {

    [SerializeField]
    private string id;
    public string Id { get { return id; } }
    public AudioSource AudioSource { get; private set; }

	void Start ()
    {
       AudioSource = GetComponent<AudioSource>();	
	}
}
