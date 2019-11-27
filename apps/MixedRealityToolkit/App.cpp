#include "pch.h"
#include "App.h"
#include "UnityGenerated.h"

using namespace MixedRealityToolkit;
using namespace Platform;
using namespace UnityPlayer;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;

void App::Initialize(CoreApplicationView^ applicationView)
{
    SetupOrientation();
    m_AppCallbacks = ref new AppCallbacks();
    m_AppCallbacks->SetCoreApplicationViewEvents(applicationView);
    applicationView->Activated += ref new TypedEventHandler<CoreApplicationView ^, IActivatedEventArgs^>(this, &App::OnActivated);
}

void App::SetWindow(CoreWindow^ window)
{
    m_CoreWindow = window;

    ApplicationView::GetForCurrentView()->SuppressSystemOverlays = true;

    m_AppCallbacks->SetCoreWindowEvents(window);
    m_AppCallbacks->InitializeD3DWindow();
}

void App::Load(String^ entryPoint)
{
}

void App::Run()
{
    m_AppCallbacks->Run();
}

void App::Uninitialize()
{
    m_AppCallbacks = nullptr;
}

IFrameworkView^ App::CreateView()
{
    return this;
}

void App::OnActivated(CoreApplicationView^ sender, IActivatedEventArgs^ args)
{
    m_CoreWindow->Activate();
}

void App::SetupOrientation()
{
    Unity::SetupDisplay();
}
