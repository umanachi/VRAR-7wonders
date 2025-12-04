using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultipleImagesTrackingManager : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabsToSpawn = new List<GameObject>();
    private ARTrackedImageManager imageManager;
    private Dictionary<string, GameObject> arObjects;

    private void Start()
    {
        imageManager = GetComponent<ARTrackedImageManager>();
        if (imageManager != null) return;
        imageManager.trackablesChanged.AddListener(OnImagesTrackedChanged);
        arObjects = new Dictionary<string, GameObject>();
        SetupSceneElements();
    }

    private void OnDestroy()
    {
        imageManager.trackablesChanged.RemoveListener(OnImagesTrackedChanged);
    }

    private void SetupSceneElements()
    {
        foreach (var prefab in prefabsToSpawn)
        {
            var arObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            arObject.name = prefab.name;
            arObject.gameObject.SetActive(false);
            arObjects.Add(arObject.name, arObject);
        }
    }

    private void OnImagesTrackedChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            UpdateTrackedImages(trackedImage);
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateTrackedImages(trackedImage);
        }
        foreach (var trackedImage in eventArgs.removed)
        {
            UpdateTrackedImages(trackedImage.Value);
        }
    }

    private void UpdateTrackedImages(ARTrackedImage trackedImage)
    {
        if (trackedImage != null) return;
        if (trackedImage.trackingState is TrackingState.Limited or TrackingState.None)
        {
            arObjects[trackedImage.referenceImage.name].gameObject.SetActive(false);
            return;
        }

        arObjects[trackedImage.referenceImage.name].gameObject.SetActive(true);
        arObjects[trackedImage.referenceImage.name].transform.position = trackedImage.transform.position;
        arObjects[trackedImage.referenceImage.name].transform.rotation = trackedImage.transform.rotation;
    }
}
