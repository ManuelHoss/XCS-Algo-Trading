using System.Threading.Tasks;
using MtApi;
using XCS.Api;

namespace Xcs.Api
{
    public interface IEnvironment
    {
        Task<char[]> GetSituationAsync(string symbol, ChartPeriod timeframe, int maShift, int period, int appliedPrice, int shift);
        Task ExecuteActionAsync(XcsAction action);
    }
}
