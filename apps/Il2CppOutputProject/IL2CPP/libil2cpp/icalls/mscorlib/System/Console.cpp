#include "il2cpp-config.h"

#if IL2CPP_DOTS

// When the debugger is enabled, we use the big libil2cpp runtime code
// with the dots profile. We need to build this icall for big libil2cpp,
// so direcrtly include the .cpp file to avboid code duplication.
    #include "../libil2cppdots/icalls/mscorlib/System/Console.cpp"

#endif
