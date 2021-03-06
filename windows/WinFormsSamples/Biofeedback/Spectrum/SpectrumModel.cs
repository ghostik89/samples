﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Neuro;

namespace Indices.Spectrum
{
    class SpectrumModel
    {
        private SpectrumChannel _spectrumChannel;

        public double FrequencyStep => _spectrumChannel?.HzPerSpectrumSample ?? 1;
        public float SamplingFrequency => _spectrumChannel?.SamplingFrequency ?? 250;
        public double WindowDuration { get; set; }
        public float LowFrequency { get; set; }
        public float HighFrequency { get; set; }
        public SpectrumWindow WindowType { get; set; }
        public Spectrum Spectrum { get; private set; }
        public double SpectrumPower { get; private set; }
        private readonly Action _calculationTask;
        private bool _stopTask = false;

        public SpectrumModel()
        {
            WindowType = SpectrumWindow.Rectangular;
            WindowDuration = 8;
            LowFrequency = 8;
            HighFrequency = 14;
            SpectrumPower = 0.0;
            Spectrum = new Spectrum("", new double[1024]);
            _calculationTask = ()=>
            {
                try
                {
                    CalculateSpectrum();
                }
                catch (Exception e) { Console.WriteLine(e.Message);}

                if (!_stopTask) Thread.Sleep(50);
                if (!_stopTask) Task.Run(_calculationTask);
            };
            Task.Run(_calculationTask);
        }

        ~SpectrumModel()
        {
            _stopTask = true;
        }

        public void SetChannels(SpectrumChannel channel)
        {
            _spectrumChannel = channel;
        }

        private void CalculateSpectrum()
        {
            if (_spectrumChannel == null) return;
            var readLength = (int) (SamplingFrequency * WindowDuration);
            if (readLength <= 0)
                return;
   
                var offset = _spectrumChannel.TotalLength - readLength;
                if (offset < 0)
                {
                    offset = 0;
                    readLength = _spectrumChannel.TotalLength;
                }
            Spectrum = new Spectrum(_spectrumChannel.Info.Name,
                _spectrumChannel.ReadData(offset, readLength, WindowType));
            SpectrumPower = SpectrumPowerChannel.SpectrumPower(LowFrequency, HighFrequency, Spectrum.Data,
                _spectrumChannel.HzPerSpectrumSample);
        }
    }
}
