using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MtApi;
using Xcs.Api;
using XCS.Api;

namespace TradingAi.Api.Xcs
{
    public class Metatrader4Env : IEnvironment
    {
        private readonly MtApiClient _apiClient = new MtApiClient();
        private readonly char[] _situation = new char[8];

        public async Task<char[]> GetSituationAsync(string symbol, ChartPeriod timeframe, int period, int appliedPrice, int shift)
        {
            // Moving Average (MA)
            _situation[0] = await GetMovingAverage(symbol, ChartPeriod.PERIOD_M15, appliedPrice, shift);

            // Commodity Channel Index (CCI)
            _situation[1] = await GetCommodityChannelIndex(symbol, timeframe, period, appliedPrice, shift);
            
            // Chaikin Money Flow (CMF)

            // Moving average convergence divergence (MACD)

            // Percentage Price Oscillator (PPO)

            // Relative Strength Index (RSI)

            // Rate of Change (ROC)

            // Williams Percent R (WPR)
            throw  new NotImplementedException();
        }

        public Task<char[]> GetSituationAsync()
        {
            throw new NotImplementedException();
        }

        #region Get situation values

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol">Stock trading symbol. --> "EURUSD"</param>
        /// <param name="maShift">Indicators line offset relate to the chart by timeframe. --> </param>
        /// <param name="timeframe">It can be any of ENUM_TIMEFRAMES enumeration values. 0 means the current chart timeframe.</param>
        /// <param name="shift">Index of the value taken from the indicator buffer (shift relative to the current bar the given amount of periods ago).</param>
        /// <returns></returns>
        private async Task<char> GetMovingAverage(string symbol, int maShift, ChartPeriod timeframe, int shift)
        {
            // Stocksymbol
            // Applyed to a 15 minute time frame
            // First 50 then 5 periods
            // MaShift ....?!
            // EMA method = 1
            // Closed price = 0
            // Shift ... ?!

            var ema_50_Val = await Execute(() => _apiClient.iMA(symbol, (int)timeframe, 50, 1, 1, 0, 1));
            var ema_5_Val = await Execute(() => _apiClient.iMA(symbol, (int)timeframe, 5, 1, 1, 0, 1));

            if (ema_50_Val <= ema_5_Val)
            {
                return '0';
            }
            else 
            {
                return '1';
            }
        }

        private async Task<char> GetCommodityChannelIndex(string symbol, ChartPeriod timeframe, int period, int appliedPrice, int shift)
        {
            var cciVal = await Execute(() => _apiClient.iCCI(symbol, (int) timeframe, period, appliedPrice, shift));

            if (cciVal <= -100)
            {
                return '1';
            }
            else if (cciVal >= 100)
            {
                return '0';
            }
            else
            {
                return '#';
            }
            // TODO: Calculate: So heuristics are applied in such a scenario that is if the current CCI is greater than past ten periods moving average of CCI then a bid signal is triggered and vice versa.
            //else if (CciVal > CcimaVal)
            //{
            //    return '1';
            //}
            //else
            //{
            //    return '0';
            //}
        }

        private async Task<char> GetChaikinMoneyFlow()
        {
            var cmaVal = await Execute(() => _apiClient.(symbol, (int)timeframe, 50, maShift, 1, appliedPrice, shift));
        }

        #endregion Get situation values



        #region Execute methods

        private Task Execute(Action action)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    action();
                }
                catch (MtConnectionException ex)
                {
                    Debug.WriteLine("MtExecutionException: " + ex.Message);
                }
                catch (MtExecutionException ex)
                {
                    Debug.WriteLine("MtExecutionException: " + ex.Message + "; ErrorCode = " + ex.ErrorCode);
                }
            });
        }

        private Task<TResult> Execute<TResult>(Func<TResult> func)
        {
            return Task.Factory.StartNew(() =>
            {
                var result = default(TResult);
                try
                {
                    result = func();
                }
                catch (MtConnectionException ex)
                {
                    Debug.WriteLine("MtExecutionException: " + ex.Message);
                }
                catch (MtExecutionException ex)
                {
                    Debug.WriteLine("MtExecutionException: " + ex.Message + "; ErrorCode = " + ex.ErrorCode);
                }

                return result;
            });
        }

        #endregion Execute methods
    }
}
