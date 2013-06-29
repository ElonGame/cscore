﻿using CSCore.Codecs.MP3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSCore.Streams.SampleConverter;

namespace CSCore
{
    public static class Extensions
    {
        public static IWaveSource ToWaveSource(this ISampleSource sampleSource, int bits)
        {
            if (sampleSource == null)
                throw new ArgumentNullException("sampleSource");

            if (bits == 8)
                return new SampleToPcm8(sampleSource);
            if (bits == 16)
                return new SampleToPcm16(sampleSource);
            if(bits == 24)
                return new SampleToPcm24(sampleSource);
            if (bits == 32)
                return new SampleToIeeeFloat32(sampleSource);
            else
                throw new ArgumentOutOfRangeException("bits");
        }

        public static ISampleSource ToSampleSource(this IWaveSource waveSource)
        {
            if (waveSource == null)
                throw new ArgumentNullException("waveSource");

            return WaveToSampleBase.CreateConverter(waveSource);
        }

        public static TimeSpan GetLength(this IWaveStream source)
        {
            return GetTime(source, source.Length);
        }

        public static TimeSpan GetPosition(this IWaveStream source)
        {
            return GetTime(source, source.Position);
        }

        public static TimeSpan GetTime(this IWaveStream source, long bytes)
        {
            return TimeSpan.FromMilliseconds(GetMilliseconds(source, bytes));
        }

        public static long GetMilliseconds(this IWaveStream source, long bytes)
        {
            if(source == null)
                throw new ArgumentNullException("source");
            if (bytes < 0)
                throw new ArgumentOutOfRangeException("bytes");

            if (source is IWaveSource)
            {
                return source.WaveFormat.BytesToMilliseconds(bytes);
            }
            else if (source is ISampleSource)
            {
                return source.WaveFormat.BytesToMilliseconds(bytes * 4);
            }
            else
            {
                throw new NotSupportedException("IWaveStream-Subtype is not supported");
            }
        }

        public static long GetBytes(this IWaveStream source, TimeSpan timespan)
        {
            return GetBytes(source, (long)timespan.TotalMilliseconds);
        }

        public static long GetBytes(this IWaveStream source, long milliseconds)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (milliseconds < 0)
                throw new ArgumentOutOfRangeException("milliseconds");

            if (source is IWaveSource)
            {
                return source.WaveFormat.MillisecondsToBytes(milliseconds);
            }
            else if (source is ISampleSource)
            {
                return source.WaveFormat.MillisecondsToBytes(milliseconds * 4);
            }
            else
            {
                throw new NotSupportedException("IWaveStream-Subtype is not supported");
            }
        }

        public static short ToShort(this MP3ChannelMode channel)
        {
            if (channel == MP3ChannelMode.Mono)
                return 1;
            else if (channel == MP3ChannelMode.Stereo)
                return 2;
            else if (channel == MP3ChannelMode.JointStereo)
                return 2;
            else if (channel == MP3ChannelMode.DualChannel)
                return 2;

            return 0;
        }

        public static T[] CheckBuffer<T>(this T[] inst, long size, bool exactSize = false)
        {
            if (inst == null || (!exactSize && inst.Length < size) || (exactSize && inst.Length != size))
            {
                return new T[size];
            }
            return inst;
        }

        public static IEnumerable<T> Add<T>(this IEnumerable<T> e, T value)
        {
            foreach (var cur in e)
            {
                yield return cur;
            }
            yield return value;
        }

        public static bool IsClosed(this Stream stream)
        {
            return stream.CanRead || stream.CanWrite;
        }

        public static int LowWord(this int number)
        { return number & 0x0000FFFF; }
        public static int LowWord(this int number, int newValue)
        { return (int)((number & 0xFFFF0000) + (newValue & 0x0000FFFF)); }
        public static int HighWord(this int number)
        { return (int)(number & 0xFFFF0000); }
        public static int HighWord(this int number, int newValue)
        { return (number & 0x0000FFFF) + (newValue << 16); }

        public static uint LowWord(this uint number)
        { return number & 0x0000FFFF; }
        public static uint LowWord(this uint number, int newValue)
        { return (uint)((number & 0xFFFF0000) + (newValue & 0x0000FFFF)); }
        public static uint HighWord(this uint number)
        { return (uint)(number & 0xFFFF0000); }
        public static uint HighWord(this uint number, int newValue)
        { return (uint)((number & 0x0000FFFF) + (newValue << 16)); }

        public static Guid GetGuid(this Object obj)
        {
            return obj.GetType().GUID;
        }
    }
}