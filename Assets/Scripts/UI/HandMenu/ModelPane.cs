using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelPane : ScrollPane
{
    [SerializeField] protected ModelUI contentPrefab;
    [SerializeField] private List<ModelUI> uiElements = new List<ModelUI>();
    [SerializeField] private RectTransform defaultElement;

    public override void populateScrollPane()
    {
        if(Config.Instance.AllModels.childCount < 1)
        {
            defaultElement.gameObject.SetActive(true);
            return;
        }
        defaultElement.gameObject.SetActive(false);
        //for each model in the _/test_objects transform
        for (int i = 0; i < Config.Instance.AllModels.childCount; i++)
        {
            //get a ref of the model
            Transform model = Config.Instance.AllModels.GetChild(i);
            //instantiate content prefab
            ModelUI content = Instantiate<ModelUI>(contentPrefab);
            //set the parent and local transforms of content
            content.transform.SetParent(contentHolder);
            content.transform.localPosition = Vector3.zero;
            content.transform.localRotation = Quaternion.identity;
            content.transform.localScale = Vector3.one;
            //attempt to generate thumbnail
            Texture2D thumbnail = ThumbnailGenerator.Instance.getModelThumbnail(model);
            //initialise content
            content.initialise(model,thumbnail);
            uiElements.Add(content);
        }
    }

}