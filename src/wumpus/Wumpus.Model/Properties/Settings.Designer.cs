﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wumpus.Model.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfInt xmlns:xsi=\"http://www.w3.org" +
            "/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <int>" +
            "5</int>\r\n  <int>3</int>\r\n  <int>1</int>\r\n</ArrayOfInt>")]
        public int[] ArrowNumbers {
            get {
                return ((int[])(this["ArrowNumbers"]));
            }
            set {
                this["ArrowNumbers"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3." +
            "org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <s" +
            "tring>Easy</string>\r\n  <string>Medium</string>\r\n  <string>Hard</string>\r\n</Array" +
            "OfString>")]
        public string[] LevelNames {
            get {
                return ((string[])(this["LevelNames"]));
            }
            set {
                this["LevelNames"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfInt xmlns:xsi=\"http://www.w3.org" +
            "/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <int>" +
            "3</int>\r\n  <int>6</int>\r\n  <int>12</int>\r\n</ArrayOfInt>")]
        public int[] TrapNumberMins {
            get {
                return ((int[])(this["TrapNumberMins"]));
            }
            set {
                this["TrapNumberMins"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfInt xmlns:xsi=\"http://www.w3.org" +
            "/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <int>" +
            "3</int>\r\n  <int>10</int>\r\n  <int>20</int>\r\n</ArrayOfInt>")]
        public int[] TrapNumberMax {
            get {
                return ((int[])(this["TrapNumberMax"]));
            }
            set {
                this["TrapNumberMax"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfInt xmlns:xsi=\"http://www.w3.org" +
            "/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <int>" +
            "4</int>\r\n  <int>6</int>\r\n  <int>8</int>\r\n</ArrayOfInt>")]
        public int[] Sizes {
            get {
                return ((int[])(this["Sizes"]));
            }
            set {
                this["Sizes"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfInt xmlns:xsi=\"http://www.w3.org" +
            "/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <int>" +
            "1</int>\r\n  <int>1</int>\r\n  <int>1</int>\r\n</ArrayOfInt>")]
        public int[] TrapNumberTypes {
            get {
                return ((int[])(this["TrapNumberTypes"]));
            }
            set {
                this["TrapNumberTypes"] = value;
            }
        }
    }
}
