using SuperChecker.Core.Model;

namespace SuperChecker.Data
{
    public interface ISuperDataReader
    {
        SuperCheckerRequest GetSuperCheckerRequest(string filePath);
    }
}
