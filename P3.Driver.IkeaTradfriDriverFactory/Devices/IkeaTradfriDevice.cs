﻿using System;
using System.Threading.Tasks;
using Automatica.Core.Driver;
using Microsoft.Extensions.Logging;
using Tomidix.NetStandard.Tradfri.Models;

namespace P3.Driver.IkeaTradfriDriverFactory.Devices
{
    public abstract class IkeaTradfriDevice : DriverBase
    {
        private readonly DeviceType _deviceType;
        internal IkeaTradfriContainerNode Container { get; }
        
        protected IkeaTradfriDevice(IDriverContext driverContext, IkeaTradfriContainerNode container, DeviceType deviceType) : base(driverContext)
        {
            _deviceType = deviceType;
            Container = container;
        }

        protected abstract void Update(TradfriDevice device);

        public override async Task<bool> Start()
        {
            await Container.Gateway.Driver.RegisterChange(a =>
            {
                try
                {
                    Update(a);
                }
                catch (Exception e)
                {
                    DriverContext.Logger.LogError(e, "Could not update tradfri device state");
                }

            }, _deviceType, Container.DeviceId);

            var device = await Container.Gateway.Driver.GetDevice(Container.DeviceId);
            Update(device);
            
            return true;
        }

        public override IDriverNode CreateDriverNode(IDriverContext ctx)
        {
            return null;
        }
    }
}
