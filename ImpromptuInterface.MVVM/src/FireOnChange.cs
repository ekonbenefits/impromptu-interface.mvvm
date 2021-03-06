﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace ImpromptuInterface.MVVM
{
    /// <summary>
    /// Proxy that gives a syntax for setting property changes calls for specific properties.
    /// </summary>
    public class FireOnPropertyChanged : DynamicObject
    {
        protected readonly INotifyPropertyChanged Parent;


        /// <summary>
        /// Initializes a new instance of the <see cref="FireOnPropertyChanged"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public FireOnPropertyChanged(INotifyPropertyChanged parent)
        {
            Parent = parent;
            Parent.PropertyChanged += OnPropertyChanged;
            EventStore = new Dictionary<string, PropertyChangedEventHandler>();
        
        }


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            PropertyChangedEventHandler tHandler;
            result = null;
            if (EventStore.TryGetValue(binder.Name, out tHandler))
            {
                result = tHandler;
            }

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            EventStore[binder.Name] = (PropertyChangedEventHandler)value;
            return true;
        }

        protected readonly IDictionary<string, PropertyChangedEventHandler> EventStore;

        /// <summary>
        /// Called when receives [property changed] event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        public virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler tHandler;


            if (EventStore.TryGetValue(e.PropertyName, out tHandler))
            {
                tHandler(sender, e);
            }
        }
    }


   
    
}
