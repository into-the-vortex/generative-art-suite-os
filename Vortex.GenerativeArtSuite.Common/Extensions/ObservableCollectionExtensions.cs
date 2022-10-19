using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Vortex.GenerativeArtSuite.Common.Models;
using Vortex.GenerativeArtSuite.Common.ViewModels;

namespace Vortex.GenerativeArtSuite.Common.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static IDisposable ConnectModelCollection<TViewModel, TModel>(this ObservableCollection<TViewModel> viewModelCollection, List<TModel> modelCollection, Func<TModel, TViewModel> createVM)
            where TViewModel : class, IViewModel<TModel>
        {
            viewModelCollection.Clear();
            viewModelCollection.AddRange(modelCollection.Select(m => createVM(m)));

            void OnObservableCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:

                        if (e.NewItems != null)
                        {
                            for (int i = 0; i < e.NewItems.Count; i++)
                            {
                                if(e.NewItems[i] is TViewModel vm)
                                {
                                    modelCollection.Insert(e.NewStartingIndex + i, vm.Model);
                                }
                            }
                        }

                        break;
                    case NotifyCollectionChangedAction.Remove:

                        if (e.OldItems != null)
                        {
                            for (int i = e.OldStartingIndex; i < e.OldStartingIndex + e.OldItems.Count; i++)
                            {
                                modelCollection.RemoveAt(i);
                            }
                        }

                        break;
                    case NotifyCollectionChangedAction.Replace:

                        if (e.OldItems != null &&
                            e.NewItems != null &&
                            e.OldItems.Count == e.NewItems.Count &&
                            e.OldStartingIndex == e.NewStartingIndex)
                        {
                            for (int i = 0; i < e.NewItems.Count; i++)
                            {
                                if (e.NewItems[i] is TViewModel newVM)
                                {
                                    modelCollection[e.NewStartingIndex + i] = newVM.Model;
                                }
                            }
                        }

                        break;

                    case NotifyCollectionChangedAction.Move:

                        if (e.OldItems != null &&
                            e.NewItems != null &&
                            e.OldItems.Count == e.NewItems.Count)
                        {
                            for (int i = 0; i < e.NewItems.Count; i++)
                            {
                                if (e.NewItems[i] is TViewModel newVM)
                                {
                                    modelCollection.RemoveAt(e.OldStartingIndex + i);
                                    modelCollection.Insert(e.NewStartingIndex + i, newVM.Model);
                                }
                            }
                        }

                        break;

                    case NotifyCollectionChangedAction.Reset:
                        modelCollection.Clear();
                        break;
                }
            }

            viewModelCollection.CollectionChanged += OnObservableCollectionChanged;
            return new DisposableAction(() => viewModelCollection.CollectionChanged -= OnObservableCollectionChanged);
        }
    }
}
