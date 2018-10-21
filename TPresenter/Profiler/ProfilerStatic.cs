using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPresenter.Profiler
{
    public static class ProfilerStatic
    {
        public static MyProfiler Profiler { get; set; }

        public static void Begin(string step)
        {
            Profiler.BeginStep(step);
        }

        public static void BeginSubstep(string substep, bool openSubstep = false)
        {
            Profiler.BeginSubstep(substep, openSubstep);
        }

        public static void EndSubstep(bool closeSubstep = false)
        {
            Profiler.EndSubstep(closeSubstep);
        }

        public static void EndCurrent()
        {
            Profiler.EndStep();
        }

        public static void End()
        {
            Profiler.EndActiveSteps();
        }

        public static void AddMemoryUsage()
        {
            Profiler.AddMemeryUsagePrint();
        }
    }
}
