using UnityEngine;

[RequireComponent(typeof(FactoryProgressDisplay))]
public class FactoryFacility : MonoBehaviour
{
    [SerializeField]
    private ResourceBase _producedResource;
    [SerializeField]
    private WarehouseFacility _storeWarehouseFacility;
    [SerializeField]
    private WarehouseFacility _produceWarehouseFacility;

    [SerializeField, Space]
    private FactoryProgressDisplay _factoryProgressDisplay;

    private bool _producing = false;
    private float _progress = 0;
    private float _progressNormalized = 0;

    private string _statusMessage;
    private const string WORK_IN_PROGRESS_STATUS = "WORK IN PROGRESS...";
    private const string NO_RESOURCE_STATUS = "NO RESOURCES!!!";
    private const string NO_SPACE_STATUS = "NO SPACE LEFT!!!";

    private void Update()
    {
        if(!_producing)
            _producing = TryScheduleProduction();
        else
            ProduceResource();

        UpdateProgress();
    }

    private void UpdateProgress()
    {
        _factoryProgressDisplay.DisplayProgress(_progressNormalized);
        _factoryProgressDisplay.DisplayStatus(_statusMessage);
    }

    private bool TryScheduleProduction()
    {       
        if(!_produceWarehouseFacility.SubstorageHasSpace(_producedResource))
        {
            _statusMessage = NO_SPACE_STATUS;
            return false;
        }

        int requiredMatsSize = _producedResource.RequiredResourceMaterials.Count;
        
        for (int i = 0; i < requiredMatsSize; i++)
        {
            if (!_storeWarehouseFacility.SubstorageHasResources(_producedResource.RequiredResourceMaterials[i].Resource, _producedResource.RequiredResourceMaterials[i].Amount))
            {
                _statusMessage = NO_RESOURCE_STATUS;
                return false;
            }
        }

        for (int i = 0; i < requiredMatsSize; i++)
            _storeWarehouseFacility.RetrieveFromSubstorage(_producedResource.RequiredResourceMaterials[i].Resource, _producedResource.RequiredResourceMaterials[i].Amount);

        _statusMessage = WORK_IN_PROGRESS_STATUS;
        return true;
    }

    private void ProduceResource()
    {
        _progress += Time.deltaTime;
        _progressNormalized = _progress / _producedResource.TimeToProduce;
        if(_progress >= _producedResource.TimeToProduce)
        {
            _progress = _progressNormalized = 0;
            _produceWarehouseFacility.AddToSubstorage(_producedResource);
            _producing = false;
        }
    }
}
