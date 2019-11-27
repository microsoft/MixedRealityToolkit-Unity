#pragma once

namespace MixedRealityToolkit
{
    ref class App sealed :
        public Windows::ApplicationModel::Core::IFrameworkView,
        public Windows::ApplicationModel::Core::IFrameworkViewSource
    {
    public:
        virtual void Initialize(Windows::ApplicationModel::Core::CoreApplicationView^ applicationView);
        virtual void SetWindow(Windows::UI::Core::CoreWindow^ window);
        virtual void Load(Platform::String^ entryPoint);
        virtual void Run();
        virtual void Uninitialize();

        virtual Windows::ApplicationModel::Core::IFrameworkView^ CreateView();

    private:
        UnityPlayer::AppCallbacks^ m_AppCallbacks;
        Windows::UI::Core::CoreWindow^ m_CoreWindow;
        void OnActivated(Windows::ApplicationModel::Core::CoreApplicationView^ sender, Windows::ApplicationModel::Activation::IActivatedEventArgs^ args);
        void SetupOrientation();
    };
}
