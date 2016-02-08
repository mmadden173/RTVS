﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Microsoft.Common.Core;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.R.Package.DataInspect {
    /// <summary>
    /// Item in Page. It implements <see cref="INotifyPropertyChanged"/> for <see cref="Data"/>, which will fire when realized
    /// </summary>
    public class PageItem<TData> : IIndexedItem, INotifyPropertyChanged {
        public PageItem(int index = -1) {
            Index = index;
        }

        public int Index { get; }

        private TData _data;
        public TData Data {
            get { return _data; }
            set {
                SetValue(ref _data, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetValue<T>(ref T storage, T value, [CallerMemberName]string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(storage, value)) {
                return false;
            }

            storage = value;
            this.OnPropertyChanged(propertyName);

            return true;
        }

        protected void OnPropertyChanged(string propertyName) {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null) {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString() {
            if (_data != null) {
                return _data.ToString();
            }
            return Index.ToString();
        }
    }
}
