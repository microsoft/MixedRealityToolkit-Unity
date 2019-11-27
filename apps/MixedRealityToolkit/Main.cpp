#include "pch.h"
#include "App.h"

struct RoInitializeWrapper
{
    inline RoInitializeWrapper() { RoInitialize(RO_INIT_MULTITHREADED); }
    inline ~RoInitializeWrapper() { RoUninitialize(); }
};

int CALLBACK wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nCmdShow)
{
    RoInitializeWrapper roInit;

    Windows::ApplicationModel::Core::CoreApplication::Run(ref new MixedRealityToolkit::App());
    return 0;
}
