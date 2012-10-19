using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common
{
    public enum Importance 
    { 
        Trivial, 
        Critical 
    }

    [Flags]
    public enum ProgramConfigurationWarningType 
    {  
        Unknown = 0,
        Performance = 1,
        Stability = 2
    }

    public enum CodeState
    {
        /// <summary>
        /// Workaround/Hack/Temporary fix
        /// </summary>
        Workaround,
        /// <summary>
        /// Works for the usual usage case, but not as general as it should be
        /// </summary>
        Incomplete,
        /// <summary>
        /// Works but untested
        /// </summary>
        Untested,
        /// <summary>
        /// Works and well tested
        /// </summary>
        Stable
    }

    public struct ProgramConfigurationWarning
    {
        public string Module { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public Importance Importance { get; set; }
        public ProgramConfigurationWarningType Type { get; set; }
    }

    public class CodeStateAttribute : Attribute
    {
        public CodeState State { get; set; }
        public String Details { get; set; }
    }

    /// <summary>
    /// Class that handles program wide configuration warnings and 
    /// </summary>
    public static class ProgramConfigurationInformation
    {
        public static void AddWarning(ProgramConfigurationWarning warning)
        {
            if (warning.Module == null)
                warning.Module = new StackFrame(1).GetMethod().DeclaringType.Name;
            warnings.Add(warning);
            if (WarningsChanged != null)
                WarningsChanged();
        }

        public static void RemoveWarning(String text)
        {
            throw new NotImplementedException();
        }

        public static event Action WarningsChanged;

        public static List<ProgramConfigurationWarning> Warnings { get { return warnings; } }

        static List<ProgramConfigurationWarning> warnings = new List<ProgramConfigurationWarning>();
    }
}
