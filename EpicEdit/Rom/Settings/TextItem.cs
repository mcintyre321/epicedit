﻿#region GPL statement
/*Epic Edit is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, version 3 of the License.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.*/
#endregion

using System;
using System.ComponentModel;
using System.Globalization;

using EpicEdit.Rom.Utility;

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// Item of a <see cref="TextCollection"/>.
    /// </summary>
    internal class TextItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly TextCollection collection;

        public TextConverter Converter
        {
            get { return this.collection.Converter; }
        }

        public TextItem(TextCollection collection, string value)
        {
            this.collection = collection;
            this.value = value;
        }

        private string value;
        public string Value
        {
            get { return this.value; }
            set
            {
                string val = this.Converter.GetValidatedText(value);

                if (this.value == val)
                {
                    return;
                }

                this.value = val;

                int diff = this.collection.TotalCharacterCount - this.collection.MaxCharacterCount;
                if (diff > 0)
                {
                    this.value = this.value.Substring(0, this.value.Length - diff);
                }

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }
            }
        }

        public string FormattedValue
        {
            get
            {
                return this.collection.Region == Region.Jap ?
                       this.value :
                       CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.value.ToLowerInvariant());
            }
        }
    }
}
