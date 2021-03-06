﻿// 
//  Copyright 2011 Ekon Benefits
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Dynamitey.DynamicObjects;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Supports Providing Property info to Binding things like DataGrids that refresh with bindings
    /// </summary>
    [Serializable]
    public class ImpromptuBindingList: List, IBindingList
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpromptuBindingList"/> class.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="members">The members.</param>
        public ImpromptuBindingList(IEnumerable<object> contents=null, 
            IEnumerable<KeyValuePair<string, object>> members =null) : base(contents, members)
        {
        }

#if !SILVERLIGHT

  

        #region Implementation of IBindingList
          
        [Obsolete("Not Supported")]
        public object AddNew()
        {
            throw new NotSupportedException();
        }

        [Obsolete("Not Supported")]
        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotSupportedException();
        }
           
        [Obsolete("Not Supported")]
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotSupportedException();
        }

         
        [Obsolete("Not Supported")]
        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotSupportedException();
        }

        [Obsolete("Not Supported")]
        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotSupportedException();
        }

        [Obsolete("Not Supported")]
        public void RemoveSort()
        {
            throw new NotSupportedException();
        }

        public bool AllowNew => false;

        public bool AllowEdit => false;

        public bool AllowRemove => false;

        public bool SupportsChangeNotification => false;

        public bool SupportsSearching => false;

        public bool SupportsSorting => false;

        public bool IsSorted => false;

        [Obsolete("Not Used")]
        public PropertyDescriptor SortProperty => null;

        [Obsolete("Not Used")]
        public ListSortDirection SortDirection => default(ListSortDirection);


        /// <summary>
        /// Occurs when the list changes or an item in the list changes.
        /// </summary>
        [Obsolete("Not Used")]
        public event ListChangedEventHandler ListChanged;
       

        #endregion 
#endif
    }
}
