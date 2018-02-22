using UnityEngine;

public class Destroyable : MonoBehaviour {

	public void DestroySelf() {
		Destroy(gameObject);
	}
}
