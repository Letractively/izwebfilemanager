// Copyright (C) 2006 Igor Zelmanovich <izwebfilemanager@gmail.com>
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.ComponentModel;
using IZ.WebFileManager.Components;
using System.Web;
using System.Collections.Specialized;
using System.Drawing.Design;

namespace IZ.WebFileManager
{
	[ToolboxData ("<{0}:FileManager runat=\"server\" width=\"400\" height=\"300\"></{0}:FileManager>")]
	public sealed class FileManager : FileManagerControlBase, IPostBackEventHandler
	{
		#region Fields

		FileView _fileView;
		FolderTree _folderTree;
		Menu _toolBar;
		BorderedPanelStyle _addressBarStyle;
		BorderedPanelStyle _addressTextBoxStyle;
		BorderedPanelStyle _toolbarStyle;
		BorderedPanelStyle _toolbarButtonStyle;
		BorderedPanelStyle _toolbarButtonHoverStyle;
		BorderedPanelStyle _toolbarButtonPressedStyle;
		ToolbarOptions _toolbarOptions;
		CustomToolbarButtonCollection _cusomToolbarButtonCollection;

		BorderedPanelStyle _defaultToolbarStyle;
		BorderedPanelStyle _defaultToolbarButtonStyle;
		BorderedPanelStyle _defaultToolbarButtonHoverStyle;
		BorderedPanelStyle _defaultToolbarButtonPressedStyle;

		#endregion

		#region Events

		[Category ("Action")]
		public event CommandEventHandler ToolbarCommand;

		#endregion

		#region Properties

		[Themeable (false)]
		[Localizable (false)]
		[DefaultValue (true)]
		[Category ("Appearance")]
		public bool ShowToolBar {
			get { return ViewState ["ShowToolBar"] == null ? true : (bool) ViewState ["ShowToolBar"]; }
			set { ViewState ["ShowToolBar"] = value; }
		}

		[Themeable (false)]
		[Localizable (false)]
		[DefaultValue (true)]
		[Category ("Appearance")]
		public bool ShowAddressBar {
			get { return ViewState ["ShowAddressBar"] == null ? true : (bool) ViewState ["ShowAddressBar"]; }
			set { ViewState ["ShowAddressBar"] = value; }
		}

		[Themeable (false)]
		[Localizable (false)]
		[DefaultValue (true)]
		[Category ("Appearance")]
		public bool ShowUploadBar {
			get { return ViewState ["ShowUploadBar"] == null ? true : (bool) ViewState ["ShowUploadBar"]; }
			set { ViewState ["ShowUploadBar"] = value; }
		}

		[Category ("Appearance")]
		[PersistenceMode (PersistenceMode.InnerProperty)]
		[Localizable (false)]
		[Themeable (true)]
		public ToolbarOptions ToolbarOptions {
			get {
				if (_toolbarOptions == null) {
					_toolbarOptions = new ToolbarOptions (ViewState);
				}
				return _toolbarOptions;
			}
		}

		[MergableProperty (false)]
		[PersistenceMode (PersistenceMode.InnerProperty)]
		[Category ("Behavior")]
		[Localizable (false)]
		[Themeable (false)]
		public CustomToolbarButtonCollection CustomToolbarButtons {
			get {
				if (_cusomToolbarButtonCollection == null) {
					_cusomToolbarButtonCollection = new CustomToolbarButtonCollection ();
					if (IsTrackingViewState)
						((IStateManager) _cusomToolbarButtonCollection).TrackViewState ();
				}
				return _cusomToolbarButtonCollection;
			}
		}

		FileView FileView {
			get {
				EnsureChildControls ();
				return _fileView;
			}
		}

		FolderTree FolderTree {
			get {
				EnsureChildControls ();
				return _folderTree;
			}
		}

		public override FileManagerItemInfo [] SelectedItems {
			get {
				return FileView.SelectedItems;
			}
		}

