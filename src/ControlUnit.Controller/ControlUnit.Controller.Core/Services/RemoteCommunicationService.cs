﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace ControlUnit.Controller.Core.Services
{
    /// <summary>
    /// Defines how the datagrams looks like
    /// </summary>
    public interface IRemoteCommunicationFormatProvider
    {
        /// <summary>
        /// Pack the requested method with params to the datagram/package
        /// </summary>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        object BuildRequestObject(MethodInfo method, Dictionary<string, ConstantExpression> parameters);
    }

    /// <summary>
    /// Common communication service for remote procedure calls
    /// with json serialization and serial bluetooth connection
    /// </summary>
    /// <typeparam name="TServiceInterface">The data contract interface</typeparam>
    public class RemoteCommunicationService<TServiceInterface>
    {
        private IRemoteCommunicationFormatProvider _formatProvider;
        private IBluetoothConnectionService _connection;

        public RemoteCommunicationService(IRemoteCommunicationFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;

            //TODO: Event verdrahten, und Mapping für Request/Response mit GUids?! und ENumtypen mb
        }

        /// <summary>
        /// Calls the specified procedure on the target endpoint
        /// </summary>
        /// <param name="proc">The target remote procedure</param>
        public async Task CallRemoteProcedureAsync(Expression<Action<IControllerService>> proc)
        {
            var expr = proc.Body as MethodCallExpression;
            var method = expr.Method;
            var paramInfos = expr.Method.GetParameters();
            var parameters = new Dictionary<string, ConstantExpression>();

            for (int i = 0; i < paramInfos.Length; i++)
            {
                parameters.Add(paramInfos[i].Name, (ConstantExpression)expr.Arguments[i]);
            }

            var requestObject = _formatProvider.BuildRequestObject(method, parameters);
            var requestJson = JsonConvert.SerializeObject(requestObject);
            await _connection.TransmitData(ToBytes(requestJson));
        }

        /// <summary>
        /// Calls the specified procedure on the target endpoint with result
        /// </summary>
        /// <typeparam name="TReturnType">The remote procedure result type</typeparam>
        /// <param name="proc">The target remote procedure</param>
        /// <returns>The remote procedure result</returns>
        public TReturnType CallRemoteFunction<TReturnType>(Expression<Action<IControllerService>> proc)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Convert json-string to byte array
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public byte[] ToBytes(string jsonString) => jsonString.ToCharArray().Select(c => Convert.ToByte(c)).ToArray();

        /// <summary>
        /// Converts byte array back to json-string
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ToString(byte[] data) => new string(data.Select(c => (char)c).ToArray());
    }
}