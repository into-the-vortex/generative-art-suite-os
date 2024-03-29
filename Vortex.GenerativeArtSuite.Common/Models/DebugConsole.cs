﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace Vortex.GenerativeArtSuite.Common.Models
{
    public class DebugConsole : IReadOnlyList<ConsoleMessage>, INotifyCollectionChanged
    {
        private readonly ObservableCollection<ConsoleMessage> messages = new();

        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => messages.CollectionChanged += value;
            remove => messages.CollectionChanged -= value;
        }

        public int Count => messages.Count;

        public ConsoleMessage this[int index] => messages[index];

        public void Log(string message) => Dispatch(() => messages.Add(ConsoleMessage.Info(message)));

        public void Warn(string message) => Dispatch(() => messages.Add(ConsoleMessage.Warning(message)));

        public void Error(string message) => Dispatch(() => messages.Add(ConsoleMessage.Error(message)));

        public void Clear() => Dispatch(() => messages.Clear());

        public IEnumerator<ConsoleMessage> GetEnumerator() => messages.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => messages.GetEnumerator();

        private static void Dispatch(Action callback)
        {
            Application.Current.Dispatcher.Invoke(callback);
        }
    }
}
