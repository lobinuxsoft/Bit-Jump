using Godot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class AudioAnalyzer : Node
{
    public static AudioAnalyzer instance;
    [Export] private int _audioBusIndex = 0;
    [Export] private int _spectrumEffectIndex = 0;
    [Export] private int decibelsDiference = -20;

    [Export(PropertyHint.Range, "1, 64")] private int _bandAmount = 20;
    
    private int _minFreq = 20;
    private int _maxFreq = 20000;

    private int _minDb = -40;
    private int _maxDb = 0;

    private float _amplitud;
    private float _maxAmplitud = 1;

    private float _accel = 5;

    private List<float> _bands = new List<float>();

    private AudioEffectSpectrumAnalyzerInstance _spectrum;

    private bool _resetValues = false;

    public List<float> Bands => _bands;

    public float Amplitud => _amplitud / _maxAmplitud;

    public override void _Ready()
    {
        if (instance != null)
        {
            this.QueueFree();
        }
        else
        {
            instance = this;
        }
        
        _spectrum = (AudioEffectSpectrumAnalyzerInstance) AudioServer.GetBusEffectInstance(_audioBusIndex, _spectrumEffectIndex);
        _bands.Clear();

        for (int i = 0; i < _bandAmount; i++)
        {
            _bands.Add(0);
        }
    }

    public override void _Process(float delta)
    {
        if (AudioServer.GetBusPeakVolumeLeftDb(1, 0) > -200 || AudioServer.GetBusPeakVolumeRightDb(1, 0) > -200)
        {
            _resetValues = false;
            var data = new VData{ delta = delta, bands = _bands};
            ThreadPool.QueueUserWorkItem(VisualizerProcess, data);
        }
        else
        {
            if (!_resetValues)
            {
                for (int i = 0; i < _bands.Count; i++)
                {
                    _bands[i] = 0;
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
        var interval = (_maxFreq - _minFreq) / _bandAmount;

        var minD = _minDb + decibelsDiference;
        var maxD = _maxDb + decibelsDiference;
        
        _amplitud = 0;

        for (int i = 0; i < _bandAmount; i++)
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

            result += .3f * (freq - _minFreq) / (_maxFreq - _minFreq);

            result = Mathf.Clamp(result, .005f, 1f);

            _amplitud += result;
            
            outBands.bands[i] = Mathf.Lerp(outBands.bands[i], result, _accel * outBands.delta);
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