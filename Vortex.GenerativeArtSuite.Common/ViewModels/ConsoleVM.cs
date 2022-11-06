using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Vortex.GenerativeArtSuite.Common.Models;
using Console = Vortex.GenerativeArtSuite.Common.Models.Console;

namespace Vortex.GenerativeArtSuite.Common.ViewModels
{
    public class ConsoleVM
    {
        public ConsoleVM(Console model, int maxDisplay = 5)
        {
            Messages = new(model.Select(m => CreateVM(m)));
            model.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:

                        if (e.NewItems is not null)
                        {
                            foreach (var item in e.NewItems)
                            {
                                if (item is ConsoleMessage m)
                                {
                                    if (Messages.Count == maxDisplay)
                                    {
                                        Messages.RemoveAt(0);
                                    }

                                    Messages.Add(CreateVM(m));
                                }
                            }
                        }

                        break;
                    case NotifyCollectionChangedAction.Reset:
                        Messages.Clear();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            };
        }

        public ObservableCollection<ConsoleMessageVM> Messages { get; } = new();

        private static ConsoleMessageVM CreateVM(ConsoleMessage model)
        {
            switch (model.Type)
            {
                case ConsoleMessageType.Information:
                    return new InfoConsoleMessageVM(model.Message);
                case ConsoleMessageType.Warning:
                    return new WarnConsoleMessageVM(model.Message);
                case ConsoleMessageType.Error:
                    return new ErrorConsoleMessageVM(model.Message);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
