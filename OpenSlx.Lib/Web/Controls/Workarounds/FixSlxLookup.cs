﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sage.SalesLogix.Web.Controls.Lookup;
using System.Web.UI;


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

namespace OpenSlx.Lib.Web.Controls.Workarounds
{
    /// <summary>
    /// Bug fixes for the SLX lookupcontrol.
    /// This defaults the sort to the first column in the lookup when the lookup is set to Initialize.
    /// </summary>
    public class FixSlxLookup : LookupControl
    {
        /// <summary>
        /// Add the sorting hack.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (InitializeLookup && LookupProperties.Count > 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(),
                    @"$(document).ready(function() { setTimeout(function() {
                        " + this.ClientID + @"_luobj.initGrid = function(seedValue, reload) {
                            LookupControl.prototype.initGrid.apply(this, [seedValue, reload]);
                            this.getGrid().getNativeGrid().getStore().setDefaultSort('" + this.LookupProperties[0].PropertyName + @"');
                        };
                    }, 500) });", true);
            }
        }

        /// <summary>
        /// Initialize the lookup image if necessary
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Fix the image - by default the SLX controls tries to load it from the current type's assembly.
            // since this is a subclass of LookupControl it is not in the "correct" assembly anymore.
            // this fix ensures that the image is loaded from the original assembly
            if (this.ViewState["LookupImageURL"] == null)
                LookupImageURL = this.Page.ClientScript.GetWebResourceUrl(typeof(LookupControl), "Sage.SalesLogix.Web.Controls.Resources.Find_16x16.gif");
        }
    }
}
