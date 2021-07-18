using System.Collections.Generic;
using SuperChecker.Core.Model;

namespace SuperChecker.Service
{
    public interface ISuperCheckerService
    {
        IEnumerable<EmployeeYearlyResult> GetCheckResults();

        void SetServiceOptions(ServiceOptions options);

        void SetSuperCheckerRequest(SuperCheckerRequest request);
    }
}
