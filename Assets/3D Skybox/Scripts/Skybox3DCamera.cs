using UnityEngine;
using System.Collections;

public class Skybox3DCamera : MonoBehaviour
{
	public Camera useSpecificCamera;

  public float skyboxScale = 1;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	void LateUpdate ()
	{
		Camera camera = useSpecificCamera ? useSpecificCamera : Camera.main;
		
		if (!camera)
		{
			Debug.LogWarning("Skybox3DCamera.Update() " + name + " no valid camera!");
			return;
		}
		
		transform.rotation = camera.transform.rotation;
    transform.localPosition = camera.transform.position / skyboxScale;
	}
}
