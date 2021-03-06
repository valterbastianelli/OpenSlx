﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/*
   OpenSlx - Open Source SalesLogix Library and Tools
   Copyright 2010 nicocrm (http://github.com/ngaller/OpenSlx)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


namespace OpenSlx.Lib.Web.Controls
{
    /// <summary>
    /// The default ASP.NET dropdownlist can't be bound by SLX because it lacks a "SelectedValueChanged" event.
    /// This control adds the event, and also adds error handling for the case of the value not matching an existing
    /// list item.
    /// </summary>
    [DefaultProperty("SelectedValue")]
    [ToolboxData("<{0}:BindableDropDownList runat=server></{0}:BindableDropDownList>")]
    [ValidationProperty("Text")]
    public class BindableDropDownList : DropDownList
    {
        /// <summary>
        /// Raised when the value of the control may have changed.
        /// Used by Saleslogix databinding.
        /// </summary>
        public event EventHandler SelectedValueChanged;

        /// <summary>
        /// If set to true, and the SelectedValue is set to an item that is not in the 
        /// list, it will be added automatically.
        /// </summary>
        public bool AddMissingItems { get; set; }

        /// <summary>
        /// Add our event hook
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.SelectedIndexChanged += delegate
            {
                if (SelectedValueChanged != null)
                    SelectedValueChanged(this, EventArgs.Empty);
            };
        }


        /// <summary>
        /// Currently selected value.
        /// If this is set to a value not available in the control, it will be cleared.
        /// </summary>
        [Bindable(true)]
        [Description("Currently selected value")]
        [Category("Data")]
        public override string SelectedValue
        {
            get
            {
                return base.SelectedValue == "" ? null : base.SelectedValue;
            }
            set
            {
                var item = this.Items.FindByValue(value);
                ClearSelection();
                if (item == null && AddMissingItems)
                {
                    item = new ListItem(value, value);
                    Items.Add(item);
                }
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }

        /// <summary>
        /// Data binding - with exception handling.
        /// </summary>
        /// <param name="dataSource"></param>
        protected override void PerformDataBinding(System.Collections.IEnumerable dataSource)
        {
            //log4net.LogManager.GetLogger(ClientID).Debug("Before Databinding, selected index = " + SelectedIndex);
            //log4net.LogManager.GetLogger(ClientID).Debug("PerformDataBinding");
            try
            {
                base.PerformDataBinding(dataSource);
            }
            catch (ArgumentOutOfRangeException)
            {
                base.SelectedValue = null;
            }
            //log4net.LogManager.GetLogger(ClientID).Debug("After Databinding, selected index = " + SelectedIndex);
        }
    }
}
