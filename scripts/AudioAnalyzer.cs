using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class AudioAnalyzer : Node
{
    public static AudioAnalyzer Instance;
    [Export] private int _audioBusIndex = 0;
    [Export] private int _spectrumEffectIndex = 0;
    [Export] private int _decibelsDiference = -20;
    [Export] private float _smoothBands = 5;

    [Export(PropertyHint.Range, "1, 256")] private int _busChannelsAmount = 64;
    
    private int _minFreq = 20;
    private int _maxFreq = 20000;

    private int _minDb = -40;
    private int _maxDb = 0;

    private float _amplitud;
    private float _maxAmplitud = 1;

    private float _accel = 5;

    private List<float> _busChannels = new List<float>();

    private AudioEffectSpectrumAnalyzerInstance _spectrum;

    private bool _resetValues = false;

    public List<float> BusChannels => _busChannels;

    public float Amplitud => _amplitud / _maxAmplitud;

    public override void _Ready()
    {
        if (Instance != null)
        {
            this.QueueFree();
        }
        else
        {
            Instance = this;
        }
        
        _spectrum = (AudioEffectSpectrumAnalyzerInstance) AudioServer.GetBusEffectInstance(_audioBusIndex, _spectrumEffectIndex);
        _busChannels.Clear();

        for (int i = 0; i < _busChannelsAmount; i++)
        {
            _busChannels.Add(0);
        }
    }

    public override void _Process(float delta)
    {
        if (AudioServer.GetBusPeakVolumeLeftDb(1, 0) > -200 || AudioServer.GetBusPeakVolumeRightDb(1, 0) > -200)
        {
            _resetValues = false;
            var data = new VData{ delta = delta, bands = _busChannels};
            ThreadPool.QueueUserWorkItem(VisualizerProcess, data);
        }
        else
        {
            if (!_resetValues)
            {
                for (int i = 0; i < _busChannels.Count; i++)
                {
                    _busChannels[i] = 0;
                }

                _amplitud = 0;
                _resetValues = true;
            }
        }
    }

    void VisualizerProcess(object data)
    {
        VData outBands = (VData)data;
        int freq = _minFreq;
        var interval = (_maxFreq - _minFreq) / _busChannelsAmount;

        var minD = _minDb + _decibelsDiference;
        var maxD = _maxDb + _decibelsDiference;
        
        _amplitud = 0;

        for (int i = 0; i < _busChannelsAmount; i++)
        {
            var freqLow = (float)(freq - _minFreq) / (_maxFreq - _minFreq);
            freqLow = freqLow * freqLow * freqLow * freqLow;
            freqLow = Mathf.Lerp(_minFreq, _maxFreq, freqLow);

            freq += interval;

            var freqHigh = (float)(freq - _minFreq) / (_maxFreq - _minFreq);
            freqHigh = freqHigh * freqHigh * freqHigh * freqHigh;
            freqHigh = Mathf.Lerp(_minFreq, _maxFreq, freqHigh);
            
            var mag = _spectrum.GetMagnitudeForFrequencyRange(freqLow, freqHigh).Length();
            
            var decibels = GD.Linear2Db(mag);

            var result = (decibels - minD) / (maxD - minD);

            result += .05f * (freq - _minFreq) / (_maxFreq - _minFreq);

            result = Mathf.Clamp(result, .005f, 1f);

            _amplitud += result;
            
            outBands.bands[i] = Mathf.Lerp(outBands.bands[i], result, _accel * outBands.delta * _smoothBands);
            //outBands.bands[i] = result;
        }

        if (_amplitud > _maxAmplitud)
            _maxAmplitud = _amplitud;
    }
}

public struct VData
{
    public float delta;
    public List<float> bands;
}