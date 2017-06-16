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

        public async Task<char[]> GetSituationAsync(string symbol, ChartPeriod timeframe, int maShift, int period, int appliedPrice, int shift)
        {
            // Moving Average (MA)
            _situation[0] = await GetMovingAverage(symbol, maShift, ChartPeriod.PERIOD_M15, shift);

            // Commodity Channel Index (CCI)
            _situation[1] = await GetCommodityChannelIndex(symbol, timeframe, period, appliedPrice, shift);

            // Chaikin Money Flow (CMF)
            _situation[2] = '#';

            // Moving average convergence divergence (MACD)
            _situation[3] = await GetMovingAverageConvergenceDivergence(null, ChartPeriod.ZERO, 0, 0, 0, 0, 0, 0);

            // Percentage Price Oscillator (PPO)
            _situation[4] = '#';

            // Relative Strength Index (RSI)
            _situation[5] = await GetRelativeStrengthIndex(null, ChartPeriod.ZERO, 0, 0, 0);

            // Rate of Change (ROC)
            _situation[6] = '#';

            // Williams Percent R (WPR)
            _situation[7] = await GetWilliamsPercentR(null, ChartPeriod.ZERO, 0, 0);

            throw  new NotImplementedException();
        }

        public Task ExecuteActionAsync(XcsAction action)
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

            var ema50Val = await Execute(() => _apiClient.iMA(symbol, (int)timeframe, 50, maShift, 1, 0, shift));
            var ema5Val = await Execute(() => _apiClient.iMA(symbol, (int)timeframe, 5, maShift, 1, 0, shift));

            if (ema50Val <= ema5Val)
            {
                return '0';
            }
            else 
            {
                return '1';
            }

            // TODO: Check parameters
            throw new NotImplementedException();
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
        
        private async Task<char> GetMovingAverageConvergenceDivergence(string symbol, ChartPeriod timeframe, int fastEmaPeriod, int slowEmaPeriod, int signalPeriod, int appliedPrice, int mode, int shift)
        {
            var macd = await Execute(() => _apiClient.iMACD(symbol, (int) timeframe, fastEmaPeriod, slowEmaPeriod, signalPeriod, appliedPrice, mode, shift));

            if (macd > 0)
            {
                return '1';
            }
            else
            {
                return '0';
            }
        }

        private async Task<char> GetRelativeStrengthIndex(string symbol, ChartPeriod timeframe, int period, int appliedPrice, int shift)
        {
            var rsi = await Execute(() => _apiClient.iRSI(symbol, (int)timeframe, period, appliedPrice, shift));

            // Overbought
            if (rsi >= 80)
            {
                return '0';
            }
            // Oversold
            else if (rsi <= 20)
            {
                return '1';
            }
            else
            {
                return '#';
            }
        }

        private async Task<char> GetWilliamsPercentR(string symbol, ChartPeriod timeframe, int period, int shift)
        {
            var wpr = await Execute(() => _apiClient.iWPR(symbol, (int)timeframe, period, shift));
            char result = '#';
            // Overbought
            if (wpr >= -20)
            {
                result = '0';
            }
            // Oversold
            else if (wpr <= -80)
            {
                result = '1';
            }
            else
            {
                result = '#';
            }

            

            return result;
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
