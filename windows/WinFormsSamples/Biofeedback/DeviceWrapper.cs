﻿using System;
using Neuro;

namespace Indices
{
    public class DeviceWrapper
    {
        private readonly DeviceInfo _deviceInfo;
        public DeviceWrapper(DeviceInfo deviceInfo)
        {
            _deviceInfo = deviceInfo;
        }

        public static implicit operator DeviceInfo(DeviceWrapper wrapper)
        {
            return wrapper._deviceInfo;
        }

        public override string ToString()
        {
            return $"{_deviceInfo.Name} [{_deviceInfo.Address:x6}]";
        }
    }
}