using System;
using System.Collections.Generic;
using System.Text;
using Sentinel.Domain.Models;

namespace Sentinel.Domain.Extensions
{
    public static class SeverityExtensions
    {
        public static Severity ElevateSeverityIfApplicable(this Severity currentSeverity, Severity newSeverity) => (int) newSeverity > (int) currentSeverity ? newSeverity : currentSeverity;
    }
}