		[Editor ("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor))]
		[DefaultValue ("")]
		[UrlProperty]
		[Bindable (true)]
		[Category ("Appearance")]
		public string SplitterImageUrl {
			get { return ViewState ["SplitterImageUrl"] == null ? String.Empty : (string) ViewState ["SplitterImageUrl"]; }
			set { ViewState ["SplitterImageUrl"] = value; }
		}

		// TODO
		[PersistenceMode (PersistenceMode.InnerProperty)]
		[Category ("Appearance")]
		internal BorderedPanelStyle AddressBarStyle {
			get {
				if (_addressBarStyle == null) {
					_addressBarStyle = new BorderedPanelStyle ();
					_addressBarStyle.PaddingLeft = Unit.Pixel (3);
					_addressBarStyle.PaddingTop = Unit.Pixel (2);
					_addressBarStyle.PaddingBottom = Unit.Pixel (2);
					_addressBarStyle.PaddingRight = Unit.Pixel (2);
					if (IsTrackingViewState)
						((IStateManager) _addressBarStyle).TrackViewState ();
				}
				return _addressBarStyle;
			}
		}

		// TODO
		[PersistenceMode (PersistenceMode.InnerProperty)]
		[Category ("Appearance")]
		internal BorderedPanelStyle AddressTextBoxStyle {
			get {
				if (_addressTextBoxStyle == null) {
					_addressTextBoxStyle = new BorderedPanelStyle ();
					_addressTextBoxStyle.Font.Names = new string [] { "Tahoma", "Verdana", "Geneva", "Arial", "Helvetica", "sans-serif" };
					_addressTextBoxStyle.Font.Size = FontUnit.Parse ("11px", null);
					_addressTextBoxStyle.Width = Unit.Percentage (98);
					_addressTextBoxStyle.BorderStyle = BorderStyle.Solid;
					_addressTextBoxStyle.BorderWidth = Unit.Pixel (1);
					_addressTextBoxStyle.BorderColor = Color.FromArgb (0xACA899);
					_addressTextBoxStyle.PaddingLeft = Unit.Pixel (2);
					_addressTextBoxStyle.PaddingTop = Unit.Pixel (2);
					_addressTextBoxStyle.PaddingBottom = Unit.Pixel (2);
					if (IsTrackingViewState)
						((IStateManager) _addressTextBoxStyle).TrackViewState ();
				}
				return _addressTextBoxStyle;
			}
		}

		[PersistenceMode (PersistenceMode.InnerProperty)]
		[Category ("Appearance")]
		public BorderedPanelStyle ToolbarStyle {
			get {
				if (_toolbarStyle == null) {
					_toolbarStyle = new BorderedPanelStyle ();
					if (IsTrackingViewState)
						((IStateManager) _toolbarStyle).TrackViewState ();
				}
				return _toolbarStyle;
			}
		}

		bool ToolbarStyleCreated {
			get { return _toolbarStyle != null; }
		}

		BorderedPanelStyle DefaultToolbarStyle {
			get {
				if (_defaultToolbarStyle == null) {
					_defaultToolbarStyle = new BorderedPanelStyle ();
					_defaultToolbarStyle.ForeColor = Color.Black;
					_defaultToolbarStyle.Font.Names = new string [] { "Tahoma", "Verdana", "Geneva", "Arial", "Helvetica", "sans-serif" };
					_defaultToolbarStyle.Font.Size = FontUnit.Parse ("11px", null);
					_defaultToolbarStyle.Height = Unit.Pixel (24);
					_defaultToolbarStyle.BackImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbg.gif");
					_defaultToolbarStyle.BackColor = Color.FromArgb (0xEBEADB);
					_defaultToolbarStyle.PaddingLeft = Unit.Pixel (3);
					_defaultToolbarStyle.PaddingRight = Unit.Pixel (3);
					_defaultToolbarStyle.PaddingTop = Unit.Pixel (2);
				}
				return _defaultToolbarStyle;
			}
		}

		[PersistenceMode (PersistenceMode.InnerProperty)]
		[Category ("Appearance")]
		public BorderedPanelStyle ToolbarButtonStyle {
			get {
				if (_toolbarButtonStyle == null) {
					_toolbarButtonStyle = new BorderedPanelStyle ();
					if (IsTrackingViewState)
						((IStateManager) _toolbarButtonStyle).TrackViewState ();
				}
				return _toolbarButtonStyle;
			}
		}

		bool ToolbarButtonStyleCreated {
			get { return _toolbarButtonStyle != null; }
		}

		BorderedPanelStyle DefaultToolbarButtonStyle {
			get {
				if (_defaultToolbarButtonStyle == null) {
					_defaultToolbarButtonStyle = new BorderedPanelStyle ();
					_defaultToolbarButtonStyle.OuterBorderStyle = OuterBorderStyle.AllSides;
					_defaultToolbarButtonStyle.OuterBorderTopWidth = Unit.Pixel (3);
					_defaultToolbarButtonStyle.OuterBorderLeftWidth = Unit.Pixel (3);
					_defaultToolbarButtonStyle.OuterBorderRightWidth = Unit.Pixel (3);
					_defaultToolbarButtonStyle.OuterBorderBottomWidth = Unit.Pixel (3);
					_defaultToolbarButtonStyle.ForeColor = Color.Black;
				}
				return _defaultToolbarButtonStyle;
			}
		}

		[PersistenceMode (PersistenceMode.InnerProperty)]
		[Category ("Appearance")]
		public BorderedPanelStyle ToolbarButtonHoverStyle {
			get {
				if (_toolbarButtonHoverStyle == null) {
					_toolbarButtonHoverStyle = new BorderedPanelStyle ();
					if (IsTrackingViewState)
						((IStateManager) _toolbarButtonHoverStyle).TrackViewState ();
				}
				return _toolbarButtonHoverStyle;
			}
		}

		bool ToolbarButtonHoverStyleCreated {
			get { return _toolbarButtonHoverStyle != null; }
		}

		BorderedPanelStyle DefaultToolbarButtonHoverStyle {
			get {
				if (_defaultToolbarButtonHoverStyle == null) {
					_defaultToolbarButtonHoverStyle = new BorderedPanelStyle ();
					_defaultToolbarButtonHoverStyle.ForeColor = Color.Black;
					_defaultToolbarButtonHoverStyle.BackColor = Color.FromArgb (0xf5f5ef);
					_defaultToolbarButtonHoverStyle.OuterBorderLeftBottomCornerImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtnhover_LB.gif");
					_defaultToolbarButtonHoverStyle.OuterBorderRightBottomCornerImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtnhover_RB.gif");
					_defaultToolbarButtonHoverStyle.OuterBorderLeftTopCornerImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtnhover_LT.gif");
					_defaultToolbarButtonHoverStyle.OuterBorderRightTopCornerImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtnhover_RT.gif");
					_defaultToolbarButtonHoverStyle.OuterBorderTopImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtnhover_T.gif");
					_defaultToolbarButtonHoverStyle.OuterBorderBottomImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtnhover_B.gif");
					_defaultToolbarButtonHoverStyle.OuterBorderLeftImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtnhover_L.gif");
					_defaultToolbarButtonHoverStyle.OuterBorderRightImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtnhover_R.gif");
				}
				return _defaultToolbarButtonHoverStyle;
			}
		}

		[PersistenceMode (PersistenceMode.InnerProperty)]
		[Category ("Appearance")]
		public BorderedPanelStyle ToolbarButtonPressedStyle {
			get {
				if (_toolbarButtonPressedStyle == null) {
					_toolbarButtonPressedStyle = new BorderedPanelStyle ();
					if (IsTrackingViewState)
						((IStateManager) _toolbarButtonPressedStyle).TrackViewState ();
				}
				return _toolbarButtonPressedStyle;
			}
		}

		bool ToolbarButtonPressedStyleCreated {
			get { return _toolbarButtonPressedStyle != null; }
		}

		BorderedPanelStyle DefaultToolbarButtonPressedStyle {
			get {
				if (_defaultToolbarButtonPressedStyle == null) {
					_defaultToolbarButtonPressedStyle = new BorderedPanelStyle ();
					_defaultToolbarButtonPressedStyle.ForeColor = Color.Black;
					_defaultToolbarButtonPressedStyle.BackColor = Color.FromArgb (0xe3e3db);
					_defaultToolbarButtonPressedStyle.OuterBorderLeftBottomCornerImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtndown_LB.gif");
					_defaultToolbarButtonPressedStyle.OuterBorderRightBottomCornerImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtndown_RB.gif");
					_defaultToolbarButtonPressedStyle.OuterBorderLeftTopCornerImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtndown_LT.gif");
					_defaultToolbarButtonPressedStyle.OuterBorderRightTopCornerImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtndown_RT.gif");
					_defaultToolbarButtonPressedStyle.OuterBorderTopImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtndown_T.gif");
					_defaultToolbarButtonPressedStyle.OuterBorderBottomImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtndown_B.gif");
					_defaultToolbarButtonPressedStyle.OuterBorderLeftImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtndown_L.gif");
					_defaultToolbarButtonPressedStyle.OuterBorderRightImageUrl = Page.ClientScript.GetWebResourceUrl (GetType (), "IZ.WebFileManager.resources.toolbarbtndown_R.gif");
				}
				return _defaultToolbarButtonPressedStyle;
			}
		}

		[Bindable (true)]
		[Category ("Behavior")]
		[DefaultValue (false)]
		[Localizable (false)]
		public bool UseLinkToOpenItem {
			get { return FileView.UseLinkToOpenItem; }
			set { FileView.UseLinkToOpenItem = value; }
		}

		[Bindable (true)]
		[Category ("Behavior")]
		[DefaultValue ("_blank")]
		[Localizable (false)]
		public string LinkToOpenItemTarget {
			get { return FileView.LinkToOpenItemTarget; }
			set { FileView.LinkToOpenItemTarget = value; }
		}

		[Bindable (true)]
		[Category ("Appearance")]
		[DefaultValue (FileViewRenderMode.Icons)]
		[Localizable (false)]
		public FileViewRenderMode FileViewMode {
			get { return FileView.View; }
			set { FileView.View = value; }
		}

		[PersistenceMode (PersistenceMode.InnerProperty)]
		[Category ("Appearance")]
		public Style FileViewStyle {
			get { return FileView.ControlStyle; }
		}

		[PersistenceMode (PersistenceMode.InnerProperty)]
		[Category ("Appearance")]
		public Style FolderTreeStyle {
			get { return FolderTree.ControlStyle; }
		}

		[Bindable (true)]
		[Category ("Data")]
		[DefaultValue ("[0]/")]
		[Localizable (false)]
		[Themeable (false)]
		public string Directory {
			get { return FileView.Directory; }
			set { FileView.Directory = value; }
		}

		public override FileManagerItemInfo CurrentDirectory {
			get {
				return FileView.CurrentDirectory;
			}
		}

		#endregion

		protected override void OnPreRender (EventArgs e) {
			base.OnPreRender (e);
			if (ShowToolBar)
				CreateToolbar ();
			RegisterSplitterClientScript ();
			RegisterLayoutSetupScript ();

			Page.Form.Enctype = "multipart/form-data";

			Page.ClientScript.RegisterExpandoAttribute (_fileView.ClientID, "rootControl", ClientID);
		}

		private void RegisterLayoutSetupScript () {
			StringBuilder sb = new StringBuilder ();
			if (!ShowAddressBar)
				sb.AppendLine ("var addressBarHeight = 0;");
			else
				sb.AppendLine ("var addressBarHeight = WebForm_GetElementPosition(WebForm_GetElementById('" + ClientID + "_AddressBar')).height;");
			if (!ShowToolBar)
				sb.AppendLine ("var toolBarHeight = 0;");
			else
				sb.AppendLine ("var toolBarHeight = WebForm_GetElementPosition(WebForm_GetElementById('" + ClientID + "_ToolBar')).height;");
			if (ReadOnly || !AllowUpload || !ShowUploadBar)
				sb.AppendLine ("var uploadBarHeight = 0;");
			else
				sb.AppendLine ("var uploadBarHeight = WebForm_GetElementPosition(WebForm_GetElementById('" + ClientID + "_UploadBar')).height;");
			sb.AppendLine ("var fileManagerHeight = WebForm_GetElementPosition(WebForm_GetElementById('" + ClientID + "')).height;");
			sb.AppendLine ("var requestedHeight = fileManagerHeight - addressBarHeight - toolBarHeight - uploadBarHeight;");
			sb.AppendLine ("WebForm_SetElementHeight(WebForm_GetElementById('" + _fileView.ClientID + "'), requestedHeight);");
			sb.AppendLine ("WebForm_SetElementHeight(WebForm_GetElementById('" + _folderTree.ClientID + "'), requestedHeight);");
			Page.ClientScript.RegisterStartupScript (typeof (FileManager), ClientID + "LayoutSetup", sb.ToString (), true);

		}

		private void RegisterSplitterClientScript () {
			StringBuilder sb = new StringBuilder ();
			sb.AppendLine ("var __fileView;");
			sb.AppendLine ("var __fileViewWidth;");
			sb.AppendLine ("var __folderTree;");
			sb.AppendLine ("var __folderTreeWidth;");
			sb.AppendLine ("var __clientX;");
			sb.AppendLine ("var __document_onmousemove;");
			sb.AppendLine ("var __document_onmouseup;");
			sb.AppendLine ("function " + ClientID + "SplitterDragStart(e) {");
			sb.AppendLine ("if(e == null) var e = event;");
			sb.AppendLine ("__fileView = WebForm_GetElementById('" + _fileView.ClientID + "');");
			sb.AppendLine ("__fileViewWidth = WebForm_GetElementPosition(__fileView).width;");
			sb.AppendLine ("__folderTree = WebForm_GetElementById('" + _folderTree.ClientID + "');");
			sb.AppendLine ("__folderTreeWidth = WebForm_GetElementPosition(__folderTree).width;");
			sb.AppendLine ("__clientX = e.clientX;");
			sb.AppendLine ("__document_onmousemove = document.onmousemove;");
			sb.AppendLine ("__document_onmouseup = document.onmouseup;");
			sb.AppendLine ("document.onmousemove = " + ClientID + "SplitterDrag;");
			sb.AppendLine ("document.onmouseup = " + ClientID + "SplitterDragEnd;");
			sb.AppendLine ("return false;");
			sb.AppendLine ("}");
			sb.AppendLine ("function " + ClientID + "SplitterDragEnd(e) {");
			sb.AppendLine ("document.onmousemove = __document_onmousemove;");
			sb.AppendLine ("document.onmouseup = __document_onmouseup;");
			sb.AppendLine ("return false;");
			sb.AppendLine ("}");
			sb.AppendLine ("function " + ClientID + "SplitterDrag(e) {");
			sb.AppendLine ("if(e == null) var e = event;");
			if (Controller.IsRightToLeft)
				sb.AppendLine ("var __delta = __clientX - e.clientX;");
			else
				sb.AppendLine ("var __delta = e.clientX - __clientX;");
			sb.AppendLine ("WebForm_SetElementWidth(__fileView, __fileViewWidth - __delta);");
			sb.AppendLine ("WebForm_SetElementWidth(__folderTree, __folderTreeWidth + __delta);");
			sb.AppendLine ("return false;");
			sb.AppendLine ("}");
			Page.ClientScript.RegisterClientScriptBlock (typeof (FileManager), ClientID + "SplitterDrag", sb.ToString (), true);
		}

		protected override void CreateChildControls () {
			base.CreateChildControls ();
			CreateFileView ();
			CreateFolderTree ();
		}

		private void CreateFolderTree () {
			_folderTree = new FolderTree (Controller, _fileView);
			_folderTree.ID = "FolderTree";
			Controls.Add (_folderTree);
		}

		private void CreateFileView () {
			_fileView = new FileView (Controller);
			_fileView.ID = "FileView";
			Controls.Add (_fileView);
		}

        class IE8Fix : Style
        {
            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                //.IE8fix
                //{
                //     z-index: 100;
                //}
                base.FillStyleAttributes(attributes, urlResolver);
                attributes.Add(HtmlTextWriterStyle.ZIndex, "100");
            }
        }

		private void CreateToolbar () {
			_toolBar = new Menu ();
			_toolBar.EnableViewState = false;
			_toolBar.StaticEnableDefaultPopOutImage = false;
			_toolBar.DynamicEnableDefaultPopOutImage = false;
			_toolBar.Orientation = Orientation.Horizontal;
			_toolBar.SkipLinkText = String.Empty;
			_toolBar.StaticItemTemplate = new CompiledTemplateBuilder (new BuildTemplateMethod (CreateToolbarButton));
			_toolBar.DynamicItemTemplate = new CompiledTemplateBuilder (new BuildTemplateMethod (CreateToolbarPopupItem));

			// TODO
			_toolBar.DynamicMenuStyle.BackColor = Color.White;
			_toolBar.DynamicMenuStyle.BorderStyle = BorderStyle.Solid;
			_toolBar.DynamicMenuStyle.BorderWidth = Unit.Pixel (1);
			_toolBar.DynamicMenuStyle.BorderColor = Color.FromArgb (0xACA899);
			_toolBar.DynamicMenuStyle.HorizontalPadding = Unit.Pixel (2);
			_toolBar.DynamicMenuStyle.VerticalPadding = Unit.Pixel (2);

            // fix IE8 bug
            var ie8fix = new IE8Fix();
            if (Page.Header != null)
                Page.Header.StyleSheet.RegisterStyle(ie8fix, this);
            _toolBar.DynamicMenuStyle.CssClass = ie8fix.RegisteredCssClass;
            

			_toolBar.DynamicMenuItemStyle.ForeColor = Color.Black;
			_toolBar.DynamicMenuItemStyle.Font.Names = new string [] { "Tahoma", "Verdana", "Geneva", "Arial", "Helvetica", "sans-serif" };
			_toolBar.DynamicMenuItemStyle.VerticalPadding = Unit.Pixel (1);
			_toolBar.DynamicMenuItemStyle.Font.Size = FontUnit.Parse ("11px", null);

			_toolBar.DynamicHoverStyle.ForeColor = Color.White;
			_toolBar.DynamicHoverStyle.BackColor = Color.FromArgb (0x316AC5);

			Controls.Add (_toolBar);

			string clientClickFunction = "javascript:" + FileManagerController.ClientScriptObjectNamePrefix + Controller.ClientID + ".On{0}(" + FileManagerController.ClientScriptObjectNamePrefix + _fileView.ClientID + ", '{1}');return false;";

			// Copy to
			if (ToolbarOptions.ShowCopyButton) {
				MenuItem itemCopyTo = new MenuItem ();
				itemCopyTo.Text = Controller.GetResourceString ("Copy", "Copy");
				itemCopyTo.ImageUrl = Controller.GetToolbarImage (ToolbarImages.Copy);
				itemCopyTo.NavigateUrl = String.Format (clientClickFunction, FileManagerCommands.SelectedItemsCopyTo, "");
				itemCopyTo.Enabled = !ReadOnly;
				_toolBar.Items.Add (itemCopyTo);
			}

			// Move to
			if (ToolbarOptions.ShowMoveButton) {
				MenuItem itemMoveTo = new MenuItem ();
				itemMoveTo.Text = Controller.GetResourceString ("Move", "Move");
				itemMoveTo.ImageUrl = Controller.GetToolbarImage (ToolbarImages.Move);
				itemMoveTo.NavigateUrl = String.Format (clientClickFunction, FileManagerCommands.SelectedItemsMoveTo, "");
				itemMoveTo.Enabled = !ReadOnly && AllowDelete;
				_toolBar.Items.Add (itemMoveTo);
			}

			// Delete
			if (ToolbarOptions.ShowDeleteButton) {
				MenuItem itemDelete = new MenuItem ();
				itemDelete.Text = Controller.GetResourceString ("Delete", "Delete");
				itemDelete.ImageUrl = Controller.GetToolbarImage (ToolbarImages.Delete);
				itemDelete.NavigateUrl = String.Format (clientClickFunction, FileManagerCommands.SelectedItemsDelete, "");
				itemDelete.Enabled = !ReadOnly && AllowDelete;
				_toolBar.Items.Add (itemDelete);
			}

			// Rename
			if (ToolbarOptions.ShowRenameButton) {
				MenuItem itemRename = new MenuItem ();
				itemRename.Text = Controller.GetResourceString ("Rename", "Rename");
				itemRename.ImageUrl = Controller.GetToolbarImage (ToolbarImages.Rename);
				itemRename.NavigateUrl = String.Format (clientClickFunction, FileManagerCommands.Rename, "");
				itemRename.Enabled = !ReadOnly && AllowDelete;
				_toolBar.Items.Add (itemRename);
			}

			// NewFolder
			if (ToolbarOptions.ShowNewFolderButton) {
				MenuItem itemNewFolder = new MenuItem ();
				itemNewFolder.Text = Controller.GetResourceString ("Create_New_Folder", "New Folder");
				itemNewFolder.ImageUrl = Controller.GetToolbarImage (ToolbarImages.NewFolder);
				itemNewFolder.NavigateUrl = String.Format (clientClickFunction, FileManagerCommands.NewFolder, "");
				itemNewFolder.Enabled = !ReadOnly;
				_toolBar.Items.Add (itemNewFolder);
			}

			// FolderUp
			if (ToolbarOptions.ShowFolderUpButton) {
				MenuItem itemFolderUp = new MenuItem ();
				itemFolderUp.Text = Controller.GetResourceString ("Up", "Up");
				itemFolderUp.ImageUrl = Controller.GetToolbarImage (ToolbarImages.FolderUp);
				itemFolderUp.NavigateUrl = String.Format (clientClickFunction, FileManagerCommands.FileViewNavigate, "..");
				_toolBar.Items.Add (itemFolderUp);
			}

			// View
			if (ToolbarOptions.ShowViewButton) {
				MenuItem itemView = new MenuItem ();
				itemView.Text = Controller.GetResourceString ("View", "View");
				itemView.ImageUrl = Controller.GetToolbarImage (ToolbarImages.View);
				itemView.NavigateUrl = "javascript: return;";
				_toolBar.Items.Add (itemView);

				// Icons
				MenuItem itemViewIcons = new MenuItem ();
				itemViewIcons.Text = Controller.GetResourceString ("Icons", "Icons");
				itemViewIcons.NavigateUrl = String.Format (clientClickFunction, FileManagerCommands.FileViewChangeView, FileViewRenderMode.Icons);
				itemView.ChildItems.Add (itemViewIcons);

				// Details
				MenuItem itemViewDetails = new MenuItem ();
				itemViewDetails.Text = Controller.GetResourceString ("Details", "Details");
				itemViewDetails.NavigateUrl = String.Format (clientClickFunction, FileManagerCommands.FileViewChangeView, FileViewRenderMode.Details);
				itemView.ChildItems.Add (itemViewDetails);

				if (Controller.SupportThumbnails) {
					// Thumbnails
					MenuItem itemViewThumbnails = new MenuItem ();
					itemViewThumbnails.Text = Controller.GetResourceString ("Thumbnails", "Thumbnails");
					itemViewThumbnails.NavigateUrl = String.Format (clientClickFunction, FileManagerCommands.FileViewChangeView, FileViewRenderMode.Thumbnails);
					itemView.ChildItems.Add (itemViewThumbnails);
				}
			}

			// Refresh
			if (ToolbarOptions.ShowRefreshButton) {
				MenuItem itemRefresh = new MenuItem ();
				itemRefresh.Text = Controller.GetResourceString ("Refresh", "Refresh");
				itemRefresh.Value = "Refresh";
				itemRefresh.ImageUrl = Controller.GetToolbarImage (ToolbarImages.Refresh);
				itemRefresh.NavigateUrl = String.Format (clientClickFunction, FileManagerCommands.Refresh, "");
				_toolBar.Items.Add (itemRefresh);
			}

			// Custom Buttons
			if (_cusomToolbarButtonCollection != null) {
				for (int i = 0; i < _cusomToolbarButtonCollection.Count; i++) {
					CustomToolbarButton button = _cusomToolbarButtonCollection [i];
					string postBackStatement = null;
					if (button.PerformPostBack) {
						postBackStatement = Page.ClientScript.GetPostBackEventReference (this, "Toolbar:" + i);
					}
					MenuItem item = new MenuItem ()
					{
						Text = button.Text,
						ImageUrl = button.ImageUrl,
						NavigateUrl = "javascript:" + button.OnClientClick + ";" + postBackStatement + ";return false;"
					};
					_toolBar.Items.Add (item);
				}
			}
		}

		void CreateToolbarPopupItem (Control control) {
			MenuItemTemplateContainer container = (MenuItemTemplateContainer) control;
			MenuItem menuItem = (MenuItem) container.DataItem;
			Table t = new Table ();
			t.CellPadding = 0;
			t.CellSpacing = 0;
			t.BorderWidth = 0;
			t.Style [HtmlTextWriterStyle.Cursor] = "default";
			t.Attributes ["onclick"] = menuItem.NavigateUrl;
			container.Controls.Add (t);
			TableRow r = new TableRow ();
			t.Rows.Add (r);
			TableCell c1 = new TableCell ();
			System.Web.UI.WebControls.Image img1 = new System.Web.UI.WebControls.Image ();
			img1.ImageUrl = Page.ClientScript.GetWebResourceUrl (typeof (FileManagerController), "IZ.WebFileManager.resources.Empty.gif");
			img1.Width = 16;
			img1.Height = 16;
			c1.Controls.Add (img1);
			r.Cells.Add (c1);
			TableCell c2 = new TableCell ();
			c2.Style [HtmlTextWriterStyle.PaddingLeft] = "2px";
			c2.Style [HtmlTextWriterStyle.PaddingRight] = "2px";
			c2.Text = "&nbsp;" + menuItem.Text;
			c2.Width = Unit.Percentage (100);
			r.Cells.Add (c2);
			TableCell c3 = new TableCell ();
			System.Web.UI.WebControls.Image img3 = new System.Web.UI.WebControls.Image ();
			img3.ImageUrl = Page.ClientScript.GetWebResourceUrl (typeof (FileManagerController), "IZ.WebFileManager.resources.Empty.gif");
			img3.Width = 16;
			img3.Height = 16;
			c3.Controls.Add (img3);
			r.Cells.Add (c3);
		}

		void CreateToolbarButton (Control control) {
			MenuItemTemplateContainer container = (MenuItemTemplateContainer) control;
			MenuItem menuItem = (MenuItem) container.DataItem;
			BorderedPanel panel = new BorderedPanel ();
			panel.ControlStyle.CopyFrom (DefaultToolbarButtonStyle);
			if (ToolbarButtonStyleCreated)
				panel.ControlStyle.CopyFrom (ToolbarButtonStyle);
			panel.Style [HtmlTextWriterStyle.Cursor] = "default";
			if (menuItem.Enabled) {
				panel.HoverSyle.CopyFrom (DefaultToolbarButtonHoverStyle);
				if (ToolbarButtonHoverStyleCreated)
					panel.HoverSyle.CopyFrom (ToolbarButtonHoverStyle);
				panel.PressedSyle.CopyFrom (DefaultToolbarButtonPressedStyle);
				if (ToolbarButtonPressedStyleCreated)
					panel.PressedSyle.CopyFrom (ToolbarButtonPressedStyle);
				panel.Attributes ["onclick"] = menuItem.NavigateUrl;
			}
			else
				panel.Style ["color"] = "gray";
			container.Controls.Add (panel);
			Table t = new Table ();
			t.CellPadding = 0;
			t.CellSpacing = 0;
			t.BorderWidth = 0;
			panel.Controls.Add (t);
			TableRow r = new TableRow ();
			t.Rows.Add (r);
			TableCell c1 = new TableCell ();
			r.Cells.Add (c1);
			System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image ();
			img.ImageUrl = ((MenuItem) container.DataItem).ImageUrl;
			c1.Controls.Add (img);
			TableCell c2 = new TableCell ();
			c2.Style [HtmlTextWriterStyle.PaddingLeft] = "2px";
			c2.Style [HtmlTextWriterStyle.PaddingRight] = "2px";
			c2.Text = "&nbsp;" + menuItem.Text;
			r.Cells.Add (c2);
		}

		protected override Style CreateControlStyle () {
			Style style = base.CreateControlStyle ();
			style.BackColor = Color.FromArgb (0xEDEBE0);
			return style;
		}

		protected override void AddAttributesToRender (HtmlTextWriter writer) {
			base.AddAttributesToRender (writer);
			writer.AddStyleAttribute (HtmlTextWriterStyle.Cursor, "default");
		}

		protected override void Render (HtmlTextWriter writer) {
			if (DesignMode) {
				writer.Write (String.Format (System.Globalization.CultureInfo.InvariantCulture,
				"<div><table width=\"{0}\" height=\"{1}\" bgcolor=\"#f5f5f5\" bordercolor=\"#c7c7c7\" cellpadding=\"0\" cellspacing=\"0\" border=\"1\"><tr><td valign=\"middle\" align=\"center\">IZWebFileManager - <b>{2}</b></td></tr></table></div>",
					Width,
					Height,
					ID));
				return;
			}

			AddAttributesToRender (writer);
			RenderBeginOuterTable (writer);
			if (ShowAddressBar)
				RenderAddressBar (writer);
			if (ShowToolBar)
				RenderToolBar (writer);

			writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
			writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
			writer.RenderBeginTag (HtmlTextWriterTag.Table);
			writer.RenderBeginTag (HtmlTextWriterTag.Tr);
			writer.RenderBeginTag (HtmlTextWriterTag.Td);

			RenderFolderTree (writer);

			writer.RenderEndTag ();

			// splitter
			writer.AddStyleAttribute (HtmlTextWriterStyle.Cursor, "col-resize");
			writer.AddAttribute ("onmousedown", ClientID + "SplitterDragStart(event)");
			writer.AddAttribute ("onselectstart", "return false");
			writer.RenderBeginTag (HtmlTextWriterTag.Td);
			if (SplitterImageUrl.Length > 0) {
				writer.AddAttribute (HtmlTextWriterAttribute.Src, ResolveClientUrl (SplitterImageUrl));
			}
			else {
				writer.AddAttribute (HtmlTextWriterAttribute.Width, "3px");
				writer.AddAttribute (HtmlTextWriterAttribute.Height, "3px");
				writer.AddAttribute (HtmlTextWriterAttribute.Src, Page.ClientScript.GetWebResourceUrl (typeof (FileManagerController), "IZ.WebFileManager.resources.Empty.gif"));
			}
			writer.AddAttribute (HtmlTextWriterAttribute.Alt, "");
			writer.RenderBeginTag (HtmlTextWriterTag.Img);
			writer.RenderEndTag ();
			writer.RenderEndTag ();

			writer.RenderBeginTag (HtmlTextWriterTag.Td);

			RenderFileView (writer);

			writer.RenderEndTag ();
			writer.RenderEndTag ();
			writer.RenderEndTag ();

			if (!ReadOnly && AllowUpload && ShowUploadBar)
				RenderFileUploadBar (writer);
			RenderEndOuterTable (writer);
		}

		private void RenderToolBar (HtmlTextWriter writer) {
			BorderedPanel p = new BorderedPanel ();
			p.Page = Page;
			p.ControlStyle.CopyFrom (DefaultToolbarStyle);
			if (ToolbarStyleCreated)
				p.ControlStyle.CopyFrom (ToolbarStyle);

			writer.AddStyleAttribute (HtmlTextWriterStyle.Position, "relative");
			writer.AddStyleAttribute (HtmlTextWriterStyle.ZIndex, "100");
			writer.AddAttribute (HtmlTextWriterAttribute.Id, ClientID + "_ToolBar");
			writer.RenderBeginTag (HtmlTextWriterTag.Div);
			p.RenderBeginTag (writer);
			_toolBar.RenderControl (writer);
			p.RenderEndTag (writer);
			writer.RenderEndTag ();
		}

		private void RenderFolderTree (HtmlTextWriter writer) {
			_folderTree.Width = new Unit (Width.Value * 0.25, Width.Type);
			_folderTree.Height = 100;
			_folderTree.RenderControl (writer);
		}

		private void RenderFileView (HtmlTextWriter writer) {
			_fileView.Width = new Unit (Width.Value * 0.75, Width.Type);
			_fileView.Height = 100;
			_fileView.RenderControl (writer);
		}

		private void RenderAddressBar (HtmlTextWriter writer) {
			AddressBarStyle.AddAttributesToRender (writer);
			writer.AddAttribute (HtmlTextWriterAttribute.Id, ClientID + "_AddressBar");
			writer.RenderBeginTag (HtmlTextWriterTag.Div);

			writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
			writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
			writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
			writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
			writer.RenderBeginTag (HtmlTextWriterTag.Table);
			writer.RenderBeginTag (HtmlTextWriterTag.Tr);


			writer.RenderBeginTag (HtmlTextWriterTag.Td);

			writer.AddAttribute (HtmlTextWriterAttribute.Id, _fileView.ClientID + "_Address");
			writer.AddAttribute (HtmlTextWriterAttribute.Value, CurrentDirectory.FileManagerPath, true);
			AddressTextBoxStyle.AddAttributesToRender (writer);
			//writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "readonly");
			writer.RenderBeginTag (HtmlTextWriterTag.Input);
			writer.RenderEndTag ();


			writer.RenderEndTag ();
			writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "16px");
			writer.RenderBeginTag (HtmlTextWriterTag.Td);
			writer.AddAttribute (HtmlTextWriterAttribute.Alt, "");
			writer.AddAttribute (HtmlTextWriterAttribute.Src, Controller.GetToolbarImage (ToolbarImages.Process));
			writer.AddAttribute (HtmlTextWriterAttribute.Id, _fileView.ClientID + "_ProgressImg");
			writer.AddStyleAttribute (HtmlTextWriterStyle.Visibility, "hidden");
			writer.RenderBeginTag (HtmlTextWriterTag.Img);
			writer.RenderEndTag ();
			writer.RenderEndTag ();
			writer.RenderEndTag ();
			writer.RenderEndTag ();

			writer.RenderEndTag ();
		}
		private void RenderFileUploadBar (HtmlTextWriter writer) {
			AddressBarStyle.AddAttributesToRender (writer);
			writer.AddAttribute (HtmlTextWriterAttribute.Id, ClientID + "_UploadBar");
			writer.RenderBeginTag (HtmlTextWriterTag.Div);
			writer.RenderBeginTag (HtmlTextWriterTag.Table);
			writer.RenderBeginTag (HtmlTextWriterTag.Tr);
			writer.AddStyleAttribute (HtmlTextWriterStyle.WhiteSpace, "nowrap");
			writer.RenderBeginTag (HtmlTextWriterTag.Td);
			writer.Write (HttpUtility.HtmlEncode (Controller.GetResourceString ("Upload_File", "Upload File")));
			writer.RenderEndTag ();
			writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
			writer.RenderBeginTag (HtmlTextWriterTag.Td);
			writer.AddAttribute (HtmlTextWriterAttribute.Id, ClientID + "_Upload");
			writer.AddAttribute (HtmlTextWriterAttribute.Name, ClientID + "_Upload");
			writer.AddAttribute (HtmlTextWriterAttribute.Type, "file");
			writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
			writer.RenderBeginTag (HtmlTextWriterTag.Input);
			writer.RenderEndTag ();
			writer.RenderEndTag ();
			writer.RenderBeginTag (HtmlTextWriterTag.Td);
			writer.AddAttribute (HtmlTextWriterAttribute.Type, "button");
			writer.AddAttribute (HtmlTextWriterAttribute.Onclick, Page.ClientScript.GetPostBackEventReference (this, "Upload"));
			writer.AddAttribute (HtmlTextWriterAttribute.Value, Controller.GetResourceString ("Submit", "Submit"));
			writer.RenderBeginTag (HtmlTextWriterTag.Input);
			writer.RenderEndTag ();
			writer.RenderEndTag ();
			writer.RenderEndTag ();
			writer.RenderEndTag ();
			writer.RenderEndTag ();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		private void RenderBeginOuterTable (HtmlTextWriter writer) {
			Controller.AddDirectionAttribute (writer);
			writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
			writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
			writer.RenderBeginTag (HtmlTextWriterTag.Table);
            writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.RenderBeginTag (HtmlTextWriterTag.Td);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		private void RenderEndOuterTable (HtmlTextWriter writer) {
			writer.RenderEndTag ();
			writer.RenderEndTag ();
			writer.RenderEndTag ();
		}

		protected override object SaveViewState () {
			object [] states = new object [6];

			states [0] = base.SaveViewState ();
			if (_toolbarStyle != null)
				states [1] = ((IStateManager) _toolbarStyle).SaveViewState ();
			if (_toolbarButtonStyle != null)
				states [2] = ((IStateManager) _toolbarButtonStyle).SaveViewState ();
			if (_toolbarButtonHoverStyle != null)
				states [3] = ((IStateManager) _toolbarButtonHoverStyle).SaveViewState ();
			if (_toolbarButtonPressedStyle != null)
				states [4] = ((IStateManager) _toolbarButtonPressedStyle).SaveViewState ();
			if (_cusomToolbarButtonCollection != null)
				states [5] = ((IStateManager) _cusomToolbarButtonCollection).SaveViewState ();

			for (int i = 0; i < states.Length; i++) {
				if (states [i] != null)
					return states;
			}
			return null;
		}

		protected override void LoadViewState (object savedState) {
			if (savedState == null)
				return;

			object [] states = (object []) savedState;

			base.LoadViewState (states [0]);
			if (states [1] != null)
				((IStateManager) ToolbarStyle).LoadViewState (states [1]);
			if (states [2] != null)
				((IStateManager) ToolbarButtonStyle).LoadViewState (states [2]);
			if (states [3] != null)
				((IStateManager) ToolbarButtonHoverStyle).LoadViewState (states [3]);
			if (states [4] != null)
				((IStateManager) ToolbarButtonPressedStyle).LoadViewState (states [4]);
			if (states [5] != null)
				((IStateManager) CustomToolbarButtons).LoadViewState (states [5]);
		}

		protected override void TrackViewState () {
			base.TrackViewState ();

			if (_toolbarStyle != null)
				((IStateManager) _toolbarStyle).TrackViewState ();
			if (_toolbarButtonStyle != null)
				((IStateManager) _toolbarButtonStyle).TrackViewState ();
			if (_toolbarButtonHoverStyle != null)
				((IStateManager) _toolbarButtonHoverStyle).TrackViewState ();
			if (_toolbarButtonPressedStyle != null)
				((IStateManager) _toolbarButtonPressedStyle).TrackViewState ();
			if (_cusomToolbarButtonCollection != null)
				((IStateManager) _cusomToolbarButtonCollection).TrackViewState ();
		}

		void RaisePostBackEvent (string eventArgument) {
			if (eventArgument == "Upload") {
				HttpPostedFile uploadedFile = Page.Request.Files [ClientID + "_Upload"];
				if (uploadedFile != null && uploadedFile.ContentLength > 0) {
					FileManagerItemInfo dir = GetCurrentDirectory ();
					Controller.ProcessFileUpload (dir, uploadedFile);
				}
			}
			else if (eventArgument.StartsWith ("Toolbar:", StringComparison.Ordinal)) {
				int i = int.Parse (eventArgument.Substring (8));
				CustomToolbarButton button = CustomToolbarButtons [i];
				OnToolbarCommand (new CommandEventArgs (button.CommandName, button.CommandArgument));
			}
		}

		void OnToolbarCommand (CommandEventArgs e) {
			if (ToolbarCommand != null)
				ToolbarCommand (this, e);
		}

		internal override FileManagerItemInfo GetCurrentDirectory () {
			return Controller.ResolveFileManagerItemInfo (_fileView.Directory);
		}

		#region IPostBackEventHandler Members

		void IPostBackEventHandler.RaisePostBackEvent (string eventArgument) {
			RaisePostBackEvent (eventArgument);
		}

		#endregion
	}
}
