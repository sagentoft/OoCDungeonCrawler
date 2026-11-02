using System;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private PlayerCharacter character;
    [SerializeField] private HumanController humanController;
    [SerializeField] private AIController aiController;

    private ControlManager _controlManager;
    
    private void Awake()
    {
        _controlManager = new ControlManager(character);

        humanController.Initialize(_controlManager);
        aiController.Initialize(_controlManager);
    }

    private void Update()
    {
        _controlManager.Tick(Time.deltaTime);
    }
}
