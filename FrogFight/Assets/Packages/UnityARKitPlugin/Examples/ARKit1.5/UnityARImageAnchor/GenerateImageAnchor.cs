using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class GenerateImageAnchor : MonoBehaviour {

    public Action OnImageFound;

	[SerializeField]
	private ARReferenceImage referenceImage;

	[SerializeField]
	private GameObject prefabToGenerate;

	private GameObject imageAnchorGO;

    private GameObject cubeInstance;

	// Use this for initialization
	void Start () {
		UnityARSessionNativeInterface.ARImageAnchorAddedEvent += AddImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent += UpdateImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorRemovedEvent += RemoveImageAnchor;

	}

	void AddImageAnchor(ARImageAnchor arImageAnchor)
	{
        if (cubeInstance != null)
            return;

        if (arImageAnchor.referenceImageName == referenceImage.imageName)
        {
            Debug.Log("Add Cube");

            var point = new ARPoint();
            point.x = 0.5f;
            point.y = 0.5f;

            var hitResults = UnityARSessionNativeInterface
                            .GetARSessionNativeInterface()
                            .HitTest(point, ARHitTestResultType.ARHitTestResultTypeFeaturePoint);

            if (hitResults.Count == 0)
                return;

            var hit = hitResults[0];
            var position = UnityARMatrixOps.GetPosition(hit.worldTransform);

            cubeInstance = Instantiate<GameObject>(prefabToGenerate, position, Quaternion.identity);

            // Notify all subscibers that he has found an image
            OnImageFound?.Invoke();
        }
	}


    void UpdateImageAnchor(ARImageAnchor arImageAnchor)
	{

	}

	void RemoveImageAnchor(ARImageAnchor arImageAnchor)
	{
		Debug.LogFormat("image anchor removed[{0}] : tracked => {1}", arImageAnchor.identifier, arImageAnchor.isTracked);
		if (imageAnchorGO) {
			GameObject.Destroy (imageAnchorGO);
		}

	}

	void OnDestroy()
	{
		UnityARSessionNativeInterface.ARImageAnchorAddedEvent -= AddImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent -= UpdateImageAnchor;
		UnityARSessionNativeInterface.ARImageAnchorRemovedEvent -= RemoveImageAnchor;

	}

	// Update is called once per frame
	void Update () {
		
	}
}
