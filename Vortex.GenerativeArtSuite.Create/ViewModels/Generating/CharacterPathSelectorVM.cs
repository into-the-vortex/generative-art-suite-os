using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace Vortex.GenerativeArtSuite.Create.ViewModels.Generating
{
    public class CharacterPathSelectorVM : BindableBase
    {
        private readonly Action<int, string> onChangedCallback;
        private readonly int index;

        private string selectedOption;

        public CharacterPathSelectorVM(int index, string selectedOption, List<string> options, Action<int, string> onChangedCallback)
        {
            this.index = index;
            this.selectedOption = selectedOption;
            this.onChangedCallback = onChangedCallback;

            Options = new ObservableCollection<string>(options);
        }

        public string SelectedOption
        {
            get => selectedOption;
            set => SetProperty(ref selectedOption, value, OnSelectedOptionChanged);
        }

        public ObservableCollection<string> Options { get; }

        private void OnSelectedOptionChanged()
        {
            onChangedCallback(index, selectedOption);
        }
    }
}
