
# Extension service creation wizard

Making the transition from singletons to services can be difficult. This wizard can supplement our other documentation and sample code by enabling devs to create new services with (roughly) the same ease as creating a new MonoBehaviour script. To learn about creating services from scratch, see our [Guide to building Registered Services](../MixedRealityConfigurationGuide.md) (Coming soon).

## Launching the wizard

Launch the wizard from the main menu: **MixedRealityToolkit/Utilities/Create Extension Service** - the wizard will then take you through the process of generating your service script, interface and profile class.

## Editing your service script

By default, your new script assets will be generated in the `MixedRealityToolkit.Generated/Extensions` folder. Once you've completed the wizard, navigate here and open your new service script.

Generated service scripts include some prompts similar to new MonoBehaviour scripts. They will let you know where to initialize and update your service.

    namespace Microsoft.MixedReality.Toolkit.Extensions
    {
        [MixedRealityExtensionService(SupportedPlatforms.WindowsStandalone|SupportedPlatforms.MacStandalone|SupportedPlatforms.LinuxStandalone|SupportedPlatforms.WindowsUniversal)]
        public class NewService : BaseExtensionService, INewService, IMixedRealityExtensionService
        {
            private NewServiceProfile newServiceProfile;
    
            public NewService(IMixedRealityServiceRegistrar registrar,  string name,  uint priority,  BaseMixedRealityProfile profile) : base(registrar, name, priority, profile) 
            {
                newServiceProfile = (NewServiceProfile)profile;
            }
    
            public override void Initialize()
            {
                // Do service initialization here.
            }
    
            public override void Update()
            {
                // Do service updates here.
            }
        }
    }

If you chose to register your service in the wizard, all you have to do is edit this script and your service will automatically be updated. Otherwise you can read about [registering your new service here](../MixedRealityConfigurationGuide.md).
