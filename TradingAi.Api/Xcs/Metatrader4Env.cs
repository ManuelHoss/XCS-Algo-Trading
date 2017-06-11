using System;
using System.Threading.Tasks;
using MtApi;
using Xcs.Api;
using XCS.Api;

namespace TradingAi.Api.Xcs
{
    public class Metatrader4Env : IEnvironment
    {
        private readonly MtApiClient ApiClient = new MtApiClient();


        public Task<char[]> GetSituationAsync()
        {
            // Moving Average (MA)
            // SMA with period 5
            //ApiClient.iMA(symbol , -1, 5, -1, 1, )
        

            // Commodity Channel Index (CCI)

            // Chaikin Money Flow (CMF)

            // Moving average convergence divergence (MACD)

            // Percentage Price Oscillator (PPO)

            // Relative Strength Index (RSI)

            // Rate of Change (ROC)

            // Williams Percent R (WPR)
            throw  new NotImplementedException();
        }

        public Task ExecuteActionAsync(XcsAction action)
        {
            throw new System.NotImplementedException();
        }
    }
}
