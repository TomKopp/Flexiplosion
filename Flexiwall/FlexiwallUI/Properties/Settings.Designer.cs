﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FlexiWallUI.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UseEmulator {
            get {
                return ((bool)(this["UseEmulator"]));
            }
            set {
                this["UseEmulator"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ShowDepth {
            get {
                return ((bool)(this["ShowDepth"]));
            }
            set {
                this["ShowDepth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public float MaxDepth {
            get {
                return ((float)(this["MaxDepth"]));
            }
            set {
                this["MaxDepth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public float MinDepth {
            get {
                return ((float)(this["MinDepth"]));
            }
            set {
                this["MinDepth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4")]
        public float BlurRadius {
            get {
                return ((float)(this["BlurRadius"]));
            }
            set {
                this["BlurRadius"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public float MaxEmulatorDiameter {
            get {
                return ((float)(this["MaxEmulatorDiameter"]));
            }
            set {
                this["MaxEmulatorDiameter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool InterpolateDepthLayers {
            get {
                return ((bool)(this["InterpolateDepthLayers"]));
            }
            set {
                this["InterpolateDepthLayers"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1920")]
        public double ResolutionX {
            get {
                return ((double)(this["ResolutionX"]));
            }
            set {
                this["ResolutionX"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1080")]
        public double ResolutionY {
            get {
                return ((double)(this["ResolutionY"]));
            }
            set {
                this["ResolutionY"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.16")]
        public double DepthThreshold {
            get {
                return ((double)(this["DepthThreshold"]));
            }
            set {
                this["DepthThreshold"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.5")]
        public double CenterOffset {
            get {
                return ((double)(this["CenterOffset"]));
            }
            set {
                this["CenterOffset"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("127")]
        public double DefaultDepthColorValue {
            get {
                return ((double)(this["DefaultDepthColorValue"]));
            }
            set {
                this["DefaultDepthColorValue"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int StateManagementStackSize {
            get {
                return ((int)(this["StateManagementStackSize"]));
            }
            set {
                this["StateManagementStackSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.3")]
        public double StateManagementPullThreshold {
            get {
                return ((double)(this["StateManagementPullThreshold"]));
            }
            set {
                this["StateManagementPullThreshold"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int StateManagerActionDelay {
            get {
                return ((int)(this["StateManagerActionDelay"]));
            }
            set {
                this["StateManagerActionDelay"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.5")]
        public double StateManagementPushThreshold {
            get {
                return ((double)(this["StateManagementPushThreshold"]));
            }
            set {
                this["StateManagementPushThreshold"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int StateManagementLayerTimer {
            get {
                return ((int)(this["StateManagementLayerTimer"]));
            }
            set {
                this["StateManagementLayerTimer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ConfigFile {
            get {
                return ((string)(this["ConfigFile"]));
            }
            set {
                this["ConfigFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.1")]
        public double EmulatorCenterSize {
            get {
                return ((double)(this["EmulatorCenterSize"]));
            }
            set {
                this["EmulatorCenterSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public float InteractionDepth {
            get {
                return ((float)(this["InteractionDepth"]));
            }
            set {
                this["InteractionDepth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SelectEllipse {
            get {
                return ((bool)(this["SelectEllipse"]));
            }
            set {
                this["SelectEllipse"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool SelectRectangle {
            get {
                return ((bool)(this["SelectRectangle"]));
            }
            set {
                this["SelectRectangle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public float ZoomFactor {
            get {
                return ((float)(this["ZoomFactor"]));
            }
            set {
                this["ZoomFactor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("200")]
        public double LenseSize {
            get {
                return ((double)(this["LenseSize"]));
            }
            set {
                this["LenseSize"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public double LenseMinDepth {
            get {
                return ((double)(this["LenseMinDepth"]));
            }
            set {
                this["LenseMinDepth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double LenseTopOffset {
            get {
                return ((double)(this["LenseTopOffset"]));
            }
            set {
                this["LenseTopOffset"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double LenseLeftOffset {
            get {
                return ((double)(this["LenseLeftOffset"]));
            }
            set {
                this["LenseLeftOffset"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double CameraTopOffset {
            get {
                return ((double)(this["CameraTopOffset"]));
            }
            set {
                this["CameraTopOffset"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double CameraLeftOffset {
            get {
                return ((double)(this["CameraLeftOffset"]));
            }
            set {
                this["CameraLeftOffset"] = value;
            }
        }
    }
}
