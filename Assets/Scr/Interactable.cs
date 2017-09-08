using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoardIt.Assets
{
    public abstract class Interactable : MonoBehaviour
    {
        protected abstract void OnHover(PlayerControl player);
        protected abstract void OnInteract();

	    // Use this for initialization
	    void Start () {
		
	    }
	
	    // Update is called once per frame
	    void Update () {
		
	    }
}
}
