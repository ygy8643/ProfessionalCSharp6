﻿using MyApplicationEvents;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Console;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<EventSource> eventSources = EventSource.GetSources();
            InitListener(eventSources);

            WriteLine($"Log Guid: {SampleEventSource.Log.Guid}");
            WriteLine($"Name: {SampleEventSource.Log.Name}");
            ParallelRequestSample();
            ReadLine();
        }

        private static void InitListener(IEnumerable<EventSource> sources)
        {
            var listener = new MyEventListener();
            foreach (var source in sources)
            {
                listener.EnableEvents(source, EventLevel.LogAlways);
            }
        }

        private static void ParallelRequestSample()
        {
            SampleEventSource.Log.RequestStart();
            Parallel.For(0, 20, x =>
            {
                ProcessTaskAsync(x).Wait();
            });
            SampleEventSource.Log.RequestStop();
            WriteLine("Activity complete");
        }

        private static async Task ProcessTaskAsync(int x)
        {
            SampleEventSource.Log.ProcessingStart(x);
            var r = new Random();
            await Task.Delay(r.Next(500));

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("http://www.bing.com");
            }

            SampleEventSource.Log.ProcessingStop(x);
        }
    }
}