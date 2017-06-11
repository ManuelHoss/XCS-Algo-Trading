using System.Threading.Tasks;
using XCS.Api;

namespace Xcs.Api
{
    public interface IEnvironment
    {
        Task<char[]> GetSituationAsync();
        Task ExecuteActionAsync(XcsAction action);
    }
}
