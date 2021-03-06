﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OpenGlove_API_C_Sharp_HL.ServiceReference1 {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Glove", Namespace="http://schemas.datacontract.org/2004/07/OpenGloveWCF")]
    [System.SerializableAttribute()]
    public partial class Glove : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string BluetoothAddressField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool ConnectedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private OpenGlove_API_C_Sharp_HL.ServiceReference1.Glove.Configuration GloveConfigurationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PortField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private OpenGlove_API_C_Sharp_HL.ServiceReference1.Side SideField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BluetoothAddress {
            get {
                return this.BluetoothAddressField;
            }
            set {
                if ((object.ReferenceEquals(this.BluetoothAddressField, value) != true)) {
                    this.BluetoothAddressField = value;
                    this.RaisePropertyChanged("BluetoothAddress");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Connected {
            get {
                return this.ConnectedField;
            }
            set {
                if ((this.ConnectedField.Equals(value) != true)) {
                    this.ConnectedField = value;
                    this.RaisePropertyChanged("Connected");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public OpenGlove_API_C_Sharp_HL.ServiceReference1.Glove.Configuration GloveConfiguration {
            get {
                return this.GloveConfigurationField;
            }
            set {
                if ((object.ReferenceEquals(this.GloveConfigurationField, value) != true)) {
                    this.GloveConfigurationField = value;
                    this.RaisePropertyChanged("GloveConfiguration");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Port {
            get {
                return this.PortField;
            }
            set {
                if ((object.ReferenceEquals(this.PortField, value) != true)) {
                    this.PortField = value;
                    this.RaisePropertyChanged("Port");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public OpenGlove_API_C_Sharp_HL.ServiceReference1.Side Side {
            get {
                return this.SideField;
            }
            set {
                if ((this.SideField.Equals(value) != true)) {
                    this.SideField = value;
                    this.RaisePropertyChanged("Side");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
        [System.Runtime.Serialization.DataContractAttribute(Name="Glove.Configuration", Namespace="http://schemas.datacontract.org/2004/07/OpenGloveWCF")]
        [System.SerializableAttribute()]
        public partial class Configuration : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
            
            [System.NonSerializedAttribute()]
            private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private int[] AllowedBaudRatesField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private int BaudRateField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private string GloveHashField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private string GloveNameField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private OpenGlove_API_C_Sharp_HL.ServiceReference1.Glove.Configuration.Profile GloveProfileField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private string[] NegativeInitField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private int[] NegativePinsField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private string[] PositiveInitField;
            
            [System.Runtime.Serialization.OptionalFieldAttribute()]
            private int[] PositivePinsField;
            
            public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
                get {
                    return this.extensionDataField;
                }
                set {
                    this.extensionDataField = value;
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public int[] AllowedBaudRates {
                get {
                    return this.AllowedBaudRatesField;
                }
                set {
                    if ((object.ReferenceEquals(this.AllowedBaudRatesField, value) != true)) {
                        this.AllowedBaudRatesField = value;
                        this.RaisePropertyChanged("AllowedBaudRates");
                    }
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public int BaudRate {
                get {
                    return this.BaudRateField;
                }
                set {
                    if ((this.BaudRateField.Equals(value) != true)) {
                        this.BaudRateField = value;
                        this.RaisePropertyChanged("BaudRate");
                    }
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public string GloveHash {
                get {
                    return this.GloveHashField;
                }
                set {
                    if ((object.ReferenceEquals(this.GloveHashField, value) != true)) {
                        this.GloveHashField = value;
                        this.RaisePropertyChanged("GloveHash");
                    }
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public string GloveName {
                get {
                    return this.GloveNameField;
                }
                set {
                    if ((object.ReferenceEquals(this.GloveNameField, value) != true)) {
                        this.GloveNameField = value;
                        this.RaisePropertyChanged("GloveName");
                    }
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public OpenGlove_API_C_Sharp_HL.ServiceReference1.Glove.Configuration.Profile GloveProfile {
                get {
                    return this.GloveProfileField;
                }
                set {
                    if ((object.ReferenceEquals(this.GloveProfileField, value) != true)) {
                        this.GloveProfileField = value;
                        this.RaisePropertyChanged("GloveProfile");
                    }
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public string[] NegativeInit {
                get {
                    return this.NegativeInitField;
                }
                set {
                    if ((object.ReferenceEquals(this.NegativeInitField, value) != true)) {
                        this.NegativeInitField = value;
                        this.RaisePropertyChanged("NegativeInit");
                    }
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public int[] NegativePins {
                get {
                    return this.NegativePinsField;
                }
                set {
                    if ((object.ReferenceEquals(this.NegativePinsField, value) != true)) {
                        this.NegativePinsField = value;
                        this.RaisePropertyChanged("NegativePins");
                    }
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public string[] PositiveInit {
                get {
                    return this.PositiveInitField;
                }
                set {
                    if ((object.ReferenceEquals(this.PositiveInitField, value) != true)) {
                        this.PositiveInitField = value;
                        this.RaisePropertyChanged("PositiveInit");
                    }
                }
            }
            
            [System.Runtime.Serialization.DataMemberAttribute()]
            public int[] PositivePins {
                get {
                    return this.PositivePinsField;
                }
                set {
                    if ((object.ReferenceEquals(this.PositivePinsField, value) != true)) {
                        this.PositivePinsField = value;
                        this.RaisePropertyChanged("PositivePins");
                    }
                }
            }
            
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
            
            protected void RaisePropertyChanged(string propertyName) {
                System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
                if ((propertyChanged != null)) {
                    propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                }
            }
            
            [System.Diagnostics.DebuggerStepThroughAttribute()]
            [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
            [System.Runtime.Serialization.DataContractAttribute(Name="Glove.Configuration.Profile", Namespace="http://schemas.datacontract.org/2004/07/OpenGloveWCF")]
            [System.SerializableAttribute()]
            public partial class Profile : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
                
                [System.NonSerializedAttribute()]
                private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
                
                [System.Runtime.Serialization.OptionalFieldAttribute()]
                private int AreaCountField;
                
                [System.Runtime.Serialization.OptionalFieldAttribute()]
                private string GloveHashField;
                
                [System.Runtime.Serialization.OptionalFieldAttribute()]
                private System.Collections.Generic.Dictionary<string, string> MappingsField;
                
                [System.Runtime.Serialization.OptionalFieldAttribute()]
                private string ProfileNameField;
                
                public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
                    get {
                        return this.extensionDataField;
                    }
                    set {
                        this.extensionDataField = value;
                    }
                }
                
                [System.Runtime.Serialization.DataMemberAttribute()]
                public int AreaCount {
                    get {
                        return this.AreaCountField;
                    }
                    set {
                        if ((this.AreaCountField.Equals(value) != true)) {
                            this.AreaCountField = value;
                            this.RaisePropertyChanged("AreaCount");
                        }
                    }
                }
                
                [System.Runtime.Serialization.DataMemberAttribute()]
                public string GloveHash {
                    get {
                        return this.GloveHashField;
                    }
                    set {
                        if ((object.ReferenceEquals(this.GloveHashField, value) != true)) {
                            this.GloveHashField = value;
                            this.RaisePropertyChanged("GloveHash");
                        }
                    }
                }
                
                [System.Runtime.Serialization.DataMemberAttribute()]
                public System.Collections.Generic.Dictionary<string, string> Mappings {
                    get {
                        return this.MappingsField;
                    }
                    set {
                        if ((object.ReferenceEquals(this.MappingsField, value) != true)) {
                            this.MappingsField = value;
                            this.RaisePropertyChanged("Mappings");
                        }
                    }
                }
                
                [System.Runtime.Serialization.DataMemberAttribute()]
                public string ProfileName {
                    get {
                        return this.ProfileNameField;
                    }
                    set {
                        if ((object.ReferenceEquals(this.ProfileNameField, value) != true)) {
                            this.ProfileNameField = value;
                            this.RaisePropertyChanged("ProfileName");
                        }
                    }
                }
                
                public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
                
                protected void RaisePropertyChanged(string propertyName) {
                    System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
                    if ((propertyChanged != null)) {
                        propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                    }
                }
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Side", Namespace="http://schemas.datacontract.org/2004/07/OpenGloveWCF")]
    public enum Side : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Right = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Left = 1,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IOGService")]
    public interface IOGService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOGService/GetGloves", ReplyAction="http://tempuri.org/IOGService/GetGlovesResponse")]
        OpenGlove_API_C_Sharp_HL.ServiceReference1.Glove[] GetGloves();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOGService/RefreshGloves", ReplyAction="http://tempuri.org/IOGService/RefreshGlovesResponse")]
        OpenGlove_API_C_Sharp_HL.ServiceReference1.Glove[] RefreshGloves();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOGService/SaveGlove", ReplyAction="http://tempuri.org/IOGService/SaveGloveResponse")]
        void SaveGlove(OpenGlove_API_C_Sharp_HL.ServiceReference1.Glove glove);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOGService/Activate", ReplyAction="http://tempuri.org/IOGService/ActivateResponse")]
        int Activate(string gloveAddress, int actuator, int intensity);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOGService/Connect", ReplyAction="http://tempuri.org/IOGService/ConnectResponse")]
        int Connect(string gloveAddress);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOGService/Disconnect", ReplyAction="http://tempuri.org/IOGService/DisconnectResponse")]
        int Disconnect(string gloveAddress);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IOGService/ActivateMany", ReplyAction="http://tempuri.org/IOGService/ActivateManyResponse")]
        int ActivateMany(string gloveAddress, int[] actuators, int[] intensityList);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IOGServiceChannel : OpenGlove_API_C_Sharp_HL.ServiceReference1.IOGService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OGServiceClient : System.ServiceModel.ClientBase<OpenGlove_API_C_Sharp_HL.ServiceReference1.IOGService>, OpenGlove_API_C_Sharp_HL.ServiceReference1.IOGService {
        
        public OGServiceClient() {
        }
        
        public OGServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public OGServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OGServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OGServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public OpenGlove_API_C_Sharp_HL.ServiceReference1.Glove[] GetGloves() {
            return base.Channel.GetGloves();
        }
        
        public OpenGlove_API_C_Sharp_HL.ServiceReference1.Glove[] RefreshGloves() {
            return base.Channel.RefreshGloves();
        }
        
        public void SaveGlove(OpenGlove_API_C_Sharp_HL.ServiceReference1.Glove glove) {
            base.Channel.SaveGlove(glove);
        }
        
        public int Activate(string gloveAddress, int actuator, int intensity) {
            return base.Channel.Activate(gloveAddress, actuator, intensity);
        }
        
        public int Connect(string gloveAddress) {
            return base.Channel.Connect(gloveAddress);
        }
        
        public int Disconnect(string gloveAddress) {
            return base.Channel.Disconnect(gloveAddress);
        }
        
        public int ActivateMany(string gloveAddress, int[] actuators, int[] intensityList) {
            return base.Channel.ActivateMany(gloveAddress, actuators, intensityList);
        }
    }
}
