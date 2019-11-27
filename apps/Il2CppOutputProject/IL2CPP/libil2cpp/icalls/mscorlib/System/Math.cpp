#include "il2cpp-config.h"
#include <cmath>
#include <limits>
#include <float.h>
#include "icalls/mscorlib/System/Math.h"
#include "vm/Exception.h"

namespace il2cpp
{
namespace icalls
{
namespace mscorlib
{
namespace System
{
    double Math::Acos(double val)
    {
        return acos(val);
    }

    double Math::Asin(double val)
    {
        return asin(val);
    }

    double Math::Atan(double val)
    {
        return atan(val);
    }

    double Math::Atan2(double y, double x)
    {
        return atan2(y, x);
    }

    double Math::Cos(double val)
    {
        return cos(val);
    }

    double Math::Cosh(double val)
    {
        return cosh(val);
    }

    double Math::Exp(double val)
    {
        return exp(val);
    }

    double Math::Floor(double x)
    {
        return floor(x);
    }

    double Math::Log(double x)
    {
        IL2CPP_NOT_IMPLEMENTED_ICALL_NO_ASSERT(Math::Log, "Determin what value of NAN to use");

        if (x == 0)
            return -HUGE_VAL;
        else if (x < 0)
            return std::numeric_limits<double>::signaling_NaN();
        //return NAN;

        return log(x);
    }

    double Math::Log10(double val)
    {
        return log10(val);
    }

    double Math::Pow(double val, double exp)
    {
        double res = pow(val, exp);
        if (std::isnan(res))
            return 1.0;

        if (res == -0.0)
            return 0.0;

        return res;
    }

    double Math::Round(double x)
    {
        double int_part, dec_part;
        int_part = floor(x);
        dec_part = x - int_part;
        if (((dec_part == 0.5) &&
             ((2.0 * ((int_part / 2.0) - floor(int_part / 2.0))) != 0.0)) ||
            (dec_part > 0.5))
        {
            int_part++;
        }
        return int_part;
    }

    double Math::Sin(double val)
    {
        return sin(val);
    }

    double Math::Sinh(double val)
    {
        return sinh(val);
    }

    double Math::Sqrt(double val)
    {
        return sqrt(val);
    }

    double Math::Tan(double val)
    {
        return tan(val);
    }

    double Math::Tanh(double val)
    {
        return tanh(val);
    }

    double Math::Round2(double value, int32_t digits, bool away_from_zero)
    {
        double p;
        if (value == HUGE_VAL)
            return HUGE_VAL;
        if (value == -HUGE_VAL)
            return -HUGE_VAL;
        if (digits == 0 && !away_from_zero)
            return Round(value);

        p = pow(10.0, digits);

// Note: Migrate to C++11 when possible
#if 0
        if (away_from_zero)
            return std::round(value * p) / p;
        else
            return std::rint(value * p) / p;
#else
        if (away_from_zero)
            return (value >= 0.0) ? floor(value * p + 0.5) / p : ceil(value * p - 0.5) / p;
        else
            return (value >= 0.0) ? ceil(value * p - 0.5) / p : floor(value * p + 0.5) / p;
#endif
    }

#if NET_4_0
    double Math::Abs(double value)
    {
        return fabs(value);
    }

    double Math::Ceiling(double a)
    {
        return ceil(a);
    }

    double Math::SplitFractionDouble(double* value)
    {
        return modf(*value, value);
    }

    float Math::Abs(float value)
    {
        return fabsf(value);
    }

#endif
} /* namespace System */
} /* namespace mscorlib */
} /* namespace icalls */
} /* namespace il2cpp */
